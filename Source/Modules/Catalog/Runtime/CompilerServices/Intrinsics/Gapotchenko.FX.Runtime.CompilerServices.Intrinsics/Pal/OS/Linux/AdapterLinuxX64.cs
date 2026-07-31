// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS9191 // The 'ref' modifier for an argument corresponding to 'in' parameter is equivalent to 'in'. Consider using 'in' instead.

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
sealed class AdapterLinuxX64 : AdapterX64
{
    protected override bool IsWriteAllowed(Span<byte> span) => true;

    protected override unsafe bool TryAllocateTrampoline(
        Span<byte> redirection,
        int size,
        out Span<byte> trampoline)
    {
        ref byte instruction = ref MemoryMarshal.GetReference(redirection);
        return TrampolineAllocator.TryAllocateNear(
            Unsafe.AsPointer(ref instruction),
            size,
            int.MaxValue - JmpRel32Size,
            out trampoline);
    }

    protected override void WriteCode(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        WriteCodeCore(destination, code);
    }

    protected override void WriteTrampoline(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        WriteCodeCore(destination, code);
    }

    static void WriteCodeCore(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        using var scope = CreateWriteScope(destination);
        code.CopyTo(destination);
        destination[code.Length] = Ret;
        scope.FlushInstructions();
    }

    protected override int RedirectionSize => JmpRel32Size;

    protected override void WriteRedirection(Span<byte> destination, Span<byte> trampoline)
    {
        nint offset = Unsafe.ByteOffset(
            ref MemoryMarshal.GetReference(destination),
            ref MemoryMarshal.GetReference(trampoline));
        int displacement = checked((int)(offset - JmpRel32Size));

        using var scope = CreateWriteScope(destination);
        destination[0] = JmpRel32;
        MemoryMarshal.Write(destination[1..], ref displacement);
        scope.FlushInstructions();
    }

    static MemoryProtectionScope CreateWriteScope(Span<byte> span)
    {
        return MemoryProtectionScope.Create(
            span,
            NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute);
    }

    protected override unsafe void GetMethodInstructions(
        MethodInfo method,
        out Span<byte> instructions,
        out Span<byte> entryPoint,
        out bool hasExactBoundaries)
    {
        hasExactBoundaries = false;
        byte* p = GetMethodCodePointer(method, false, out entryPoint);
        instructions = Unwind.GetMethodInstructions(p);
    }
}

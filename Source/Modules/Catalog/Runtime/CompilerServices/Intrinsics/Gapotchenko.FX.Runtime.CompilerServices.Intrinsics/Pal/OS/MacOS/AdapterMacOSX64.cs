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

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

#if NET
[SupportedOSPlatform("macos")]
#endif
sealed class AdapterMacOSX64 : AdapterX64
{
    protected override bool IsWriteAllowed(Span<byte> span)
    {
        return MemoryMap.IsWriteAllowed(span);
    }

    protected override bool TryAllocateTrampoline(
        Span<byte> redirection,
        int size,
        out Span<byte> trampoline)
    {
        trampoline = TrampolineAllocator.Allocate(size);
        return true;
    }

    protected override void WriteCode(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        using var scope = CreateWriteScope(destination);
        code.CopyTo(destination);
        destination[code.Length] = InstructionsX64.Ret;
        scope.FlushInstructions();
    }

    protected override void WriteTrampoline(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        using var scope = JitWriteProtectionScope.Create(destination);
        code.CopyTo(destination);
        destination[code.Length] = InstructionsX64.Ret;
        scope.FlushInstructions();
    }

    protected override int RedirectionSize => InstructionsX64.JmpAbs64Size;

    protected override unsafe void WriteRedirection(Span<byte> destination, Span<byte> trampoline)
    {
        using var scope = CreateWriteScope(destination);
        InstructionsX64.JmpAbs64.CopyTo(destination);
        nint address = (nint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(trampoline));
        MemoryMarshal.Write(destination[InstructionsX64.JmpAbs64.Length..], ref address);
        scope.FlushInstructions();
    }

    protected override int BodyRedirectionSize => InstructionsX64.JmpRel32Size;

    protected override void WriteBodyRedirection(Span<byte> destination, Span<byte> entryPoint)
    {
        nint offset = Unsafe.ByteOffset(
            ref MemoryMarshal.GetReference(destination),
            ref MemoryMarshal.GetReference(entryPoint));
        int displacement = checked((int)(offset - InstructionsX64.JmpRel32Size));

        using var scope = CreateWriteScope(destination);
        destination[0] = InstructionsX64.JmpRel32;
        MemoryMarshal.Write(destination[1..], ref displacement);
        scope.FlushInstructions();
    }

    static JitWriteProtectionScope CreateWriteScope(Span<byte> span)
    {
        return JitWriteProtectionScope.Create(span);
    }

    protected override unsafe void GetMethodInstructions(
        MethodInfo method,
        out Span<byte> instructions,
        out Span<byte> entryPoint,
        out bool hasExactBoundaries)
    {
        hasExactBoundaries = false;
        byte* p = GetMethodCodePointer(method, true, out entryPoint);
        instructions = Unwind.GetMethodInstructions(p);
    }
}

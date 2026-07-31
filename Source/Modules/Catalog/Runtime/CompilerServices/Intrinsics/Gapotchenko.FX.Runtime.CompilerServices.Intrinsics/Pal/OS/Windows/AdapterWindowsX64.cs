// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS9191 // The 'ref' modifier for an argument corresponding to 'in' parameter is equivalent to 'in'. Consider using 'in' instead.

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Intrinsic adapter for Windows OS and AMD-based 64-bit processor architecture.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
sealed class AdapterWindowsX64 : AdapterX64
{
    protected override unsafe void GetMethodInstructions(
        MethodInfo method,
        out Span<byte> instructions,
        out Span<byte> entryPoint,
        out bool hasExactBoundaries)
    {
        hasExactBoundaries = true;
        byte* p = GetMethodCodePointer(method, false, out entryPoint);

        // Get the exact method instruction boundaries.
        var runtimeFunction = (NativeMethods.RuntimeFunctionX64*)NativeMethods.RtlLookupFunctionEntry(p, out void* imageBase, null);
        if (runtimeFunction == null)
        {
            instructions = [];
            return;
        }

        byte* functionStart = (byte*)imageBase + runtimeFunction->BeginAddress;
        byte* functionEnd = (byte*)imageBase + runtimeFunction->EndAddress;
        if (p < functionStart || p >= functionEnd)
        {
            instructions = [];
            return;
        }

        nuint functionLength = (nuint)(functionEnd - p);
        if (functionLength > int.MaxValue)
        {
            instructions = [];
            return;
        }

        instructions = new(p, (int)functionLength);
    }

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

    protected override void WriteCode(Span<byte> destination, ReadOnlySpan<byte> code) =>
        WriteCodeCore(destination, code);

    protected override void WriteTrampoline(Span<byte> destination, ReadOnlySpan<byte> code) =>
        WriteCodeCore(destination, code);

    static void WriteCodeCore(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        using var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite);
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

        using var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite);
        destination[0] = JmpRel32;
        MemoryMarshal.Write(destination[1..], ref displacement);
        scope.FlushInstructions();
    }
}

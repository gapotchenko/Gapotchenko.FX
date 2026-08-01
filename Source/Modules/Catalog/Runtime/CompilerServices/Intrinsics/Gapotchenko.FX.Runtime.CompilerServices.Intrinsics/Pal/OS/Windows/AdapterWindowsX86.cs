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

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Intrinsic adapter for Windows OS and Intel-based 32-bit processor architecture.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
sealed class AdapterWindowsX86 : AdapterX86
{
    protected override unsafe void GetMethodInstructions(
        MethodInfo method,
        out Span<byte> instructions,
        out bool hasExactBoundaries)
    {
        hasExactBoundaries = false;
        // Compile the method.
        RuntimeHelpers.PrepareMethod(method.MethodHandle);

        // Get pointer to the first instruction.
        byte* p = (byte*)method.MethodHandle.GetFunctionPointer();
        p = SkipBranches(p);

        // It is impossible to determine the exact method instruction boundaries for x86 architecture.
        // Instead, use memory region information as a coarse approximation.
        if (NativeMethods.VirtualQuery(p, out var memoryInfo, (nuint)sizeof(NativeMethods.MemoryBasicInformation)) == 0 ||
            memoryInfo.State != NativeMethods.PageState.MemCommit ||
            (memoryInfo.Protect & (NativeMethods.PageProtect.NoAccess | NativeMethods.PageProtect.Guard)) != 0)
        {
            instructions = [];
            return;
        }

        byte* regionStart = (byte*)memoryInfo.BaseAddress;
        byte* regionEnd = regionStart + memoryInfo.RegionSize;
        if (p < regionStart || p >= regionEnd)
        {
            instructions = [];
            return;
        }

        nuint regionLength = (nuint)(regionEnd - p);
        if (regionLength > int.MaxValue)
        {
            instructions = [];
            return;
        }

        instructions = new(p, (int)regionLength);
    }

    protected override bool IsWriteAllowed(Span<byte> span) => true;

    protected override Span<byte> AllocateTrampoline(int size)
    {
        return TrampolineAllocator.Allocate(size);
    }

    protected override void WriteCode(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        using var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite);
        code.CopyTo(destination);
        destination[code.Length] = InstructionsX86.Ret;
        scope.FlushInstructions();
    }

    protected override void WriteRedirection(Span<byte> destination, Span<byte> trampoline)
    {
        nint offset = Unsafe.ByteOffset(
            ref MemoryMarshal.GetReference(destination),
            ref MemoryMarshal.GetReference(trampoline));
        int displacement = (int)(offset - InstructionsX86.JmpRel32Size);

        using var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite);
        destination[0] = InstructionsX86.JmpRel32;
        MemoryMarshal.Write(destination[1..], ref displacement);
        scope.FlushInstructions();
    }
}

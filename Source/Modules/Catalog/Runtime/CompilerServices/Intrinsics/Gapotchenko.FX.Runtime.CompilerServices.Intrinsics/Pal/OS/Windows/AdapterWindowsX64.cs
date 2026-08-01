// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Utils;
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
    protected override PatchResult ValidateCode(ReadOnlySpan<byte> code)
    {
        return UnwindX64.Analyze(code, out _) == UnwindX64.AnalysisResult.Unsupported ?
            PatchResult.UnsupportedUnwindPrologue :
            PatchResult.Success;
    }

    protected override bool RequiresTrampoline(ReadOnlySpan<byte> code)
    {
        return UnwindX64.Analyze(code, out _) == UnwindX64.AnalysisResult.Supported;
    }

    protected override int GetTrampolineAllocationSize(ReadOnlySpan<byte> code)
    {
        int codeSize = checked(code.Length + 1);
        var analysisResult = UnwindX64.Analyze(code, out int prologueSize);
        if (analysisResult == UnwindX64.AnalysisResult.Leaf)
            return codeSize;
        if (analysisResult != UnwindX64.AnalysisResult.Supported)
            throw new InvalidOperationException("The intrinsic has an unsupported Windows x64 unwind prologue.");

        int unwindInfoOffset = MemoryArithmetics.Align4(codeSize);
        int unwindInfoSize = UnwindX64.GetSize(code, prologueSize);
        return checked(unwindInfoOffset + unwindInfoSize + Unsafe.SizeOf<NativeMethods.RuntimeFunctionX64>());
    }

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
            int.MaxValue - InstructionsX64.JmpRel32Size,
            out trampoline);
    }

    protected override void WriteCode(Span<byte> destination, ReadOnlySpan<byte> code) =>
        WriteCodeCore(destination, code);

    protected override unsafe void WriteTrampoline(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        var analysisResult = UnwindX64.Analyze(code, out int prologueSize);
        if (analysisResult == UnwindX64.AnalysisResult.Leaf)
        {
            WriteCodeCore(destination, code);
            return;
        }
        if (analysisResult != UnwindX64.AnalysisResult.Supported)
            throw new InvalidOperationException("The intrinsic has an unsupported Windows x64 unwind prologue.");

        int codeSize = checked(code.Length + 1);
        int unwindInfoOffset = MemoryArithmetics.Align4(codeSize);
        int unwindInfoSize = UnwindX64.GetSize(code, prologueSize);
        int runtimeFunctionOffset = checked(unwindInfoOffset + unwindInfoSize);
        int allocationSize = checked(runtimeFunctionOffset + Unsafe.SizeOf<NativeMethods.RuntimeFunctionX64>());
        destination = destination[..allocationSize];

        ref byte baseAddress = ref MemoryMarshal.GetReference(destination);
        ref var runtimeFunction = ref Unsafe.As<byte, NativeMethods.RuntimeFunctionX64>(
            ref destination[runtimeFunctionOffset]);

        using (var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite))
        {
            code.CopyTo(destination);
            destination[code.Length] = InstructionsX64.Ret;
            UnwindX64.Write(destination[unwindInfoOffset..runtimeFunctionOffset], code, prologueSize);

            runtimeFunction.BeginAddress = 0;
            runtimeFunction.EndAddress = checked((uint)codeSize);
            runtimeFunction.UnwindData = checked((uint)unwindInfoOffset);
            scope.FlushInstructions();
        }

        if (!NativeMethods.RtlAddFunctionTable(
            (NativeMethods.RuntimeFunctionX64*)Unsafe.AsPointer(ref runtimeFunction),
            1,
            (nuint)Unsafe.AsPointer(ref baseAddress)))
        {
            throw new InvalidOperationException("Failed to register unwind information for an x64 trampoline.");
        }
    }

    static void WriteCodeCore(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        using var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite);
        code.CopyTo(destination);
        destination[code.Length] = InstructionsX64.Ret;
        scope.FlushInstructions();
    }

    protected override int RedirectionSize => InstructionsX64.JmpRel32Size;

    protected override void WriteRedirection(Span<byte> destination, Span<byte> trampoline)
    {
        nint offset = Unsafe.ByteOffset(
            ref MemoryMarshal.GetReference(destination),
            ref MemoryMarshal.GetReference(trampoline));
        int displacement = checked((int)(offset - InstructionsX64.JmpRel32Size));

        using var scope = VirtualProtectionScope.Create(destination, NativeMethods.PageProtect.ExecuteReadWrite);
        destination[0] = InstructionsX64.JmpRel32;
        MemoryMarshal.Write(destination[1..], ref displacement);
        scope.FlushInstructions();
    }
}

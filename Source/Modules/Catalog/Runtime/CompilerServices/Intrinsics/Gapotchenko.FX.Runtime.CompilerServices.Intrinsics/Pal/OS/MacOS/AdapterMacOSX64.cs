// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Pal.Formats.Dwarf;
using Gapotchenko.FX.Runtime.CompilerServices.Utils;
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
    protected override UnwindX64.AnalysisLevel UnwindAnalysisLevel => UnwindX64.AnalysisLevel.Epilogue;

    protected override UnwindX64.Analysis AnalyzeUnwind(
        ReadOnlySpan<byte> code,
        Span<UnwindX64.UnwindOperation> operations,
        Span<UnwindX64.EpilogueOperation> epilogueOperations)
    {
        return DwarfUnwindX64.Analyze(code, operations, epilogueOperations);
    }

    protected override PatchResult ValidateCode(in UnwindX64.Analysis unwindAnalysis)
    {
        return unwindAnalysis.Result == UnwindAnalysisResult.Unsupported ?
            PatchResult.UnsupportedUnwindPrologue :
            PatchResult.Success;
    }

    protected override bool RequiresTrampoline(in UnwindX64.Analysis unwindAnalysis)
    {
        return unwindAnalysis.Result == UnwindAnalysisResult.Supported;
    }

    protected override int GetTrampolineAllocationSize(ReadOnlySpan<byte> code, in UnwindX64.Analysis unwindAnalysis)
    {
        int codeSize = checked(code.Length + 1);
        if (unwindAnalysis.Result == UnwindAnalysisResult.Leaf)
            return codeSize;

        int unwindOffset = MemoryArithmetics.Align4(codeSize);
        return checked(unwindOffset + DwarfUnwindX64.GetSize(unwindAnalysis));
    }

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

    protected override unsafe void WriteTrampoline(
        Span<byte> destination,
        ReadOnlySpan<byte> code,
        in UnwindX64.Analysis unwindAnalysis)
    {
        if (unwindAnalysis.Result == UnwindAnalysisResult.Leaf)
        {
            using var leafScope = JitWriteProtectionScope.Create(destination);
            code.CopyTo(destination);
            destination[code.Length] = InstructionsX64.Ret;
            leafScope.FlushInstructions();
            return;
        }
        int codeSize = checked(code.Length + 1);
        int unwindOffset = MemoryArithmetics.Align4(codeSize);
        int unwindSize = DwarfUnwindX64.GetSize(unwindAnalysis);
        destination = destination[..checked(unwindOffset + unwindSize)];

        ref byte baseAddress = ref MemoryMarshal.GetReference(destination);
        var unwindDestination = destination.Slice(unwindOffset, unwindSize);
        using (var scope = JitWriteProtectionScope.Create(destination))
        {
            code.CopyTo(destination);
            destination[code.Length] = InstructionsX64.Ret;
            destination[codeSize..unwindOffset].Clear();
            DwarfUnwindX64.Write(
                unwindDestination,
                code,
                in unwindAnalysis,
                Unsafe.AsPointer(ref baseAddress));
            scope.FlushInstructions();
        }

        ref byte fde = ref unwindDestination[DwarfUnwindX64.FdeOffset];
        NativeMethods.__register_frame(Unsafe.AsPointer(ref fde));
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

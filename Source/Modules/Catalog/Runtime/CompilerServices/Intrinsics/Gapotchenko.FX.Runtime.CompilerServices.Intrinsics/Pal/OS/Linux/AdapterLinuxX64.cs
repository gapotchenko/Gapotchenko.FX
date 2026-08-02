// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.x64;
using Gapotchenko.FX.Runtime.CompilerServices.Pal.Formats.Dwarf;
using Gapotchenko.FX.Runtime.CompilerServices.Utils;
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
    protected override UnwindX64.AnalysisLevel UnwindAnalysisLevel => UnwindX64.AnalysisLevel.Epilogue;

    protected override UnwindX64.Analysis AnalyzeUnwind(
        ReadOnlySpan<byte> code,
        Span<UnwindX64.UnwindOperation> operations,
        Span<UnwindX64.EpilogueOperation> epilogue)
    {
        return DwarfUnwindX64.Analyze(code, operations, epilogue);
    }

    protected override PatchResult ValidateCode(in UnwindX64.Analysis unwindAnalysis)
    {
        return unwindAnalysis.Result == UnwindAnalysisResult.Unsupported ?
            PatchResult.UnsupportedUnwindPrologue :
            PatchResult.Success;
    }

    protected override int GetTrampolineAllocationSize(ReadOnlySpan<byte> code, in UnwindX64.Analysis unwindAnalysis)
    {
        int codeSize = checked(code.Length + 1);
        int unwindOffset = MemoryArithmetics.Align4(codeSize);
        return checked(unwindOffset + DwarfUnwindX64.GetSize(unwindAnalysis) + sizeof(uint));
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

    protected override void WriteCode(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        WriteCodeCore(destination, code);
    }

    protected override unsafe void WriteTrampoline(
        Span<byte> destination,
        ReadOnlySpan<byte> code,
        in UnwindX64.Analysis unwindAnalysis)
    {
        int codeSize = checked(code.Length + 1);
        int unwindOffset = MemoryArithmetics.Align4(codeSize);
        int unwindSize = DwarfUnwindX64.GetSize(unwindAnalysis);
        destination = destination[..checked(unwindOffset + unwindSize + sizeof(uint))];

        ref byte baseAddress = ref MemoryMarshal.GetReference(destination);
        var unwindDestination = destination.Slice(unwindOffset, unwindSize);
        using (var scope = CreateWriteScope(destination))
        {
            code.CopyTo(destination);
            destination[code.Length] = InstructionsX64.Ret;
            destination[codeSize..unwindOffset].Clear();
            DwarfUnwindX64.Write(
                unwindDestination,
                code,
                in unwindAnalysis,
                Unsafe.AsPointer(ref baseAddress));
            destination[(unwindOffset + unwindSize)..].Clear();
            scope.FlushInstructions();
        }

        NativeMethods.RegisterFrame(Unsafe.AsPointer(ref MemoryMarshal.GetReference(unwindDestination)));
    }

    static void WriteCodeCore(Span<byte> destination, ReadOnlySpan<byte> code)
    {
        using var scope = CreateWriteScope(destination);
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

        using var scope = CreateWriteScope(destination);
        destination[0] = InstructionsX64.JmpRel32;
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

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Formats.Dwarf;

/// <summary>
/// Encodes DWARF unwind information for x86 intrinsic trampolines.
/// </summary>
static class DwarfUnwindX86
{
    public static UnwindX86.Analysis Analyze(
        ReadOnlySpan<byte> code,
        Span<UnwindX86.UnwindOperation> operations,
        Span<UnwindX86.EpilogueOperation> epilogue)
    {
        var analysis = UnwindX86.Analyze(code, operations);
        if (analysis.Result != UnwindAnalysisResult.Supported)
            return analysis;

        var analyzedOperations = analysis.Operations;
        int cfaOffset = InitialCfaOffset;
        foreach (ref readonly var operation in analyzedOperations)
        {
            switch (operation.Kind)
            {
                case UnwindX86.UnwindOperationKind.PushNonvolatile:
                    cfaOffset = checked(cfaOffset + StackSlotSize);
                    break;
                case UnwindX86.UnwindOperationKind.StackAllocation:
                    cfaOffset = checked(cfaOffset + (int)operation.Value);
                    break;
            }
        }

        int epilogueOperationCount = 0;
        int end = code.Length;
        // Decode backwards from the end of the method. Stack operations are
        // undone in reverse execution order, so they match prologue operations
        // in their original order here.
        for (int i = 0; i < analyzedOperations.Length; ++i)
        {
            ref readonly var operation = ref analyzedOperations[i];
            int start;
            switch (operation.Kind)
            {
                case UnwindX86.UnwindOperationKind.SetFramePointer:
                    continue;
                case UnwindX86.UnwindOperationKind.StackAllocation:
                    if (!UnwindX86.TryDecodeStackDeallocationEndingAt(code, end, operation.Value, out start))
                        return analysis with { Result = UnwindAnalysisResult.Unsupported };
                    break;
                case UnwindX86.UnwindOperationKind.PushNonvolatile:
                    if (!UnwindX86.TryDecodePopNonvolatileEndingAt(code, end, operation.Register, out start))
                        return analysis with { Result = UnwindAnalysisResult.Unsupported };
                    break;
                default:
                    return analysis with { Result = UnwindAnalysisResult.Unsupported };
            }
            if (start < analysis.Info.PrologueSize || epilogueOperationCount >= epilogue.Length)
                return analysis with { Result = UnwindAnalysisResult.Unsupported };
            epilogue[epilogueOperationCount++] = new(start, end, i);
            end = start;
        }

        return analysis with { Epilogue = epilogue[..epilogueOperationCount] };
    }

    public static int GetSize(in UnwindX86.Analysis analysis)
    {
        var serializer = new DwarfSerializer();
        WriteInstructions(ref serializer, analysis.Operations, analysis.Epilogue);
        return DwarfSerializer.GetFdeSize(CieSize, serializer.Position);
    }

    public static unsafe void Write(
        Span<byte> destination,
        ReadOnlySpan<byte> code,
        in UnwindX86.Analysis analysis,
        void* codeAddress)
    {
        int size = GetSize(analysis);
        destination = destination[..size];
        byte* unwindAddress = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(destination));
        Cie.CopyTo(destination);
        var serializer = new DwarfSerializer(destination[CieSize..]);
        int fdeSize = size - CieSize;
        serializer.WriteFdeHeader(CieSize, unwindAddress, codeAddress, checked(code.Length + 1), fdeSize);
        WriteInstructions(ref serializer, analysis.Operations, analysis.Epilogue);
        serializer.CompleteFde(fdeSize);
    }

    static void WriteInstructions(
        ref DwarfSerializer serializer,
        ReadOnlySpan<UnwindX86.UnwindOperation> operations,
        ReadOnlySpan<UnwindX86.EpilogueOperation> epilogue)
    {
        int location = 0;
        int espCfaOffset = InitialCfaOffset;
        int cfaRegister = InstructionsX86.RegisterSp;
        foreach (ref readonly var operation in operations)
        {
            serializer.AdvanceLocation(operation.CodeOffset - location);
            location = operation.CodeOffset;
            switch (operation.Kind)
            {
                case UnwindX86.UnwindOperationKind.PushNonvolatile:
                    espCfaOffset = checked(espCfaOffset + StackSlotSize);
                    if (cfaRegister == InstructionsX86.RegisterSp)
                        serializer.DefCfaOffset(espCfaOffset);
                    serializer.RegisterOffset(operation.Register, checked((uint)(espCfaOffset / StackSlotSize)));
                    break;
                case UnwindX86.UnwindOperationKind.StackAllocation:
                    espCfaOffset = checked(espCfaOffset + (int)operation.Value);
                    if (cfaRegister == InstructionsX86.RegisterSp)
                        serializer.DefCfaOffset(espCfaOffset);
                    break;
                case UnwindX86.UnwindOperationKind.SetFramePointer:
                    cfaRegister = InstructionsX86.RegisterBp;
                    serializer.DefCfa((uint)cfaRegister, checked((uint)espCfaOffset));
                    break;
            }
        }

        int nextOperationIndex = operations.Length - 1;
        for (int i = epilogue.Length - 1; i >= 0; --i)
        {
            ref readonly var item = ref epilogue[i];
            bool leavesFramePointer = false;
            for (int j = nextOperationIndex; j > item.OperationIndex; --j)
                leavesFramePointer |= operations[j].Kind == UnwindX86.UnwindOperationKind.SetFramePointer;
            if (leavesFramePointer)
            {
                serializer.AdvanceLocation(item.Start - location);
                location = item.Start;
                cfaRegister = InstructionsX86.RegisterSp;
                serializer.DefCfa((uint)cfaRegister, checked((uint)espCfaOffset));
            }

            serializer.AdvanceLocation(item.End - location);
            location = item.End;
            ref readonly var operation = ref operations[item.OperationIndex];
            switch (operation.Kind)
            {
                case UnwindX86.UnwindOperationKind.StackAllocation:
                    espCfaOffset = checked(espCfaOffset - (int)operation.Value);
                    if (cfaRegister == InstructionsX86.RegisterSp)
                        serializer.DefCfaOffset(espCfaOffset);
                    break;
                case UnwindX86.UnwindOperationKind.PushNonvolatile:
                    serializer.SameValue(operation.Register);
                    espCfaOffset -= StackSlotSize;
                    if (cfaRegister == InstructionsX86.RegisterSp)
                        serializer.DefCfaOffset(espCfaOffset);
                    break;
            }
            nextOperationIndex = item.OperationIndex - 1;
        }
    }

    static ReadOnlySpan<byte> Cie =>
    [
        0x14, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00,
        0x01,
        0x7a, 0x52, 0x00,
        0x01,
        0x7c,
        0x08,
        0x01, 0x1b,
        0x0c, 0x04, 0x04,
        0x88, 0x01,
        0x00, 0x00
    ];

    const int CieSize = 24;
    const int InitialCfaOffset = 4;
    const int StackSlotSize = 4;
}

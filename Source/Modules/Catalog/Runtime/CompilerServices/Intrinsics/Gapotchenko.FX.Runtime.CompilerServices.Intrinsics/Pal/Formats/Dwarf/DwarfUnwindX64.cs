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
/// Encodes DWARF unwind information for x64 intrinsic trampolines.
/// </summary>
static class DwarfUnwindX64
{
    public static UnwindX64.Analysis Analyze(
        ReadOnlySpan<byte> code,
        Span<UnwindX64.UnwindOperation> unwindOperations,
        Span<UnwindX64.EpilogueOperation> epilogueOperations)
    {
        var analysis = UnwindX64.Analyze(code, unwindOperations);
        if (analysis.Result != UnwindAnalysisResult.Supported)
            return analysis;

        var analyzedUnwindOperations = analysis.UnwindOperations;
        int cfaOffset = InitialCfaOffset;
        foreach (ref readonly var unwindOperation in analyzedUnwindOperations)
        {
            switch (unwindOperation.Kind)
            {
                case UnwindX64.UnwindOperationKind.PushNonvolatile:
                    cfaOffset = checked(cfaOffset + StackSlotSize);
                    break;

                case UnwindX64.UnwindOperationKind.StackAllocation:
                    cfaOffset = checked(cfaOffset + (int)unwindOperation.Value);
                    break;

                case UnwindX64.UnwindOperationKind.SetFramePointer:
                    if (unwindOperation.Value > cfaOffset)
                        return analysis with { Result = UnwindAnalysisResult.Unsupported };
                    break;
            }
        }

        int epilogueOperationCount = 0;
        int end = code.Length;
        for (int i = analyzedUnwindOperations.Length - 1; i >= 0; --i)
        {
            ref readonly var operation = ref analyzedUnwindOperations[i];
            int start;
            switch (operation.Kind)
            {
                case UnwindX64.UnwindOperationKind.SetFramePointer:
                    continue;

                case UnwindX64.UnwindOperationKind.StackAllocation:
                    if (!UnwindX64.TryDecodeStackDeallocationEndingAt(code, end, operation.Value, out start))
                        return analysis with { Result = UnwindAnalysisResult.Unsupported };
                    break;

                case UnwindX64.UnwindOperationKind.PushNonvolatile:
                    if (!UnwindX64.TryDecodePopNonvolatileEndingAt(code, end, operation.Register, out start))
                        return analysis with { Result = UnwindAnalysisResult.Unsupported };
                    break;

                default:
                    return analysis with { Result = UnwindAnalysisResult.Unsupported };
            }

            if (start < analysis.Info.PrologueSize || epilogueOperationCount >= epilogueOperations.Length)
                return analysis with { Result = UnwindAnalysisResult.Unsupported };
            epilogueOperations[epilogueOperationCount++] = new(start, end, i);
            end = start;
        }

        return analysis with { EpilogueOperations = epilogueOperations[..epilogueOperationCount] };
    }

    public static int GetSize(in UnwindX64.Analysis analysis)
    {
        var serializer = new DwarfSerializer();
        WriteInstructions(
            ref serializer,
            analysis.UnwindOperations,
            analysis.EpilogueOperations);
        return DwarfSerializer.GetFdeSize(CieSize, serializer.Position);
    }

    public static unsafe void Write(
        Span<byte> destination,
        ReadOnlySpan<byte> code,
        scoped ref readonly UnwindX64.Analysis analysis,
        void* codeAddress)
    {
        int size = GetSize(in analysis);
        destination = destination[..size];
        byte* unwindAddress = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(destination));
        Cie.CopyTo(destination);

        var serializer = new DwarfSerializer(destination[CieSize..]);
        int fdeSize = size - CieSize;
        serializer.WriteFdeHeader(
            CieSize,
            unwindAddress,
            codeAddress,
            checked(code.Length + 1),
            fdeSize);
        WriteInstructions(
            ref serializer,
            analysis.UnwindOperations,
            analysis.EpilogueOperations);
        serializer.CompleteFde(fdeSize);
    }

    static void WriteInstructions(
        scoped ref DwarfSerializer serializer,
        scoped ReadOnlySpan<UnwindX64.UnwindOperation> operations,
        scoped ReadOnlySpan<UnwindX64.EpilogueOperation> epilogue)
    {
        int location = 0;
        int rspCfaOffset = InitialCfaOffset;
        int cfaRegister = InstructionsX64.RegisterSp;

        foreach (ref readonly var operation in operations)
        {
            serializer.AdvanceLocation(operation.CodeOffset - location);
            location = operation.CodeOffset;

            switch (operation.Kind)
            {
                case UnwindX64.UnwindOperationKind.PushNonvolatile:
                    rspCfaOffset = checked(rspCfaOffset + StackSlotSize);
                    if (cfaRegister == InstructionsX64.RegisterSp)
                        serializer.DefCfaOffset(rspCfaOffset);
                    serializer.RegisterOffset(
                        GetDwarfRegister(operation.Register),
                        checked((uint)(rspCfaOffset / StackSlotSize)));
                    break;

                case UnwindX64.UnwindOperationKind.StackAllocation:
                    rspCfaOffset = checked(rspCfaOffset + (int)operation.Value);
                    if (cfaRegister == InstructionsX64.RegisterSp)
                        serializer.DefCfaOffset(rspCfaOffset);
                    break;

                case UnwindX64.UnwindOperationKind.SetFramePointer:
                    cfaRegister = InstructionsX64.RegisterBp;
                    serializer.DefCfa(
                        GetDwarfRegister(cfaRegister),
                        checked((uint)(rspCfaOffset - operation.Value)));
                    break;

                default:
                    throw new InvalidOperationException("Unknown x64 unwind operation.");
            }
        }

        int nextOperationIndex = operations.Length - 1;
        for (int i = epilogue.Length - 1; i >= 0; --i)
        {
            ref readonly var item = ref epilogue[i];
            bool leavesFramePointer = false;
            for (int j = nextOperationIndex; j > item.OperationIndex; --j)
            {
                if (operations[j].Kind == UnwindX64.UnwindOperationKind.SetFramePointer)
                    leavesFramePointer = true;
            }

            if (leavesFramePointer)
            {
                serializer.AdvanceLocation(item.Start - location);
                location = item.Start;
                cfaRegister = InstructionsX64.RegisterSp;
                serializer.DefCfa(GetDwarfRegister(cfaRegister), checked((uint)rspCfaOffset));
            }

            serializer.AdvanceLocation(item.End - location);
            location = item.End;
            ref readonly var operation = ref operations[item.OperationIndex];
            switch (operation.Kind)
            {
                case UnwindX64.UnwindOperationKind.StackAllocation:
                    rspCfaOffset = checked(rspCfaOffset - (int)operation.Value);
                    if (cfaRegister == InstructionsX64.RegisterSp)
                        serializer.DefCfaOffset(rspCfaOffset);
                    break;

                case UnwindX64.UnwindOperationKind.PushNonvolatile:
                    serializer.SameValue(GetDwarfRegister(operation.Register));
                    rspCfaOffset = checked(rspCfaOffset - StackSlotSize);
                    if (cfaRegister == InstructionsX64.RegisterSp)
                        serializer.DefCfaOffset(rspCfaOffset);
                    break;
            }

            nextOperationIndex = item.OperationIndex - 1;
        }
    }

    static uint GetDwarfRegister(int register) =>
        register switch
        {
            0 => 0,  // RAX
            1 => 2,  // RCX
            2 => 1,  // RDX
            3 => 3,  // RBX
            4 => 7,  // RSP
            5 => 6,  // RBP
            6 => 4,  // RSI
            7 => 5,  // RDI
            >= 8 and <= 15 => (uint)register,
            _ => throw new ArgumentOutOfRangeException(nameof(register))
        };

    public const int FdeOffset = CieSize;

    // A version 1, zR-augmented x64 CIE. The incoming CFA is RSP+8 and
    // the return address is stored at CFA-8. FDE addresses use pcrel|sdata4.
    static ReadOnlySpan<byte> Cie =>
    [
        0x14, 0x00, 0x00, 0x00, // Length
        0x00, 0x00, 0x00, 0x00, // CIE identifier
        0x01,                   // Version
        0x7a, 0x52, 0x00,       // "zR"
        0x01,                   // Code alignment factor
        0x78,                   // Data alignment factor (-8)
        0x10,                   // Return-address register (RIP)
        0x01, 0x1b,             // Augmentation length; pcrel | sdata4
        0x0c, 0x07, 0x08,       // DW_CFA_def_cfa RSP, 8
        0x90, 0x01,             // DW_CFA_offset RIP, 1
        0x00, 0x00              // Alignment padding
    ];

    const int CieSize = 24;
    const int InitialCfaOffset = 8;
    const int StackSlotSize = 8;
}

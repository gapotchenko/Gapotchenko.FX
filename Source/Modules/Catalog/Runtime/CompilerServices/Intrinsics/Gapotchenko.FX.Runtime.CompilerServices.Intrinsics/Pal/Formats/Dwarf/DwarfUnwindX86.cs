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
    public static UnwindAnalysisResult Analyze(ReadOnlySpan<byte> code, out UnwindX86.UnwindInfo info)
    {
        Span<UnwindX86.UnwindOperation> operations = stackalloc UnwindX86.UnwindOperation[UnwindX86.MaximumOperationCount];
        Span<EpilogueOperation> epilogue = stackalloc EpilogueOperation[UnwindX86.MaximumOperationCount];
        return Analyze(code, operations, out _, epilogue, out _, out info);
    }

    public static int GetSize(ReadOnlySpan<byte> code, UnwindX86.UnwindInfo expectedInfo)
    {
        Span<UnwindX86.UnwindOperation> operations = stackalloc UnwindX86.UnwindOperation[UnwindX86.MaximumOperationCount];
        Span<EpilogueOperation> epilogue = stackalloc EpilogueOperation[UnwindX86.MaximumOperationCount];
        var result = Analyze(code, operations, out int operationCount, epilogue, out int epilogueCount, out var info);
        if (result != UnwindAnalysisResult.Supported || info != expectedInfo)
            throw new ArgumentException("The code does not have a supported x86 DWARF unwind prologue.", nameof(code));

        var serializer = new DwarfSerializer();
        WriteInstructions(ref serializer, operations[..operationCount], epilogue[..epilogueCount]);
        return DwarfSerializer.GetFdeSize(CieSize, serializer.Position);
    }

    public static unsafe void Write(Span<byte> destination, ReadOnlySpan<byte> code,
        UnwindX86.UnwindInfo expectedInfo, void* codeAddress)
    {
        Span<UnwindX86.UnwindOperation> operations = stackalloc UnwindX86.UnwindOperation[UnwindX86.MaximumOperationCount];
        Span<EpilogueOperation> epilogue = stackalloc EpilogueOperation[UnwindX86.MaximumOperationCount];
        var result = Analyze(code, operations, out int operationCount, epilogue, out int epilogueCount, out var info);
        if (result != UnwindAnalysisResult.Supported || info != expectedInfo)
            throw new ArgumentException("The code does not have a supported x86 DWARF unwind prologue.", nameof(code));

        int size = GetSize(code, info);
        destination = destination[..size];
        byte* unwindAddress = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(destination));
        Cie.CopyTo(destination);
        var serializer = new DwarfSerializer(destination[CieSize..]);
        int fdeSize = size - CieSize;
        serializer.WriteFdeHeader(CieSize, unwindAddress, codeAddress, checked(code.Length + 1), fdeSize);
        WriteInstructions(ref serializer, operations[..operationCount], epilogue[..epilogueCount]);
        serializer.CompleteFde(fdeSize);
    }

    static UnwindAnalysisResult Analyze(ReadOnlySpan<byte> code,
        scoped Span<UnwindX86.UnwindOperation> operations, out int operationCount,
        scoped Span<EpilogueOperation> epilogue, out int epilogueCount,
        out UnwindX86.UnwindInfo info)
    {
        epilogueCount = 0;
        var result = UnwindX86.Analyze(code, operations, out operationCount, out info);
        if (result != UnwindAnalysisResult.Supported)
            return result;

        operations = operations[..operationCount];
        int cfaOffset = InitialCfaOffset;
        foreach (ref readonly var operation in operations)
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

        int end = code.Length;
        // Decode backwards from the end of the method. Stack operations are
        // undone in reverse execution order, so they match prologue operations
        // in their original order here.
        for (int i = 0; i < operations.Length; ++i)
        {
            ref readonly var operation = ref operations[i];
            int start;
            switch (operation.Kind)
            {
                case UnwindX86.UnwindOperationKind.SetFramePointer:
                    continue;
                case UnwindX86.UnwindOperationKind.StackAllocation:
                    if (!UnwindX86.TryDecodeStackDeallocationEndingAt(code, end, operation.Value, out start))
                        return UnwindAnalysisResult.Unsupported;
                    break;
                case UnwindX86.UnwindOperationKind.PushNonvolatile:
                    if (!UnwindX86.TryDecodePopNonvolatileEndingAt(code, end, operation.Register, out start))
                        return UnwindAnalysisResult.Unsupported;
                    break;
                default:
                    return UnwindAnalysisResult.Unsupported;
            }
            if (start < info.PrologueSize || epilogueCount >= epilogue.Length)
                return UnwindAnalysisResult.Unsupported;
            epilogue[epilogueCount++] = new(start, end, i);
            end = start;
        }
        return UnwindAnalysisResult.Supported;
    }

    static void WriteInstructions(scoped ref DwarfSerializer serializer,
        scoped ReadOnlySpan<UnwindX86.UnwindOperation> operations,
        scoped ReadOnlySpan<EpilogueOperation> epilogue)
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

    readonly record struct EpilogueOperation(int Start, int End, int OperationIndex);

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

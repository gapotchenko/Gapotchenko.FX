// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Buffers.Binary;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

/// <summary>
/// Analyzes x86 unwind prologues.
/// </summary>
static class UnwindX86
{
    public static UnwindAnalysisResult Analyze(ReadOnlySpan<byte> code, out UnwindInfo info)
    {
        Span<UnwindOperation> operations = stackalloc UnwindOperation[MaximumOperationCount];
        return Analyze(code, operations, out _, out info);
    }

    public static UnwindAnalysisResult Analyze(
        ReadOnlySpan<byte> code,
        Span<UnwindOperation> operations,
        out int operationCount,
        out UnwindInfo info)
    {
        operationCount = 0;
        int prologueSize = 0;
        int frameRegister = 0;
        bool ebpSaved = false;

        while (prologueSize < code.Length && prologueSize < byte.MaxValue)
        {
            var remaining = code[prologueSize..];
            if (TryDecodePushNonvolatile(remaining, out int instructionSize, out int register))
            {
                if (!TryAddOperation(
                    operations, ref operationCount, ref prologueSize,
                    instructionSize, UnwindOperationKind.PushNonvolatile, register, 0))
                {
                    return Unsupported(out info);
                }

                if (register == InstructionsX86.RegisterBp)
                    ebpSaved = true;
                continue;
            }

            if (TryDecodeStackAllocation(remaining, out instructionSize, out uint allocationSize))
            {
                if (allocationSize == 0 ||
                    (allocationSize & 3) != 0 ||
                    !TryAddOperation(
                        operations, ref operationCount, ref prologueSize,
                        instructionSize, UnwindOperationKind.StackAllocation, 0, allocationSize))
                {
                    return Unsupported(out info);
                }

                continue;
            }

            if (TryDecodeFrameRegister(remaining, out instructionSize))
            {
                if (!ebpSaved ||
                    frameRegister != 0 ||
                    !TryAddOperation(
                        operations, ref operationCount, ref prologueSize,
                        instructionSize, UnwindOperationKind.SetFramePointer, InstructionsX86.RegisterBp, 0))
                {
                    return Unsupported(out info);
                }

                frameRegister = InstructionsX86.RegisterBp;
                continue;
            }

            if (IsPotentialPrologueInstruction(remaining))
                return Unsupported(out info);

            break;
        }

        info = new(prologueSize, frameRegister);
        return operationCount == 0 ? UnwindAnalysisResult.Leaf : UnwindAnalysisResult.Supported;
    }

    static UnwindAnalysisResult Unsupported(out UnwindInfo info)
    {
        info = default;
        return UnwindAnalysisResult.Unsupported;
    }

    public static bool TryDecodeStackDeallocationEndingAt(ReadOnlySpan<byte> code, int end, uint size, out int start)
    {
        if (end >= 3 &&
            code[end - 3] == InstructionsX86.Group1Immediate8 &&
            code[end - 2] == InstructionsX86.ModRmAddEsp && code[end - 1] == size)
        {
            start = end - 3;
            return true;
        }
        if (end >= 6 &&
            code[end - 6] == InstructionsX86.Group1Immediate32 &&
            code[end - 5] == InstructionsX86.ModRmAddEsp && BinaryPrimitives.ReadUInt32LittleEndian(code[(end - 4)..]) == size)
        {
            start = end - 6;
            return true;
        }
        start = 0;
        return false;
    }

    public static bool TryDecodePopNonvolatileEndingAt(ReadOnlySpan<byte> code, int end, int register, out int start)
    {
        if (end >= 1 && code[end - 1] == InstructionsX86.PopRegister + register)
        {
            start = end - 1;
            return true;
        }
        start = 0;
        return false;
    }

    static bool TryDecodePushNonvolatile(ReadOnlySpan<byte> code, out int size, out int register)
    {
        size = 0;
        register = 0;
        if (code.IsEmpty)
            return false;
        byte operation = code[0];
        if (operation is >= InstructionsX86.PushRegister + 3 and <= InstructionsX86.PushRegisterLast &&
            operation != InstructionsX86.PushRegister + InstructionsX86.RegisterSp)
        {
            size = 1;
            register = operation - InstructionsX86.PushRegister;
            return true;
        }
        return false;
    }

    static bool TryDecodeStackAllocation(ReadOnlySpan<byte> code, out int size, out uint allocationSize)
    {
        size = 0;
        allocationSize = 0;
        if (code.Length >= 3 && code[0] == InstructionsX86.Group1Immediate8 && code[1] == InstructionsX86.ModRmSubEsp)
        {
            size = 3;
            allocationSize = code[2] <= sbyte.MaxValue ? code[2] : 0u;
            return true;
        }
        if (code.Length >= 6 && code[0] == InstructionsX86.Group1Immediate32 && code[1] == InstructionsX86.ModRmSubEsp)
        {
            size = 6;
            allocationSize = BinaryPrimitives.ReadUInt32LittleEndian(code[2..]);
            if (allocationSize > int.MaxValue)
                allocationSize = 0;
            return true;
        }
        return false;
    }

    static bool TryDecodeFrameRegister(ReadOnlySpan<byte> code, out int size)
    {
        size = 0;
        if (code.Length >= 2 &&
            (code[0] == InstructionsX86.MovRegisterRm && code[1] == InstructionsX86.ModRmMovEbpEsp ||
             code[0] == InstructionsX86.MovRmRegister && code[1] == InstructionsX86.ModRmMovEspEbp))
        {
            size = 2;
            return true;
        }
        return false;
    }

    static bool IsPotentialPrologueInstruction(ReadOnlySpan<byte> code)
    {
        if (code.IsEmpty)
            return false;

        byte operation = code[0];
        if (operation is >= InstructionsX86.PushRegister and <= InstructionsX86.PushRegisterLast or
            InstructionsX86.PushImmediate32 or InstructionsX86.PushImmediate8 or
            InstructionsX86.PushFlags or InstructionsX86.Enter)
        {
            return true;
        }

        if (code.Length >= 2 && operation is InstructionsX86.Group1Immediate32 or InstructionsX86.Group1Immediate8 &&
            code[1] is InstructionsX86.ModRmAddEsp or InstructionsX86.ModRmAndEsp or InstructionsX86.ModRmSubEsp)
        {
            return true;
        }

        if (code.Length >= 2 && operation is InstructionsX86.MovRmRegister or InstructionsX86.MovRegisterRm or InstructionsX86.Lea)
        {
            int destination = operation == InstructionsX86.MovRmRegister ? code[1] & 7 : code[1] >> 3 & 7;
            if (destination is InstructionsX86.RegisterSp or InstructionsX86.RegisterBp)
                return true;
        }

        return false;
    }

    static bool TryAddOperation(Span<UnwindOperation> operations, ref int count, ref int offset,
        int size, UnwindOperationKind kind, int register, uint value)
    {
        int end = offset + size;
        if (end > byte.MaxValue || count >= operations.Length)
            return false;
        operations[count++] = new(checked((byte)end), kind, checked((byte)register), value);
        offset = end;
        return true;
    }

    public readonly record struct UnwindInfo(int PrologueSize, int FrameRegister);
    public readonly record struct UnwindOperation(byte CodeOffset, UnwindOperationKind Kind, byte Register, uint Value);
    public enum UnwindOperationKind { PushNonvolatile, StackAllocation, SetFramePointer }
    public const int MaximumOperationCount = byte.MaxValue;
}

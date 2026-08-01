// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Buffers.Binary;
using Instructions = Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.InstructionsX64;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

/// <summary>
/// Analyzes x64 unwind prologues.
/// </summary>
static class UnwindX64
{
    public static Analysis Analyze(
        ReadOnlySpan<byte> code,
        Span<UnwindOperation> operations)
    {
        var result = Analyze(code, operations, out int operationCount, out var info);
        return new(result, info, operations[..operationCount]);
    }

    static UnwindAnalysisResult Analyze(
        ReadOnlySpan<byte> code,
        Span<UnwindOperation> operations,
        out int operationCount,
        out UnwindInfo info)
    {
        operationCount = 0;
        int prologueSize = 0;
        int frameRegister = 0;
        int frameOffset = 0;
        bool rbpSaved = false;

        while (prologueSize < code.Length && prologueSize < byte.MaxValue)
        {
            var remainingCode = code[prologueSize..];
            if (TryDecodePushNonvolatile(remainingCode, out int instructionSize, out int register))
            {
                if (!TryAddOperation(
                    operations,
                    ref operationCount,
                    ref prologueSize,
                    instructionSize,
                    UnwindOperationKind.PushNonvolatile,
                    register,
                    0))
                {
                    info = default;
                    return UnwindAnalysisResult.Unsupported;
                }
                if (register == Instructions.RegisterBp)
                    rbpSaved = true;
                continue;
            }

            if (TryDecodeStackAllocation(remainingCode, out instructionSize, out uint allocationSize))
            {
                if (allocationSize == 0 || (allocationSize & 7) != 0 ||
                    !TryAddOperation(
                        operations,
                        ref operationCount,
                        ref prologueSize,
                        instructionSize,
                        UnwindOperationKind.StackAllocation,
                        0,
                        allocationSize))
                {
                    info = default;
                    return UnwindAnalysisResult.Unsupported;
                }
                continue;
            }

            if (TryDecodeFrameRegister(remainingCode, out instructionSize, out int offset))
            {
                if (!rbpSaved || frameRegister != 0 ||
                    !TryAddOperation(
                        operations,
                        ref operationCount,
                        ref prologueSize,
                        instructionSize,
                        UnwindOperationKind.SetFramePointer,
                        Instructions.RegisterBp,
                        checked((uint)offset)))
                {
                    info = default;
                    return UnwindAnalysisResult.Unsupported;
                }

                frameRegister = Instructions.RegisterBp;
                frameOffset = offset;
                continue;
            }

            if (IsPotentialPrologueInstruction(remainingCode))
            {
                info = default;
                return UnwindAnalysisResult.Unsupported;
            }

            break;
        }

        info = new(prologueSize, frameRegister, frameOffset);
        return operationCount == 0 ? UnwindAnalysisResult.Leaf : UnwindAnalysisResult.Supported;
    }

    public static bool TryDecodeStackDeallocationEndingAt(
        ReadOnlySpan<byte> code,
        int end,
        uint allocationSize,
        out int start)
    {
        if (end >= 4 &&
            code[end - 4] == Instructions.RexW &&
            code[end - 3] == Instructions.Group1Immediate8 &&
            code[end - 2] == Instructions.ModRmAddRsp &&
            code[end - 1] == allocationSize)
        {
            start = end - 4;
            return true;
        }

        if (end >= 7 &&
            code[end - 7] == Instructions.RexW &&
            code[end - 6] == Instructions.Group1Immediate32 &&
            code[end - 5] == Instructions.ModRmAddRsp &&
            BinaryPrimitives.ReadUInt32LittleEndian(code[(end - 4)..]) == allocationSize)
        {
            start = end - 7;
            return true;
        }

        start = 0;
        return false;
    }

    public static bool TryDecodePopNonvolatileEndingAt(
        ReadOnlySpan<byte> code,
        int end,
        int register,
        out int start)
    {
        if (register < Instructions.ExtendedRegisterOffset)
        {
            if (end >= 1 && code[end - 1] == Instructions.PopRegister + register)
            {
                start = end - 1;
                return true;
            }
        }
        else if (end >= 2 &&
            code[end - 2] == Instructions.RexB &&
            code[end - 1] == Instructions.PopRegister + register - Instructions.ExtendedRegisterOffset)
        {
            start = end - 2;
            return true;
        }

        start = 0;
        return false;
    }

    static bool TryAddOperation(
        Span<UnwindOperation> operations,
        ref int operationCount,
        ref int prologueSize,
        int instructionSize,
        UnwindOperationKind kind,
        int register,
        uint value)
    {
        int codeOffset = prologueSize + instructionSize;
        if (codeOffset > byte.MaxValue || operationCount >= operations.Length)
            return false;

        operations[operationCount++] = new(
            checked((byte)codeOffset),
            kind,
            checked((byte)register),
            value);
        prologueSize = codeOffset;
        return true;
    }

    static bool TryDecodePushNonvolatile(ReadOnlySpan<byte> code, out int instructionSize, out int register)
    {
        instructionSize = 0;
        register = 0;
        if (code.IsEmpty)
            return false;

        byte operation = code[0];
        if (operation is >= Instructions.PushRegister + 3 and <= Instructions.PushRegisterLast &&
            operation != Instructions.PushRegister + Instructions.RegisterSp)
        {
            instructionSize = 1;
            register = operation - Instructions.PushRegister;
            return true;
        }

        if (code.Length >= 2 &&
            operation == Instructions.RexB &&
            code[1] is >= Instructions.PushRegister + 4 and <= Instructions.PushRegisterLast)
        {
            instructionSize = 2;
            register = code[1] - Instructions.PushRegister + Instructions.ExtendedRegisterOffset;
            return true;
        }

        return false;
    }

    static bool TryDecodeStackAllocation(ReadOnlySpan<byte> code, out int instructionSize, out uint allocationSize)
    {
        instructionSize = 0;
        allocationSize = 0;

        if (code.Length >= 4 &&
            code[0] == Instructions.RexW &&
            code[1] == Instructions.Group1Immediate8 &&
            code[2] == Instructions.ModRmSubRsp)
        {
            instructionSize = 4;
            allocationSize = code[3] <= sbyte.MaxValue ? code[3] : 0u;
            return true;
        }

        if (code.Length >= 7 &&
            code[0] == Instructions.RexW &&
            code[1] == Instructions.Group1Immediate32 &&
            code[2] == Instructions.ModRmSubRsp)
        {
            instructionSize = 7;
            allocationSize = BinaryPrimitives.ReadUInt32LittleEndian(code[3..]);
            if (allocationSize > int.MaxValue)
                allocationSize = 0;
            return true;
        }

        return false;
    }

    static bool TryDecodeFrameRegister(ReadOnlySpan<byte> code, out int instructionSize, out int frameOffset)
    {
        instructionSize = 0;
        frameOffset = 0;

        if (code.Length >= 3 && code[0] == Instructions.RexW &&
            (code[1] == Instructions.MovRegisterRm && code[2] == Instructions.ModRmMovRbpRsp ||
             code[1] == Instructions.MovRmRegister && code[2] == Instructions.ModRmMovRspRbp))
        {
            instructionSize = 3;
            return true;
        }

        if (code.Length >= 5 &&
            code[0] == Instructions.RexW &&
            code[1] == Instructions.Lea &&
            code[2] == Instructions.ModRmLeaRbpRspDisp8 &&
            code[3] == Instructions.SibRsp)
        {
            instructionSize = 5;
            frameOffset = code[4];
            return true;
        }

        if (code.Length >= 8 &&
            code[0] == Instructions.RexW &&
            code[1] == Instructions.Lea &&
            code[2] == Instructions.ModRmLeaRbpRspDisp32 &&
            code[3] == Instructions.SibRsp)
        {
            uint offset = BinaryPrimitives.ReadUInt32LittleEndian(code[4..]);
            if (offset > int.MaxValue)
                return false;
            instructionSize = 8;
            frameOffset = (int)offset;
            return true;
        }

        return false;
    }

    static bool IsPotentialPrologueInstruction(ReadOnlySpan<byte> code)
    {
        if (code.IsEmpty)
            return false;

        byte operation = code[0];
        if (operation is >= Instructions.PushRegister and <= Instructions.PushRegisterLast or
            Instructions.PushImmediate32 or
            Instructions.PushImmediate8 or
            Instructions.PushFlags or
            Instructions.Enter)
        {
            return true;
        }

        if (code.Length >= 2 &&
            operation == Instructions.RexB &&
            code[1] is >= Instructions.PushRegister and <= Instructions.PushRegisterLast)
            return true;

        if (code.Length >= 3 && operation == Instructions.RexW &&
            code[1] is Instructions.Group1Immediate32 or Instructions.Group1Immediate8 &&
            code[2] is Instructions.ModRmAddRsp or Instructions.ModRmAndRsp or Instructions.ModRmSubRsp)
        {
            return true;
        }

        if (code.Length >= 3 && operation == Instructions.RexW &&
            code[1] is Instructions.MovRmRegister or Instructions.MovRegisterRm or Instructions.Lea)
        {
            int destinationRegister =
                code[1] == Instructions.MovRmRegister ? code[2] & 7 : code[2] >> 3 & 7;
            if (destinationRegister is Instructions.RegisterSp or Instructions.RegisterBp)
                return true;
        }

        return false;
    }

    public readonly record struct UnwindInfo(
        int PrologueSize,
        int FrameRegister,
        int FrameOffset);

    public readonly record struct UnwindOperation(
        byte CodeOffset,
        UnwindOperationKind Kind,
        byte Register,
        uint Value);

    public readonly record struct EpilogueOperation(int Start, int End, int OperationIndex);

    public enum UnwindOperationKind
    {
        PushNonvolatile,
        StackAllocation,
        SetFramePointer
    }

    public readonly ref struct Analysis(
        UnwindAnalysisResult result,
        UnwindInfo info,
        ReadOnlySpan<UnwindOperation> operations,
        ReadOnlySpan<EpilogueOperation> epilogue = default)
    {
        public UnwindAnalysisResult Result { get; init; } = result;

        public UnwindInfo Info { get; } = info;

        public ReadOnlySpan<UnwindOperation> Operations { get; } = operations;

        public ReadOnlySpan<EpilogueOperation> Epilogue { get; init; } = epilogue;
    }

    public enum AnalysisLevel
    {
        None,
        Operations,
        Epilogue
    }

    public const int MaximumOperationCount = byte.MaxValue;
}

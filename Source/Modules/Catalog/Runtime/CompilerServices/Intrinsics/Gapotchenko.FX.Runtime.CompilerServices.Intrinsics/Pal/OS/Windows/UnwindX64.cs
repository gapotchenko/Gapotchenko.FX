// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Analyzes and encodes Windows x64 unwind information for intrinsic prologues.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
static class UnwindX64
{
    public static UnwindAnalysisResult Analyze(ReadOnlySpan<byte> code, out int prologueSize)
    {
        Span<Operation> operations = stackalloc Operation[byte.MaxValue];
        return Analyze(code, operations, out int operationCount, out prologueSize, out _, out _);
    }

    public static int GetSize(ReadOnlySpan<byte> code, int prologueSize)
    {
        Span<Operation> operations = stackalloc Operation[byte.MaxValue];
        var result = Analyze(code, operations, out int operationCount, out int actualPrologueSize, out _, out _);
        if (result != UnwindAnalysisResult.Supported || actualPrologueSize != prologueSize)
            throw new ArgumentException("The code does not have a supported Windows x64 prologue.", nameof(code));

        return HeaderSize + UnwindCodeSize * AlignUnwindCodeCount(CountUnwindCodes(operations[..operationCount]));
    }

    public static void Write(Span<byte> destination, ReadOnlySpan<byte> code, int prologueSize)
    {
        Span<Operation> operations = stackalloc Operation[byte.MaxValue];
        var result = Analyze(
            code,
            operations,
            out int operationCount,
            out int actualPrologueSize,
            out int frameRegister,
            out int frameOffset);
        if (result != UnwindAnalysisResult.Supported || actualPrologueSize != prologueSize)
            throw new ArgumentException("The code does not have a supported Windows x64 prologue.", nameof(code));

        operations = operations[..operationCount];
        int unwindCodeCount = CountUnwindCodes(operations);
        int size = HeaderSize + UnwindCodeSize * AlignUnwindCodeCount(unwindCodeCount);
        destination = destination[..size];
        destination.Clear();

        destination[0] = Version;
        destination[1] = checked((byte)prologueSize);
        destination[2] = checked((byte)unwindCodeCount);
        destination[3] = checked((byte)(frameOffset << 4 | frameRegister));

        // UNWIND_CODE entries are stored in descending prologue-offset order.
        int destinationOffset = HeaderSize;
        for (int i = operations.Length - 1; i >= 0; --i)
        {
            ref readonly var operation = ref operations[i];
            destination[destinationOffset] = operation.CodeOffset;
            destination[destinationOffset + 1] = checked((byte)(operation.OperationInfo << 4 | (byte)operation.OperationCode));
            destinationOffset += UnwindCodeSize;

            switch (operation.ExtraSlotCount)
            {
                case 1:
                    WriteUInt16(destination[destinationOffset..], checked((ushort)operation.ExtraData));
                    destinationOffset += UnwindCodeSize;
                    break;

                case 2:
                    WriteUInt32(destination[destinationOffset..], operation.ExtraData);
                    destinationOffset += 2 * UnwindCodeSize;
                    break;
            }
        }
    }

    static UnwindAnalysisResult Analyze(
        ReadOnlySpan<byte> code,
        Span<Operation> operations,
        out int operationCount,
        out int prologueSize,
        out int frameRegister,
        out int frameOffset)
    {
        operationCount = 0;
        prologueSize = 0;
        frameRegister = 0;
        frameOffset = 0;
        bool rbpSaved = false;

        while (prologueSize < code.Length && prologueSize < byte.MaxValue)
        {
            var remainingCode = code[prologueSize..];
            if (TryDecodePushNonvolatile(remainingCode, out int instructionSize, out int register))
            {
                if (!TryAddOperation(operations, ref operationCount, ref prologueSize, instructionSize, NativeMethods.UnwindOperation.PushNonvolatile, register))
                    return UnwindAnalysisResult.Unsupported;
                if (register == InstructionsX64.RegisterBp)
                    rbpSaved = true;
                continue;
            }

            if (TryDecodeStackAllocation(remainingCode, out instructionSize, out uint allocationSize))
            {
                if (!TryAddStackAllocation(operations, ref operationCount, ref prologueSize, instructionSize, allocationSize))
                    return UnwindAnalysisResult.Unsupported;
                continue;
            }

            if (TryDecodeFrameRegister(remainingCode, out instructionSize, out int offset))
            {
                if (!rbpSaved || frameRegister != 0 || offset % 16 != 0 || offset > 240 ||
                    !TryAddOperation(operations, ref operationCount, ref prologueSize, instructionSize, NativeMethods.UnwindOperation.SetFramePointer, 0))
                {
                    return UnwindAnalysisResult.Unsupported;
                }

                frameRegister = InstructionsX64.RegisterBp;
                frameOffset = offset / 16;
                continue;
            }

            if (IsPotentialPrologueInstruction(remainingCode))
                return UnwindAnalysisResult.Unsupported;

            break;
        }

        if (CountUnwindCodes(operations[..operationCount]) > byte.MaxValue)
            return UnwindAnalysisResult.Unsupported;

        return operationCount == 0 ? UnwindAnalysisResult.Leaf : UnwindAnalysisResult.Supported;
    }

    static bool TryAddStackAllocation(
        Span<Operation> operations,
        ref int operationCount,
        ref int prologueSize,
        int instructionSize,
        uint allocationSize)
    {
        if (allocationSize == 0 || (allocationSize & 7) != 0)
            return false;

        if (allocationSize <= 128)
        {
            return TryAddOperation(
                operations,
                ref operationCount,
                ref prologueSize,
                instructionSize,
                NativeMethods.UnwindOperation.AllocateSmall,
                checked((int)(allocationSize / 8 - 1)));
        }

        if (allocationSize / 8 <= ushort.MaxValue)
        {
            return TryAddOperation(
                operations,
                ref operationCount,
                ref prologueSize,
                instructionSize,
                NativeMethods.UnwindOperation.AllocateLarge,
                0,
                allocationSize / 8,
                1);
        }

        return TryAddOperation(
            operations,
            ref operationCount,
            ref prologueSize,
            instructionSize,
            NativeMethods.UnwindOperation.AllocateLarge,
            1,
            allocationSize,
            2);
    }

    static bool TryAddOperation(
        Span<Operation> operations,
        ref int operationCount,
        ref int prologueSize,
        int instructionSize,
        NativeMethods.UnwindOperation operationCode,
        int operationInfo,
        uint extraData = 0,
        int extraSlotCount = 0)
    {
        int codeOffset = prologueSize + instructionSize;
        if (codeOffset > byte.MaxValue || operationCount >= operations.Length)
            return false;

        operations[operationCount++] = new(
            checked((byte)codeOffset),
            operationCode,
            checked((byte)operationInfo),
            extraData,
            checked((byte)extraSlotCount));
        prologueSize = codeOffset;
        return true;
    }

    static int CountUnwindCodes(ReadOnlySpan<Operation> operations)
    {
        int count = 0;
        foreach (ref readonly var operation in operations)
            count = checked(count + 1 + operation.ExtraSlotCount);
        return count;
    }

    static int AlignUnwindCodeCount(int count) => (count + 1) & ~1;

    static bool TryDecodePushNonvolatile(ReadOnlySpan<byte> code, out int instructionSize, out int register)
    {
        instructionSize = 0;
        register = 0;
        if (code.IsEmpty)
            return false;

        byte operation = code[0];
        if (operation is >= InstructionsX64.PushRegister + 3 and <= InstructionsX64.PushRegisterLast &&
            operation != InstructionsX64.PushRegister + InstructionsX64.RegisterSp)
        {
            instructionSize = 1;
            register = operation - InstructionsX64.PushRegister;
            return true;
        }

        if (code.Length >= 2 &&
            operation == InstructionsX64.RexB &&
            code[1] is >= InstructionsX64.PushRegister + 4 and <= InstructionsX64.PushRegisterLast)
        {
            instructionSize = 2;
            register = code[1] - InstructionsX64.PushRegister + InstructionsX64.ExtendedRegisterOffset;
            return true;
        }

        return false;
    }

    static bool TryDecodeStackAllocation(ReadOnlySpan<byte> code, out int instructionSize, out uint allocationSize)
    {
        instructionSize = 0;
        allocationSize = 0;

        if (code.Length >= 4 &&
            code[0] == InstructionsX64.RexW &&
            code[1] == InstructionsX64.Group1Immediate8 &&
            code[2] == InstructionsX64.ModRmSubRsp)
        {
            instructionSize = 4;
            // The immediate is sign-extended by the processor. A negative
            // value increases RSP and therefore is not a stack allocation.
            allocationSize = code[3] <= sbyte.MaxValue ? code[3] : 0u;
            return true;
        }

        if (code.Length >= 7 &&
            code[0] == InstructionsX64.RexW &&
            code[1] == InstructionsX64.Group1Immediate32 &&
            code[2] == InstructionsX64.ModRmSubRsp)
        {
            instructionSize = 7;
            allocationSize = ReadUInt32(code[3..]);
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

        if (code.Length >= 3 && code[0] == InstructionsX64.RexW &&
            (code[1] == InstructionsX64.MovRegisterRm && code[2] == InstructionsX64.ModRmMovRbpRsp ||
             code[1] == InstructionsX64.MovRmRegister && code[2] == InstructionsX64.ModRmMovRspRbp))
        {
            instructionSize = 3;
            return true;
        }

        if (code.Length >= 5 &&
            code[0] == InstructionsX64.RexW &&
            code[1] == InstructionsX64.Lea &&
            code[2] == InstructionsX64.ModRmLeaRbpRspDisp8 &&
            code[3] == InstructionsX64.SibRsp)
        {
            instructionSize = 5;
            frameOffset = code[4];
            return true;
        }

        if (code.Length >= 8 &&
            code[0] == InstructionsX64.RexW &&
            code[1] == InstructionsX64.Lea &&
            code[2] == InstructionsX64.ModRmLeaRbpRspDisp32 &&
            code[3] == InstructionsX64.SibRsp)
        {
            uint offset = ReadUInt32(code[4..]);
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
        if (operation is >= InstructionsX64.PushRegister and <= InstructionsX64.PushRegisterLast or
            InstructionsX64.PushImmediate32 or
            InstructionsX64.PushImmediate8 or
            InstructionsX64.PushFlags or
            InstructionsX64.Enter)
            return true;

        if (code.Length >= 2 &&
            operation == InstructionsX64.RexB &&
            code[1] is >= InstructionsX64.PushRegister and <= InstructionsX64.PushRegisterLast)
            return true;

        if (code.Length >= 3 && operation == InstructionsX64.RexW &&
            code[1] is InstructionsX64.Group1Immediate32 or InstructionsX64.Group1Immediate8 &&
            code[2] is InstructionsX64.ModRmAddRsp or InstructionsX64.ModRmAndRsp or InstructionsX64.ModRmSubRsp)
        {
            return true;
        }

        // MOV/LEA with RSP or RBP as the destination can establish a stack or
        // frame state that must be represented by unwind information.
        if (code.Length >= 3 && operation == InstructionsX64.RexW &&
            code[1] is InstructionsX64.MovRmRegister or InstructionsX64.MovRegisterRm or InstructionsX64.Lea)
        {
            int destinationRegister =
                code[1] == InstructionsX64.MovRmRegister ? code[2] & 7 : code[2] >> 3 & 7;
            if (destinationRegister is InstructionsX64.RegisterSp or InstructionsX64.RegisterBp)
                return true;
        }

        return false;
    }

    static uint ReadUInt32(ReadOnlySpan<byte> value) =>
        (uint)(value[0] | value[1] << 8 | value[2] << 16 | value[3] << 24);

    static void WriteUInt16(Span<byte> destination, ushort value)
    {
        destination[0] = (byte)value;
        destination[1] = (byte)(value >> 8);
    }

    static void WriteUInt32(Span<byte> destination, uint value)
    {
        WriteUInt16(destination, (ushort)value);
        WriteUInt16(destination[2..], (ushort)(value >> 16));
    }

    readonly record struct Operation(
        byte CodeOffset,
        NativeMethods.UnwindOperation OperationCode,
        byte OperationInfo,
        uint ExtraData,
        byte ExtraSlotCount);

    const byte Version = 1;
    const int HeaderSize = 4;
    const int UnwindCodeSize = 2;
}

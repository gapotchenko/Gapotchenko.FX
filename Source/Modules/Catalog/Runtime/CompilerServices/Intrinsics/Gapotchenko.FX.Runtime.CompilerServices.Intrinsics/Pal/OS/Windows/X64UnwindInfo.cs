// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Encodes Windows x64 unwind information for supported intrinsic prologues.
/// </summary>
static class X64UnwindInfo
{
    public static int GetPrologueSize(ReadOnlySpan<byte> code)
    {
        int offset = 0;
        while (offset < code.Length && offset < byte.MaxValue)
        {
            int instructionSize = DecodePushNonvolatile(code[offset..], out _);
            if (instructionSize == 0 || offset + instructionSize > byte.MaxValue)
                break;
            offset += instructionSize;
        }
        return offset;
    }

    public static int GetSize(ReadOnlySpan<byte> code, int prologueSize)
    {
        int unwindCodeCount = CountUnwindCodes(code, prologueSize);
        return HeaderSize + UnwindCodeSize * AlignUnwindCodeCount(unwindCodeCount);
    }

    public static void Write(Span<byte> destination, ReadOnlySpan<byte> code, int prologueSize)
    {
        int unwindCodeCount = CountUnwindCodes(code, prologueSize);
        int size = HeaderSize + UnwindCodeSize * AlignUnwindCodeCount(unwindCodeCount);
        destination = destination[..size];
        destination.Clear();

        destination[0] = Version;
        destination[1] = checked((byte)prologueSize);
        destination[2] = checked((byte)unwindCodeCount);

        // UNWIND_CODE entries are stored in descending prologue-offset order.
        Span<byte> instructionOffsets = stackalloc byte[unwindCodeCount];
        Span<byte> registers = stackalloc byte[unwindCodeCount];
        int offset = 0;
        for (int i = 0; i < unwindCodeCount; ++i)
        {
            int instructionSize = DecodePushNonvolatile(code[offset..], out int register);
            offset += instructionSize;
            instructionOffsets[i] = checked((byte)offset);
            registers[i] = checked((byte)register);
        }

        int destinationOffset = HeaderSize;
        for (int i = unwindCodeCount - 1; i >= 0; --i)
        {
            destination[destinationOffset] = instructionOffsets[i];
            destination[destinationOffset + 1] = checked((byte)(registers[i] << 4 | UnwindOperationPushNonvolatile));
            destinationOffset += UnwindCodeSize;
        }
    }

    static int CountUnwindCodes(ReadOnlySpan<byte> code, int prologueSize)
    {
        int count = 0;
        for (int offset = 0; offset < prologueSize; ++count)
            offset += DecodePushNonvolatile(code[offset..], out _);
        return count;
    }

    static int AlignUnwindCodeCount(int count) => (count + 1) & ~1;

    static int DecodePushNonvolatile(ReadOnlySpan<byte> code, out int register)
    {
        register = 0;
        if (code.IsEmpty)
            return 0;

        byte operation = code[0];
        if (operation is >= 0x53 and <= 0x57 && operation != 0x54)
        {
            register = operation - 0x50;
            return 1;
        }

        if (code.Length >= 2 && operation == 0x41 && code[1] is >= 0x54 and <= 0x57)
        {
            register = code[1] - 0x50 + 8;
            return 2;
        }

        return 0;
    }

    const byte Version = 1;
    const byte UnwindOperationPushNonvolatile = 0;
    const int HeaderSize = 4;
    const int UnwindCodeSize = 2;
}

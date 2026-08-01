// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

/// <summary>
/// Encodes DWARF unwind information for macOS ARM64 intrinsic trampolines.
/// </summary>
#if NET
[SupportedOSPlatform("macos")]
#endif
static class UnwindMacOSArm64
{
    public static int GetSize(ReadOnlySpan<uint> code, UnwindArm64.UnwindInfo info)
    {
        int instructionSize = GetInstructionSize(code, info);
        return checked(CieSize + Align4(FdeHeaderSize + instructionSize));
    }

    public static unsafe void Write(
        Span<byte> destination,
        ReadOnlySpan<uint> code,
        UnwindArm64.UnwindInfo info,
        void* codeAddress)
    {
        int size = GetSize(code, info);
        destination = destination[..size];
        Cie.CopyTo(destination);

        var writer = new Writer(destination[CieSize..]);
        int fdeSize = size - CieSize;
        writer.WriteUInt32(checked((uint)(fdeSize - sizeof(uint))));
        writer.WriteUInt32(CiePointer);

        byte* initialLocationAddress =
            (byte*)codeAddress + checked((code.Length + 1) * sizeof(uint)) + CieSize + InitialLocationOffset;
        writer.WriteInt32(checked((int)((byte*)codeAddress - initialLocationAddress)));
        writer.WriteInt32(checked((code.Length + 1) * sizeof(uint)));
        writer.WriteByte(0); // FDE augmentation data length.

        WriteInstructions(ref writer, code, info);
        while (writer.Position < fdeSize)
            writer.WriteByte(DwCfaNop);
    }

    static int GetInstructionSize(ReadOnlySpan<uint> code, UnwindArm64.UnwindInfo info)
    {
        int returnOffset = checked(code.Length * sizeof(uint));
        return
            info.FrameKind switch
            {
                UnwindArm64.FrameKind.StackAllocation =>
                    GetAdvanceLocationSize(sizeof(uint)) +
                    GetDefCfaOffsetSize(info.FrameSize) +
                    GetAdvanceLocationSize(checked(returnOffset - sizeof(uint))) +
                    GetDefCfaOffsetSize(0),

                UnwindArm64.FrameKind.FrameChain =>
                    GetAdvanceLocationSize(sizeof(uint)) +
                    GetDefCfaOffsetSize(info.FrameSize) +
                    GetRegisterOffsetSize(DwarfRegisterFp, 2) +
                    GetRegisterOffsetSize(DwarfRegisterLr, 1) +
                    GetAdvanceLocationSize(sizeof(uint)) +
                    GetDefCfaRegisterSize(DwarfRegisterFp) +
                    GetAdvanceLocationSize(checked(returnOffset - 2 * sizeof(uint))) +
                    GetDefCfaRegisterSize(DwarfRegisterSp) +
                    GetDefCfaOffsetSize(0) +
                    GetSameValueSize(DwarfRegisterFp) +
                    GetSameValueSize(DwarfRegisterLr),

                _ => throw new InvalidOperationException("Unknown ARM64 unwind frame kind.")
            };
    }

    static void WriteInstructions(
        ref Writer writer,
        ReadOnlySpan<uint> code,
        UnwindArm64.UnwindInfo info)
    {
        int returnOffset = checked(code.Length * sizeof(uint));
        switch (info.FrameKind)
        {
            case UnwindArm64.FrameKind.StackAllocation:
                WriteAdvanceLocation(ref writer, sizeof(uint));
                WriteDefCfaOffset(ref writer, info.FrameSize);
                WriteAdvanceLocation(ref writer, checked(returnOffset - sizeof(uint)));
                WriteDefCfaOffset(ref writer, 0);
                break;

            case UnwindArm64.FrameKind.FrameChain:
                WriteAdvanceLocation(ref writer, sizeof(uint));
                WriteDefCfaOffset(ref writer, info.FrameSize);
                WriteRegisterOffset(ref writer, DwarfRegisterFp, 2);
                WriteRegisterOffset(ref writer, DwarfRegisterLr, 1);
                WriteAdvanceLocation(ref writer, sizeof(uint));
                WriteDefCfaRegister(ref writer, DwarfRegisterFp);
                WriteAdvanceLocation(ref writer, checked(returnOffset - 2 * sizeof(uint)));
                WriteDefCfaRegister(ref writer, DwarfRegisterSp);
                WriteDefCfaOffset(ref writer, 0);
                WriteSameValue(ref writer, DwarfRegisterFp);
                WriteSameValue(ref writer, DwarfRegisterLr);
                break;

            default:
                throw new InvalidOperationException("Unknown ARM64 unwind frame kind.");
        }
    }

    static int GetAdvanceLocationSize(int offset)
    {
        if (offset == 0)
            return 0;
        if ((uint)offset <= DwCfaAdvanceLocationMask)
            return 1;
        if ((uint)offset <= byte.MaxValue)
            return 2;
        if ((uint)offset <= ushort.MaxValue)
            return 3;
        return 5;
    }

    static void WriteAdvanceLocation(ref Writer writer, int offset)
    {
        if (offset == 0)
            return;
        if ((uint)offset <= DwCfaAdvanceLocationMask)
        {
            writer.WriteByte((byte)(DwCfaAdvanceLocation | offset));
        }
        else if ((uint)offset <= byte.MaxValue)
        {
            writer.WriteByte(DwCfaAdvanceLocation1);
            writer.WriteByte((byte)offset);
        }
        else if ((uint)offset <= ushort.MaxValue)
        {
            writer.WriteByte(DwCfaAdvanceLocation2);
            writer.WriteUInt16((ushort)offset);
        }
        else
        {
            writer.WriteByte(DwCfaAdvanceLocation4);
            writer.WriteUInt32((uint)offset);
        }
    }

    static int GetDefCfaOffsetSize(int offset) => 1 + GetUleb128Size((uint)offset);

    static void WriteDefCfaOffset(ref Writer writer, int offset)
    {
        writer.WriteByte(DwCfaDefCfaOffset);
        writer.WriteUleb128((uint)offset);
    }

    static int GetDefCfaRegisterSize(uint register) => 1 + GetUleb128Size(register);

    static void WriteDefCfaRegister(ref Writer writer, uint register)
    {
        writer.WriteByte(DwCfaDefCfaRegister);
        writer.WriteUleb128(register);
    }

    static int GetRegisterOffsetSize(uint register, uint offset) =>
        1 + GetUleb128Size(register) + GetUleb128Size(offset);

    static void WriteRegisterOffset(ref Writer writer, uint register, uint offset)
    {
        writer.WriteByte(DwCfaOffsetExtended);
        writer.WriteUleb128(register);
        writer.WriteUleb128(offset);
    }

    static int GetSameValueSize(uint register) => 1 + GetUleb128Size(register);

    static void WriteSameValue(ref Writer writer, uint register)
    {
        writer.WriteByte(DwCfaSameValue);
        writer.WriteUleb128(register);
    }

    static int GetUleb128Size(uint value)
    {
        int size = 1;
        while ((value >>= 7) != 0)
            ++size;
        return size;
    }

    static int Align4(int value) => checked((value + 3) & ~3);

    ref struct Writer
    {
        public Writer(Span<byte> destination)
        {
            m_Destination = destination;
            Position = 0;
        }

        public int Position { get; private set; }

        public void WriteByte(byte value) => m_Destination[Position++] = value;

        public void WriteUInt16(ushort value)
        {
            WriteByte((byte)value);
            WriteByte((byte)(value >> 8));
        }

        public void WriteUInt32(uint value)
        {
            WriteUInt16((ushort)value);
            WriteUInt16((ushort)(value >> 16));
        }

        public void WriteInt32(int value) => WriteUInt32((uint)value);

        public void WriteUleb128(uint value)
        {
            do
            {
                byte chunk = (byte)(value & 0x7f);
                value >>= 7;
                if (value != 0)
                    chunk |= 0x80;
                WriteByte(chunk);
            }
            while (value != 0);
        }

        Span<byte> m_Destination;
    }

    public const int FdeOffset = CieSize;

    // A version 1, zR-augmented ARM64 CIE. It defines CFA as SP and the
    // unchanged return address as X30, and uses PC-relative signed 32-bit FDEs.
    static ReadOnlySpan<byte> Cie =>
    [
        0x14, 0x00, 0x00, 0x00, // Length
        0x00, 0x00, 0x00, 0x00, // CIE identifier
        0x01,                   // Version
        0x7a, 0x52, 0x00,       // "zR"
        0x01,                   // Code alignment factor
        0x78,                   // Data alignment factor (-8)
        0x1e,                   // Return-address register (X30)
        0x01, 0x1b,             // Augmentation length; pcrel | sdata4
        0x0c, 0x1f, 0x00,       // DW_CFA_def_cfa SP, 0
        0x08, 0x1e,             // DW_CFA_same_value X30
        0x00, 0x00              // Alignment padding
    ];

    const int CieSize = 24;
    const int FdeHeaderSize = 17;
    const uint CiePointer = 28;
    const int InitialLocationOffset = 8;

    const byte DwCfaNop = 0x00;
    const byte DwCfaAdvanceLocation1 = 0x02;
    const byte DwCfaAdvanceLocation2 = 0x03;
    const byte DwCfaAdvanceLocation4 = 0x04;
    const byte DwCfaOffsetExtended = 0x05;
    const byte DwCfaSameValue = 0x08;
    const byte DwCfaDefCfaRegister = 0x0d;
    const byte DwCfaDefCfaOffset = 0x0e;
    const byte DwCfaAdvanceLocation = 0x40;
    const byte DwCfaAdvanceLocationMask = 0x3f;

    const uint DwarfRegisterFp = 29;
    const uint DwarfRegisterLr = 30;
    const uint DwarfRegisterSp = 31;
}

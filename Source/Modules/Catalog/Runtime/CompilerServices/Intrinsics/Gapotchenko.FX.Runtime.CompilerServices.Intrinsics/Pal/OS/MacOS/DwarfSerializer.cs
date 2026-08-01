// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Utils;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS9191 // The 'ref' modifier for an argument corresponding to 'in' parameter is equivalent to 'in'. Consider using 'in' instead.

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

/// <summary>
/// Serializes DWARF call-frame information used by dynamically registered FDEs.
/// </summary>
#if NET
[SupportedOSPlatform("macos")]
#endif
ref struct DwarfSerializer
{
    public DwarfSerializer()
    {
        m_Destination = default;
        m_IsCounting = true;
        Position = 0;
    }

    public DwarfSerializer(Span<byte> destination)
    {
        m_Destination = destination;
        m_IsCounting = false;
        Position = 0;
    }

    public int Position { get; private set; }

    public static int GetFdeSize(int cieSize, int instructionSize) =>
        checked(cieSize + MemoryArithmetics.Align4(FdeHeaderSize + instructionSize));

    public unsafe void WriteFdeHeader(
        int cieSize,
        void* unwindAddress,
        void* codeAddress,
        int codeSize,
        int fdeSize)
    {
        WriteUInt32(checked((uint)(fdeSize - sizeof(uint))));
        WriteUInt32(checked((uint)(cieSize + sizeof(uint))));

        byte* initialLocationAddress = (byte*)unwindAddress + cieSize + InitialLocationOffset;
        WriteInt32(checked((int)((byte*)codeAddress - initialLocationAddress)));
        WriteInt32(codeSize);
        WriteByte(0); // FDE augmentation data length.
    }

    public void CompleteFde(int fdeSize)
    {
        while (Position < fdeSize)
            WriteByte(DwCfaNop);
    }

    public void AdvanceLocation(int offset)
    {
        if (offset == 0)
        {
            return;
        }
        else if ((uint)offset <= DwCfaAdvanceLocationMask)
        {
            WriteByte((byte)(DwCfaAdvanceLocation | offset));
        }
        else if ((uint)offset <= byte.MaxValue)
        {
            WriteByte(DwCfaAdvanceLocation1);
            WriteByte((byte)offset);
        }
        else if ((uint)offset <= ushort.MaxValue)
        {
            WriteByte(DwCfaAdvanceLocation2);
            WriteUInt16((ushort)offset);
        }
        else
        {
            WriteByte(DwCfaAdvanceLocation4);
            WriteUInt32(checked((uint)offset));
        }
    }

    public void DefCfa(uint register, uint offset)
    {
        WriteByte(DwCfaDefCfa);
        WriteUleb128(register);
        WriteUleb128(offset);
    }

    public void DefCfaRegister(uint register)
    {
        WriteByte(DwCfaDefCfaRegister);
        WriteUleb128(register);
    }

    public void DefCfaOffset(int offset)
    {
        WriteByte(DwCfaDefCfaOffset);
        WriteUleb128(checked((uint)offset));
    }

    public void RegisterOffset(uint register, uint offset)
    {
        WriteByte(DwCfaOffsetExtended);
        WriteUleb128(register);
        WriteUleb128(offset);
    }

    public void SameValue(uint register)
    {
        WriteByte(DwCfaSameValue);
        WriteUleb128(register);
    }

    void WriteByte(byte value)
    {
        if (!m_IsCounting)
            m_Destination[Position] = value;
        ++Position;
    }

    void WriteUInt16(ushort value) => Write(value);

    void WriteUInt32(uint value) => Write(value);

    void WriteInt32(int value) => Write(value);

    void Write<T>(T value) where T : struct
    {
        int size = Unsafe.SizeOf<T>();
        if (!m_IsCounting)
            MemoryMarshal.Write(m_Destination[Position..], ref value);
        Position = checked(Position + size);
    }

    void WriteUleb128(uint value)
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

    readonly Span<byte> m_Destination;
    readonly bool m_IsCounting;

    const int FdeHeaderSize = 17;
    const int InitialLocationOffset = 2 * sizeof(uint);

    const byte DwCfaNop = 0x00;
    const byte DwCfaAdvanceLocation1 = 0x02;
    const byte DwCfaAdvanceLocation2 = 0x03;
    const byte DwCfaAdvanceLocation4 = 0x04;
    const byte DwCfaOffsetExtended = 0x05;
    const byte DwCfaSameValue = 0x08;
    const byte DwCfaDefCfa = 0x0c;
    const byte DwCfaDefCfaRegister = 0x0d;
    const byte DwCfaDefCfaOffset = 0x0e;
    const byte DwCfaAdvanceLocation = 0x40;
    const byte DwCfaAdvanceLocationMask = 0x3f;
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

/// <summary>
/// Encodes DWARF unwind information for macOS x64 intrinsic trampolines.
/// </summary>
#if NET
[SupportedOSPlatform("macos")]
#endif
static class UnwindMacOSX64
{
    public static UnwindAnalysisResult Analyze(ReadOnlySpan<byte> code, out UnwindX64.UnwindInfo info)
    {
        Span<UnwindX64.UnwindOperation> operations = stackalloc UnwindX64.UnwindOperation[UnwindX64.MaximumOperationCount];
        Span<EpilogueOperation> epilogue = stackalloc EpilogueOperation[UnwindX64.MaximumOperationCount];
        return Analyze(code, operations, out _, epilogue, out _, out info);
    }

    public static int GetSize(ReadOnlySpan<byte> code, UnwindX64.UnwindInfo expectedInfo)
    {
        Span<UnwindX64.UnwindOperation> operations = stackalloc UnwindX64.UnwindOperation[UnwindX64.MaximumOperationCount];
        Span<EpilogueOperation> epilogue = stackalloc EpilogueOperation[UnwindX64.MaximumOperationCount];
        var result = Analyze(
            code,
            operations,
            out int operationCount,
            epilogue,
            out int epilogueCount,
            out var info);
        if (result != UnwindAnalysisResult.Supported || info != expectedInfo)
            throw new ArgumentException("The code does not have a supported macOS x64 unwind prologue.", nameof(code));

        var writer = new Writer([]);
        WriteInstructions(
            ref writer,
            operations[..operationCount],
            epilogue[..epilogueCount]);
        return checked(CieSize + Align4(FdeHeaderSize + writer.Position));
    }

    public static unsafe void Write(
        Span<byte> destination,
        ReadOnlySpan<byte> code,
        UnwindX64.UnwindInfo expectedInfo,
        void* codeAddress)
    {
        Span<UnwindX64.UnwindOperation> operations = stackalloc UnwindX64.UnwindOperation[UnwindX64.MaximumOperationCount];
        Span<EpilogueOperation> epilogue = stackalloc EpilogueOperation[UnwindX64.MaximumOperationCount];
        var result = Analyze(
            code,
            operations,
            out int operationCount,
            epilogue,
            out int epilogueCount,
            out var info);
        if (result != UnwindAnalysisResult.Supported || info != expectedInfo)
            throw new ArgumentException("The code does not have a supported macOS x64 unwind prologue.", nameof(code));

        int size = GetSize(code, info);
        destination = destination[..size];
        byte* unwindAddress = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(destination));
        Cie.CopyTo(destination);

        var writer = new Writer(destination[CieSize..]);
        int fdeSize = size - CieSize;
        writer.WriteUInt32(checked((uint)(fdeSize - sizeof(uint))));
        writer.WriteUInt32(CiePointer);

        byte* initialLocationAddress = unwindAddress + CieSize + InitialLocationOffset;
        writer.WriteInt32(checked((int)((byte*)codeAddress - initialLocationAddress)));
        writer.WriteInt32(checked(code.Length + 1));
        writer.WriteByte(0); // FDE augmentation data length.

        WriteInstructions(
            ref writer,
            operations[..operationCount],
            epilogue[..epilogueCount]);
        while (writer.Position < fdeSize)
            writer.WriteByte(DwCfaNop);
    }

    static UnwindAnalysisResult Analyze(
        ReadOnlySpan<byte> code,
        scoped Span<UnwindX64.UnwindOperation> operations,
        out int operationCount,
        scoped Span<EpilogueOperation> epilogue,
        out int epilogueCount,
        out UnwindX64.UnwindInfo info)
    {
        epilogueCount = 0;
        var result = UnwindX64.Analyze(code, operations, out operationCount, out info);
        if (result != UnwindAnalysisResult.Supported)
            return result;

        operations = operations[..operationCount];
        int cfaOffset = InitialCfaOffset;
        foreach (ref readonly var operation in operations)
        {
            switch (operation.Kind)
            {
                case UnwindX64.UnwindOperationKind.PushNonvolatile:
                    cfaOffset = checked(cfaOffset + StackSlotSize);
                    break;

                case UnwindX64.UnwindOperationKind.StackAllocation:
                    cfaOffset = checked(cfaOffset + (int)operation.Value);
                    break;

                case UnwindX64.UnwindOperationKind.SetFramePointer:
                    if (operation.Value > cfaOffset)
                        return UnwindAnalysisResult.Unsupported;
                    break;
            }
        }

        int end = code.Length;
        for (int i = operations.Length - 1; i >= 0; --i)
        {
            ref readonly var operation = ref operations[i];
            int start;
            switch (operation.Kind)
            {
                case UnwindX64.UnwindOperationKind.SetFramePointer:
                    continue;

                case UnwindX64.UnwindOperationKind.StackAllocation:
                    if (!UnwindX64.TryDecodeStackDeallocationEndingAt(code, end, operation.Value, out start))
                        return UnwindAnalysisResult.Unsupported;
                    break;

                case UnwindX64.UnwindOperationKind.PushNonvolatile:
                    if (!UnwindX64.TryDecodePopNonvolatileEndingAt(code, end, operation.Register, out start))
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

    static void WriteInstructions(
        scoped ref Writer writer,
        scoped ReadOnlySpan<UnwindX64.UnwindOperation> operations,
        scoped ReadOnlySpan<EpilogueOperation> epilogue)
    {
        int location = 0;
        int rspCfaOffset = InitialCfaOffset;
        int cfaRegister = InstructionsX64.RegisterSp;

        foreach (ref readonly var operation in operations)
        {
            writer.AdvanceLocation(operation.CodeOffset - location);
            location = operation.CodeOffset;

            switch (operation.Kind)
            {
                case UnwindX64.UnwindOperationKind.PushNonvolatile:
                    rspCfaOffset = checked(rspCfaOffset + StackSlotSize);
                    if (cfaRegister == InstructionsX64.RegisterSp)
                        writer.DefCfaOffset(rspCfaOffset);
                    writer.RegisterOffset(
                        GetDwarfRegister(operation.Register),
                        checked((uint)(rspCfaOffset / StackSlotSize)));
                    break;

                case UnwindX64.UnwindOperationKind.StackAllocation:
                    rspCfaOffset = checked(rspCfaOffset + (int)operation.Value);
                    if (cfaRegister == InstructionsX64.RegisterSp)
                        writer.DefCfaOffset(rspCfaOffset);
                    break;

                case UnwindX64.UnwindOperationKind.SetFramePointer:
                    cfaRegister = InstructionsX64.RegisterBp;
                    writer.DefCfa(
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
                writer.AdvanceLocation(item.Start - location);
                location = item.Start;
                cfaRegister = InstructionsX64.RegisterSp;
                writer.DefCfa(GetDwarfRegister(cfaRegister), checked((uint)rspCfaOffset));
            }

            writer.AdvanceLocation(item.End - location);
            location = item.End;
            ref readonly var operation = ref operations[item.OperationIndex];
            switch (operation.Kind)
            {
                case UnwindX64.UnwindOperationKind.StackAllocation:
                    rspCfaOffset = checked(rspCfaOffset - (int)operation.Value);
                    if (cfaRegister == InstructionsX64.RegisterSp)
                        writer.DefCfaOffset(rspCfaOffset);
                    break;

                case UnwindX64.UnwindOperationKind.PushNonvolatile:
                    writer.SameValue(GetDwarfRegister(operation.Register));
                    rspCfaOffset = checked(rspCfaOffset - StackSlotSize);
                    if (cfaRegister == InstructionsX64.RegisterSp)
                        writer.DefCfaOffset(rspCfaOffset);
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

    static int Align4(int value) => checked((value + 3) & ~3);

    ref struct Writer
    {
        public Writer(Span<byte> destination)
        {
            m_Destination = destination;
            Position = 0;
        }

        public int Position { get; private set; }

        public void AdvanceLocation(int offset)
        {
            if (offset == 0)
                return;
            if ((uint)offset <= DwCfaAdvanceLocationMask)
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
                WriteUInt32((uint)offset);
            }
        }

        public void DefCfa(uint register, uint offset)
        {
            WriteByte(DwCfaDefCfa);
            WriteUleb128(register);
            WriteUleb128(offset);
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

        public void WriteByte(byte value)
        {
            if (!m_Destination.IsEmpty)
                m_Destination[Position] = value;
            ++Position;
        }

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

        Span<byte> m_Destination;
    }

    readonly record struct EpilogueOperation(int Start, int End, int OperationIndex);

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
    const int FdeHeaderSize = 17;
    const uint CiePointer = 28;
    const int InitialLocationOffset = 8;
    const int InitialCfaOffset = 8;
    const int StackSlotSize = 8;

    const byte DwCfaNop = 0x00;
    const byte DwCfaAdvanceLocation1 = 0x02;
    const byte DwCfaAdvanceLocation2 = 0x03;
    const byte DwCfaAdvanceLocation4 = 0x04;
    const byte DwCfaOffsetExtended = 0x05;
    const byte DwCfaSameValue = 0x08;
    const byte DwCfaDefCfa = 0x0c;
    const byte DwCfaDefCfaOffset = 0x0e;
    const byte DwCfaAdvanceLocation = 0x40;
    const byte DwCfaAdvanceLocationMask = 0x3f;
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;
using Gapotchenko.FX.Runtime.CompilerServices.Pal.Formats.Dwarf;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Formats.Dwarf;

/// <summary>
/// Encodes DWARF unwind information for ARM64 intrinsic trampolines.
/// </summary>
static class DwarfUnwindArm64
{
    public static int GetSize(ReadOnlySpan<uint> code, UnwindArm64.UnwindInfo info)
    {
        var serializer = new DwarfSerializer();
        WriteInstructions(ref serializer, code, info);
        return DwarfSerializer.GetFdeSize(CieSize, serializer.Position);
    }

    public static unsafe void Write(
        Span<byte> destination,
        ReadOnlySpan<uint> code,
        UnwindArm64.UnwindInfo info,
        void* codeAddress)
    {
        int size = GetSize(code, info);
        destination = destination[..size];
        byte* unwindAddress = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(destination));
        Cie.CopyTo(destination);

        var serializer = new DwarfSerializer(destination[CieSize..]);
        int fdeSize = size - CieSize;
        serializer.WriteFdeHeader(
            CieSize,
            unwindAddress,
            codeAddress,
            checked((code.Length + 1) * sizeof(uint)),
            fdeSize);
        WriteInstructions(ref serializer, code, info);
        serializer.CompleteFde(fdeSize);
    }

    static void WriteInstructions(
        scoped ref DwarfSerializer serializer,
        ReadOnlySpan<uint> code,
        UnwindArm64.UnwindInfo info)
    {
        int returnOffset = checked(code.Length * sizeof(uint));
        switch (info.FrameKind)
        {
            case UnwindArm64.FrameKind.StackAllocation:
                serializer.AdvanceLocation(sizeof(uint));
                serializer.DefCfaOffset(info.FrameSize);
                serializer.AdvanceLocation(checked(returnOffset - sizeof(uint)));
                serializer.DefCfaOffset(0);
                break;

            case UnwindArm64.FrameKind.FrameChain:
                serializer.AdvanceLocation(sizeof(uint));
                serializer.DefCfaOffset(info.FrameSize);
                serializer.RegisterOffset(DwarfRegisterFp, 2);
                serializer.RegisterOffset(DwarfRegisterLr, 1);
                serializer.AdvanceLocation(sizeof(uint));
                serializer.DefCfaRegister(DwarfRegisterFp);
                serializer.AdvanceLocation(checked(returnOffset - 2 * sizeof(uint)));
                serializer.DefCfaRegister(DwarfRegisterSp);
                serializer.DefCfaOffset(0);
                serializer.SameValue(DwarfRegisterFp);
                serializer.SameValue(DwarfRegisterLr);
                break;

            default:
                throw new InvalidOperationException("Unknown ARM64 unwind frame kind.");
        }
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
    const uint DwarfRegisterFp = 29;
    const uint DwarfRegisterLr = 30;
    const uint DwarfRegisterSp = 31;
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures.Arm.Arm64;

abstract class InstructionsArm64
{
    public static bool IsStackFrameSetup(uint instruction) =>
        (instruction & StpFpLrPreIndexMask) == StpFpLrPreIndex ||
        (instruction & AddSubSpSpImmediateMask) == SubSpSpImmediate;

    public static unsafe bool TryGetIndirectBranchTargetSlot(uint* p, out nuint* targetSlot)
    {
        if ((p[0] & LdrLiteralRegisterMask) == LdrLiteralRegister &&
            (p[1] & BranchRegisterMask) == BranchRegister &&
            (p[0] & RegisterMask) == (p[1] >> BranchRegisterOffset & RegisterMask))
        {
            int displacement =
                ((int)(p[0] >> LdrLiteralImmediateOffset & LdrLiteralImmediateMask) << 13 >> 13) << 2;
            targetSlot = (nuint*)((byte*)p + displacement);
            return true;
        }

        targetSlot = null;
        return false;
    }

    public static bool TryEncodeBranch(nint offset, out uint displacement)
    {
        if ((offset & (sizeof(uint) - 1)) != 0 ||
            offset < -BranchMaximumDistance || offset >= BranchMaximumDistance)
        {
            displacement = 0;
            return false;
        }

        displacement = (uint)(offset >> 2) & BranchImmediateMask;
        return true;
    }

    public static bool TryEncodeLongBranch(
        nuint sourceAddress,
        nuint targetAddress,
        out uint adrp,
        out uint add)
    {
        nint pageOffset =
            (nint)(targetAddress & ~(nuint)PageOffsetMask) -
            (nint)(sourceAddress & ~(nuint)PageOffsetMask);
        nint pageDisplacement = pageOffset >> PageOffsetBits;
        if (pageDisplacement < -(1 << AdrpImmediateHighBits) || pageDisplacement >= 1 << AdrpImmediateHighBits)
        {
            adrp = 0;
            add = 0;
            return false;
        }

        uint immediate = (uint)pageDisplacement & AdrpImmediateMask;
        adrp = AdrpX16 | (immediate & 3) << 29 | (immediate >> 2) << 5;
        add = AddX16X16Immediate | (uint)(targetAddress & PageOffsetMask) << Immediate12Offset;
        return true;
    }

    public static bool TryDecodeBranch(uint instruction, out int displacement)
    {
        if ((instruction & BranchMask) == B)
        {
            // Sign-extend imm26 and scale it by the four-byte instruction size.
            displacement = (int)(instruction << 6) >> 4;
            return true;
        }

        displacement = 0;
        return false;
    }

    public static int DecodeImmediate12(uint instruction)
    {
        int value = (int)(instruction >> Immediate12Offset & Immediate12Mask);
        if ((instruction & Immediate12ShiftFlag) != 0)
            value <<= Immediate12Shift;
        return value;
    }

    public static int DecodeSignedImmediate7Scaled8(uint instruction)
    {
        return ((int)(instruction >> Immediate7Offset & Immediate7Mask) << 25 >> 25) * Immediate7Scale;
    }

    public const uint AddSpSpImmediate = 0x910003ff;
    public const uint SubSpSpImmediate = 0xd10003ff;
    public const uint AddSubSpSpImmediateMask = 0xff8003ff;

    public const uint StpFpLrPreIndex = 0xa9807bfd;
    public const uint StpFpLrPreIndexMask = 0xffc07fff;
    public const uint StpRegistersPreIndexSp = 0xa980001f;
    public const uint StpRegistersPreIndexSpMask = 0xffc0001f;
    public const uint LdpRegistersPostIndexSp = 0xa8c0001f;
    public const uint LdpRegistersPostIndexSpMask = 0xffc0001f;
    public const uint LdpFpLrPostIndex = 0xa8c07bfd;
    public const uint LdpFpLrPostIndexMask = 0xffc07fff;

    public const uint MovFpSp = 0x910003fd;
    public const uint Pacibsp = 0xd503237f;
    public const uint Ret = 0xd65f03c0;

    /// <summary>
    /// The <c>B</c> instruction with a zero immediate displacement.
    /// </summary>
    public const uint B = 0x14000000;

    public const uint BranchMask = 0xfc000000;
    public const uint BranchImmediateMask = 0x03ffffff;

    /// <summary>
    /// The <c>BR X16</c> instruction.
    /// </summary>
    public const uint BrX16 = 0xd61f0200;

    public const nint BranchMaximumDistance = 1 << 27;
    public const ulong LongBranchMaximumDistance = 1UL << 32;
    public const int LongBranchInstructionCount = 3;

    public const int Immediate12Offset = 10;
    public const uint Immediate12Mask = 0xfff;
    public const uint Immediate12ShiftFlag = 1u << 22;
    public const int Immediate12Shift = 12;

    public const int Immediate7Offset = 15;
    public const uint Immediate7Mask = 0x7f;
    public const int Immediate7Scale = 8;

    const uint LdrLiteralRegister = 0x58000000;
    const uint LdrLiteralRegisterMask = 0xff000000;
    const uint LdrLiteralImmediateMask = 0x7ffff;
    const int LdrLiteralImmediateOffset = 5;
    const uint BranchRegister = 0xd61f0000;
    const uint BranchRegisterMask = 0xfffffc1f;
    const int BranchRegisterOffset = 5;
    const uint RegisterMask = 0x1f;

    const uint AdrpX16 = 0x90000010;
    const uint AddX16X16Immediate = 0x91000210;
    const int AdrpImmediateHighBits = 20;
    const uint AdrpImmediateMask = 0x1fffff;
    const int PageOffsetBits = 12;
    const uint PageOffsetMask = (1u << PageOffsetBits) - 1;
}

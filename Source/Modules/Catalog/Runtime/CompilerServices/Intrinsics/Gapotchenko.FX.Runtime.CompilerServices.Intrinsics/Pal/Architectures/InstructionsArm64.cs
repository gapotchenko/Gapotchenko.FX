// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class InstructionsArm64
{
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
    public const uint LdpFpLrPostIndex = 0xa8c07bfd;
    public const uint LdpFpLrPostIndexMask = 0xffc07fff;

    public const uint MovFpSp = 0x910003fd;
    public const uint Pacibsp = 0xd503237f;
    public const uint Ret = 0xd65f03c0;

    public const int Immediate12Offset = 10;
    public const uint Immediate12Mask = 0xfff;
    public const uint Immediate12ShiftFlag = 1u << 22;
    public const int Immediate12Shift = 12;

    public const int Immediate7Offset = 15;
    public const uint Immediate7Mask = 0x7f;
    public const int Immediate7Scale = 8;
}

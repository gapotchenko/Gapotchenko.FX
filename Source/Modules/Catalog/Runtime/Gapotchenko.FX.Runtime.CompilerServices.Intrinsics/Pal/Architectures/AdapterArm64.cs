// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class AdapterArm64 : AdapterArm
{
    protected static unsafe uint* SkipBranches(uint* p)
    {
        for (; ; )
        {
            // B label: the signed imm26 operand is measured in four-byte instructions.
            if ((*p & 0xfc000000) == 0x14000000)
            {
                // Sign-extend the operand and scale it by four to obtain a byte displacement in one go.
                int displacement = (int)(*p << 6) >> 4;
                p = (uint*)((byte*)p + displacement);
            }
            // LDR Xt, label; BR Xt
            else if (
                (p[0] & 0xff000000) == 0x58000000 &&
                (p[1] & 0xfffffc1f) == 0xd61f0000 &&
                (p[0] & 0x1f) == ((p[1] >> 5) & 0x1f))
            {
                // Sign-extend the imm19 operand and scale it by four.
                int displacement = ((int)((p[0] >> 5) & 0x7ffff) << 13 >> 13) << 2;
                p = *(uint**)((byte*)p + displacement);
            }
            else
            {
                return p;
            }
        }
    }

    protected const uint Ret = 0xd65f03c0;
}

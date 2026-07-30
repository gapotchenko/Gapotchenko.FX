// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class AdapterArm32 : AdapterArm
{
    protected static unsafe ushort* SkipBranches(ushort* p)
    {
        // Function pointers for Thumb code have bit 0 set.
        p = (ushort*)((nuint)p & ~(nuint)1);

        for (; ; )
        {
            // LDR.W PC, [PC, #imm12]
            if (p[0] == 0xf8df && (p[1] & 0xf000) == 0xf000)
            {
                byte* pc = (byte*)(((nuint)p + 4) & ~(nuint)3);
                p = (ushort*)(nuint)(*(uint*)(pc + (p[1] & 0x0fff)));
                p = (ushort*)((nuint)p & ~(nuint)1);
            }
            // B label: the signed imm11 operand is measured in two-byte instructions.
            else if ((p[0] & 0xf800) == 0xe000)
            {
                int displacement = ((p[0] & 0x07ff) << 21 >> 20) + 4;
                p = (ushort*)((byte*)p + displacement);
            }
            else
            {
                return p;
            }
        }
    }

    protected const ushort BxLr = 0x4770;
}

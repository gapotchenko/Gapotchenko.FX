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
            if (TryGetIndirectBranchTargetSlot(p, out uint* targetSlot))
            {
                p = (ushort*)(nuint)(*targetSlot);
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

    protected static unsafe bool TryGetIndirectBranchTargetSlot(ushort* p, out uint* targetSlot)
    {
        if (p[0] == 0xf8df && (p[1] & 0xf000) == 0xf000)
        {
            byte* pc = (byte*)(((nuint)p + 4) & ~(nuint)3);
            targetSlot = (uint*)(pc + (p[1] & 0x0fff));
            return true;
        }

        targetSlot = null;
        return false;
    }

    protected static bool TryEncodeBranch(nint offset, out ushort first, out ushort second)
    {
        if ((offset & 1) != 0 || offset < -BranchMaximumDistance || offset >= BranchMaximumDistance)
        {
            first = second = 0;
            return false;
        }

        uint displacement = (uint)offset;
        uint s = displacement >> 24 & 1;
        uint i1 = displacement >> 23 & 1;
        uint i2 = displacement >> 22 & 1;
        uint j1 = ~(i1 ^ s) & 1;
        uint j2 = ~(i2 ^ s) & 1;

        first = (ushort)(0xf000 | s << 10 | displacement >> 12 & 0x03ff);
        second = (ushort)(0x9000 | j1 << 13 | j2 << 11 | displacement >> 1 & 0x07ff);
        return true;
    }

    protected const nint BranchMaximumDistance = 1 << 24;

    protected const ushort BxLr = 0x4770;
}

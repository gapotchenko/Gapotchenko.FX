// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NET
using System.Runtime.Intrinsics.Arm;
#endif

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class AdapterArm64 : Adapter
{
    public override bool IsFeatureSupported(MachineCodeIntrinsicFeature feature)
    {
        return feature switch
        {
#if NET
            MachineCodeIntrinsicFeature.AdvSimd => AdvSimd.IsSupported,
#else
            // TODO
#endif
            _ => base.IsFeatureSupported(feature)
        };
    }

    protected static unsafe uint* SkipBranches(uint* p)
    {
        // B label: the signed imm26 operand is measured in four-byte instructions.
        while ((*p & 0xfc000000) == 0x14000000)
        {
            // Sign-extend the operand and scale it by four to obtain a byte displacement in one go.
            int displacement = (int)(*p << 6) >> 4;
            p = (uint*)((byte*)p + displacement);
        }
        return p;
    }
}

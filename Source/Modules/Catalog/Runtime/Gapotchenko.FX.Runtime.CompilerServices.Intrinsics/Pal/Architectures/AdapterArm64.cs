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
}

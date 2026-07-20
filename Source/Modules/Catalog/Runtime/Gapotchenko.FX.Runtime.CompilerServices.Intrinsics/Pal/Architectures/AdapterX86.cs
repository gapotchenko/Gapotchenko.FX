// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NET
using System.Runtime.Intrinsics.X86;
#endif

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Architectures;

abstract class AdapterX86 : Adapter
{
    public override bool IsFeatureSupported(MachineCodeIntrinsicFeature feature)
    {
        return feature switch
        {
#if NET
            MachineCodeIntrinsicFeature.Popcnt => Popcnt.IsSupported,
#else
            // TODO
#endif
            _ => base.IsFeatureSupported(feature)
        };
    }
}

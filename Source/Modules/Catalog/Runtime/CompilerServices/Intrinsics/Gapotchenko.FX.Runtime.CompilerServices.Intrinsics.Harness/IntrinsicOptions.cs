// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Harness;

static class IntrinsicOptions
{
    public const MethodImplOptions NoTieredCompilation =
#if NET
        MethodImplOptions.AggressiveOptimization;
#else
        0;
#endif
}

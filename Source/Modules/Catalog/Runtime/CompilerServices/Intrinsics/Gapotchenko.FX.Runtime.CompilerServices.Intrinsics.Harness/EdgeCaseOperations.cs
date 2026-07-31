// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Harness;

static class EdgeCaseOperations
{
    static EdgeCaseOperations()
    {
        Intrinsics.InitializeType(typeof(EdgeCaseOperations));
    }

    // The very first call to an intrinsic operation may still be executing managed code,
    // consequential calls will execute intrinsic machine code once it is applied.

    // TODO
}

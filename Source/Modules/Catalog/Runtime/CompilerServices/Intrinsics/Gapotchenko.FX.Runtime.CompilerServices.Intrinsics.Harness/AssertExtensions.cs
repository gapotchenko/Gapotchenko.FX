// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Harness;

static class AssertExtensions
{
    public static void FirstIntrinsicInvocationIsOK(this Assert _, int result)
    {
        Assert.IsTrue(
            result == OperationResults.Managed || result == OperationResults.Intrinsic,
            "The first intrinsic invocation returned an unexpected value.");
    }
}

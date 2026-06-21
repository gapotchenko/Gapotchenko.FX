// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography;

sealed class Arc4UnapprovedImpl : Arc4ManagedBase
{
    private protected override void EnforceAlgorithmPolicy()
    {
        // Policy compliance is not enforced by design.
    }
}

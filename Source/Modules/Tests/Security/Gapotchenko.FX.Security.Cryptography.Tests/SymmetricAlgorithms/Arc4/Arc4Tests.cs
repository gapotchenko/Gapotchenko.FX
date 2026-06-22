// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms.Arc4;

using Arc4 = Cryptography.Arc4;

[TestClass]
public sealed class Arc4Tests : Arc4TestsBase
{
    protected override Arc4 CreateAlgorithm()
    {
        if (CryptographyPolicy.AllowOnlyFipsAlgorithms)
            Assert.Inconclusive("ARC4 is not permitted under FIPS policy.");

        return Arc4.Create();
    }
}

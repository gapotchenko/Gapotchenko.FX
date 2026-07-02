// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms.TripleDES;

using TripleDES = System.Security.Cryptography.TripleDES;

[TestClass]
public sealed class TripleDESTests : TripleDESTest
{
    protected override TripleDES CreateAlgorithm()
    {
        if (CryptographyPolicy.AllowOnlyFipsAlgorithms)
            Assert.Inconclusive("Triple DES may be not permitted under FIPS policy.");

        return TripleDES.Create();
    }

#if NETFRAMEWORK
    protected override bool ThrowsCryptographicExceptionOnInvalidCipherArguments => true;
#endif
}

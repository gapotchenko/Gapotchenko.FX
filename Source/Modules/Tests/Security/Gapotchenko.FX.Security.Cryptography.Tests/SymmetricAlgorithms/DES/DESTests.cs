// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms.DES;

using DES = System.Security.Cryptography.DES;

[TestClass]
public sealed class DESTests : DESTestsBase
{
    protected override DES CreateAlgorithm()
    {
        if (CryptographyPolicy.AllowOnlyFipsAlgorithms)
            Assert.Inconclusive("DES is not permitted under FIPS policy.");

        return DES.Create();
    }

    protected override bool IsWeakKey(byte[] key)
    {
        return DES.IsWeakKey(key);
    }

    protected override bool IsSemiWeakKey(byte[] key)
    {
        return DES.IsSemiWeakKey(key);
    }

#if NETFRAMEWORK
    protected override bool ThrowsCryptographicExceptionOnInvalidCipherArguments => true;
#endif
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Reflection;

#pragma warning disable MSTEST0058 // Do not use asserts in catch blocks

namespace Gapotchenko.FX.Security.Cryptography.Tests.HashAlgorithms.MD5;

using MD5 = System.Security.Cryptography.MD5;

[TestClass]
public sealed class MD5Tests : MD5Test
{
    protected override MD5 CreateAlgorithm()
    {
        try
        {
            return MD5.Create();
        }
        catch (TargetInvocationException)
        {
            if (CryptographyPolicy.AllowOnlyFipsAlgorithms)
                Assert.Inconclusive("MD5 is not permitted under FIPS policy.");

            throw;
        }
    }
}

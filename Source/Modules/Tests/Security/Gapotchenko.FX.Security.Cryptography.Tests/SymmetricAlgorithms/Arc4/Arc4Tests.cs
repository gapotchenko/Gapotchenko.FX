// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using System.Reflection;

#pragma warning disable MSTEST0058 // Do not use asserts in catch blocks

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms.Arc4;

using Arc4 = Cryptography.Arc4;

[TestClass]
public sealed class Arc4Tests : Arc4Test
{
    protected override Arc4 CreateAlgorithm()
    {
        try
        {
            return Arc4.Create();
        }
        catch (TargetInvocationException)
        {
            if (CryptographyPolicy.AllowOnlyFipsAlgorithms)
                Assert.Inconclusive("ARC4 is not permitted under FIPS policy.");

            throw;
        }
    }
}

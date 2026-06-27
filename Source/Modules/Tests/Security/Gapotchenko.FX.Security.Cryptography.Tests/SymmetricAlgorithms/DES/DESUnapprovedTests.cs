// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms.DES;

using DES = System.Security.Cryptography.DES;

[TestClass]
public sealed class DESUnapprovedTests : DESTest
{
    protected override DES CreateAlgorithm()
    {
        return DESUnapproved.Create();
    }
}

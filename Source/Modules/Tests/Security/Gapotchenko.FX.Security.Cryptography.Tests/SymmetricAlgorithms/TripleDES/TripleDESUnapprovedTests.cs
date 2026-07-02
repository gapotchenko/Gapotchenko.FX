// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms.TripleDES;

using TripleDES = System.Security.Cryptography.TripleDES;

[TestClass]
public sealed class TripleDESUnapprovedTests : TripleDESTest
{
    protected override TripleDES CreateAlgorithm()
    {
        return TripleDESUnapproved.Create();
    }
}

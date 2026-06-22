// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms.Arc4;

using Arc4 = Cryptography.Arc4;

[TestClass]
public sealed class Arc4UnapprovedTests : Arc4TestsBase
{
    protected override Arc4 CreateAlgorithm()
    {
        return Arc4Unapproved.Create();
    }
}

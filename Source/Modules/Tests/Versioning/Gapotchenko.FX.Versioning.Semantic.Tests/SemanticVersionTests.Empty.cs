// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Versioning.Semantic.Tests;

partial class SemanticVersionTests
{
    [TestMethod]
    public void SemanticVersion_Empty_IsEmpty()
    {
        Assert.IsTrue(SemanticVersion.Empty.IsEmpty);
    }

    [TestMethod]
    public void SemanticVersion_Empty_EqualsToNewWithDefaultArguments()
    {
        Assert.AreEqual(new SemanticVersion(default, default, default), SemanticVersion.Empty);
    }
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Versioning.Semantic.Tests;

partial class SemanticVersionTests
{
    [TestMethod]
    public void SemanticVersion_Empty_IsEmpty()
    {
        Assert.IsTrue(SemanticVersion.Empty.IsEmpty);
    }

    [TestMethod]
    public void SemanticVersion_Empty_EqualsToNewWithDefaultMajorVersion()
    {
        Assert.AreEqual(new SemanticVersion((int)default), SemanticVersion.Empty);
    }
}

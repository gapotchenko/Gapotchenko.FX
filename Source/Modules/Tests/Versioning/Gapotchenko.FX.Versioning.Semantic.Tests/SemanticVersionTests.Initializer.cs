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
    public void SemanticVersion_Initializer_CloneCompleteness()
    {
        var version = new SemanticVersion("1.2.3+a-b");
        var clonedVersion = version with { Patch = version.Patch };

        Assert.AreNotSame(version, clonedVersion);
        Assert.AreEqual(version, clonedVersion);
    }

    [TestMethod]
    public void SemanticVersion_Initializer_NoCache()
    {
        var version = new SemanticVersion("1.2.3");

        // Cause a string representation of the object to be cached inside the object.
        _ = version.ToString();

        // Produce another object by cloning the original and changing it.
        version = version with { Patch = 4 };

        Assert.AreEqual(
            "1.2.4", version.ToString(),
            "A cloned object should not inherit the cached state.");
    }
}

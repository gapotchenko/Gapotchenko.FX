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
    public void SemanticVersion_Constructor_NumericVersion()
    {
        var version = new SemanticVersion("1.2.3");
        Assert.AreEqual(1, version.Major);
        Assert.AreEqual(2, version.Minor);
        Assert.AreEqual(3, version.Patch);
        Assert.IsNull(version.Prerelease);
        Assert.IsNull(version.Build);
    }

    [TestMethod]
    public void SemanticVersion_Constructor_LabeledVersion()
    {
        var version = new SemanticVersion("1.2.3-alpha.1+build.123");
        Assert.AreEqual(1, version.Major);
        Assert.AreEqual(2, version.Minor);
        Assert.AreEqual(3, version.Patch);
        Assert.AreEqual("alpha.1", version.Prerelease);
        Assert.AreEqual("build.123", version.Build);
    }

    [TestMethod]
    [DynamicData(nameof(SemanticVersion_Parse_TestData_ValidInputs))]
    public void SemanticVersion_Constructor_ValidVersion(string version)
    {
        _ = new SemanticVersion(version);
    }

    [TestMethod]
    [DynamicData(nameof(SemanticVersion_Parse_TestData_InvalidInputs))]
    public void SemanticVersion_Constructor_InvalidVersion(string version)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new SemanticVersion(version));
    }

    [TestMethod]
    public void SemanticVersion_Constructor_Contract()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new SemanticVersion(-1, 0, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new SemanticVersion(0, -1, 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new SemanticVersion(0, 0, -1));

        var version = new SemanticVersion(0, 0, 0);
        Assert.IsNull(version.Prerelease);
        Assert.IsNull(version.Build);

        version = new SemanticVersion(0, 0, 0, null);
        Assert.IsNull(version.Prerelease);
        Assert.IsNull(version.Build);

        version = new SemanticVersion(0, 0, 0, "");
        Assert.IsNull(version.Prerelease);
        Assert.IsNull(version.Build);

        version = new SemanticVersion(0, 0, 0, null, null);
        Assert.IsNull(version.Prerelease);
        Assert.IsNull(version.Build);

        version = new SemanticVersion(0, 0, 0, "", null);
        Assert.IsNull(version.Prerelease);
        Assert.IsNull(version.Build);

        version = new SemanticVersion(0, 0, 0, null, "");
        Assert.IsNull(version.Prerelease);
        Assert.IsNull(version.Build);

        version = new SemanticVersion(0, 0, 0, "", "");
        Assert.IsNull(version.Prerelease);
        Assert.IsNull(version.Build);

        Assert.ThrowsExactly<ArgumentNullException>(() => new SemanticVersion((string)null!));

        Assert.ThrowsExactly<ArgumentNullException>(() => new SemanticVersion((Version)null!));
    }

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

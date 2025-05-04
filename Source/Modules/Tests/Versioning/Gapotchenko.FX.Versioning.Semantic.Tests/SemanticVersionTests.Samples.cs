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
    public void SemanticVersion_ConstructorInitializesBaseVersionNumbers()
    {
        var version = new SemanticVersion("1.2.3");
        Assert.AreEqual(1, version.Major);
        Assert.AreEqual(2, version.Minor);
        Assert.AreEqual(3, version.Patch);
        Assert.IsNull(version.PreReleaseLabel);
        Assert.IsNull(version.BuildLabel);
    }

    [TestMethod]
    public void SemanticVersion_ConstructorParsesFullVersionNumber()
    {
        var version = new SemanticVersion("1.2.3-alpha.1+build.123");
        Assert.AreEqual(1, version.Major);
        Assert.AreEqual(2, version.Minor);
        Assert.AreEqual(3, version.Patch);
        Assert.AreEqual("alpha.1", version.PreReleaseLabel);
        Assert.AreEqual("build.123", version.BuildLabel);
    }

    [TestMethod]
    public void SemanticVersion_ConstructorThrowsAnExceptionIfVersionIsInvalid()
    {
        Assert.ThrowsException<ArgumentException>(() => new SemanticVersion("1.abc.3"));
    }

    [TestMethod]
    public void SemanticVersion_ContractFailsIfMajorVersionIsLessThanZero()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SemanticVersion(-1, 0, 0));
    }

    [TestMethod]
    public void SemanticVersion_ContractFailsIfMinorVersionIsLessThanZero()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SemanticVersion(0, -1, 0));
    }

    [TestMethod]
    public void SemanticVersion_ContractFailsIfPatchVersionIsLessThanZero()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SemanticVersion(0, 0, -1));
    }

    [TestMethod]
    public void SemanticVersion_CompareToComparesTwoSemanticVersionObjects()
    {
        var version1 = new SemanticVersion(1, 0, 0);
        object version2 = new SemanticVersion(1, 0, 0);
        Assert.AreEqual(0, version1.CompareTo(version2));
    }

    [TestMethod]
    public void SemanticVersion_MajorVersionIsLessThanOther()
    {
        var version1 = new SemanticVersion(1, 2, 3);
        var version2 = new SemanticVersion(2, 0, 0);
        Assert.IsTrue(version1 < version2);
    }

    [TestMethod]
    public void SemanticVersion_MinorVersionIsGreaterThanOther()
    {
        var version1 = new SemanticVersion(1, 2, 0);
        var version2 = new SemanticVersion(1, 1, 0);
        Assert.IsTrue(version1 > version2);
    }

    [TestMethod]
    public void SemanticVersion_PatchVersionIsLessThanOther()
    {
        var version1 = new SemanticVersion(1, 1, 3);
        var version2 = new SemanticVersion(1, 1, 4);
        Assert.IsTrue(version1 < version2);
    }

    [TestMethod]
    public void SemanticVersion_ReleaseVersionIsGreaterThanPrereleaseVersion()
    {
        var version1 = new SemanticVersion("1.0.0-alpha");
        var version2 = new SemanticVersion(1, 0, 0);
        Assert.IsTrue(version1 < version2);
        Assert.IsTrue(version2 > version1);
    }

    [TestMethod]
    public void SemanticVersion_SemanticVersionCannotBeComparedToString()
    {
        var version = new SemanticVersion(1, 0, 0);
        Assert.ThrowsException<ArgumentException>(() => version.CompareTo("1.3.0"));
    }

    [TestMethod]
    public void SemanticVersion_VersionIsEqualToItself()
    {
        var version = new SemanticVersion(1, 0, 0);
        Assert.IsTrue(version.Equals(version));
    }

    [TestMethod]
    public void SemanticVersion_VersionIsNotEqualToNull()
    {
        var version = new SemanticVersion(1, 0, 0);
        Assert.IsFalse(version == null);
        Assert.IsFalse(null == version);
        Assert.IsTrue(null != version);
        Assert.IsTrue(version != null);
        object? other = null;
        Assert.IsFalse(version.Equals(other));
    }

    [TestMethod]
    public void SemanticVersion_VersionIsNotEqualToString()
    {
        var version = new SemanticVersion(1, 0, 0);
        object versionNumber = "1.0.0";
        Assert.IsFalse(version.Equals(versionNumber));
    }

    [TestMethod]
    public void SemanticVersion_VersionIsTheSameAsItself()
    {
        var version = new SemanticVersion(1, 0, 0);
        Assert.AreEqual(0, version.CompareTo(version));
        Assert.IsTrue(version.Equals(version));
    }

    [TestMethod]
    public void SemanticVersion_VersionsAreComparedCorrectly()
    {
        var version1 = new SemanticVersion("1.0.0-alpha");
        var version2 = new SemanticVersion("1.0.0-alpha.1");
        var version3 = new SemanticVersion("1.0.0-beta.2");
        var version4 = new SemanticVersion("1.0.0-beta.11");
        var version5 = new SemanticVersion("1.0.0-rc.1");
        var version6 = new SemanticVersion("1.0.0-rc.1+build.1");
        var version7 = new SemanticVersion("1.0.0");
        var version8 = new SemanticVersion("1.0.0+0.3.7");
        var version9 = new SemanticVersion("1.3.7+build");
        var version10 = new SemanticVersion("1.3.7+build.2.b8f12d7");
        var version11 = new SemanticVersion("1.3.7+build.11.e0f985a");
        var version12 = new SemanticVersion("1.0.0-beta");
        var version13 = new SemanticVersion("1.0.0+0.3");
        Assert.IsTrue(version1 < version2);
        Assert.IsTrue(version2 < version3);
        Assert.IsTrue(version3 < version4);
        Assert.IsTrue(version4 < version5);
        Assert.IsTrue(version5 == version6);
        Assert.IsTrue(version6 < version7);
        Assert.IsTrue(version7 == version8);
        Assert.IsTrue(version8 < version9);
        Assert.IsTrue(version9 == version10);
        Assert.IsTrue(version10 == version11);
        Assert.IsTrue(version4 > version12);
        Assert.IsTrue(version8 == version7);
        Assert.IsTrue(version8 == version13);
    }

    [TestMethod]
    public void SemanticVersion_VersionsAreEqual()
    {
        var version1 = new SemanticVersion("1.0.0-alpha+build.1");
        var version2 = new SemanticVersion("1.0.0-alpha+build.2");
        object version3 = version2;
        object version4 = version1;
        Assert.IsTrue(version1 == version2);
        Assert.IsTrue(version1.Equals(version3));
        Assert.IsTrue(version1.Equals(version4));
    }

    [TestMethod]
    public void SemanticVersion_VersionsAreNotEqual()
    {
        var version1 = new SemanticVersion("1.0.0");
        var version2 = new SemanticVersion("1.0.0-alpha+build.1");
        object version3 = version2;
        Assert.IsTrue(version1 != version2);
        Assert.IsFalse(version1.Equals(version3));
    }
}

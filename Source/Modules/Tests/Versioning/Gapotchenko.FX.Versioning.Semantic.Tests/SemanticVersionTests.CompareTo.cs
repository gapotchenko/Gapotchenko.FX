// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Linq.Operators;

namespace Gapotchenko.FX.Versioning.Semantic.Tests;

partial class SemanticVersionTests
{
    [TestMethod]
    [DataRow("1.0.0", "1.0.0", 0)]
    [DataRow("1.2.3", "2.0.0", -1)]
    [DataRow("1.2.0", "1.1.0", 1)]
    [DataRow("1.1.3", "1.1.4", -1)]
    [DataRow("1.0.0-alpha", "1.0.0", -1)]
    [DataRow("1.0.0", null, 1)]
    [DataRow("0.0.0", null, 1)]
    // ---
    [DataRow("1.0.0-alpha", "1.0.0-alpha.1", -1)]
    [DataRow("1.0.0-alpha.1", "1.0.0-beta.2", -1)]
    [DataRow("1.0.0-beta.2", "1.0.0-beta.11", -1)]
    [DataRow("1.0.0-beta.11", "1.0.0-rc.1", -1)]
    [DataRow("1.0.0-rc.1", "1.0.0-rc.1+build.1", 0)]
    [DataRow("1.0.0-rc.1+build.1", "1.0.0", -1)]
    [DataRow("1.0.0", "1.0.0+0.3.7", 0)]
    [DataRow("1.0.0+0.3.7", "1.3.7+build", -1)]
    [DataRow("1.3.7+build", "1.3.7+build.2.b8f12d7", 0)]
    [DataRow("1.3.7+build.2.b8f12d7", "1.3.7+build.11.e0f985a", 0)]
    [DataRow("1.0.0-beta.11", "1.0.0-beta", 1)]
    [DataRow("1.0.0+0.3.7", "1.0.0", 0)]
    [DataRow("1.0.0+0.3.7", "1.0.0+0.3", 0)]
    // ---
    public void SemanticVersion_CompareTo_Relation(string? versionA, string? versionB, int relation) =>
        SemanticVersion_CompareTo_Test(versionA, versionB, relation);

    [TestMethod]
    [DataRow(["1.0.0-alpha", "1.0.0-alpha.1", "1.0.0-alpha.beta", "1.0.0-beta", "1.0.0-beta.2", "1.0.0-beta.11", "1.0.0-rc.1", "1.0.0"])]
    public void SemanticVersion_CompareTo_Order(string?[] versions)
    {
        for (int i = 1; i < versions.Length; ++i)
            SemanticVersion_CompareTo_Test(versions[i - 1], versions[i], -1);
    }

    static void SemanticVersion_CompareTo_Test(string? versionA, string? versionB, int relation)
    {
        var a = versionA?.PipeTo(x => new SemanticVersion(x));
        var b = versionB?.PipeTo(x => new SemanticVersion(x));

        SemanticVersion_CompareTo_Test(a, b, relation);
        SemanticVersion_CompareTo_Test(b, a, -relation);

        SemanticVersion_CompareTo_Test(a, a, 0);
        if (versionA != versionB)
            SemanticVersion_CompareTo_Test(b, b, 0);
    }

    static void SemanticVersion_CompareTo_Test(SemanticVersion? a, SemanticVersion? b, int relation)
    {
        if (relation > 0)
        {
            if (a is not null)
            {
                Assert.IsTrue(a.CompareTo(b) > 0);
                Assert.IsTrue(a.CompareTo((object?)b) > 0);

                Assert.IsFalse(a.Equals(b));
                Assert.IsFalse(a.Equals((object?)b));
            }

            Assert.IsTrue(a > b);
            Assert.IsTrue(a >= b);
            Assert.IsFalse(a < b);
            Assert.IsFalse(a <= b);
            Assert.IsFalse(a == b);
        }
        else if (relation < 0)
        {
            if (a is not null)
            {
                Assert.IsTrue(a.CompareTo(b) < 0);
                Assert.IsTrue(a.CompareTo((object?)b) < 0);

                Assert.IsFalse(a.Equals(b));
                Assert.IsFalse(a.Equals((object?)b));
            }

            Assert.IsTrue(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a > b);
            Assert.IsFalse(a >= b);
            Assert.IsFalse(a == b);
        }
        else
        {
            if (a is not null)
            {
                Assert.AreEqual(0, a.CompareTo(b));
                Assert.AreEqual(0, a.CompareTo((object?)b));

                Assert.IsTrue(a.Equals(b));
                Assert.IsTrue(a.Equals((object?)b));
            }

            Assert.IsTrue(a == b);
            Assert.IsTrue(a >= b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a > b);
            Assert.IsFalse(a < b);
        }
    }

    [TestMethod]
    public void SemanticVersion_CompareTo_AnotherType()
    {
        const string s = "1.0.0";
        var version = new SemanticVersion(s);
        Assert.ThrowsException<ArgumentException>(() => version.CompareTo(s));
    }
}

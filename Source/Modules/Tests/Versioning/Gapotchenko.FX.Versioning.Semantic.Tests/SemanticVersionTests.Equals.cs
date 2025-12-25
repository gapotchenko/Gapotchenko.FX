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
    [DataRow("1.0.0")]
    [DataRow("1.0.3-label")]
    [DataRow("2.5.0-label+build")]
    [DataRow(null)]
    public void SemanticVersion_Equals_EqualValues(string? version) =>
        SemanticVersion_Equals_EqualValues(version, version);

    [TestMethod]
    [DataRow("1.0.0-label", "1.0.0-label+build")]
    [DataRow("1.0.0-alpha+build.1", "1.0.0-alpha+build.2")]
    public void SemanticVersion_Equals_EqualValues(string? versionA, string? versionB) =>
        SemanticVersion_Equals_Test(versionA, versionB, true);

    [TestMethod]
    [DataRow("1.0.0", "2.0.0")]
    [DataRow("0.0.0", null)]
    [DataRow("0.0.0", "1.0.0")]
    [DataRow("0.0.0", "0.1.0")]
    [DataRow("0.0.0", "0.0.1")]
    [DataRow("1.0.0", "1.0.0-label")]
    [DataRow("1.0.0", "1.0.0-alpha+build.1")]
    [DataRow("1.0.0-label1", "1.0.0-label2")]
    public void SemanticVersion_Equals_UnequalValues(string? versionA, string? versionB) =>
        SemanticVersion_Equals_Test(versionA, versionB, false);

    static void SemanticVersion_Equals_Test(string? versionA, string? versionB, bool match)
    {
        var a = versionA?.PipeTo(x => new SemanticVersion(x));
        var b = versionB?.PipeTo(x => new SemanticVersion(x));

        SemanticVersion_Equals_Test(a, b, match);
        SemanticVersion_Equals_Test(b, a, match);

        if (match)
        {
            // Test same values.
            SemanticVersion_Equals_Test(a, a, true);
            if (versionA != versionB)
                SemanticVersion_Equals_Test(b, b, true);
        }
    }

    static void SemanticVersion_Equals_Test(SemanticVersion? a, SemanticVersion? b, bool match)
    {
        if (a is not null)
        {
            Assert.AreEqual(match, a.Equals(b));
            Assert.AreEqual(match, a.Equals((object?)b));

            Assert.AreEqual(match, a.CompareTo(b) == 0);
            Assert.AreEqual(match, a.CompareTo((object?)b) == 0);
        }

        if (b is not null)
        {
            Assert.AreEqual(match, b.Equals(a));
            Assert.AreEqual(match, b.Equals((object?)a));

            Assert.AreEqual(match, b.CompareTo(a) == 0);
            Assert.AreEqual(match, b.CompareTo((object?)a) == 0);
        }

        Assert.AreEqual(match, a == b);
        Assert.AreNotEqual(match, a != b);

        if (match)
        {
            Assert.IsTrue(a >= b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a > b);
            Assert.IsFalse(a < b);
        }
        else
        {
            bool greater = a > b;
            bool less = a < b;
            Assert.IsTrue(greater || less);
            Assert.IsFalse(greater && less);

            Assert.IsFalse(a >= b && a <= b);
        }

        if (match && a is not null && b is not null)
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void SemanticVersion_Equals_AnotherType()
    {
        const string s = "1.0.0";
        var version = new SemanticVersion(s);
        Assert.IsFalse(version.Equals(s));
    }
}

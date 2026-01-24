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
    public void SemanticVersion_Comparison(string? versionA, string? versionB, int relation) =>
        TestComparison(versionA, versionB, relation);

    [TestMethod]
    [DataRow(["1.0.0-alpha", "1.0.0-alpha.1", "1.0.0-alpha.beta", "1.0.0-beta", "1.0.0-beta.2", "1.0.0-beta.11", "1.0.0-rc.1", "1.0.0"])]
    public void SemanticVersion_Comparison_Order(string?[] versions)
    {
        for (int i = 1; i < versions.Length; ++i)
            TestComparison(versions[i - 1], versions[i], -1);
    }

    [TestMethod]
    public void SemanticVersion_Comparison_AnotherType()
    {
        const string s = "1.0.0";
        var version = new SemanticVersion(s);
        Assert.ThrowsExactly<ArgumentException>(() => version.CompareTo(s));
    }

    // ----------------------------------------------------------------------

    static void TestComparison(string? a, string? b, int comparison)
    {
        var objA = a?.PipeTo(x => new SemanticVersion(x));
        var objB = b?.PipeTo(x => new SemanticVersion(x));

        CompareCore(objA, objB, comparison);
        if (comparison != 0)
        {
            // Check the inverse comparison.
            CompareCore(objB, objA, -comparison);

            // Check the equality.
            CompareCore(objA, objA, 0);
            if (a != b)
                CompareCore(objB, objB, 0);
        }

        static void CompareCore(SemanticVersion? a, SemanticVersion? b, int comparison)
        {
            #region Comparison

            Assert.AreEqual(
                comparison,
                Math.Sign(Comparer<SemanticVersion>.Default.Compare(a, b)));

            if (a is not null)
            {
                Assert.AreEqual(comparison, Math.Sign(a.CompareTo(b)));
                Assert.AreEqual(comparison, Math.Sign(a.CompareTo((object?)b)));
            }
            if (b is not null)
            {
                Assert.AreEqual(comparison, -Math.Sign(b.CompareTo(a)));
                Assert.AreEqual(comparison, -Math.Sign(b.CompareTo((object?)a)));
            }

            Assert.AreEqual(comparison >= 0, a >= b);
            Assert.AreEqual(comparison <= 0, a <= b);
            Assert.AreEqual(comparison > 0, a > b);
            Assert.AreEqual(comparison < 0, a < b);

            #endregion

            #region Equality

            bool equality = comparison == 0;
            var equalityComparer = EqualityComparer<SemanticVersion>.Default;

            Assert.AreEqual(equality, equalityComparer.Equals(a, b));

            if (equality && a is not null && b is not null)
            {
                Assert.AreEqual(
                    equalityComparer.GetHashCode(a),
                    equalityComparer.GetHashCode(b));
            }

            Assert.AreEqual(equality, a == b);
            Assert.AreNotEqual(equality, a != b);

            if (a is not null)
            {
                Assert.AreEqual(equality, a.Equals(b));
                Assert.AreEqual(equality, a.Equals((object?)b));
            }
            if (b is not null)
            {
                Assert.AreEqual(equality, b.Equals(a));
                Assert.AreEqual(equality, b.Equals((object?)a));
            }

            #endregion
        }
    }
}

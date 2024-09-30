﻿using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.Tests.StringDistanceAlgorithms;

[TestClass]
public class LcsTests
{
    [TestMethod]
    public void Metrics_String_Distance_Lcs_Basics()
    {
        Assert.AreEqual(4, EditDistance("abra", ""));
        Assert.AreEqual(4, EditDistance("", "abra"));
        Assert.AreEqual(0, EditDistance("abra", "abra"));
        Assert.AreEqual(0, EditDistance("", ""));
        Assert.AreEqual(2, EditDistance("a", "b"));

        Assert.AreEqual(1, EditDistance("abr", "abra"));
        Assert.AreEqual(1, EditDistance("abra", "abr"));
        Assert.AreEqual(2, EditDistance("abra", "abrr"));
        Assert.AreEqual(3, EditDistance("abra", "a"));
        Assert.AreEqual(4, EditDistance("abra", "abcd"));
        Assert.AreEqual(8, EditDistance("abra", "1234"));
        Assert.AreEqual(6, EditDistance("abra", "a123"));
        Assert.AreEqual(4, EditDistance("abra", "1br2"));
    }

    [TestMethod]
    public void Metrics_String_Distance_Lcs_Range()
    {
        for (var maxDistance = 0; maxDistance <= 16; ++maxDistance)
        {
            Assert.AreEqual(
                maxDistance,
                EditDistance("abcdefghijklmnop", "ponmlkjihgfedcba", maxDistance));
        }
    }

    static int EditDistance(string a, string b, int? maxDistance = default) =>
        StringMetrics.Distance.Lcs.Measure(
            a, b,
            ValueInterval.Inclusive(null, maxDistance));
}

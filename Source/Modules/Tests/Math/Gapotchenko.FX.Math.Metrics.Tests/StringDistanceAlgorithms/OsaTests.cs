using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.Tests.StringDistanceAlgorithms;

[TestClass]
public sealed class OsaTests : IStringDistanceAlgorithmTests
{
    [TestMethod]
    public void StringDistance_Osa_Basics()
    {
        Assert.AreEqual(5, EditDistance("", "abcde"));
        Assert.AreEqual(5, EditDistance("abcde", ""));
        Assert.AreEqual(0, EditDistance("abcde", "abcde"));
        Assert.AreEqual(0, EditDistance("", ""));
        Assert.AreEqual(1, EditDistance("ab", "aa"));
        Assert.AreEqual(1, EditDistance("ab", "ba"));
        Assert.AreEqual(2, EditDistance("ab", "aaa"));
        Assert.AreEqual(3, EditDistance("bbb", "a"));
        Assert.AreEqual(3, EditDistance("ca", "abc"));
        Assert.AreEqual(4, EditDistance("a cat", "an abct"));
        Assert.AreEqual(4, EditDistance("dixon", "dicksonx"));
        Assert.AreEqual(2, EditDistance("jellyfish", "smellyfish"));
        Assert.AreEqual(1, EditDistance("smtih", "smith"));
        Assert.AreEqual(2, EditDistance("snapple", "apple"));
        Assert.AreEqual(2, EditDistance("testing", "testtn"));
        Assert.AreEqual(3, EditDistance("saturday", "sunday"));
        Assert.AreEqual(7, EditDistance("orange", "pumpkin"));
        Assert.AreEqual(5, EditDistance("gifts", "profit"));
        Assert.AreEqual(1, EditDistance("tt", "t"));
        Assert.AreEqual(1, EditDistance("t", "tt"));
    }

    [TestMethod]
    public void StringDistance_Osa_Range()
    {
        for (var maxDistance = 0; maxDistance <= 16; ++maxDistance)
        {
            Assert.AreEqual(
                maxDistance,
                EditDistance("abcdefghijklmnop", "rtsuvwxyz0123456", maxDistance));
        }
    }

    int EditDistance(string a, string b, int? maxDistance = default) =>
        DistanceAlgorithm.Calculate(
            a, b,
            ValueInterval.Inclusive(null, maxDistance));

    // ----------------------------------------------------------------------

    protected override IStringDistanceAlgorithm DistanceAlgorithm => StringMetrics.Distance.Osa;
}

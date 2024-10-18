using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.Tests.StringDistanceAlgorithms;

[TestClass]
public sealed class DamerauLevenshteinTests : IStringDistanceAlgorithmTests
{
    [TestMethod]
    [DataRow("", "abcde", 5)]
    [DataRow("abcde", "", 5)]
    [DataRow("abcde", "abcde", 0)]
    [DataRow("", "", 0)]
    [DataRow("ab", "aa", 1)]
    [DataRow("ab", "ba", 1)]
    [DataRow("ab", "aaa", 2)]
    [DataRow("bbb", "a", 3)]
    [DataRow("ca", "abc", 2)]
    [DataRow("a cat", "an abct", 3)]
    [DataRow("dixon", "dicksonx", 4)]
    [DataRow("jellyfish", "smellyfish", 2)]
    [DataRow("smtih", "smith", 1)]
    [DataRow("snapple", "apple", 2)]
    [DataRow("testing", "testtn", 2)]
    [DataRow("saturday", "sunday", 3)]
    [DataRow("orange", "pumpkin", 7)]
    [DataRow("gifts", "profit", 5)]
    [DataRow("tt", "t", 1)]
    [DataRow("t", "tt", 1)]
    public void StringDistance_DamerauLevenshtein_Basics(string a, string b, int distance)
    {
        Assert.AreEqual(distance, DistanceAlgorithm.Calculate(a, b));
    }

    [TestMethod]
    public void StringDistance_DamerauLevenshtein_Range()
    {
        for (var maxDistance = 0; maxDistance <= 16; ++maxDistance)
        {
            Assert.AreEqual(
                maxDistance,
                DistanceAlgorithm.Calculate(
                    "abcdefghijklmnop",
                    "rtsuvwxyz0123456",
                    ValueInterval.Inclusive((int?)null, maxDistance)));
        }
    }

    // ----------------------------------------------------------------------

    protected override IStringDistanceAlgorithm DistanceAlgorithm => StringMetrics.Distance.DamerauLevenshtein;
}

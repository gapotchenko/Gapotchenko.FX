using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.Tests.StringDistanceAlgorithms;

[TestClass]
public class HammingTests
{
    [TestMethod]
    public void Metrics_String_Distance_Hamming_Basics()
    {
        Assert.ThrowsException<ArgumentException>(() => EditDistance("abra", "abr"));

        Assert.AreEqual(0, EditDistance("abra", "abra"));
        Assert.AreEqual(0, EditDistance("", ""));
        Assert.AreEqual(1, EditDistance("a", "b"));

        Assert.AreEqual(1, EditDistance("abra", "abrr"));
        Assert.AreEqual(2, EditDistance("abra", "abcd"));
        Assert.AreEqual(4, EditDistance("abra", "1234"));
        Assert.AreEqual(3, EditDistance("abra", "a123"));
        Assert.AreEqual(2, EditDistance("abra", "1br2"));
    }

    [TestMethod]
    public void Metrics_String_Distance_Hamming_Range()
    {
        for (var maxDistance = 0; maxDistance <= 16; ++maxDistance)
        {
            Assert.AreEqual(
                maxDistance,
                EditDistance("abcdefghijklmnop", "ponmlkjihgfedcba", maxDistance));
        }
    }

    static int EditDistance(string a, string b, int? maxDistance = default) =>
        StringMetrics.Distance.Hamming.Measure(
            a, b,
            ValueInterval.Inclusive(null, maxDistance));
}

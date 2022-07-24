using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Geometry.Tests;

[TestClass]
public class HammingDistanceTests
{
    [TestMethod]
    public void HammingDistance_Basics()
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
    public void HammingDistance_MaxDistance()
    {
        for (var maxDistance = 0; maxDistance <= 16; ++maxDistance)
        {
            Assert.AreEqual(
                maxDistance,
                EditDistance("abcdefghijklmnop", "ponmlkjihgfedcba", maxDistance));
        }
    }

    static int EditDistance(string a, string b, int? maxDistance = default)
    {
        return StringMetrics.HammingDistance(a, b, maxDistance);
    }
}

using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.Tests.StringDistanceAlgorithms;

[TestCategory("string-distance")]
public abstract class IStringDistanceAlgorithmTests : IStringMetricAlgorithmTests
{
    [TestMethod]
    [DataRow("")]
    [DataRow("abc")]
    public void StringDistance_ZeroForEqual(string value)
    {
        Assert.AreEqual(0, DistanceAlgorithm.Calculate(value, value));
        Assert.AreEqual(0, DistanceAlgorithm.Calculate(value, new string(value.ToCharArray())));
    }

    [TestMethod]
    public void StringDistance_Range()
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

    protected void TestVector(string a, string b, int expectedDistance)
    {
        var actualDistance = DistanceAlgorithm.Calculate(a, b);
        Assert.AreEqual(expectedDistance, actualDistance);
    }

    // ----------------------------------------------------------------------

    protected abstract IStringDistanceAlgorithm DistanceAlgorithm { get; }

    protected sealed override IStringMetricAlgorithm MetricAlgorithm => DistanceAlgorithm;
}

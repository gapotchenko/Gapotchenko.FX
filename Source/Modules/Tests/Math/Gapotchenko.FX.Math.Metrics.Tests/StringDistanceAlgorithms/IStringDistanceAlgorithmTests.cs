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
        TestVector(value, value, 0);
        TestVector(value, new string(value.ToCharArray()), 0);
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
        foreach (var i in EnumerateStringVariations(a, b))
            Run(i.Item1, i.Item2);

        void Run(string a, string b)
        {
            var actualDistance = DistanceAlgorithm.Calculate(a, b);
            Assert.AreEqual(expectedDistance, actualDistance);

            var actualDistance2 = (int)MetricAlgorithm.Calculate(a, b);
            Assert.AreEqual(actualDistance, actualDistance2);
        }
    }

    // ----------------------------------------------------------------------

    protected abstract IStringDistanceAlgorithm DistanceAlgorithm { get; }

    protected sealed override IStringMetricAlgorithm MetricAlgorithm => DistanceAlgorithm;
}

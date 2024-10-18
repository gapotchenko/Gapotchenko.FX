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

    // ----------------------------------------------------------------------

    protected abstract IStringDistanceAlgorithm DistanceAlgorithm { get; }

    protected sealed override IStringMetricAlgorithm MetricAlgorithm => DistanceAlgorithm;
}

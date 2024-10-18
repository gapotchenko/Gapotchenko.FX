namespace Gapotchenko.FX.Math.Metrics.Tests;

[TestCategory("string-metrics")]
public abstract class IStringMetricAlgorithmTests
{
    [TestMethod]
    public void StringMetric_ReturnsCapabilities()
    {
        _ = MetricAlgorithm.Capabilities;
    }

    // ----------------------------------------------------------------------

    protected abstract IStringMetricAlgorithm MetricAlgorithm { get; }
}

namespace Gapotchenko.FX.Math.Metrics.Tests;

[TestCategory("string-metrics")]
public abstract class IStringMetricAlgorithmTests
{
    [TestMethod]
    public void StringMetric_ReturnsMetricTraits()
    {
        _ = MetricAlgorithm.MetricTraits;
    }

    // ----------------------------------------------------------------------

    protected IEnumerable<(string, string)> EnumerateStringVariations(string a, string b)
    {
        bool swap = true;
        bool reverse =
            (MetricAlgorithm.MetricTraits & (StringMetricTraits.Transposition | StringMetricTraits.Substitution)) != StringMetricTraits.Transposition;

        // ----------------------------

        yield return (a, b);

        bool doSwap = swap && a != b;
        if (doSwap)
            yield return (b, a);

        if (reverse)
        {
            var ra = string.Concat(a.Reverse());
            var rb = string.Concat(b.Reverse());
            yield return (ra, rb);

            if (doSwap)
                yield return (rb, ra);
        }
    }

    // ----------------------------------------------------------------------

    protected abstract IStringMetricAlgorithm MetricAlgorithm { get; }
}

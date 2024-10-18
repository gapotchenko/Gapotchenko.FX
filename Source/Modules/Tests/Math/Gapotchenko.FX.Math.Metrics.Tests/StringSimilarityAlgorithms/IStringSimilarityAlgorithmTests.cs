namespace Gapotchenko.FX.Math.Metrics.Tests.StringSimilarityAlgorithms;

[TestCategory("string-similarity")]
public abstract class IStringSimilarityAlgorithmTests : IStringMetricAlgorithmTests
{
    [TestMethod]
    [DataRow("")]
    [DataRow("abc")]
    public void String_Similarity_OneForEqual(string value)
    {
        Assert.AreEqual(1, SimilarityAlgorithm.Calculate(value, value));
        Assert.AreEqual(1, SimilarityAlgorithm.Calculate(value, new string(value.ToCharArray())));
    }

    [TestMethod]
    [DataRow("abc", "xyz")]
    public void String_Similarity_ZeroForDifferent(string a, string b)
    {
        Assert.AreEqual(0, SimilarityAlgorithm.Calculate(a, b));
    }

    // ----------------------------------------------------------------------

    protected abstract IStringSimilarityAlgorithm SimilarityAlgorithm { get; }

    protected sealed override IStringMetricAlgorithm MetricAlgorithm => SimilarityAlgorithm;
}

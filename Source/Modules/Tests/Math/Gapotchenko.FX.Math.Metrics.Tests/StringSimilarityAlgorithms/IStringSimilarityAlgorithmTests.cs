namespace Gapotchenko.FX.Math.Metrics.Tests.StringSimilarityAlgorithms;

[TestCategory("string-similarity")]
public abstract class IStringSimilarityAlgorithmTests : IStringMetricAlgorithmTests
{
    [TestMethod]
    [DataRow("")]
    [DataRow("abc")]
    public void StringSimilarity_OneForEqual(string value)
    {
        TestVector(value, value, 1);
        TestVector(value, new string(value.ToCharArray()), 1);
    }

    [TestMethod]
    [DataRow("abc", "xyz")]
    public void StringSimilarity_ZeroForDifferent(string a, string b) => TestVector(a, b, 0);

    // ----------------------------------------------------------------------

    protected void TestVector(string a, string b, double expectedSimilarity, double delta = 0)
    {
        foreach (var i in EnumerateStringVariations(a, b))
            Run(i.Item1, i.Item2);

        void Run(string a, string b)
        {
            var actualSimilarity = SimilarityAlgorithm.Calculate(a, b);

            Assert.IsTrue(actualSimilarity >= 0);
            Assert.IsTrue(actualSimilarity <= 1);
            Assert.AreEqual(expectedSimilarity, actualSimilarity, delta);

            var actualSimilarity2 = (double)MetricAlgorithm.Calculate(a, b);
            Assert.AreEqual(actualSimilarity, actualSimilarity2);
        }
    }

    // ----------------------------------------------------------------------

    protected abstract IStringSimilarityAlgorithm SimilarityAlgorithm { get; }

    protected sealed override IStringMetricAlgorithm MetricAlgorithm => SimilarityAlgorithm;
}

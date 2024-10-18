namespace Gapotchenko.FX.Math.Metrics.Tests.StringSimilarityAlgorithms;

[TestClass]
public sealed class JaroTests : IStringSimilarityAlgorithmTests
{
    [TestMethod]
    [DataRow("loans and accounts", "loan account", 0.83)]
    [DataRow("loan account", "loans and accounts", 0.83)]
    [DataRow("trace", "crate", 0.73)]
    [DataRow("trace", "trace", 1)]
    [DataRow("trace", "", 0)]
    [DataRow("", "trace", 0)]
    [DataRow("", "", 1)]
    [DataRow("abcd", "badc", 0.83)]
    [DataRow("abcd", "dcba", 0.5)]
    [DataRow("washington", "notgnihsaw", 0.43)]
    [DataRow("washington", "washingtonx", 0.97)]
    [DataRow("daniel", "danielle", 0.92)]
    [DataRow("sat", "urn", 0)]
    public void StringSimilarity_Jaro_Basics(string a, string b, double expectedSimilarity)
    {
        var actualSimilarity = SimilarityAlgorithm.Calculate(a, b);

        Assert.IsTrue(actualSimilarity >= 0);
        Assert.IsTrue(actualSimilarity <= 1);
        Assert.AreEqual(expectedSimilarity, actualSimilarity, 0.01);
    }

    // ----------------------------------------------------------------------

    protected override IStringSimilarityAlgorithm SimilarityAlgorithm => StringMetrics.Similarity.Jaro;
}

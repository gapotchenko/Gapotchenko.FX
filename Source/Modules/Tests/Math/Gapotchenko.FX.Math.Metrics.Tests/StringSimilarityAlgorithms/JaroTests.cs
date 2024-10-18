namespace Gapotchenko.FX.Math.Metrics.Tests.StringSimilarityAlgorithms;

[TestClass]
public class JaroTests
{
    [TestMethod]
    [DataRow("loans and accounts", "loan account", 0.17)]
    [DataRow("loan account", "loans and accounts", 0.17)]
    [DataRow("trace", "crate", 0.27)]
    [DataRow("trace", "trace", 0)]
    [DataRow("trace", "", 1)]
    [DataRow("", "trace", 1)]
    [DataRow("", "", 0)]
    [DataRow("abcd", "badc", 0.17)]
    [DataRow("abcd", "dcba", 0.5)]
    [DataRow("washington", "notgnihsaw", 0.57)]
    [DataRow("washington", "washingtonx", 0.03)]
    [DataRow("daniel", "danielle", 0.08)]
    [DataRow("sat", "urn", 1)]
    public void Metrics_String_Similarity_Jaro_Basics(string a, string b, double expectedDistance)
    {
        var expectedSimilarity = 1.0 - expectedDistance;
        var actualSimilarity = StringMetrics.Similarity.Jaro.Calculate(a, b);

        Assert.IsTrue(actualSimilarity >= 0);
        Assert.IsTrue(actualSimilarity <= 1);
        Assert.AreEqual(expectedSimilarity, actualSimilarity, 0.01);
    }
}

namespace Gapotchenko.FX.Math.Metrics.Tests.StringDistanceAlgorithms;

[TestClass]
public sealed class LevenshteinTests : IStringDistanceAlgorithmTests
{
    [TestMethod]
    [DataRow("abra", "", 4)]
    [DataRow("", "abra", 4)]
    [DataRow("abra", "abra", 0)]
    [DataRow("", "", 0)]
    [DataRow("a", "b", 1)]
    [DataRow("abr", "abra", 1)]
    [DataRow("abra", "abr", 1)]
    [DataRow("abra", "abrr", 1)]
    [DataRow("abra", "a", 3)]
    [DataRow("abra", "abcd", 2)]
    [DataRow("abra", "1234", 4)]
    [DataRow("abra", "a123", 3)]
    [DataRow("abra", "1br2", 2)]
    public void StringDistance_Levenshtein_TestVectors(string a, string b, int expectedDistance) =>
        TestVector(a, b, expectedDistance);

    // ----------------------------------------------------------------------

    protected override IStringDistanceAlgorithm DistanceAlgorithm => StringMetrics.Distance.Levenshtein;
}

namespace Gapotchenko.FX.Math.Metrics.Tests.StringDistanceAlgorithms;

[TestClass]
public sealed class LcsTests : IStringDistanceAlgorithmTests
{
    [TestMethod]
    [DataRow("abra", "", 4)]
    [DataRow("", "abra", 4)]
    [DataRow("abra", "abra", 0)]
    [DataRow("", "", 0)]
    [DataRow("a", "b", 2)]
    [DataRow("abr", "abra", 1)]
    [DataRow("abra", "abr", 1)]
    [DataRow("abra", "abrr", 2)]
    [DataRow("abra", "a", 3)]
    [DataRow("abra", "abcd", 4)]
    [DataRow("abra", "1234", 8)]
    [DataRow("abra", "a123", 6)]
    [DataRow("abra", "1br2", 4)]
    public void StringDistance_Lcs_TestVectors(string a, string b, int expectedDistance) =>
        TestVector(a, b, expectedDistance);

    // ----------------------------------------------------------------------

    protected override IStringDistanceAlgorithm DistanceAlgorithm => StringMetrics.Distance.Lcs;
}

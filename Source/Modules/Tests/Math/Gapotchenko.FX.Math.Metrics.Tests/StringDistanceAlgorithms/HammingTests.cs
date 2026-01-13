namespace Gapotchenko.FX.Math.Metrics.Tests.StringDistanceAlgorithms;

[TestClass]
public sealed class HammingTests : IStringDistanceAlgorithmTests
{
    [TestMethod]
    [DataRow("abra", "abra", 0)]
    [DataRow("", "", 0)]
    [DataRow("a", "b", 1)]
    [DataRow("abra", "abrr", 1)]
    [DataRow("abra", "abcd", 2)]
    [DataRow("abra", "1234", 4)]
    [DataRow("abra", "a123", 3)]
    [DataRow("abra", "1br2", 2)]
    public void StringDistance_Hamming_TestVectors(string a, string b, int expectedDistance) =>
        TestVector(a, b, expectedDistance);

    [TestMethod]
    public void StringDistance_Hamming_DifferentLengths()
    {
        Assert.ThrowsExactly<ArgumentException>(() => DistanceAlgorithm.Calculate("abra", "abr", cancellationToken: TestContext.CancellationToken));
    }

    // ----------------------------------------------------------------------

    protected override IStringDistanceAlgorithm DistanceAlgorithm => StringMetrics.Distance.Hamming;

    public TestContext TestContext { get; set; }
}

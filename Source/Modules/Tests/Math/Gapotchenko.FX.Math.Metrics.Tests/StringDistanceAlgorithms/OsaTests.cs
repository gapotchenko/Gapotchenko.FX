namespace Gapotchenko.FX.Math.Metrics.Tests.StringDistanceAlgorithms;

[TestClass]
public sealed class OsaTests : IStringDistanceAlgorithmTests
{
    [TestMethod]
    [DataRow("", "abcde", 5)]
    [DataRow("abcde", "", 5)]
    [DataRow("abcde", "abcde", 0)]
    [DataRow("", "", 0)]
    [DataRow("ab", "aa", 1)]
    [DataRow("ab", "ba", 1)]
    [DataRow("ab", "aaa", 2)]
    [DataRow("bbb", "a", 3)]
    [DataRow("ca", "abc", 3)]
    [DataRow("a cat", "an abct", 4)]
    [DataRow("dixon", "dicksonx", 4)]
    [DataRow("jellyfish", "smellyfish", 2)]
    [DataRow("smtih", "smith", 1)]
    [DataRow("snapple", "apple", 2)]
    [DataRow("testing", "testtn", 2)]
    [DataRow("saturday", "sunday", 3)]
    [DataRow("orange", "pumpkin", 7)]
    [DataRow("gifts", "profit", 5)]
    [DataRow("tt", "t", 1)]
    [DataRow("t", "tt", 1)]
    public void StringDistance_Osa_TestVectors(string a, string b, int expectedDistance) =>
        TestVector(a, b, expectedDistance);

    // ----------------------------------------------------------------------

    protected override IStringDistanceAlgorithm DistanceAlgorithm => StringMetrics.Distance.Osa;
}

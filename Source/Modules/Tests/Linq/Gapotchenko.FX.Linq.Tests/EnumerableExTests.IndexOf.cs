namespace Gapotchenko.FX.Linq.Tests;

partial class EnumerableExTests
{
    [TestMethod]
    [DataRow("123", '3', 2)]
    [DataRow("123", '4', -1)]
    [DataRow("123", '2', 1)]
    public void Linq_Enumerable_IndexOf_Element(string sequence, char element, int expectedIndex)
    {
        Assert.AreEqual(expectedIndex, sequence.AsEnumerable().IndexOf(element));
    }

    [TestMethod]
    [DataRow("abc", "")]
    [DataRow("abc", "a")]
    [DataRow("abc", "b")]
    [DataRow("abc", "c")]
    [DataRow("abc", "d")]
    [DataRow("abc", "ab")]
    [DataRow("abc", "bc")]
    [DataRow("abc", "abc")]
    [DataRow("abc", "abcd")]
    [DataRow("abc", "efg")]
    [DataRow("abc", "abe")]
    [DataRow("abc", "aec")]
    [DataRow("abc", "ebc")]
    [DataRow("", "")]
    [DataRow("a", "a")]
    [DataRow("a", "abc")]
    [DataRow("b", "abc")]
    [DataRow("c", "abc")]
    [DataRow("12123", "123")]
    public void Linq_Enumerable_IndexOf_Sequence_Discrepancy(string source, string value)
    {
        int expected = source.IndexOf(value, StringComparison.Ordinal);

        int actual = EnumerableEx.IndexOf(source, value);
        Assert.AreEqual(expected, actual);

        long actualLong = EnumerableEx.LongIndexOf(source, value);
        Assert.AreEqual(expected, actualLong);
    }
}

namespace Gapotchenko.FX.Text.Tests;

[TestClass]
public sealed class StringTests
{
    const string Sample = "aBcabC";

    [TestMethod]
    [DataRow(StringComparison.CurrentCulture, 4)]
    [DataRow(StringComparison.CurrentCultureIgnoreCase, 1)]
    [DataRow(StringComparison.InvariantCulture, 4)]
    [DataRow(StringComparison.InvariantCultureIgnoreCase, 1)]
    [DataRow(StringComparison.Ordinal, 4)]
    [DataRow(StringComparison.OrdinalIgnoreCase, 1)]
    public void String_IndexOf_Char_Comparison(StringComparison comparisonType, int expected)
    {
        Assert.AreEqual(expected, StringPolyfills.IndexOf(Sample, 'b', comparisonType));
    }

    [TestMethod]
    [DataRow(StringComparison.CurrentCulture)]
    [DataRow(StringComparison.CurrentCultureIgnoreCase)]
    [DataRow(StringComparison.InvariantCulture)]
    [DataRow(StringComparison.InvariantCultureIgnoreCase)]
    [DataRow(StringComparison.Ordinal)]
    [DataRow(StringComparison.OrdinalIgnoreCase)]
    public void String_IndexOf_Char_StartIndex_Comparison(StringComparison comparisonType)
    {
        Assert.AreEqual(4, StringPolyfills.IndexOf(Sample, 'b', 2, comparisonType));
        Assert.AreEqual(-1, StringPolyfills.IndexOf(Sample, 'b', Sample.Length, comparisonType));
    }

    [TestMethod]
    [DataRow(StringComparison.CurrentCulture, -1)]
    [DataRow(StringComparison.CurrentCultureIgnoreCase, 1)]
    [DataRow(StringComparison.InvariantCulture, -1)]
    [DataRow(StringComparison.InvariantCultureIgnoreCase, 1)]
    [DataRow(StringComparison.Ordinal, -1)]
    [DataRow(StringComparison.OrdinalIgnoreCase, 1)]
    public void String_IndexOf_Char_StartIndex_Count_Comparison(StringComparison comparisonType, int expected)
    {
        Assert.AreEqual(expected, StringPolyfills.IndexOf(Sample, 'b', 0, 3, comparisonType));
        Assert.AreEqual(-1, StringPolyfills.IndexOf(Sample, 'b', 0, 0, comparisonType));
    }

    [TestMethod]
    [DataRow(StringComparison.CurrentCulture, 1)]
    [DataRow(StringComparison.CurrentCultureIgnoreCase, 4)]
    [DataRow(StringComparison.InvariantCulture, 1)]
    [DataRow(StringComparison.InvariantCultureIgnoreCase, 4)]
    [DataRow(StringComparison.Ordinal, 1)]
    [DataRow(StringComparison.OrdinalIgnoreCase, 4)]
    public void String_LastIndexOf_Char_Comparison(StringComparison comparisonType, int expected)
    {
        Assert.AreEqual(expected, StringPolyfills.LastIndexOf(Sample, 'B', comparisonType));
        Assert.AreEqual(-1, StringPolyfills.LastIndexOf(string.Empty, 'B', comparisonType));
    }

    [TestMethod]
    [DataRow(StringComparison.CurrentCulture, -1)]
    [DataRow(StringComparison.CurrentCultureIgnoreCase, 1)]
    [DataRow(StringComparison.InvariantCulture, -1)]
    [DataRow(StringComparison.InvariantCultureIgnoreCase, 1)]
    [DataRow(StringComparison.Ordinal, -1)]
    [DataRow(StringComparison.OrdinalIgnoreCase, 1)]
    public void String_LastIndexOf_Char_StartIndex_Comparison(StringComparison comparisonType, int expected)
    {
        Assert.AreEqual(expected, StringPolyfills.LastIndexOf(Sample, 'b', 3, comparisonType));
    }

    [TestMethod]
    [DataRow(StringComparison.CurrentCulture, -1)]
    [DataRow(StringComparison.CurrentCultureIgnoreCase, 4)]
    [DataRow(StringComparison.InvariantCulture, -1)]
    [DataRow(StringComparison.InvariantCultureIgnoreCase, 4)]
    [DataRow(StringComparison.Ordinal, -1)]
    [DataRow(StringComparison.OrdinalIgnoreCase, 4)]
    public void String_LastIndexOf_Char_StartIndex_Count_Comparison(StringComparison comparisonType, int expected)
    {
        Assert.AreEqual(expected, StringPolyfills.LastIndexOf(Sample, 'B', 5, 2, comparisonType));
        Assert.AreEqual(-1, StringPolyfills.LastIndexOf(Sample, 'B', 5, 0, comparisonType));
    }

    [TestMethod]
    public void String_IndexOf_Char_Comparison_Exceptions()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => StringPolyfills.IndexOf(null!, 'a', StringComparison.Ordinal));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => StringPolyfills.IndexOf(Sample, 'a', -1, StringComparison.Ordinal));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => StringPolyfills.IndexOf(Sample, 'a', 0, Sample.Length + 1, StringComparison.Ordinal));
        Assert.ThrowsExactly<ArgumentException>(() => StringPolyfills.IndexOf(Sample, 'a', (StringComparison)(-1)));
    }

    [TestMethod]
    public void String_LastIndexOf_Char_Comparison_Exceptions()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => StringPolyfills.LastIndexOf(null!, 'a', StringComparison.Ordinal));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => StringPolyfills.LastIndexOf(Sample, 'a', Sample.Length, StringComparison.Ordinal));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => StringPolyfills.LastIndexOf(Sample, 'a', Sample.Length - 1, Sample.Length + 1, StringComparison.Ordinal));
        Assert.ThrowsExactly<ArgumentException>(() => StringPolyfills.LastIndexOf(Sample, 'a', (StringComparison)(-1)));
    }
}

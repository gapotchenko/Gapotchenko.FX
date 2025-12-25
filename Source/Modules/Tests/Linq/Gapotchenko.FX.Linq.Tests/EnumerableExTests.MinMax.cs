using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Linq.Tests;

partial class EnumerableExTests
{
    [TestMethod]
    public void Linq_Enumerable_Min_0_ValueType()
    {
        int[] seq = [];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.Min(comparer: null));
    }

    [TestMethod]
    public void Linq_Enumerable_Min_0_ReferenceType()
    {
        string[] seq = [];
        string? result = seq.Min(comparer: null);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_Min_1()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.Min(comparer: null);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_Min_2()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.Min(Comparer<int>.Default);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_Min_3()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.Min(Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
        Assert.AreEqual(7, result);
    }

    [TestMethod]
    public void Linq_Enumerable_Min_4()
    {
        int[] seq = [];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.Min(comparer: null));
    }

    [TestMethod]
    public void Linq_Enumerable_Max_0_ValueType()
    {
        int[] seq = [];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.Max(comparer: null));
    }

    [TestMethod]
    public void Linq_Enumerable_Max_0_ReferenceType()
    {
        string[] seq = [];
        string? result = seq.Max(comparer: null);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_Max_1()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.Max(comparer: null);
        Assert.AreEqual(7, result);
    }

    [TestMethod]
    public void Linq_Enumerable_Max_2()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.Max(Comparer<int>.Default);
        Assert.AreEqual(7, result);
    }

    [TestMethod]
    public void Linq_Enumerable_Max_3()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.Max(Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinOrDefault_1()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.MinOrDefault(null);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinOrDefault_2()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.MinOrDefault(Comparer<int>.Default);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinOrDefault_3()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.MinOrDefault(Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
        Assert.AreEqual(7, result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinOrDefault_4()
    {
        int[] seq = [];
        int result = seq.MinOrDefault(null);
        Assert.AreEqual(0, result);

        string[] seq2 = [];
        string? result2 = seq2.MinOrDefault(comparer: null);
        Assert.IsNull(result2);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxOrDefault_1()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.MaxOrDefault(null);
        Assert.AreEqual(7, result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxOrDefault_2()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.MaxOrDefault(Comparer<int>.Default);
        Assert.AreEqual(7, result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxOrDefault_3()
    {
        int[] seq = [3, 2, 1, 4, 7, 5];
        int result = seq.MaxOrDefault(Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxOrDefault_4()
    {
        int[] seq = [];
        int result = seq.MaxOrDefault(null);
        Assert.AreEqual(0, result);

        string[] seq2 = [];
        string? result2 = seq2.MaxOrDefault(comparer: null);
        Assert.IsNull(result2);
    }

    [TestMethod]
    public void Linq_Enumerable_MinBy_1()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MinBy(int.Parse);
        Assert.AreEqual("1", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinBy_2()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MinBy(int.Parse, Comparer<int>.Default);
        Assert.AreEqual("1", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinBy_3()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MinBy(int.Parse, Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
        Assert.AreEqual("7", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinBy_4()
    {
        string[] seq = [];
        string? result = seq.MinBy(int.Parse);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinBy_5()
    {
        string[] seq = ["3", "2", " 1 ", "4", " 7 ", "5"];
        string? result = seq.MinBy(int.Parse);
        Assert.AreEqual(" 1 ", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinBy_6()
    {
        int[] seq = [];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.MinBy(Fn.Identity));
    }

    [TestMethod]
    public void Linq_Enumerable_MaxBy_1()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MaxBy(int.Parse);
        Assert.AreEqual("7", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxBy_2()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MaxBy(int.Parse, Comparer<int>.Default);
        Assert.AreEqual("7", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxBy_3()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MaxBy(int.Parse, Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
        Assert.AreEqual("1", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxBy_4()
    {
        string[] seq = [];
        string? result = seq.MaxBy(int.Parse);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxBy_5()
    {
        string[] seq = ["3", "2", " 1 ", "4", " 7 ", "5"];
        string? result = seq.MaxBy(int.Parse);
        Assert.AreEqual(" 7 ", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxBy_6()
    {
        int[] seq = [];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.MaxBy(Fn.Identity));
    }

    [TestMethod]
    public void Linq_Enumerable_MinOrDefaultBy_1()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MinOrDefaultBy(int.Parse);
        Assert.AreEqual("1", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinOrDefaultBy_2()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MinOrDefaultBy(int.Parse, Comparer<int>.Default);
        Assert.AreEqual("1", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinOrDefaultBy_3()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MinOrDefaultBy(int.Parse, Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
        Assert.AreEqual("7", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinOrDefaultBy_4()
    {
        string[] seq = [];
        string? result = seq.MinOrDefaultBy(int.Parse);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_MinOrDefaultBy_5()
    {
        string[] seq = ["3", "2", " 1 ", "4", " 7 ", "5"];
        string? result = seq.MinOrDefaultBy(int.Parse);
        Assert.AreEqual(" 1 ", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxOrDefaultBy_1()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MaxOrDefaultBy(int.Parse);
        Assert.AreEqual("7", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxOrDefaultBy_2()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MaxOrDefaultBy(int.Parse, Comparer<int>.Default);
        Assert.AreEqual("7", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxOrDefaultBy_3()
    {
        string[] seq = ["3", "2", "1", "4", "7", "5"];
        string? result = seq.MaxOrDefaultBy(int.Parse, Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
        Assert.AreEqual("1", result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxOrDefaultBy_4()
    {
        string[] seq = [];
        string? result = seq.MaxOrDefaultBy(int.Parse);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_MaxOrDefaultBy_5()
    {
        string[] seq = ["3", "2", " 1 ", "4", " 7 ", "5"];
        string? result = seq.MaxOrDefaultBy(int.Parse);
        Assert.AreEqual(" 7 ", result);
    }
}

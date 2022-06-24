using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Linq.Tests;

[TestClass]
public partial class EnumerableExTests
{
    [TestMethod]
    public void Linq_Enumerable_StartsWith_1()
    {
        var seq1 = new int[] { 1, 2, 3 };
        var seq2 = new int[] { 1, 2 };
        Assert.IsTrue(seq1.StartsWith(seq2));
    }

    [TestMethod]
    public void Linq_Enumerable_StartsWith_2()
    {
        var seq1 = new int[] { 1, 2, 3 };
        var seq2 = new int[] { 2, 3 };
        Assert.IsFalse(seq1.StartsWith(seq2));
    }

    [TestMethod]
    public void Linq_Enumerable_StartsWith_3()
    {
        var seq1 = new int[] { 1, 2, 3 };
        var seq2 = new int[] { 1, 2, 3 };
        Assert.IsTrue(seq1.StartsWith(seq2));
    }

    [TestMethod]
    public void Linq_Enumerable_StartsWith_4()
    {
        var seq1 = new int[] { 1, 2, 3 };
        var seq2 = new int[] { };
        Assert.IsTrue(seq1.StartsWith(seq2));
    }

    [TestMethod]
    public void Linq_Enumerable_StartsWith_5()
    {
        var seq1 = new int[] { };
        var seq2 = new int[] { };
        Assert.IsTrue(seq1.StartsWith(seq2));
    }

    [TestMethod]
    public void Linq_Enumerable_StartsWith_6()
    {
        var seq1 = new int[] { };
        var seq2 = new int[] { 1, 2, 3 };
        Assert.IsFalse(seq1.StartsWith(seq2));
    }

    [TestMethod]
    public void Linq_Enumerable_StartsWith_7()
    {
        var seq1 = new int[] { 1, 2, 3 };
        var seq2 = new int[] { 1, 2, 3, 4 };
        Assert.IsFalse(seq1.StartsWith(seq2));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_StartsWith_8()
    {
        int[]? seq1 = null;
        int[] seq2 = new int[] { 1, 2, 3 };
        seq1!.StartsWith(seq2);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_StartsWith_9()
    {
        int[] seq1 = new int[] { 1, 2, 3 };
        int[]? seq2 = null;
        seq1.StartsWith(seq2!);
    }

    [TestMethod]
    public void Linq_Enumerable_StartsWith_10()
    {
        var seq1 = new int[] { 1, 2, 3 };
        var seq2 = new int[] { 3, 4, 5 };
        Assert.IsFalse(seq1.StartsWith(seq2));
    }

    [TestMethod]
    public void Linq_Enumerable_IndexOf_1()
    {
        IEnumerable<int> seq = new int[] { 10, 20, 30 };
        Assert.AreEqual(2, seq.IndexOf(30));
    }

    [TestMethod]
    public void Linq_Enumerable_IndexOf_2()
    {
        IEnumerable<int> seq = new int[] { 10, 20, 30 };
        Assert.AreEqual(-1, seq.IndexOf(100));
    }

    [TestMethod]
    public void Linq_Enumerable_IndexOf_3()
    {
        IEnumerable<char> seq = new char[] { 'A', 'B', 'C' };
        Assert.AreEqual(1, seq.IndexOf('B'));
    }

    [TestMethod]
    public void Linq_Enumerable_IndexOf_Match_1()
    {
        static void Check(string source, string value)
        {
            int expected = source.IndexOf(value);
            int actual = EnumerableEx.IndexOf(source, value);
            Assert.AreEqual(expected, actual);
        }

        Check("abc", "");
        Check("abc", "a");
        Check("abc", "b");
        Check("abc", "c");
        Check("abc", "d");

        Check("abc", "ab");
        Check("abc", "bc");
        Check("abc", "abc");
        Check("abc", "abcd");

        Check("abc", "efg");
        Check("abc", "abe");
        Check("abc", "aec");
        Check("abc", "ebc");

        Check("", "");
        Check("a", "a");

        Check("a", "abc");
        Check("b", "abc");
        Check("c", "abc");
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_Empty_1()
    {
        var seq = new int[0];
        bool result = seq.AnyAndAll(_ => true);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_Empty_2()
    {
        var seq = new int[0];
        bool result = seq.AnyAndAll(_ => false);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_OneArg_1()
    {
        var seq = new[] { 5 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_OneArg_2()
    {
        var seq = new[] { 20 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_TwoArgs_1()
    {
        var seq = new[] { 1, 2 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_TwoArgs_2()
    {
        var seq = new[] { 1, 22 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_TwoArgs_3()
    {
        var seq = new[] { 11, 2 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_TwoArgs_4()
    {
        var seq = new[] { 11, 22 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_ThreeArgs_1()
    {
        var seq = new[] { 1, 2, 3 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_ThreeArgs_2()
    {
        var seq = new[] { 1, 2, 33 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_ThreeArgs_3()
    {
        var seq = new[] { 1, 22, 3 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_ThreeArgs_4()
    {
        var seq = new[] { 1, 22, 33 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_ThreeArgs_5()
    {
        var seq = new[] { 11, 2, 3 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_ThreeArgs_6()
    {
        var seq = new[] { 11, 2, 33 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_ThreeArgs_7()
    {
        var seq = new[] { 11, 22, 3 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Linq_Enumerable_AnyAndAll_ThreeArgs_8()
    {
        var seq = new[] { 11, 22, 33 };
        bool result = seq.AnyAndAll(x => x < 10);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_AnyAndAll_NullArg_1()
    {
        IEnumerable<int>? source = null;
        Func<int, bool> predicate = x => false;
        source!.AnyAndAll(predicate);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_AnyAndAll_NullArg_2()
    {
        IEnumerable<int> source = new int[0];
        Func<int, bool>? predicate = null;
        source.AnyAndAll(predicate!);
    }

    [TestMethod]
    public void Linq_Enumerable_ContainsSequence_1()
    {
        Assert.IsTrue(EnumerableEx.Contains("ABCDEF", "CD"));
        Assert.IsFalse(EnumerableEx.Contains("ABCDEF", "DC"));
    }

    [TestMethod]
    public void Linq_Enumerable_ContainsSequence_2()
    {
        Assert.IsTrue(EnumerableEx.Contains("ABCDEF", ""));
    }

    [TestMethod]
    public void Linq_Enumerable_ContainsSequence_3()
    {
        Assert.IsTrue(EnumerableEx.Contains("", ""));
    }

    [TestMethod]
    public void Linq_Enumerable_ContainsSequence_4()
    {
        Assert.IsFalse(EnumerableEx.Contains("", "A"));
    }

    [TestMethod]
    public void Linq_Enumerable_ContainsSequence_5()
    {
        Assert.IsTrue(EnumerableEx.Contains("ABCDEF", "DEF"));
    }

    [TestMethod]
    public void Linq_Enumerable_ContainsSequence_6()
    {
        Assert.IsFalse(EnumerableEx.Contains("ABCDEF", "DEFG"));
    }
}

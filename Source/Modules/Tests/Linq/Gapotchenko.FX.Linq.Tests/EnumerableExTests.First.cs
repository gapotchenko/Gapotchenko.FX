using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Linq.Tests;

partial class EnumerableExTests
{
    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Value_NullSourceArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ((IEnumerable<int>)null!).FirstOrDefault(0));
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Value_Empty()
    {
        int[] seq = [];
        int result = seq.FirstOrDefault(10);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Value_Single()
    {
        int[] seq = [1];
        int result = seq.FirstOrDefault(10);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Value_Multiple()
    {
        int[] seq = [1, 2];
        int result = seq.FirstOrDefault(10);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_NullSourceArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => ((IEnumerable<int>)null!).FirstOrDefault(_ => true, 10));
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_NullPredicateArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => Enumerable.Empty<int>().FirstOrDefault(null!, 10));
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Empty()
    {
        int[] seq = [];
        int result = seq.FirstOrDefault(_ => true, 10);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Empty_NoMatch()
    {
        int[] seq = [];
        int result = seq.FirstOrDefault(_ => false, 10);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Single()
    {
        int[] seq = [1];
        int result = seq.FirstOrDefault(_ => true, 10);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Single_NoMatch()
    {
        int[] seq = [1];
        int result = seq.FirstOrDefault(_ => false, 10);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Multiple()
    {
        int[] seq = [1, 2];
        int result = seq.FirstOrDefault(_ => true, 10);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Multiple_NoMatch()
    {
        int[] seq = [1, 2];
        int result = seq.FirstOrDefault(_ => false, 10);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_NoMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string result = seq.FirstOrDefault(x => x.StartsWith("X", StringComparison.Ordinal), "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_SingleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string result = seq.FirstOrDefault(x => x.StartsWith("D", StringComparison.Ordinal), "X");
        Assert.AreEqual("DEF", result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_MultipleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ", "AMBER"];
        string result = seq.FirstOrDefault(x => x.StartsWith("A", StringComparison.Ordinal), "X");
        Assert.AreEqual("ABC", result);
    }
}

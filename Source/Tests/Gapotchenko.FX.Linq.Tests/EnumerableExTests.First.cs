using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Linq.Tests;

partial class EnumerableExTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_FirstOrDefault_Value_NullSourceArg()
    {
        EnumerableEx.FirstOrDefault<int>(null!, 0);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Value_Empty()
    {
        var seq = new int[0];
        var result = seq.FirstOrDefault(10);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Value_Single()
    {
        var seq = new int[] { 1 };
        var result = seq.FirstOrDefault(10);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Value_Multiple()
    {
        var seq = new int[] { 1, 2 };
        var result = seq.FirstOrDefault(10);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_NullSourceArg()
    {
        EnumerableEx.FirstOrDefault(null!, _ => true, 10);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_NullPredicateArg()
    {
        EnumerableEx.FirstOrDefault(new int[0], null!, 10);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Empty()
    {
        var seq = new int[0];
        var result = seq.FirstOrDefault(_ => true, 10);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Empty_NoMatch()
    {
        var seq = new int[0];
        var result = seq.FirstOrDefault(_ => false, 10);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Single()
    {
        var seq = new int[] { 1 };
        var result = seq.FirstOrDefault(_ => true, 10);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Single_NoMatch()
    {
        var seq = new int[] { 1 };
        var result = seq.FirstOrDefault(_ => false, 10);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Multiple()
    {
        var seq = new int[] { 1, 2 };
        var result = seq.FirstOrDefault(_ => true, 10);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_Multiple_NoMatch()
    {
        var seq = new int[] { 1, 2 };
        var result = seq.FirstOrDefault(_ => false, 10);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_NoMatch()
    {
        var seq = new[] { "ABC", "DEF", "GHJ" };
        var result = seq.FirstOrDefault(x => x.StartsWith("X", StringComparison.Ordinal), "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_SingleMatch()
    {
        var seq = new[] { "ABC", "DEF", "GHJ" };
        var result = seq.FirstOrDefault(x => x.StartsWith("D", StringComparison.Ordinal), "X");
        Assert.AreEqual("DEF", result);
    }

    [TestMethod]
    public void Linq_Enumerable_FirstOrDefault_Predicate_Value_MultpipleMatch()
    {
        var seq = new[] { "ABC", "DEF", "GHJ", "AMBER" };
        var result = seq.FirstOrDefault(x => x.StartsWith("A", StringComparison.Ordinal), "X");
        Assert.AreEqual("ABC", result);
    }
}

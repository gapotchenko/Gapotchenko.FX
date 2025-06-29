using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Linq.Tests;

partial class EnumerableExTests
{
    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_NullSourceArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => EnumerableEx.ScalarOrDefault<int>(null!));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Value_NullSourceArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => EnumerableEx.ScalarOrDefault(null!, "X"));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Empty()
    {
        string[] seq = [];
        string? result = seq.ScalarOrDefault();
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Single()
    {
        string[] seq = ["ABC"];
        string? result = seq.ScalarOrDefault();
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        string? result = seq.ScalarOrDefault();
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        string? result = seq.ScalarOrDefault();
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Value_Empty()
    {
        string[] seq = [];
        string result = seq.ScalarOrDefault("X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Value_Single()
    {
        string[] seq = ["ABC"];
        string result = seq.ScalarOrDefault("X");
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Value_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        string result = seq.ScalarOrDefault("X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Value_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        string result = seq.ScalarOrDefault("X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_NullSeqArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => EnumerableEx.ScalarOrDefault<int>(null!, _ => true));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_NullPredicateArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => EnumerableEx.ScalarOrDefault(new int[0], null!));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_NullSeqArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => EnumerableEx.ScalarOrDefault(null!, _ => true, 10));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_NullPredicateArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => EnumerableEx.ScalarOrDefault([], null!, 10));
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Empty()
    {
        string[] seq = [];
        string? result = seq.ScalarOrDefault(_ => true);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Empty_NoMatch()
    {
        string[] seq = [];
        string? result = seq.ScalarOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Empty()
    {
        string[] seq = [];
        string result = seq.ScalarOrDefault(_ => true, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Empty_NoMatch()
    {
        string[] seq = [];
        string result = seq.ScalarOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Single()
    {
        string[] seq = ["ABC"];
        string? result = seq.ScalarOrDefault(_ => true);
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Single_NoMatch()
    {
        string[] seq = ["ABC"];
        string? result = seq.ScalarOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Single()
    {
        string[] seq = ["ABC"];
        string result = seq.ScalarOrDefault(_ => true, "X");
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Single_NoMatch()
    {
        string[] seq = ["ABC"];
        string result = seq.ScalarOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        string? result = seq.ScalarOrDefault(_ => true);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleDiff_NoMatch()
    {
        string[] seq = ["ABC", "DEF"];
        string? result = seq.ScalarOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        string result = seq.ScalarOrDefault(_ => true, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleDiff_NoMatch()
    {
        string[] seq = ["ABC", "DEF"];
        string result = seq.ScalarOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        string? result = seq.ScalarOrDefault(_ => true);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleSame_NoMatch()
    {
        string[] seq = ["ABC", "ABC"];
        string? result = seq.ScalarOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        string result = seq.ScalarOrDefault(_ => true, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleSame_NoMatch()
    {
        string[] seq = ["ABC", "ABC"];
        string result = seq.ScalarOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_NoMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string? result = seq.ScalarOrDefault(x => x.StartsWith("Z", StringComparison.Ordinal));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_SingleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string? result = seq.ScalarOrDefault(x => x.StartsWith("D", StringComparison.Ordinal));
        Assert.AreEqual("DEF", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_MultpipleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ", "AMBER"];
        string? result = seq.ScalarOrDefault(x => x.StartsWith("A", StringComparison.Ordinal));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_NoMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string result = seq.ScalarOrDefault(x => x.StartsWith("Z", StringComparison.Ordinal), "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_SingleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string result = seq.ScalarOrDefault(x => x.StartsWith("D", StringComparison.Ordinal), "X");
        Assert.AreEqual("DEF", result);
    }

    [TestMethod]
    public void Linq_Enumerable_ScalarOrDefault_Predicate_Valye_MultpipleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ", "AMBER"];
        string result = seq.ScalarOrDefault(x => x.StartsWith("A", StringComparison.Ordinal), "X");
        Assert.AreEqual("X", result);
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Linq.Tests;

partial class EnumerableExTests
{
    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_NullSourceArg()
    {
        IEnumerable<int>? seq = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => seq!.SingleOrDefault());
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Value_NullSourceArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => Utils.NullEnumerable<string>().SingleOrDefault("X"));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Empty()
    {
        string[] seq = [];
        string? result = seq.SingleOrDefault();
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Single()
    {
        string[] seq = ["ABC"];
        string? result = seq.SingleOrDefault();
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.SingleOrDefault());
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.SingleOrDefault());
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Value_Empty()
    {
        string[] seq = [];
        string result = seq.SingleOrDefault("X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Value_Single()
    {
        string[] seq = ["ABC"];
        string result = seq.SingleOrDefault("X");
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Value_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.SingleOrDefault("X"));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Value_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.SingleOrDefault("X"));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_NullSeqArg()
    {
        IEnumerable<int>? seq = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => seq!.SingleOrDefault(_ => true));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_NullPredicateArg()
    {
        int[] seq = [];
        Assert.ThrowsExactly<ArgumentNullException>(() => seq.SingleOrDefault(null!));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_NullSeqArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => Utils.NullEnumerable<int>().SingleOrDefault(_ => true, 10));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_NullPredicateArg()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => Enumerable.Empty<int>().SingleOrDefault(null!, 10));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Empty()
    {
        string[] seq = [];
        string? result = seq.SingleOrDefault(_ => true);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Empty_NoMatch()
    {
        string[] seq = [];
        string? result = seq.SingleOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_Empty()
    {
        string[] seq = [];
        string result = seq.SingleOrDefault(_ => true, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_Empty_NoMatch()
    {
        string[] seq = [];
        string result = seq.SingleOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Single()
    {
        string[] seq = ["ABC"];
        string? result = seq.SingleOrDefault(_ => true);
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Single_NoMatch()
    {
        string[] seq = ["ABC"];
        string? result = seq.SingleOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_Single()
    {
        string[] seq = ["ABC"];
        string result = seq.SingleOrDefault(_ => true, "X");
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_Single_NoMatch()
    {
        string[] seq = ["ABC"];
        string result = seq.SingleOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.SingleOrDefault(_ => true));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_MultipleDiff_NoMatch()
    {
        string[] seq = ["ABC", "DEF"];
        string? result = seq.SingleOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_MultipleDiff()
    {
        string[] seq = ["ABC", "DEF"];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.SingleOrDefault(_ => true, "X"));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_MultipleDiff_NoMatch()
    {
        string[] seq = ["ABC", "DEF"];
        string result = seq.SingleOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.SingleOrDefault(_ => true));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_MultipleSame_NoMatch()
    {
        string[] seq = ["ABC", "ABC"];
        string? result = seq.SingleOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_MultipleSame()
    {
        string[] seq = ["ABC", "ABC"];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.SingleOrDefault(_ => true, "X"));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_MultipleSame_NoMatch()
    {
        string[] seq = ["ABC", "ABC"];
        string result = seq.SingleOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_NoMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string? result = seq.SingleOrDefault(x => x.StartsWith("Z", StringComparison.Ordinal));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_SingleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string? result = seq.SingleOrDefault(x => x.StartsWith("D", StringComparison.Ordinal));
        Assert.AreEqual("DEF", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_MultipleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ", "AMBER"];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.SingleOrDefault(x => x.StartsWith("A", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_NoMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string result = seq.SingleOrDefault(x => x.StartsWith("Z", StringComparison.Ordinal), "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_SingleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ"];
        string result = seq.SingleOrDefault(x => x.StartsWith("D", StringComparison.Ordinal), "X");
        Assert.AreEqual("DEF", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_MultipleMatch()
    {
        string[] seq = ["ABC", "DEF", "GHJ", "AMBER"];
        Assert.ThrowsExactly<InvalidOperationException>(() => seq.SingleOrDefault(x => x.StartsWith("A", StringComparison.Ordinal), "X"));
    }
}

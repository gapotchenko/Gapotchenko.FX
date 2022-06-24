using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Linq.Tests;

partial class EnumerableExTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_SingleOrDefault_NullSourceArg()
    {
        IEnumerable<int>? seq = null;
        seq!.SingleOrDefault();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_SingleOrDefault_Value_NullSourceArg()
    {
        EnumerableEx.SingleOrDefault(null!, "X");
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Empty()
    {
        var seq = new string[0];
        var result = seq.SingleOrDefault();
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Single()
    {
        var seq = new[] { "ABC" };
        var result = seq.SingleOrDefault();
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Linq_Enumerable_SingleOrDefault_MultipleDiff()
    {
        var seq = new[] { "ABC", "DEF" };
        seq.SingleOrDefault();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Linq_Enumerable_SingleOrDefault_MultipleSame()
    {
        var seq = new[] { "ABC", "ABC" };
        seq.SingleOrDefault();
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Value_Empty()
    {
        var seq = new string[0];
        var result = seq.SingleOrDefault("X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Value_Single()
    {
        var seq = new[] { "ABC" };
        var result = seq.SingleOrDefault("X");
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Linq_Enumerable_SingleOrDefault_Value_MultipleDiff()
    {
        var seq = new[] { "ABC", "DEF" };
        seq.SingleOrDefault("X");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Linq_Enumerable_SingleOrDefault_Value_MultipleSame()
    {
        var seq = new[] { "ABC", "ABC" };
        seq.SingleOrDefault("X");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_SingleOrDefault_Predicate_NullSeqArg()
    {
        IEnumerable<int>? seq = null;
        seq!.SingleOrDefault(_ => true);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_SingleOrDefault_Predicate_NullPredicateArg()
    {
        var seq = new int[0];
        seq.SingleOrDefault(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_NullSeqArg()
    {
        EnumerableEx.SingleOrDefault(null!, _ => true, 10);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_NullPredicateArg()
    {
        EnumerableEx.SingleOrDefault(new int[0], null!, 10);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Empty()
    {
        var seq = new string[0];
        var result = seq.SingleOrDefault(_ => true);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Empty_NoMatch()
    {
        var seq = new string[0];
        var result = seq.SingleOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_Empty()
    {
        var seq = new string[0];
        var result = seq.SingleOrDefault(_ => true, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_Empty_NoMatch()
    {
        var seq = new string[0];
        var result = seq.SingleOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Single()
    {
        var seq = new[] { "ABC" };
        var result = seq.SingleOrDefault(_ => true);
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Single_NoMatch()
    {
        var seq = new[] { "ABC" };
        var result = seq.SingleOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_Single()
    {
        var seq = new[] { "ABC" };
        var result = seq.SingleOrDefault(_ => true, "X");
        Assert.AreEqual("ABC", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_Single_NoMatch()
    {
        var seq = new[] { "ABC" };
        var result = seq.SingleOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Linq_Enumerable_SingleOrDefault_Predicate_MultipleDiff()
    {
        var seq = new[] { "ABC", "DEF" };
        seq.SingleOrDefault(_ => true);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_MultipleDiff_NoMatch()
    {
        var seq = new[] { "ABC", "DEF" };
        var result = seq.SingleOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_MultipleDiff()
    {
        var seq = new[] { "ABC", "DEF" };
        seq.SingleOrDefault(_ => true, "X");
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_MultipleDiff_NoMatch()
    {
        var seq = new[] { "ABC", "DEF" };
        var result = seq.SingleOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Linq_Enumerable_SingleOrDefault_Predicate_MultipleSame()
    {
        var seq = new[] { "ABC", "ABC" };
        seq.SingleOrDefault(_ => true);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_MultipleSame_NoMatch()
    {
        var seq = new[] { "ABC", "ABC" };
        var result = seq.SingleOrDefault(_ => false);
        Assert.IsNull(result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_MultipleSame()
    {
        var seq = new[] { "ABC", "ABC" };
        seq.SingleOrDefault(_ => true, "X");
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_MultipleSame_NoMatch()
    {
        var seq = new[] { "ABC", "ABC" };
        var result = seq.SingleOrDefault(_ => false, "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_NoMatch()
    {
        var seq = new[] { "ABC", "DEF", "GHJ" };
        var result = seq.SingleOrDefault(x => x.StartsWith("Z", StringComparison.Ordinal));
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_SingleMatch()
    {
        var seq = new[] { "ABC", "DEF", "GHJ" };
        var result = seq.SingleOrDefault(x => x.StartsWith("D", StringComparison.Ordinal));
        Assert.AreEqual("DEF", result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Linq_Enumerable_SingleOrDefault_Predicate_MultpipleMatch()
    {
        var seq = new[] { "ABC", "DEF", "GHJ", "AMBER" };
        seq.SingleOrDefault(x => x.StartsWith("A", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_NoMatch()
    {
        var seq = new[] { "ABC", "DEF", "GHJ" };
        var result = seq.SingleOrDefault(x => x.StartsWith("Z", StringComparison.Ordinal), "X");
        Assert.AreEqual("X", result);
    }

    [TestMethod]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Value_SingleMatch()
    {
        var seq = new[] { "ABC", "DEF", "GHJ" };
        var result = seq.SingleOrDefault(x => x.StartsWith("D", StringComparison.Ordinal), "X");
        Assert.AreEqual("DEF", result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Linq_Enumerable_SingleOrDefault_Predicate_Valye_MultpipleMatch()
    {
        var seq = new[] { "ABC", "DEF", "GHJ", "AMBER" };
        seq.SingleOrDefault(x => x.StartsWith("A", StringComparison.Ordinal), "X");
    }
}

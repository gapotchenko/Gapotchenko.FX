using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Gapotchenko.FX.Linq.Tests
{
    partial class EnumerableExTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Linq_Enumerable_ScalarOrDefault_NullSourceArg()
        {
            EnumerableEx.ScalarOrDefault<int>(null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Linq_Enumerable_ScalarOrDefault_Value_NullSourceArg()
        {
            EnumerableEx.ScalarOrDefault(null!, "X");
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Empty()
        {
            var seq = new string[0];
            var result = seq.ScalarOrDefault();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Single()
        {
            var seq = new[] { "ABC" };
            var result = seq.ScalarOrDefault();
            Assert.AreEqual("ABC", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_MultipleDiff()
        {
            var seq = new[] { "ABC", "DEF" };
            var result = seq.ScalarOrDefault();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_MultipleSame()
        {
            var seq = new[] { "ABC", "ABC" };
            var result = seq.ScalarOrDefault();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Value_Empty()
        {
            var seq = new string[0];
            var result = seq.ScalarOrDefault("X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Value_Single()
        {
            var seq = new[] { "ABC" };
            var result = seq.ScalarOrDefault("X");
            Assert.AreEqual("ABC", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Value_MultipleDiff()
        {
            var seq = new[] { "ABC", "DEF" };
            var result = seq.ScalarOrDefault("X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Value_MultipleSame()
        {
            var seq = new[] { "ABC", "ABC" };
            var result = seq.ScalarOrDefault("X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_NullSeqArg()
        {
            EnumerableEx.ScalarOrDefault<int>(null!, _ => true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_NullPredicateArg()
        {
            EnumerableEx.ScalarOrDefault(new int[0], null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_NullSeqArg()
        {
            EnumerableEx.ScalarOrDefault(null!, _ => true, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_NullPredicateArg()
        {
            EnumerableEx.ScalarOrDefault(new int[0], null!, 10);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Empty()
        {
            var seq = new string[0];
            var result = seq.ScalarOrDefault(_ => true);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Empty_NoMatch()
        {
            var seq = new string[0];
            var result = seq.ScalarOrDefault(_ => false);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Empty()
        {
            var seq = new string[0];
            var result = seq.ScalarOrDefault(_ => true, "X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Empty_NoMatch()
        {
            var seq = new string[0];
            var result = seq.ScalarOrDefault(_ => false, "X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Single()
        {
            var seq = new[] { "ABC" };
            var result = seq.ScalarOrDefault(_ => true);
            Assert.AreEqual("ABC", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Single_NoMatch()
        {
            var seq = new[] { "ABC" };
            var result = seq.ScalarOrDefault(_ => false);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Single()
        {
            var seq = new[] { "ABC" };
            var result = seq.ScalarOrDefault(_ => true, "X");
            Assert.AreEqual("ABC", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_Single_NoMatch()
        {
            var seq = new[] { "ABC" };
            var result = seq.ScalarOrDefault(_ => false, "X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleDiff()
        {
            var seq = new[] { "ABC", "DEF" };
            var result = seq.ScalarOrDefault(_ => true);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleDiff_NoMatch()
        {
            var seq = new[] { "ABC", "DEF" };
            var result = seq.ScalarOrDefault(_ => false);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleDiff()
        {
            var seq = new[] { "ABC", "DEF" };
            var result = seq.ScalarOrDefault(_ => true, "X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleDiff_NoMatch()
        {
            var seq = new[] { "ABC", "DEF" };
            var result = seq.ScalarOrDefault(_ => false, "X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleSame()
        {
            var seq = new[] { "ABC", "ABC" };
            var result = seq.ScalarOrDefault(_ => true);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleSame_NoMatch()
        {
            var seq = new[] { "ABC", "ABC" };
            var result = seq.ScalarOrDefault(_ => false);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleSame()
        {
            var seq = new[] { "ABC", "ABC" };
            var result = seq.ScalarOrDefault(_ => true, "X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_MultipleSame_NoMatch()
        {
            var seq = new[] { "ABC", "ABC" };
            var result = seq.ScalarOrDefault(_ => false, "X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_NoMatch()
        {
            var seq = new[] { "ABC", "DEF", "GHJ" };
            var result = seq.ScalarOrDefault(x => x.StartsWith("Z", StringComparison.Ordinal));
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_SingleMatch()
        {
            var seq = new[] { "ABC", "DEF", "GHJ" };
            var result = seq.ScalarOrDefault(x => x.StartsWith("D", StringComparison.Ordinal));
            Assert.AreEqual("DEF", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_MultpipleMatch()
        {
            var seq = new[] { "ABC", "DEF", "GHJ", "AMBER" };
            var result = seq.ScalarOrDefault(x => x.StartsWith("A", StringComparison.Ordinal));
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_NoMatch()
        {
            var seq = new[] { "ABC", "DEF", "GHJ" };
            var result = seq.ScalarOrDefault(x => x.StartsWith("Z", StringComparison.Ordinal), "X");
            Assert.AreEqual("X", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Value_SingleMatch()
        {
            var seq = new[] { "ABC", "DEF", "GHJ" };
            var result = seq.ScalarOrDefault(x => x.StartsWith("D", StringComparison.Ordinal), "X");
            Assert.AreEqual("DEF", result);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Valye_MultpipleMatch()
        {
            var seq = new[] { "ABC", "DEF", "GHJ", "AMBER" };
            var result = seq.ScalarOrDefault(x => x.StartsWith("A", StringComparison.Ordinal), "X");
            Assert.AreEqual("X", result);
        }
    }
}

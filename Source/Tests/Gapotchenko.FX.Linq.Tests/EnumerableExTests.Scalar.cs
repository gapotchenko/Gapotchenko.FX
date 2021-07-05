using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Gapotchenko.FX.Linq.Tests
{
    partial class EnumerableExTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Linq_Enumerable_ScalarOrDefault_NullArg()
        {
            EnumerableEx.ScalarOrDefault<int>(null!);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Empty()
        {
            var seq = new string[0];
            var scalar = seq.ScalarOrDefault();
            Assert.IsNull(scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Single()
        {
            var seq = new[] { "ABC" };
            var scalar = seq.ScalarOrDefault();
            Assert.AreEqual("ABC", scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_MultipleDiff()
        {
            var seq = new[] { "ABC", "DEF" };
            var scalar = seq.ScalarOrDefault();
            Assert.IsNull(scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_MultipleSame()
        {
            var seq = new[] { "ABC", "ABC" };
            var scalar = seq.ScalarOrDefault();
            Assert.IsNull(scalar);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_NullSeqArg()
        {
            EnumerableEx.ScalarOrDefault<int>(null!, x => true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_NullPredicateArg()
        {
            EnumerableEx.ScalarOrDefault(new int[0], null!);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Empty()
        {
            var seq = new string[0];
            var scalar = seq.ScalarOrDefault(x => true);
            Assert.IsNull(scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Empty_NoMatch()
        {
            var seq = new string[0];
            var scalar = seq.ScalarOrDefault(x => false);
            Assert.IsNull(scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Single()
        {
            var seq = new[] { "ABC" };
            var scalar = seq.ScalarOrDefault(x => true);
            Assert.AreEqual("ABC", scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_Single_NoMatch()
        {
            var seq = new[] { "ABC" };
            var scalar = seq.ScalarOrDefault(x => false);
            Assert.IsNull(scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleDiff()
        {
            var seq = new[] { "ABC", "DEF" };
            var scalar = seq.ScalarOrDefault(x => true);
            Assert.IsNull(scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleDiff_NoMatch()
        {
            var seq = new[] { "ABC", "DEF" };
            var scalar = seq.ScalarOrDefault(x => false);
            Assert.IsNull(scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleSame()
        {
            var seq = new[] { "ABC", "ABC" };
            var scalar = seq.ScalarOrDefault(x => true);
            Assert.IsNull(scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_MultipleSame_NoMatch()
        {
            var seq = new[] { "ABC", "ABC" };
            var scalar = seq.ScalarOrDefault(x => false);
            Assert.IsNull(scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_NoMatch()
        {
            var seq = new[] { "ABC", "DEF", "GHJ" };
            var scalar = seq.ScalarOrDefault(x => x.StartsWith("X", StringComparison.Ordinal));
            Assert.IsNull(scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_SingleMatch()
        {
            var seq = new[] { "ABC", "DEF", "GHJ" };
            var scalar = seq.ScalarOrDefault(x => x.StartsWith("D", StringComparison.Ordinal));
            Assert.AreEqual("DEF", scalar);
        }

        [TestMethod]
        public void Linq_Enumerable_ScalarOrDefault_Predicate_MultpipleMatch()
        {
            var seq = new[] { "ABC", "DEF", "GHJ", "AMBER" };
            var scalar = seq.ScalarOrDefault(x => x.StartsWith("A", StringComparison.Ordinal));
            Assert.IsNull(scalar);
        }
    }
}

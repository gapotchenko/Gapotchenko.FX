using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Linq.Tests
{
    [TestClass]
    public class EnumerableExTests
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void Linq_Enumerable_FirstOrDefault_Value_NullArg()
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Linq_Enumerable_Min_0_ValueType()
        {
            var seq = new int[0];
            seq.Min(comparer: null);
        }

        [TestMethod]
        public void Linq_Enumerable_Min_0_ReferenceType()
        {
            var seq = new string[0];
            var result = seq.Min(comparer: null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_Min_1()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.Min(comparer: null);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Linq_Enumerable_Min_2()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.Min(Comparer<int>.Default);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Linq_Enumerable_Min_3()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.Min(Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Linq_Enumerable_Min_4()
        {
            var seq = new int[0];
            seq.Min(comparer: null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Linq_Enumerable_Max_0_ValueType()
        {
            var seq = new int[0];
            seq.Max(comparer: null);
        }

        [TestMethod]
        public void Linq_Enumerable_Max_0_ReferenceType()
        {
            var seq = new string[0];
            var result = seq.Max(comparer: null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_Max_1()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.Max(comparer: null);
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void Linq_Enumerable_Max_2()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.Max(Comparer<int>.Default);
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void Linq_Enumerable_Max_3()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.Max(Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinOrDefault_1()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.MinOrDefault(null);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinOrDefault_2()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.MinOrDefault(Comparer<int>.Default);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinOrDefault_3()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.MinOrDefault(Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinOrDefault_4()
        {
            var seq = new int[0];
            var result = seq.MinOrDefault(null);
            Assert.AreEqual(0, result);

            var seq2 = new string[0];
            var result2 = seq2.MinOrDefault(null);
            Assert.IsNull(result2);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxOrDefault_1()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.MaxOrDefault(null);
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxOrDefault_2()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.MaxOrDefault(Comparer<int>.Default);
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxOrDefault_3()
        {
            var seq = new[] { 3, 2, 1, 4, 7, 5 };
            var result = seq.MaxOrDefault(Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxOrDefault_4()
        {
            var seq = new int[0];
            var result = seq.MaxOrDefault(null);
            Assert.AreEqual(0, result);

            var seq2 = new string[0];
            var result2 = seq2.MaxOrDefault(null);
            Assert.IsNull(result2);
        }

        [TestMethod]
        public void Linq_Enumerable_MinBy_1()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MinBy(int.Parse);
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinBy_2()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MinBy(int.Parse, Comparer<int>.Default);
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinBy_3()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MinBy(int.Parse, Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
            Assert.AreEqual("7", result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Linq_Enumerable_MinBy_4()
        {
            var seq = new string[0];
            seq.MinBy(int.Parse);
        }

        [TestMethod]
        public void Linq_Enumerable_MinBy_5()
        {
            var seq = new[] { "3", "2", " 1 ", "4", " 7 ", "5" };
            var result = seq.MinBy(int.Parse);
            Assert.AreEqual(" 1 ", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxBy_1()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MaxBy(int.Parse);
            Assert.AreEqual("7", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxBy_2()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MaxBy(int.Parse, Comparer<int>.Default);
            Assert.AreEqual("7", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxBy_3()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MaxBy(int.Parse, Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Linq_Enumerable_MaxBy_4()
        {
            var seq = new string[0];
            seq.MaxBy(int.Parse);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxBy_5()
        {
            var seq = new[] { "3", "2", " 1 ", "4", " 7 ", "5" };
            var result = seq.MaxBy(int.Parse);
            Assert.AreEqual(" 7 ", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinOrDefaultBy_1()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MinOrDefaultBy(int.Parse);
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinOrDefaultBy_2()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MinOrDefaultBy(int.Parse, Comparer<int>.Default);
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinOrDefaultBy_3()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MinOrDefaultBy(int.Parse, Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
            Assert.AreEqual("7", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinOrDefaultBy_4()
        {
            var seq = new string[0];
            var result = seq.MinOrDefaultBy(int.Parse);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_MinOrDefaultBy_5()
        {
            var seq = new[] { "3", "2", " 1 ", "4", " 7 ", "5" };
            var result = seq.MinOrDefaultBy(int.Parse);
            Assert.AreEqual(" 1 ", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxOrDefaultBy_1()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MaxOrDefaultBy(int.Parse);
            Assert.AreEqual("7", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxOrDefaultBy_2()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MaxOrDefaultBy(int.Parse, Comparer<int>.Default);
            Assert.AreEqual("7", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxOrDefaultBy_3()
        {
            var seq = new[] { "3", "2", "1", "4", "7", "5" };
            var result = seq.MaxOrDefaultBy(int.Parse, Comparer<int>.Create((x, y) => -Comparer<int>.Default.Compare(x, y)));
            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxOrDefaultBy_4()
        {
            var seq = new string[0];
            var result = seq.MaxOrDefaultBy(int.Parse);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Linq_Enumerable_MaxOrDefaultBy_5()
        {
            var seq = new[] { "3", "2", " 1 ", "4", " 7 ", "5" };
            var result = seq.MaxOrDefaultBy(int.Parse);
            Assert.AreEqual(" 7 ", result);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Linq.Tests
{
    partial class EnumerableExTests
    {
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
            var result2 = seq2.MinOrDefault(comparer: null);
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
            var result2 = seq2.MaxOrDefault(comparer: null);
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Collections.Tests.Generic.Kit
{
    [TestClass]
    public class ReadOnlySetBaseTests
    {
        [TestMethod]
        public void ReadOnlySetBase_IsProperSubsetOf()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsTrue(s1.IsProperSubsetOf(s));

            foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
                Assert.IsFalse(s1.IsProperSubsetOf(s));

            foreach (var s in Util.Sets(new[] { 1, 2 }))
                Assert.IsFalse(s1.IsProperSubsetOf(s));

            foreach (var s in Util.SetsEnumerable(s1))
                Assert.IsFalse(s1.IsProperSubsetOf(s));

            var s2 = ReadOnlySetChimera<int>.Empty;

            foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s2.IsProperSubsetOf(s));

            foreach (var s in Util.Sets(Empty<int>.Array))
                Assert.IsFalse(s2.IsProperSubsetOf(s));

            foreach (var s in Util.SetsEnumerable(s2))
                Assert.IsFalse(s2.IsProperSubsetOf(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_IsProperSupersetOf()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Util.Sets(Empty<int>.Array))
                Assert.IsTrue(s1.IsProperSupersetOf(s));

            foreach (var s in Util.Sets(new[] { 1, 2 }))
                Assert.IsTrue(s1.IsProperSupersetOf(s));

            foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
                Assert.IsFalse(s1.IsProperSupersetOf(s));

            foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsFalse(s1.IsProperSupersetOf(s));

            foreach (var s in Util.SetsEnumerable(s1))
                Assert.IsFalse(s1.IsProperSupersetOf(s));

            var s2 = ReadOnlySetChimera<int>.Empty;

            foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
                Assert.IsFalse(s2.IsProperSupersetOf(s));

            foreach (var s in Util.Sets(Empty<int>.Array))
                Assert.IsFalse(s2.IsProperSupersetOf(s));

            foreach (var s in Util.SetsEnumerable(s2))
                Assert.IsFalse(s2.IsProperSupersetOf(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_IsSubsetOf()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsTrue(s1.IsSubsetOf(s));

            foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s1.IsSubsetOf(s));

            foreach (var s in Util.Sets(new[] { 1, 2 }))
                Assert.IsFalse(s1.IsSubsetOf(s));

            foreach (var s in Util.SetsEnumerable(s1))
                Assert.IsTrue(s1.IsSubsetOf(s));

            var s2 = ReadOnlySetChimera<int>.Empty;

            foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s2.IsSubsetOf(s));

            foreach (var s in Util.Sets(Empty<int>.Array))
                Assert.IsTrue(s2.IsSubsetOf(s));

            foreach (var s in Util.SetsEnumerable(s2))
                Assert.IsTrue(s2.IsSubsetOf(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_IsSupersetOf()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Util.Sets(Empty<int>.Array))
                Assert.IsTrue(s1.IsSupersetOf(s));

            foreach (var s in Util.Sets(new[] { 1, 2 }))
                Assert.IsTrue(s1.IsSupersetOf(s));

            foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s1.IsSupersetOf(s));

            foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsFalse(s1.IsSupersetOf(s));

            foreach (var s in Util.SetsEnumerable(s1))
                Assert.IsTrue(s1.IsSupersetOf(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_Overlaps()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Util.Sets(Empty<int>.Array))
                Assert.IsFalse(s1.Overlaps(s));

            foreach (var s in Util.Sets(new[] { 1, 2 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Util.Sets(new[] { 10, 20 }))
                Assert.IsFalse(s1.Overlaps(s));

            foreach (var s in Util.Sets(new[] { 10, 20, 30 }))
                Assert.IsFalse(s1.Overlaps(s));

            foreach (var s in Util.Sets(new[] { 10, 20, 30, 40 }))
                Assert.IsFalse(s1.Overlaps(s));

            foreach (var s in Util.Sets(new[] { 1, 20 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Util.Sets(new[] { 10, 2, 30 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Util.Sets(new[] { 10, 20, 3, 40 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Util.SetsEnumerable(s1))
                Assert.IsTrue(s1.Overlaps(s));

            var s2 = ReadOnlySetChimera<int>.Empty;

            foreach (var s in Util.SetsEnumerable(s2))
                Assert.IsFalse(s2.Overlaps(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_SetEquals()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Util.Sets(Empty<int>.Array))
                Assert.IsFalse(s1.SetEquals(s));

            foreach (var s in Util.Sets(new[] { 1, 2 }))
                Assert.IsFalse(s1.SetEquals(s));

            foreach (var s in Util.Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s1.SetEquals(s));

            foreach (var s in Util.Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsFalse(s1.SetEquals(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_CopyTo()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 1, 2, 3 });

            int[] arr;

            arr = new int[3];
            s1.CopyTo(arr);
            Assert.IsTrue(new HashSet<int>(arr).SetEquals(s1));

            arr = new int[2];
            Assert.ThrowsException<ArgumentException>(() => s1.CopyTo(arr));

            arr = new int[4];
            s1.CopyTo(arr);
            Assert.IsTrue(new HashSet<int>(arr.Take(3)).SetEquals(new[] { 1, 2, 3 }));
            Assert.AreEqual(0, arr[3]);

            arr = new int[4];
            s1.CopyTo(arr, 1);
            Assert.IsTrue(new HashSet<int>(arr.Skip(1)).SetEquals(new[] { 1, 2, 3 }));
            Assert.AreEqual(0, arr[0]);

            arr = new int[4];
            s1.CopyTo(arr, 1, 2);
            Assert.IsTrue(new HashSet<int>(arr.Skip(1).Take(2)).SetEquals(new[] { 1, 2 }));
            Assert.AreEqual(0, arr[0]);
            Assert.AreEqual(0, arr[3]);
        }
    }
}

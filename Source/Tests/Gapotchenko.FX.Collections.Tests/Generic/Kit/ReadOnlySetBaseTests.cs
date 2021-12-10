using Gapotchenko.FX.Collections.Generic.Kit;
using Gapotchenko.FX.Math.Combinatorics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Collections.Tests.Generic.Kit
{
    [TestClass]
    public class ReadOnlySetBaseTests
    {
        sealed class ReadOnlySetChimera<T> : ReadOnlySetBase<T>
        {
            public ReadOnlySetChimera(HashSet<T> baseSet)
            {
                m_BaseSet = baseSet;
            }

            readonly HashSet<T> m_BaseSet;

            public override IEqualityComparer<T> Comparer => m_BaseSet.Comparer;

            public override int Count => m_BaseSet.Count;

            public override bool Contains(T item) => m_BaseSet.Contains(item);

            public override IEnumerator<T> GetEnumerator() => m_BaseSet.GetEnumerator();

            public static ReadOnlySetChimera<T> Empty => EmptyFactory.Instance;

            static class EmptyFactory
            {
                public static ReadOnlySetChimera<T> Instance { get; } = new(new());
            }
        }

        static IEnumerable<IEnumerable<T>> SetsEnumerable<T>(IEnumerable<T> elements)
        {
            yield return elements;

            static IEnumerable<IEnumerable<T>> Repack(IEnumerable<T> elements)
            {
                yield return elements;

                static IEnumerable<T> Enumerate(IEnumerable<T> source)
                {
                    foreach (var i in source)
                        yield return i;
                }

                yield return Enumerate(elements);

                if (elements is not Array)
                    yield return elements.ToArray();

                var hs = new HashSet<T>(elements);
                yield return hs;

                yield return new ReadOnlySetChimera<T>(hs);
            }

            foreach (var p in elements.Permute())
                foreach (var i in Repack(p))
                    yield return i;
        }

        static IEnumerable<IEnumerable<T>> Sets<T>(params T[] elements) => SetsEnumerable(elements);

        [TestMethod]
        public void ReadOnlySetBase_IsProperSubsetOf()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsTrue(s1.IsProperSubsetOf(s));

            foreach (var s in Sets(new[] { 1, 2, 3 }))
                Assert.IsFalse(s1.IsProperSubsetOf(s));

            foreach (var s in Sets(new[] { 1, 2 }))
                Assert.IsFalse(s1.IsProperSubsetOf(s));

            foreach (var s in SetsEnumerable(s1))
                Assert.IsFalse(s1.IsProperSubsetOf(s));

            var s2 = ReadOnlySetChimera<int>.Empty;

            foreach (var s in Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s2.IsProperSubsetOf(s));

            foreach (var s in Sets(Empty<int>.Array))
                Assert.IsFalse(s2.IsProperSubsetOf(s));

            foreach (var s in SetsEnumerable(s2))
                Assert.IsFalse(s2.IsProperSubsetOf(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_IsProperSupersetOf()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Sets(Empty<int>.Array))
                Assert.IsTrue(s1.IsProperSupersetOf(s));

            foreach (var s in Sets(new[] { 1, 2 }))
                Assert.IsTrue(s1.IsProperSupersetOf(s));

            foreach (var s in Sets(new[] { 1, 2, 3 }))
                Assert.IsFalse(s1.IsProperSupersetOf(s));

            foreach (var s in Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsFalse(s1.IsProperSupersetOf(s));

            foreach (var s in SetsEnumerable(s1))
                Assert.IsFalse(s1.IsProperSupersetOf(s));

            var s2 = ReadOnlySetChimera<int>.Empty;

            foreach (var s in Sets(new[] { 1, 2, 3 }))
                Assert.IsFalse(s2.IsProperSupersetOf(s));

            foreach (var s in Sets(Empty<int>.Array))
                Assert.IsFalse(s2.IsProperSupersetOf(s));

            foreach (var s in SetsEnumerable(s2))
                Assert.IsFalse(s2.IsProperSupersetOf(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_IsSubsetOf()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsTrue(s1.IsSubsetOf(s));

            foreach (var s in Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s1.IsSubsetOf(s));

            foreach (var s in Sets(new[] { 1, 2 }))
                Assert.IsFalse(s1.IsSubsetOf(s));

            foreach (var s in SetsEnumerable(s1))
                Assert.IsTrue(s1.IsSubsetOf(s));

            var s2 = ReadOnlySetChimera<int>.Empty;

            foreach (var s in Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s2.IsSubsetOf(s));

            foreach (var s in Sets(Empty<int>.Array))
                Assert.IsTrue(s2.IsSubsetOf(s));

            foreach (var s in SetsEnumerable(s2))
                Assert.IsTrue(s2.IsSubsetOf(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_IsSupersetOf()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Sets(Empty<int>.Array))
                Assert.IsTrue(s1.IsSupersetOf(s));

            foreach (var s in Sets(new[] { 1, 2 }))
                Assert.IsTrue(s1.IsSupersetOf(s));

            foreach (var s in Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s1.IsSupersetOf(s));

            foreach (var s in Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsFalse(s1.IsSupersetOf(s));

            foreach (var s in SetsEnumerable(s1))
                Assert.IsTrue(s1.IsSupersetOf(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_Overlaps()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Sets(Empty<int>.Array))
                Assert.IsFalse(s1.Overlaps(s));

            foreach (var s in Sets(new[] { 1, 2 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Sets(new[] { 1, 2, 3, 4 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Sets(new[] { 10, 20 }))
                Assert.IsFalse(s1.Overlaps(s));

            foreach (var s in Sets(new[] { 10, 20, 30 }))
                Assert.IsFalse(s1.Overlaps(s));

            foreach (var s in Sets(new[] { 10, 20, 30, 40 }))
                Assert.IsFalse(s1.Overlaps(s));

            foreach (var s in Sets(new[] { 1, 20 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Sets(new[] { 10, 2, 30 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in Sets(new[] { 10, 20, 3, 40 }))
                Assert.IsTrue(s1.Overlaps(s));

            foreach (var s in SetsEnumerable(s1))
                Assert.IsTrue(s1.Overlaps(s));

            var s2 = ReadOnlySetChimera<int>.Empty;

            foreach (var s in SetsEnumerable(s2))
                Assert.IsFalse(s2.Overlaps(s));
        }

        [TestMethod]
        public void ReadOnlySetBase_SetEquals()
        {
            var s1 = new ReadOnlySetChimera<int>(new HashSet<int> { 3, 2, 1 });

            foreach (var s in Sets(Empty<int>.Array))
                Assert.IsFalse(s1.SetEquals(s));

            foreach (var s in Sets(new[] { 1, 2 }))
                Assert.IsFalse(s1.SetEquals(s));

            foreach (var s in Sets(new[] { 1, 2, 3 }))
                Assert.IsTrue(s1.SetEquals(s));

            foreach (var s in Sets(new[] { 1, 2, 3, 4 }))
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
            Assert.IsTrue(new HashSet<int>(arr.Take(3)).SetEquals(s1));
            Assert.AreEqual(0, arr[3]);

            arr = new int[4];
            s1.CopyTo(arr, 1);
            Assert.IsTrue(new HashSet<int>(arr.Skip(1)).SetEquals(s1));
            Assert.AreEqual(0, arr[0]);

            arr = new int[4];
            s1.CopyTo(arr, 1, 2);
            Assert.IsTrue(new HashSet<int>(arr.Skip(1).Take(2)).SetEquals(new[] { 1, 2 }));
            Assert.AreEqual(0, arr[0]);
            Assert.AreEqual(0, arr[3]);
        }
    }
}

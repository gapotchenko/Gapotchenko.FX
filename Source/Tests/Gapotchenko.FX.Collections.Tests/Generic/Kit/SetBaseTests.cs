using Gapotchenko.FX.Collections.Generic.Kit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Gapotchenko.FX.Collections.Tests.Generic.Kit
{
    [TestClass]
    public class SetBaseTests
    {
        sealed class SetChimera<T> : SetBase<T>
        {
            public SetChimera(HashSet<T> baseSet)
            {
                m_BaseSet = baseSet;
            }

            readonly HashSet<T> m_BaseSet;

            public override IEqualityComparer<T> Comparer => m_BaseSet.Comparer;

            public override int Count => m_BaseSet.Count;

            public override bool Contains(T item) => m_BaseSet.Contains(item);

            public override IEnumerator<T> GetEnumerator() => m_BaseSet.GetEnumerator();

            public override bool Add(T item) => m_BaseSet.Add(item);

            public override bool Remove(T item) => m_BaseSet.Remove(item);

            public override void Clear() => m_BaseSet.Clear();
        }

        [TestMethod]
        public void SetBase_ExceptWith()
        {
            var s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3, 4, 5 });
            s1.ExceptWith(new[] { 1, 3 });
            s1.SetEquals(new[] { 2, 4, 5 });

            s1.ExceptWith(s1);
            Assert.AreEqual(0, s1.Count);
        }

        [TestMethod]
        public void SetBase_IntersectWith()
        {
            var s1 = new SetChimera<int>(new HashSet<int>());
            s1.IntersectWith(new[] { 1, 2, 3 });
            Assert.AreEqual(0, s1.Count);

            s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3 });
            s1.IntersectWith(Empty<int>.Array);
            Assert.AreEqual(0, s1.Count);

            foreach (var s2 in Util.Sets(Empty<int>.Array))
            {
                s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3, 4, 5 });
                s1.IntersectWith(s2);
                Assert.AreEqual(0, s1.Count);
            }

            foreach (var s2 in Util.Sets(new[] { 10, 2, 3, 40 }))
            {
                s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3, 4, 5 });
                s1.IntersectWith(s2);
                s1.SetEquals(new[] { 2, 3 });
            }
        }

        [TestMethod]
        public void SetBase_SymmetricExceptWith()
        {
            foreach (var s2 in Util.Sets(new[] { 3, 4, 5 }))
            {
                var s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3 });
                s1.SymmetricExceptWith(s2);
                s1.SetEquals(new[] { 1, 2, 4, 5 });
            }
        }

        [TestMethod]
        public void SetBase_UnionWith()
        {
            var s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3 });
            s1.UnionWith(new[] { 3, 4, 5 });
            s1.SetEquals(new[] { 1, 2, 3, 4, 5 });
        }
    }
}

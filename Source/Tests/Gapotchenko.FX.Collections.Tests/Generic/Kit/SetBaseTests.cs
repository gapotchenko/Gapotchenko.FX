using Gapotchenko.FX.Collections.Generic.Kit;
using System;
using System.Collections.Generic;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Kit;

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

    [Fact]
    public void SetBase_ExceptWith()
    {
        var s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3, 4, 5 });
        s1.ExceptWith(new[] { 1, 3 });
        s1.SetEquals(new[] { 2, 4, 5 });

        s1.ExceptWith(s1);
        Assert.Equal(0, s1.Count);
    }

    [Fact]
    public void SetBase_IntersectWith()
    {
        var s1 = new SetChimera<int>(new HashSet<int>());
        s1.IntersectWith(new[] { 1, 2, 3 });
        Assert.Equal(0, s1.Count);

        s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3 });
        s1.IntersectWith(Array.Empty<int>());
        Assert.Equal(0, s1.Count);

        foreach (var s2 in Util.Sets(Array.Empty<int>()))
        {
            s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3, 4, 5 });
            s1.IntersectWith(s2);
            Assert.Equal(0, s1.Count);
        }

        foreach (var s2 in Util.Sets(new[] { 10, 2, 3, 40 }))
        {
            s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3, 4, 5 });
            s1.IntersectWith(s2);
            s1.SetEquals(new[] { 2, 3 });
        }
    }

    [Fact]
    public void SetBase_SymmetricExceptWith()
    {
        foreach (var s2 in Util.Sets(new[] { 3, 4, 5 }))
        {
            var s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3 });
            s1.SymmetricExceptWith(s2);
            s1.SetEquals(new[] { 1, 2, 4, 5 });
        }
    }

    [Fact]
    public void SetBase_UnionWith()
    {
        var s1 = new SetChimera<int>(new HashSet<int> { 1, 2, 3 });
        s1.UnionWith(new[] { 3, 4, 5 });
        s1.SetEquals(new[] { 1, 2, 3, 4, 5 });
    }
}

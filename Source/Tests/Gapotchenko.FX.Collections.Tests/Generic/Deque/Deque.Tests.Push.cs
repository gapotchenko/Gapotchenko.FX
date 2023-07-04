﻿using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.Utils;
using Gapotchenko.FX.Linq;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    [Fact]
    public void PushFront()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(3).ReifyCollection();
        var item = source.First();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.Insert(0, item);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.PushFront(item));

        Assert.Equal(list, deque);
    }

    [Fact]
    public void PushBack()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(3).ReifyCollection();
        var item = source.First();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.Add(item);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.PushBack(item));

        Assert.Equal(list, deque);
    }

    #region PushBackRange

    [Fact]
    public void PushBackRange()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(3).ReifyCollection();
        var items = source.Take(2).ReifyCollection();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.AddRange(items);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.PushBackRange(items));

        Assert.Equal(list, deque);
    }

    [Fact]
    public void PushBackRange_ThrowsOnNull()
    {
        var deque = new Deque<T>();
        Assert.Throws<ArgumentNullException>(() => deque.PushBackRange(null!));
    }

    [Fact]
    public void PushBackRange_Collection_DuplicatesSelf()
    {
        var data = Enumerable.Range(1, 3).Select(CreateT).Distinct().Memoize();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.AddRange(list);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.PushBackRange(deque));

        Assert.Equal(list, deque);
    }

    [Fact]
    public void PushBackRange_Enumerable_ThrowsOnSelf()
    {
        var data = Enumerable.Range(1, 3).Select(CreateT).Distinct();

        var deque = new Deque<T>(data);
        Assert.Throws<InvalidOperationException>(() => deque.PushBackRange(deque.Enumerate()));
    }

    #endregion
}

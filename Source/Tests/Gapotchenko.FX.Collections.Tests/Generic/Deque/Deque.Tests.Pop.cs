// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.Utils;
using Gapotchenko.FX.Linq;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    #region PopFront

    [Fact]
    public void PopFront()
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(3).ReifyList();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.RemoveAt(0);
        Assert.Equal(
            data[0],
            ModificationTrackingVerifier.EnsureModified(
                deque,
                deque.PopFront));

        Assert.Equal(list, deque);
    }

    [Fact]
    public void PopFront_ThrowsOnEmpty()
    {
        var deque = new Deque<T>();
        Assert.Throws<InvalidOperationException>(() => deque.PopFront());
    }

    #endregion

    #region PopBack

    [Fact]
    public void PopBack()
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(3).ReifyList();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.RemoveAt(list.Count - 1);
        Assert.Equal(
            data[^1],
            ModificationTrackingVerifier.EnsureModified(
                deque,
                deque.PopBack));

        Assert.Equal(list, deque);
    }

    [Fact]
    public void PopBack_ThrowsOnEmpty()
    {
        var deque = new Deque<T>();
        Assert.Throws<InvalidOperationException>(() => deque.PopBack());
    }

    #endregion

    #region TryPopFront

    [Fact]
    public void TryPopFront()
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(3).ReifyList();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.RemoveAt(0);

        ModificationTrackingVerifier.EnsureModified(
            deque,
            () =>
            {
                Assert.True(deque.TryPopFront(out var item));
                Assert.Equal(data[0], item);
            });

        Assert.Equal(list, deque);
    }

    [Fact]
    public void TryPopFront_Empty()
    {
        var deque = new Deque<T>();
        Assert.False(deque.TryPopFront(out _));
    }

    #endregion

    #region TryPopBack

    [Fact]
    public void TryPopBack()
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(3).ReifyList();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.RemoveAt(list.Count - 1);

        ModificationTrackingVerifier.EnsureModified(
            deque,
            () =>
            {
                Assert.True(deque.TryPopBack(out var item));
                Assert.Equal(data[^1], item);
            });

        Assert.Equal(list, deque);
    }

    [Fact]
    public void TryPopBack_Empty()
    {
        var deque = new Deque<T>();
        Assert.False(deque.TryPopBack(out _));
    }

    #endregion
}

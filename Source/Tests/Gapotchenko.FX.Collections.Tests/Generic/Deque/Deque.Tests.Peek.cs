// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Linq;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    #region PeekFront

    [Fact]
    public void PeekFront()
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(3).ReifyList();

        var deque = new Deque<T>(data);
        Assert.Equal(data[0], deque.PeekFront());
    }

    [Fact]
    public void PeekFront_ThrowsOnEmpty()
    {
        var deque = new Deque<T>();
        Assert.Throws<InvalidOperationException>(() => deque.PeekFront());
    }

    #endregion

    #region PeekBack

    [Fact]
    public void PeekBack()
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(3).ReifyList();

        var deque = new Deque<T>(data);
        Assert.Equal(data[^1], deque.PeekBack());
    }

    [Fact]
    public void PeekBack_ThrowsOnEmpty()
    {
        var deque = new Deque<T>();
        Assert.Throws<InvalidOperationException>(() => deque.PeekBack());
    }

    #endregion

    #region TryPeekFront

    [Fact]
    public void TryPeekFront()
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(3).ReifyList();

        var deque = new Deque<T>(data);
        Assert.True(deque.TryPeekFront(out var item));
        Assert.Equal(data[0], item);
    }

    [Fact]
    public void TryPeekFront_Empty()
    {
        var deque = new Deque<T>();
        Assert.False(deque.TryPeekFront(out _));
    }

    #endregion

    #region TryPeekBack

    [Fact]
    public void TryPeekBack()
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(3).ReifyList();

        var deque = new Deque<T>(data);
        Assert.True(deque.TryPeekBack(out var item));
        Assert.Equal(data[^1], item);
    }

    [Fact]
    public void TryPeekBack_Empty()
    {
        var deque = new Deque<T>();
        Assert.False(deque.TryPeekBack(out _));
    }

    #endregion
}

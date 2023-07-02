// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.Utils;
using Gapotchenko.FX.Linq;
using Xunit;

#pragma warning disable xUnit2017 // Do not use Contains() to check if a value exists in a collection

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    T CreateUniqueT(IEnumerable<T> valuesToSkip)
    {
        valuesToSkip = valuesToSkip.Memoize();
        int seed = 5337;
        T value;
        do
        {
            value = CreateT(seed++);
        }
        while (valuesToSkip.Contains(value));
        return value;
    }

    [Fact]
    public void IndexOf_ItemPresent_ReturnsItemIndex()
    {
        var data = Enumerable.Range(1, 2).Select(CreateT).Distinct().ReifyList();

        var deque = new Deque<T>(data);
        const int index = 1;
        var result = deque.IndexOf(data[index]);
        Assert.Equal(index, result);
    }

    [Fact]
    public void IndexOf_ItemNotPresent_ReturnsNegativeOne()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().GetEnumerator();
        var data = source.Take(3);
        var value = source.Take();

        var deque = new Deque<T>(data);
        var result = deque.IndexOf(value);
        Assert.Equal(-1, result);
    }

    [Fact]
    public void IndexOf_ItemPresentAndSplit_ReturnsItemIndex()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().GetEnumerator();
        var data = source.Take(3).ReifyList();
        var value = source.Take();

        var deque = new Deque<T>(data);
        deque.PopBack();
        deque.PushFront(value);

        Assert.Equal(0, deque.IndexOf(value));
        Assert.Equal(1, deque.IndexOf(data[0]));
        Assert.Equal(2, deque.IndexOf(data[1]));
    }

    [Fact]
    public void Contains_ItemPresent_ReturnsTrue()
    {
        var data = Enumerable.Range(1, 2).Select(CreateT).Distinct().ReifyList();

        var deque = new Deque<T>(data);
        Assert.True(deque.Contains(data[1]));
    }

    [Fact]
    public void Contains_ItemNotPresent_ReturnsFalse()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().GetEnumerator();
        var data = source.Take(2).ReifyList();
        var value = source.Take();

        var deque = new Deque<T>(data);
        Assert.False(deque.Contains(value));
    }
}

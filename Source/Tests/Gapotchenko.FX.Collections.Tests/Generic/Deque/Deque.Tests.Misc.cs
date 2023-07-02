// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Linq;
using Xunit;

#pragma warning disable xUnit2017 // Do not use Contains() to check if a value exists in a collection

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
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
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(3);
        var value = source.First();

        var deque = new Deque<T>(data);
        var result = deque.IndexOf(value);
        Assert.Equal(-1, result);
    }

    [Fact]
    public void IndexOf_ItemPresentAndSplit_ReturnsItemIndex()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(3).ReifyList();
        var value = source.First();

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
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(2);
        var value = source.First();

        var deque = new Deque<T>(data);
        Assert.False(deque.Contains(value));
    }

    [Fact]
    public void Contains_ItemPresentAndSplit_ReturnsTrue()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(3).ReifyList();
        var value = source.First();

        var deque = new Deque<T>(data);
        deque.PopBack();
        deque.PushFront(value);
        Assert.True(deque.Contains(value));
        Assert.True(deque.Contains(data[0]));
        Assert.True(deque.Contains(data[1]));
        Assert.False(deque.Contains(data[2]));
    }

    [Fact]
    public void Add_IsPushBack()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(2).ReifyList();
        var value = source.First();

        var deque1 = new Deque<T>(data);
        var deque2 = new Deque<T>(data);
        ((ICollection<T>)deque1).Add(value);
        deque2.PushBack(value);
        Assert.Equal(deque1, deque2);
    }

    [Fact]
    public void NonGenericEnumerator_EnumeratesItems()
    {
        var data = Enumerable.Range(1, 2).Select(CreateT).Distinct();

        var deque = new Deque<T>(data);

        var results = new List<T>();
        var nonGenericEnumerable = ((System.Collections.IEnumerable)deque).GetEnumerator();
        while (nonGenericEnumerable.MoveNext())
            results.Add((T)nonGenericEnumerable.Current!);

        Assert.Equal(results, deque);
    }

    [Fact]
    public void Insert_AtIndex0_IsSameAsPushFront()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(2).Memoize();
        var value = source.First();

        var deque1 = new Deque<T>(data);
        var deque2 = new Deque<T>(data);
        deque1.Insert(0, value);
        deque2.PushFront(value);
        Assert.Equal(deque1, deque2);
    }

    [Fact]
    public void Insert_AtCount_IsSameAsPushBack()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(2).Memoize();
        var value = source.First();

        var deque1 = new Deque<T>(data);
        var deque2 = new Deque<T>(data);
        deque1.Insert(deque1.Count, value);
        deque2.PushBack(value);
        Assert.Equal(deque1, deque2);
    }

    [Fact]
    public void RemoveAt_RemovesElementAtIndex()
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(3).ReifyList();
        var value = source.First();

        var deque = new Deque<T>(data);
        deque.PopBack();
        deque.PushFront(value);
        deque.RemoveAt(1);
        Assert.Equal(new[] { value, data[1] }, deque);
    }
}

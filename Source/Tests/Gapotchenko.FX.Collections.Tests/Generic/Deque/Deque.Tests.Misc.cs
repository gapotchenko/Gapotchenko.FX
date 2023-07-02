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

    [Fact]
    public void RemoveAt_Index0_IsSameAsPopFront()
    {
        var data = Enumerable.Range(1, 2).Select(CreateT).Distinct().Memoize();

        var deque1 = new Deque<T>(data);
        var deque2 = new Deque<T>(data);
        deque1.RemoveAt(0);
        deque2.PopFront();
        Assert.Equal(deque1, deque2);
    }

    [Fact]
    public void RemoveAt_LastIndex_IsSameAsPopBack()
    {
        var data = Enumerable.Range(1, 2).Select(CreateT).Distinct().Memoize();

        var deque1 = new Deque<T>(data);
        var deque2 = new Deque<T>(data);
        deque1.RemoveAt(deque1.Count - 1);
        deque2.PopBack();
        Assert.Equal(deque1, deque2);
    }

    [Theory(Skip = "TODO")]
    [InlineData(3, 2)]
    [InlineData(4, 2)]
    public void Insert_Multiple(int initialCount, int itemCount)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();

        InsertTest(
            source.Take(initialCount).ReifyCollection(),
            source.Take(itemCount).ReifyCollection());
    }

    [Fact]
    public void Insert_Multiple_Debug()
    {
        InsertTest(new[] { 1, 2, 3 }, new[] { 7, 13 });
        //InsertTest(new[] { 1, 2, 3, 4 }, new[] { 7, 13 });
    }

    static void InsertTest<TElement>(IReadOnlyCollection<TElement> initial, IReadOnlyCollection<TElement> items)
    {
        var totalCapacity = initial.Count + items.Count;
        for (int rotated = 0; rotated <= totalCapacity; ++rotated)
        {
            for (int index = 0; index <= initial.Count; ++index)
            {
                // Calculate the expected result using the slower List<int>.
                var result = new List<TElement>(initial);
                for (int i = 0; i != rotated; ++i)
                {
                    var item = result[0];
                    result.RemoveAt(0);
                    result.Add(item);
                }
                result.InsertRange(index, items);

                // First, start off the deque with the initial items.
                var deque = new Deque<TElement>(initial);

                // Ensure there's enough room for the inserted items.
                //deque.Capacity += items.Count;

                // Rotate the existing items.
                for (int i = 0; i != rotated; ++i)
                {
                    var item = deque[0];
                    deque.PopFront();
                    deque.PushBack(item);
                }

                // Do the insert.
                deque.InsertRange(index, items);

                // Ensure the results are as expected.
                Assert.Equal(result, deque);
            }
        }
    }
}

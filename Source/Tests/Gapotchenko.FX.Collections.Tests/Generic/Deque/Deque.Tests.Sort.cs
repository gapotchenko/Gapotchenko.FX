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
    const int Sort_ShuffledDataSize = 50;

    [Fact]
    public void Sort_HandlesEmpty()
    {
        var deque = new Deque<T>();
        deque.Sort();
        Assert.Empty(deque);
    }

    [Fact]
    public void Sort_SortsShuffledData()
    {
        var data = Enumerable.Range(1, Sort_ShuffledDataSize).Select(CreateT).Distinct().Memoize();

        var list = new List<T>(data);
        list.Sort();

        var deque = new Deque<T>(data);
        deque.Sort();

        Assert.Equal(list, deque);
    }

    static void Sort_IComparer_HandlesEmpty_Core(IComparer<T>? comparer)
    {
        var deque = new Deque<T>();
        deque.Sort(comparer);
        Assert.Empty(deque);
    }

    void Sort_IComparer_SortsShuffledData_Core(IComparer<T>? comparer)
    {
        var data = Enumerable.Range(1, Sort_ShuffledDataSize).Select(CreateT).Distinct().Memoize();

        var list = new List<T>(data);
        list.Sort(comparer);

        var deque = new Deque<T>(data);
        deque.Sort(comparer);

        Assert.Equal(list, deque);
    }

    [Fact]
    public void Sort_IComparer_Null_HandlesEmpty() =>
        Sort_IComparer_HandlesEmpty_Core(null);

    [Fact]
    public void Sort_IComparer_Null_SortsShuffledData() =>
        Sort_IComparer_SortsShuffledData_Core(null);

    static IComparer<TElement> CreateReverseComparer<TElement>(IComparer<TElement> comparer) =>
        Comparer<TElement>.Create((a, b) => comparer.Compare(b, a));

    [Fact]
    public void Sort_IComparer_Default_HandlesEmpty() =>
        Sort_IComparer_HandlesEmpty_Core(Comparer<T>.Default);

    [Fact]
    public void Sort_IComparer_Default_SortsShuffledData() =>
        Sort_IComparer_SortsShuffledData_Core(Comparer<T>.Default);

    [Fact]
    public void Sort_IComparer_Reverse_HandlesEmpty() =>
        Sort_IComparer_HandlesEmpty_Core(CreateReverseComparer<T>(Comparer<T>.Default));

    [Fact]
    public void Sort_IComparer_Reverse_SortsShuffledData() =>
        Sort_IComparer_SortsShuffledData_Core(CreateReverseComparer<T>(Comparer<T>.Default));

    [Fact]
    public void Sort_Comparison_ThrowsOnNull()
    {
        var deque = new Deque<T>();
        Assert.Throws<ArgumentNullException>(() => deque.Sort((Comparison<T>)null!));
    }

    [Fact]
    public void Sort_Comparison_HandlesEmpty()
    {
        var deque = new Deque<T>();
        deque.Sort(Comparer<T>.Default.Compare);
        Assert.Empty(deque);
    }

    void Sort_Comparison_SortsShuffledData_Core(Comparison<T> comparison)
    {
        var data = Enumerable.Range(1, Sort_ShuffledDataSize).Select(CreateT).Distinct().Memoize();

        var list = new List<T>(data);
        list.Sort(comparison);

        var deque = new Deque<T>(data);
        deque.Sort(comparison);

        Assert.Equal(list, deque);
    }

    [Fact]
    public void Sort_Comparison_Default_SortsShuffledData() =>
        Sort_Comparison_SortsShuffledData_Core(Comparer<T>.Default.Compare);

    [Fact]
    public void Sort_Comparison_Reverse_SortsShuffledData() =>
        Sort_Comparison_SortsShuffledData_Core((a, b) => Comparer<T>.Default.Compare(b, a));

    public static IEnumerable<object[]> SortRange_ValidRanges()
    {
        yield return new object[] { 10, 5 };
        yield return new object[] { 20, 10 };
    }

    [Theory]
    [MemberData(nameof(SortRange_ValidRanges))]
    public void SortRange_IComparer_SortsShuffledData(int index, int count)
    {
        var data = Enumerable.Range(1, Sort_ShuffledDataSize).Select(CreateT).Distinct().Memoize();

        var comparer = Comparer<T>.Default;

        var list = new List<T>(data);
        list.Sort(index, count, comparer);

        var deque = new Deque<T>(data);
        deque.Sort(index, count, comparer);

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(SortRange_ValidRanges))]
    public void SortRange_Comparison_SortsShuffledData(int index, int count)
    {
        var data = Enumerable.Range(1, Sort_ShuffledDataSize).Select(CreateT).Distinct().Memoize();

        Comparison<T> comparison = Comparer<T>.Default.Compare;

        var list = new List<T>(data);
        list.Sort(index, count, Comparer<T>.Create(comparison));

        var deque = new Deque<T>(data);
        deque.Sort(index, count, comparison);

        Assert.Equal(list, deque);
    }
}

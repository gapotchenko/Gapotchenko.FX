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

using Assert = Xunit.Assert;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    const int Sort_TestData_Size = 5;

    #region Sort

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, Sort_TestData_Size)]
    public void Sort(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var list = new List<T>(data);
        list.Sort();

        TestData_FillDeque(deque, data);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.Sort());

        Assert.Equal(list, deque);
    }

    #endregion

    #region Sort(IComparer)

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, Sort_TestData_Size)]
    public void Sort_IComparer_Null(int size, Deque<T> deque) =>
        Sort_IComparer_Core(size, deque, null);

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, Sort_TestData_Size)]
    public void Sort_IComparer_Default(int size, Deque<T> deque) =>
        Sort_IComparer_Core(size, deque, Comparer<T>.Default);

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, Sort_TestData_Size)]
    public void Sort_IComparer_Reverse(int size, Deque<T> deque) =>
        Sort_IComparer_Core(size, deque, Comparer<T>.Default.Reverse());

    void Sort_IComparer_Core(int size, Deque<T> deque, IComparer<T>? comparer)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var list = new List<T>(data);
        list.Sort(comparer);

        TestData_FillDeque(deque, data);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.Sort(comparer));

        Assert.Equal(list, deque);
    }

    #endregion

    #region Sort(Comparison)

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, Sort_TestData_Size)]
    public void Sort_Comparison_Default(int size, Deque<T> deque) =>
        Sort_Comparison_Core(size, deque, Comparer<T>.Default.Compare);

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, Sort_TestData_Size)]
    public void Sort_Comparison_Reverse(int size, Deque<T> deque) =>
        Sort_Comparison_Core(size, deque, (a, b) => Comparer<T>.Default.Compare(b, a));

    void Sort_Comparison_Core(int size, Deque<T> deque, Comparison<T> comparison)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var list = new List<T>(data);
        list.Sort(comparison);

        TestData_FillDeque(deque, data);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.Sort(comparison));

        Assert.Equal(list, deque);
    }

    [Fact]
    public void Sort_Comparison_ThrowsOnNull()
    {
        var deque = new Deque<T>();
        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentNullException>(() => deque.Sort((Comparison<T>)null!)));
    }

    #endregion

    #region Sort(int, int, IComparer)

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithIndexAndCount), 0, Sort_TestData_Size, 0)]
    public void Sort_Int32_Int32_IComparer_Null(int size, Deque<T> deque, int index, int count) =>
        Sort_Int32_Int32_IComparer_Core(size, deque, index, count, null);

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithIndexAndCount), 0, Sort_TestData_Size, 0)]
    public void Sort_Int32_Int32_IComparer_Default(int size, Deque<T> deque, int index, int count) =>
        Sort_Int32_Int32_IComparer_Core(size, deque, index, count, Comparer<T>.Default);

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithIndexAndCount), 0, Sort_TestData_Size, 0)]
    public void Sort_Int32_Int32_IComparer_Reverse(int size, Deque<T> deque, int index, int count) =>
        Sort_Int32_Int32_IComparer_Core(size, deque, index, count, Comparer<T>.Default.Reverse());

    void Sort_Int32_Int32_IComparer_Core(
        int size, Deque<T> deque,
        int index, int count,
        IComparer<T>? comparer)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var list = new List<T>(data);
        list.Sort(index, count, comparer);

        TestData_FillDeque(deque, data);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.Sort(index, count, comparer));

        Assert.Equal(list, deque);
    }

    #endregion

    #region Sort(int, int, Comparison)

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithIndexAndCount), 0, Sort_TestData_Size, 0)]
    public void Sort_Int32_Int32_Comparison_Default(int size, Deque<T> deque, int index, int count) =>
        Sort_Int32_Int32_Comparison_Core(
            size, deque,
            index, count,
            Comparer<T>.Default.Compare);

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithIndexAndCount), 0, Sort_TestData_Size, 0)]
    public void Sort_Int32_Int32_Comparison_Reverse(int size, Deque<T> deque, int index, int count) =>
        Sort_Int32_Int32_Comparison_Core(
            size, deque,
            index, count,
            (a, b) => Comparer<T>.Default.Compare(b, a));

    void Sort_Int32_Int32_Comparison_Core(
        int size, Deque<T> deque,
        int index, int count,
        Comparison<T> comparison)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var list = new List<T>(data);
        list.Sort(index, count, Comparer<T>.Create(comparison));

        TestData_FillDeque(deque, data);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.Sort(index, count, comparison));

        Assert.Equal(list, deque);
    }

    [Fact]
    public void Sort_Int32_Int32_Comparison_ThrowsOnNull()
    {
        var deque = new Deque<T>();
        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentNullException>(() => deque.Sort(0, 0, (Comparison<T>)null!)));
    }

    #endregion
}

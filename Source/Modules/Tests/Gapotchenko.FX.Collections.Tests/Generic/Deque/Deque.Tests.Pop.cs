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
    const int Pop_TestData_Size = 8;

    #region PopFront

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, Pop_TestData_Size)]
    public void PopFront(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyList();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.RemoveAt(0);
        Assert.Equal(
            data[0],
            ModificationTrackingVerifier.EnsureModified(
                deque,
                deque.PopFront));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(TestData_DequeLayoutCombinations), 0, Pop_TestData_Size)]
    public void PopFront_ThrowsOnEmpty(Deque<T> deque)
    {
        Assert.Throws<InvalidOperationException>(() => deque.PopFront());
    }

    #endregion

    #region PopBack

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, Pop_TestData_Size)]
    public void PopBack(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyList();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.RemoveAt(list.Count - 1);
        Assert.Equal(
            data[^1],
            ModificationTrackingVerifier.EnsureModified(
                deque,
                deque.PopBack));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(TestData_DequeLayoutCombinations), 0, Pop_TestData_Size)]
    public void PopBack_ThrowsOnEmpty(Deque<T> deque)
    {
        Assert.Throws<InvalidOperationException>(() => deque.PopBack());
    }

    #endregion

    #region TryPopFront

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, Pop_TestData_Size)]
    public void TryPopFront(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyList();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

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

    [Theory]
    [MemberData(nameof(TestData_DequeLayoutCombinations), 0, Pop_TestData_Size)]
    public void TryPopFront_Empty(Deque<T> deque)
    {
        Assert.False(deque.TryPopFront(out _));
    }

    #endregion

    #region TryPopBack

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, Pop_TestData_Size)]
    public void TryPopBack(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyList();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

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

    [Theory]
    [MemberData(nameof(TestData_DequeLayoutCombinations), 0, Pop_TestData_Size)]
    public void TryPopBack_Empty(Deque<T> deque)
    {
        Assert.False(deque.TryPopBack(out _));
    }

    #endregion
}

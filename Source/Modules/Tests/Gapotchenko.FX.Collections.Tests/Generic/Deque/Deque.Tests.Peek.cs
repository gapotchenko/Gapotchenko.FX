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

using Assert = Xunit.Assert;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    const int Peek_TestData_Size = 8;

    #region PeekFront

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, Peek_TestData_Size)]
    public void PeekFront(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyList();

        TestData_FillDeque(deque, data);

        Assert.Equal(data[0], deque.PeekFront());
    }

    [Theory]
    [MemberData(nameof(TestData_DequeLayoutCombinations), 0, Peek_TestData_Size)]
    public void PeekFront_ThrowsOnEmpty(Deque<T> deque)
    {
        Assert.Throws<InvalidOperationException>(() => deque.PeekFront());
    }

    #endregion

    #region PeekBack

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, Peek_TestData_Size)]
    public void PeekBack(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyList();

        TestData_FillDeque(deque, data);

        Assert.Equal(data[^1], deque.PeekBack());
    }

    [Theory]
    [MemberData(nameof(TestData_DequeLayoutCombinations), 0, Peek_TestData_Size)]
    public void PeekBack_ThrowsOnEmpty(Deque<T> deque)
    {
        Assert.Throws<InvalidOperationException>(() => deque.PeekBack());
    }

    #endregion

    #region TryPeekFront

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, Peek_TestData_Size)]
    public void TryPeekFront(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyList();

        TestData_FillDeque(deque, data);

        Assert.True(deque.TryPeekFront(out var item));
        Assert.Equal(data[0], item);
    }

    [Theory]
    [MemberData(nameof(TestData_DequeLayoutCombinations), 0, Peek_TestData_Size)]
    public void TryPeekFront_Empty(Deque<T> deque)
    {
        Assert.False(deque.TryPeekFront(out _));
    }

    #endregion

    #region TryPeekBack

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, Peek_TestData_Size)]
    public void TryPeekBack(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyList();

        TestData_FillDeque(deque, data);

        Assert.True(deque.TryPeekBack(out var item));
        Assert.Equal(data[^1], item);
    }

    [Theory]
    [MemberData(nameof(TestData_DequeLayoutCombinations), 0, Peek_TestData_Size)]
    public void TryPeekBack_Empty(Deque<T> deque)
    {
        Assert.False(deque.TryPeekBack(out _));
    }

    #endregion
}

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
    const int Indexer_TestData_Size = 8;

    #region Indexer_Get

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, Indexer_TestData_Size)]
    public void Indexer_Get(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyList();

        TestData_FillDeque(deque, data);

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () =>
            {
                for (int i = 0; i < size; ++i)
                    Assert.Equal(data[i], deque[i]);
            });
    }

    [Fact]
    public void Indexer_Get_ThrowsOnInvalidIndex()
    {
        const int size = TestData_SampleSize;

        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Take(size);

        var deque = new Deque<T>(data);

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => deque[-1]);
                Assert.Throws<ArgumentOutOfRangeException>(() => deque[size]);
            });
    }

    #endregion

    #region Indexer_Set

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, Indexer_TestData_Size)]
    public void Indexer_Set(int size, Deque<T> deque)
    {
        TestData_FillDeque(deque, Enumerable.Repeat(default(T)!, size));

        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyList();

        for (int i = 0; i < size; ++i)
        {
            ModificationTrackingVerifier.EnsureModified(
                deque,
                () => deque[i] = data[i]);
        }

        Assert.Equal(data, deque);
    }

    [Fact]
    public void Indexer_Set_ThrowsOnInvalidIndex()
    {
        const int size = TestData_SampleSize;

        T item = default!;

        var deque = new Deque<T>(size);
        TestData_FillDeque(deque, Enumerable.Repeat(default(T)!, size));

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => deque[-1] = item);
                Assert.Throws<ArgumentOutOfRangeException>(() => deque[size] = item);
            });
    }

    #endregion
}

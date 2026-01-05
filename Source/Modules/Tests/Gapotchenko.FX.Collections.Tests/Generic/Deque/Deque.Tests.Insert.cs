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
    const int Insert_TestData_Size = 8;
    const int Insert_TestData_InsertionSize = 2;

    #region Insert

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithInsertionIndex), 0, Insert_TestData_Size)]
    public void Insert(int size, Deque<T> deque, int index)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(size).ReifyCollection();
        var item = source.First();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.Insert(index, item);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.Insert(index, item));

        Assert.Equal(list, deque);
    }

    [Fact]
    public void Insert_ThrowsOnInvalidIndex()
    {
        const int size = TestData_SampleSize;

        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(size).ReifyCollection();
        var item = source.First();

        var deque = new Deque<T>(data);
        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.Insert(-1, item));
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.Insert(size + 1, item));
            });
    }

    #endregion

    #region InsertRange

    [Fact]
    public void InsertRange_ThrowsOnNull()
    {
        var deque = new Deque<T>();
        Assert.Throws<ArgumentNullException>(() => deque.InsertRange(0, null!));
    }

    [Theory]
    [MemberData(
        nameof(TestData_SizeAndDequeLayoutCombinationsWithInsertionIndexAndCount),
        0, Insert_TestData_Size,
        0, Insert_TestData_InsertionSize)]
    public void InsertRange_Collection(int size, Deque<T> deque, int index, int count) =>
        InsertRange_Core(size, deque, index, count, Fn.Identity);

    [Theory]
    [MemberData(
        nameof(TestData_SizeAndDequeLayoutCombinationsWithInsertionIndexAndCount),
        0, Insert_TestData_Size,
        0, Insert_TestData_InsertionSize)]
    public void InsertRange_Enumeration(int size, Deque<T> deque, int index, int count) =>
        InsertRange_Core(size, deque, index, count, x => x.Enumerate());

    void InsertRange_Core(
        int size, Deque<T> deque,
        int index, int count,
        Func<IReadOnlyCollection<T>, IEnumerable<T>> collectionSelector)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(size).ReifyCollection();
        var items = collectionSelector(source.Take(count).ReifyCollection());

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.InsertRange(index, items);
        Assert.Equal(
            count != 0,
            ModificationTrackingVerifier.IsModified(
                deque,
                () => deque.InsertRange(index, items)));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithInsertionIndex), 0, Insert_TestData_Size)]
    public void InsertRange_Collection_Self(int size, Deque<T> deque, int index)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.InsertRange(index, list);
        Assert.Equal(
            size != 0,
            ModificationTrackingVerifier.IsModified(
                deque,
                () => deque.InsertRange(index, deque)));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithInsertionIndex), 1, Insert_TestData_Size)]
    public void InsertRange_Enumeration_ThrowsOnShortCircuit(int size, Deque<T> deque, int index)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size);

        TestData_FillDeque(deque, data);

        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => Assert.Throws<InvalidOperationException>(() => deque.InsertRange(index, deque.Enumerate())));
    }

    [Fact]
    public void InsertRange_ThrowsOnInvalidIndex()
    {
        const int size = TestData_SampleSize;

        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(size).ReifyCollection();
        var items = source.Take(Insert_TestData_InsertionSize).ReifyCollection();

        var deque = new Deque<T>(data);

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.InsertRange(-1, items));
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.InsertRange(size + 1, items));
            });
    }

    #endregion
}

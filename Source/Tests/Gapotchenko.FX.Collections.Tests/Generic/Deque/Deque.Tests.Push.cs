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

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, 8)]
    public void PushFront(int size, Deque<T> deque)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(size).ReifyCollection();
        var item = source.First();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.Insert(0, item);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.PushFront(item));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, 8)]
    public void PushBack(int size, Deque<T> deque)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(size).ReifyCollection();
        var item = source.First();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.Add(item);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.PushBack(item));

        Assert.Equal(list, deque);
    }

    #region PushFrontRange

    [Fact]
    public void PushFrongRange_ThrowsOnNull()
    {
        var deque = new Deque<T>();
        Assert.Throws<ArgumentNullException>(() => deque.PushFrontRange(null!));
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithRangeLength), 0, 8, 0, 2)]
    public void PushFrontRange_Collection(int size, Deque<T> deque, int length) =>
        PushFrontRange_Core(size, deque, length, Fn.Identity);

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithRangeLength), 0, 8, 0, 2)]
    public void PushFrontRange_Enumerable(int size, Deque<T> deque, int length) =>
        PushFrontRange_Core(size, deque, length, x => x.Enumerate());

    void PushFrontRange_Core(
        int size, Deque<T> deque,
        int length,
        Func<IReadOnlyCollection<T>, IEnumerable<T>> collectionSelector)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(size).ReifyCollection();
        var items = collectionSelector(source.Take(length).ReifyCollection());

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.InsertRange(0, items);
        Assert.Equal(
            length != 0,
            ModificationTrackingVerifier.IsModified(
                deque,
                () => deque.PushFrontRange(items)));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, 8)]
    public void PushFrontRange_Enumerable_ThrowsOnShortCircuit(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size);

        TestData_FillDeque(deque, data);

        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => Assert.Throws<InvalidOperationException>(() => deque.PushFrontRange(deque.Enumerate())));
    }

    #endregion

    #region PushBackRange

    [Fact]
    public void PushBackRange_ThrowsOnNull()
    {
        var deque = new Deque<T>();
        Assert.Throws<ArgumentNullException>(() => deque.PushBackRange(null!));
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithRangeLength), 0, 8, 0, 2)]
    public void PushBackRange_Collection(int size, Deque<T> deque, int length) =>
        PushBackRange_Core(size, deque, length, Fn.Identity);

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithRangeLength), 0, 8, 0, 2)]
    public void PushBackRange_Enumerable(int size, Deque<T> deque, int length) =>
        PushBackRange_Core(size, deque, length, x => x.Enumerate());

    void PushBackRange_Core(int size, Deque<T> deque, int length, Func<IReadOnlyCollection<T>, IEnumerable<T>> collectionSelector)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(size).ReifyCollection();
        var items = collectionSelector(source.Take(length).ReifyCollection());

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.AddRange(items);
        Assert.Equal(
            length != 0,
            ModificationTrackingVerifier.IsModified(
                deque,
                () => deque.PushBackRange(items)));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, 8)]
    public void PushBackRange_Collection_DuplicatesSelf(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.AddRange(list);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.PushBackRange(deque));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, 8)]
    public void PushBackRange_Enumerable_ThrowsOnShortCircuit(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size);

        TestData_FillDeque(deque, data);

        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => Assert.Throws<InvalidOperationException>(() => deque.PushBackRange(deque.Enumerate())));
    }

    #endregion
}

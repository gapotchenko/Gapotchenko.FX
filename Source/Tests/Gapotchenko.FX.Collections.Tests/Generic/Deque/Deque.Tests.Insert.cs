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
    const int Insert_DataSize = 24;

    static IEnumerable<int> Insert_EnumerateValidIndeces()
    {
        const int n = Insert_DataSize;

        yield return 0;
        yield return 1;

        int half = n / 2;
        yield return half - 1;
        yield return half;
        yield return half + 1;

        yield return n - 1;
        yield return n;
    }

    #region Insert

    public static IEnumerable<object[]> Insert_ValidIndeces()
    {
        foreach (var i in Insert_EnumerateValidIndeces())
            yield return new object[] { i };
    }

    public static IEnumerable<object[]> Insert_InvalidIndeces()
    {
        const int n = Insert_DataSize;

        yield return new object[] { -1 };
        yield return new object[] { n + 1 };
    }

    [Theory]
    [MemberData(nameof(Insert_ValidIndeces))]
    public void Insert(int index)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(Insert_DataSize).ReifyCollection();
        var item = source.First();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.Insert(index, item);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.Insert(index, item));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(Insert_InvalidIndeces))]
    public void Insert_InvalidIndex(int index)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(Insert_DataSize).ReifyCollection();
        var item = source.First();

        var deque = new Deque<T>(data);
        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentOutOfRangeException>(() => deque.Insert(index, item)));
    }

    #endregion

    #region InsertRange

    [Fact]
    public void InsertRange_ThrowsOnNull()
    {
        var deque = new Deque<T>();
        Assert.Throws<ArgumentNullException>(() => deque.InsertRange(0, null!));
    }

    public static IEnumerable<object[]> InsertRange_ValidRanges()
    {
        for (int count = 0; count < 5; ++count)
            foreach (int index in Insert_EnumerateValidIndeces())
                yield return new object[] { index, count };
    }

    [Theory]
    [MemberData(nameof(InsertRange_ValidRanges))]
    public void InsertRange_Collection(int index, int count) =>
        InsertRange_Core(index, count, Fn.Identity);

    [Theory]
    [MemberData(nameof(InsertRange_ValidRanges))]
    public void InsertRange_Enumeration(int index, int count) =>
        InsertRange_Core(index, count, x => x.Enumerate());

    void InsertRange_Core(int index, int count, Func<IReadOnlyCollection<T>, IEnumerable<T>> collectionSelector)
    {
        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(Insert_DataSize).ReifyCollection();
        var items = collectionSelector(source.Take(count).ReifyCollection());

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.InsertRange(index, items);
        Assert.Equal(
            count != 0,
            ModificationTrackingVerifier.IsModified(
                deque,
                () => deque.InsertRange(index, items)));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(Insert_ValidIndeces))]
    public void InsertRange_Collection_Self(int index)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(Insert_DataSize).Memoize();

        var list = new List<T>(data);
        var deque = new Deque<T>(data);

        list.InsertRange(index, list);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.InsertRange(index, deque));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(Insert_ValidIndeces))]
    public void InsertRange_Enumeration_ThrowsOnShortCircuit(int index)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(Insert_DataSize);

        var deque = new Deque<T>(data);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => Assert.Throws<InvalidOperationException>(() => deque.InsertRange(index, deque.Enumerate())));
    }

    #endregion
}

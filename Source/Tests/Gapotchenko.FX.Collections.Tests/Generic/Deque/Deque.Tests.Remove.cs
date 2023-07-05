using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.Utils;
using Gapotchenko.FX.Linq;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    const int Remove_TestData_Size = 8;

    #region Remove

    [Theory]
    [MemberData(nameof(TestData_DequeLayoutCombinations), 0, Remove_TestData_Size)]
    public void Remove_Empty(Deque<T> deque)
    {
        var item = CreateT(1);

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.False(deque.Remove(item)));
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithRandom), 0, Remove_TestData_Size)]
    public void Remove(int size, Deque<T> deque, Random random)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var itemsToRemove = new HashSet<T>();
        foreach (var item in data)
        {
            if (random.Next(2) == 0)
                itemsToRemove.Add(item);
        }

        var list = new List<T>(itemsToRemove);
        TestData_FillDeque(deque, itemsToRemove);

        foreach (var item in data)
        {
            list.Remove(item);

            if (itemsToRemove.Contains(item))
            {
                ModificationTrackingVerifier.EnsureModified(
                    deque,
                    () => Assert.True(deque.Remove(item)));
            }
            else
            {
                ModificationTrackingVerifier.EnsureNotModified(
                    deque,
                    () => Assert.False(deque.Remove(item)));
            }

            Assert.Equal(list, deque);
        }

        Assert.Empty(deque);
    }

    #endregion

    #region RemoveAt

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithIndex), 1, Remove_TestData_Size)]
    public void RemoveAt(int size, Deque<T> deque, int index)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.RemoveAt(index);
        ModificationTrackingVerifier.EnsureModified(
            deque,
            () => deque.RemoveAt(index));

        Assert.Equal(list, deque);
    }

    [Fact]
    public void RemoveAt_Empty()
    {
        var deque = new Deque<T>();

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentOutOfRangeException>(() => deque.RemoveAt(0)));
    }

    [Fact]
    public void RemoveAt_ThrowsOnInvalidIndex()
    {
        const int size = TestData_SampleSize;

        var source = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Stream();
        var data = source.Take(size).ReifyCollection();

        var deque = new Deque<T>(data);

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.RemoveAt(-1));
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.RemoveAt(size));
            });
    }

    #endregion

    #region RemoveRange

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithIndexAndCount), 0, Remove_TestData_Size, 0)]
    public void RemoveRange(int size, Deque<T> deque, int index, int count)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.RemoveRange(index, count);
        Assert.Equal(
            count != 0,
            ModificationTrackingVerifier.IsModified(
                deque,
                () => deque.RemoveRange(index, count)));

        Assert.Equal(list, deque);
    }

    [Fact]
    public void RemoveRange_ThrowsOnInvalidRange()
    {
        const int size = TestData_SampleSize;

        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Take(size);

        var deque = new Deque<T>(data);

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.RemoveRange(-1, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.RemoveRange(0, -1));
                Assert.Throws<ArgumentException>(() => deque.RemoveRange(size + 1, 0));
                Assert.Throws<ArgumentException>(() => deque.RemoveRange(size, 1));
                Assert.Throws<ArgumentException>(() => deque.RemoveRange(0, size + 1));
            });
    }

    #endregion

    #region RemoveWhere

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithRandom), 0, Remove_TestData_Size)]
    public void RemoveWhere(int size, Deque<T> deque, Random random)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).Memoize();

        var itemsToRemove = new HashSet<T>();
        foreach (var item in data)
        {
            if (random.Next(2) == 0)
                itemsToRemove.Add(item);
        }

        var list = new List<T>(data);
        TestData_FillDeque(deque, data);

        list.RemoveAll(itemsToRemove.Contains);
        Assert.Equal(
            itemsToRemove.Count != 0,
            ModificationTrackingVerifier.IsModified(
                deque,
                () => Assert.Equal(itemsToRemove.Count, deque.RemoveWhere(itemsToRemove.Contains))));

        Assert.Equal(list, deque);
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, Remove_TestData_Size)]
    public void RemoveWhere_All(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Take(size);

        TestData_FillDeque(deque, data);

        Assert.Equal(
            size != 0,
            ModificationTrackingVerifier.IsModified(
                deque,
                () => Assert.Equal(size, deque.RemoveWhere(_ => true))));

        Assert.Empty(deque);
    }

    [Fact]
    public void RemoveWhere_ThrowsOnNull()
    {
        var deque = new Deque<T>();

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentNullException>(() => deque.RemoveWhere(null!)));
    }

    #endregion
}

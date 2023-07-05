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
    const int CopyTo_TestData_Size = 8;

    #region CopyTo(Array)

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, CopyTo_TestData_Size)]
    public void CopyTo_Array(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyCollection();

        TestData_FillDeque(deque, data);
        var array = new T[data.Count];

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => deque.CopyTo(array));

        Assert.Equal(data, array);
    }

    [Fact]
    public void CopyTo_Array_ThrowsOnNull()
    {
        var deque = new Deque<T>();

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentNullException>(() => deque.CopyTo(null!)));
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 1, CopyTo_TestData_Size)]
    public void CopyTo_Array_SmallerArray(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyCollection();

        TestData_FillDeque(deque, data);
        var array = new T[data.Count - 1];

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentException>(() => deque.CopyTo(array)));

        // Ensure that the array remains intact, i.e. filled with the default T values.
        Assert.Equal(
            Enumerable.Range(1, array.Length).Select(_ => default(T)!),
            array);
    }

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinations), 0, CopyTo_TestData_Size)]
    public void CopyTo_Array_LargerArray(int size, Deque<T> deque)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyCollection();

        TestData_FillDeque(deque, data);
        var array = new T[data.Count + 1];

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => deque.CopyTo(array));

        Assert.Equal(data, array.Take(data.Count));
        Assert.Equal(default, array[^1]);
    }

    #endregion

    #region CopyTo(Array, int, int)

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithIndexAndCount), 0, CopyTo_TestData_Size, 0)]
    public void CopyTo_Array_Int32_Int32(
        int size, Deque<T> deque,
        int index, int count)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyCollection();

        TestData_FillDeque(deque, data);
        var list = new List<T>(data);

        var listArray = new T[size];
        var dequeArray = new T[size];

        list.CopyTo(0, listArray, index, count);
        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => deque.CopyTo(dequeArray, index, count));

        Assert.Equal(listArray, dequeArray);
    }

    [Fact]
    public void CopyTo_Array_Int32_Int32_ThrowsOnNull()
    {
        var deque = new Deque<T>();

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentNullException>(() => deque.CopyTo(null!, 0, 0)));
    }

    [Fact]
    public void CopyTo_Array_Int32_Int32_ThrowsOnOutOfRangeArguments()
    {
        const int length = 3;

        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(length);

        var deque = new Deque<T>(data);
        var array = new T[length];

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () =>
            {
                int count = deque.Count;
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(array, -1, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(array, 0, -1));
                Assert.Throws<ArgumentException>(() => deque.CopyTo(array, 0, count + 1));
                Assert.Throws<ArgumentException>(() => deque.CopyTo(array, length + 1, 0));
                Assert.Throws<ArgumentException>(() => deque.CopyTo(array, length, 1));
                Assert.Throws<ArgumentException>(() => deque.CopyTo(array, 0, length + 1));
            });
    }

    #endregion

    #region CopyTo(int, Array, int, int)

    [Theory]
    [MemberData(nameof(TestData_SizeAndDequeLayoutCombinationsWithIndexAndCount), 0, CopyTo_TestData_Size, 0)]
    public void CopyTo_Int32_Array_Int32_Int32(
        int size, Deque<T> deque,
        int index, int count)
    {
        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(size).ReifyCollection();

        TestData_FillDeque(deque, data);
        var list = new List<T>(data);

        var listArray = new T[size * 2];
        var dequeArray = new T[size * 2];
        int arrayIndex = index * 2;

        list.CopyTo(index, listArray, arrayIndex, count);
        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => deque.CopyTo(index, dequeArray, arrayIndex, count));

        Assert.Equal(listArray, dequeArray);
    }

    [Fact]
    public void CopyTo_Int32_Array_Int32_Int32_ThrowsOnNull()
    {
        var deque = new Deque<T>();

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentNullException>(() => deque.CopyTo(0, null!, 0, 0)));
    }

    [Fact]
    public void CopyTo_Int32_Array_Int32_Int32_ThrowsOnOutOfRangeArguments()
    {
        const int length = 3;

        var data = Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(length);

        var deque = new Deque<T>(data);
        var array = new T[length];

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () =>
            {
                int count = deque.Count;
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(-1, array, 0, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(0, array, -1, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(0, array, 0, -1));
                Assert.Throws<ArgumentException>(() => deque.CopyTo(count + 1, array, 0, 0));
                Assert.Throws<ArgumentException>(() => deque.CopyTo(count, array, 0, 1));
                Assert.Throws<ArgumentException>(() => deque.CopyTo(0, array, 0, count + 1));
                Assert.Throws<ArgumentException>(() => deque.CopyTo(0, array, length + 1, 0));
                Assert.Throws<ArgumentException>(() => deque.CopyTo(0, array, length, 1));
                Assert.Throws<ArgumentException>(() => deque.CopyTo(0, array, 0, length + 1));
            });
    }

    #endregion
}

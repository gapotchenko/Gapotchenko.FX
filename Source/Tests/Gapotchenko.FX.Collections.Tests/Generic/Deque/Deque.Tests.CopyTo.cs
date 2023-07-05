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
    const int CopyTo_DataSize = 24;

    IEnumerable<T> CopyTo_GetData() =>
        Enumerable.Range(1, int.MaxValue).Select(CreateT).Distinct().Take(CopyTo_DataSize);

    #region CopyTo_Array

    [Fact]
    public void CopyTo_Array()
    {
        var data = CopyTo_GetData().ReifyCollection();

        var deque = new Deque<T>(data);
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

    [Fact]
    public void CopyTo_Array_Empty()
    {
        var deque = new Deque<T>();
        var array = Array.Empty<T>();

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => deque.CopyTo(array));

        Assert.Empty(array);
    }

    [Fact]
    public void CopyTo_Array_SmallerArray()
    {
        var data = CopyTo_GetData().ReifyCollection();

        var deque = new Deque<T>(data);
        var array = new T[data.Count - 1];

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentException>(() => deque.CopyTo(array)));

        // Ensure that the array remains intact, i.e. filled with the default T values.
        Assert.Equal(
            Enumerable.Range(1, array.Length).Select(_ => default(T)!),
            array);
    }

    [Fact]
    public void CopyTo_Array_LargerArray()
    {
        var data = CopyTo_GetData().ReifyCollection();

        var deque = new Deque<T>(data);
        var array = new T[data.Count + 1];

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => deque.CopyTo(array));

        Assert.Equal(data, array.Take(data.Count));
        Assert.Equal(default, array[^1]);
    }

    #endregion

    #region CopyTo_Array_Int32_Int32

    [Fact]
    public void CopyTo_Array_Int32_Int32_ThrowsOnNull()
    {
        var deque = new Deque<T>();

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => Assert.Throws<ArgumentNullException>(() => deque.CopyTo(null!, 0, 0)));
    }

    [Fact]
    public void CopyTo_Array_Int32_Int32_Empty()
    {
        var deque = new Deque<T>();
        var array = Array.Empty<T>();

        ModificationTrackingVerifier.EnsureNotModified(
            deque,
            () => deque.CopyTo(array, 0, 0));

        Assert.Empty(array);
    }

    public static IEnumerable<object[]> CopyTo_Array_ValidShorterArrayLengths()
    {
        yield return new object[] { 2 };
        yield return new object[] { 3 };
        yield return new object[] { 5 };
    }

    [Theory]
    [MemberData(nameof(CopyTo_Array_ValidShorterArrayLengths))]
    public void CopyTo_Array_Int32_Int32_ThrowsOnOutOfRangeArguments(int length)
    {
        var data = CopyTo_GetData();

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

    #region CopyTo_Int32_Array_Int32_Int32

    [Theory]
    [MemberData(nameof(CopyTo_Array_ValidShorterArrayLengths))]
    public void CopyTo_Int32_Array_Int32_Int32_ThrowsOnOutOfRangeArguments(int length)
    {
        var data = CopyTo_GetData();

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

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Linq;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

using Math = System.Math;

partial class Deque_Tests<T>
{
    public static IEnumerable<object[]> ValidCapacityValues()
    {
        yield return new object[] { 0 };
        yield return new object[] { 1 };
        yield return new object[] { 17 };
    }

    public static IEnumerable<object[]> InvalidCapacityValues()
    {
        yield return new object[] { -1 };
        yield return new object[] { -2 };
        yield return new object[] { int.MinValue };
    }

    [Theory]
    [MemberData(nameof(InvalidCapacityValues))]
    public void Capacity_Ensure_ThrowsOnInvalidValue(int capacity)
    {
        var deque = new Deque<T>();
        Assert.Throws<ArgumentOutOfRangeException>(nameof(capacity), () => deque.EnsureCapacity(capacity));
    }

    [Theory]
    [MemberData(nameof(ValidCapacityValues))]
    public void Capacity_Ensure_UsesSpecifiedValue(int capacity)
    {
        var deque = new Deque<T>(Math.Min(capacity, 1));
        Assert.Equal(capacity, deque.EnsureCapacity(capacity));
    }

    [Theory]
    [MemberData(nameof(ValidCapacityValues))]
    public void Capacity_Ensure_SmallerValueDoesNothing(int capacity)
    {
        var deque = new Deque<T>(capacity);
        int newCapacity = Math.Max(capacity - 1, 0);
        Assert.Equal(capacity, deque.EnsureCapacity(newCapacity));
    }

    [Theory]
    [MemberData(nameof(ValidCapacityValues))]
    public void Capacity_Ensure_ExistingValueDoesNothing(int capacity)
    {
        var deque = new Deque<T>(capacity);
        Assert.Equal(capacity, deque.EnsureCapacity(capacity));
    }

    [Theory]
    [MemberData(nameof(ValidCapacityValues))]
    public void Capacity_Ensure_ChangePreservesData(int capacity)
    {
        var data = Enumerable.Range(1, 3).Select(CreateT).Memoize();

        var deque = new Deque<T>(data);
        deque.EnsureCapacity(capacity);
        Assert.Equal(data, deque);
    }

    [Fact]
    public void Capacity_OnAddGrowsFromZero()
    {
        var deque = new Deque<T>(0);
        deque.PushBack(CreateT(0));
        Assert.True(deque.EnsureCapacity(0) > 0);
    }
}

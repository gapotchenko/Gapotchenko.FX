// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © Stephen Cleary
// Portions © Masashi Mizuno
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Generic;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

partial class Deque_Tests<T>
{
    [Theory]
    [MemberData(nameof(InvalidCapacityValues))]
    public void Constructor_Capacity_ThrowsOnInvalidValue(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(nameof(capacity), () => new Deque<T>(capacity));
    }

    [Theory]
    [MemberData(nameof(ValidCapacityValues))]
    public void Constructor_Capacity_UsesSpecifiedValue(int capacity)
    {
        var deque = new Deque<T>(capacity);
        Assert.Equal(capacity, deque.EnsureCapacity(0));
    }

    [Fact]
    public void Constructor_Collection_GetsAdded()
    {
        var data = Enumerable.Range(1, 3).Select(CreateT).ToArray();
        var deque = new Deque<T>(data);
        Assert.Equal(data, deque);
    }

    [Theory]
    [MemberData(nameof(ValidCapacityValues))]
    public void Constructor_Collection_SetsCapacity(int capacity)
    {
        var data = Enumerable.Range(1, capacity).Select(CreateT);
        var deque = new Deque<T>(data);
        Assert.Equal(capacity, deque.EnsureCapacity(0));
    }
}

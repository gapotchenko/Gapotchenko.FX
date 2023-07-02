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
    [MemberData(nameof(ValidCapacityValues))]
    public void Capacity_SetToExistingValueDoesNothing(int capacity)
    {
        var deque = new Deque<int>(capacity);
        Assert.Equal(capacity, deque.EnsureCapacity(capacity));
    }

    [Theory]
    [MemberData(nameof(ValidCapacityValues))]
    public void Capacity_ChangePreservesData(int capacity)
    {
        var data = Enumerable.Range(1, 3).Select(CreateT).ToArray();
        var deque = new Deque<T>(data);
        deque.EnsureCapacity(capacity);
        Assert.Equal(data, deque);
    }
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals.Tests;

[TestClass]
public sealed class ValueIntervalTests : IntervalCoreTests
{
    public override IInterval<T> NewInterval<T>(T from, T to) => new ValueInterval<T>(from, to);

    public override IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) => new ValueInterval<T>(from, to);

    public override IInterval<T> InfiniteInterval<T>() => ValueInterval.Infinite<T>();

    public override IInterval<T> EmptyInterval<T>() => ValueInterval.Empty<T>();

    [TestMethod]
    public void ValueInterval_Default()
    {
        ValueInterval<string?> interval = default;
        Assert.IsTrue(interval.IsEmpty);
        Assert.IsFalse(interval.Contains(default));
    }

    [TestMethod]
    public void ValueInterval_Equality()
    {
        var interval = ValueInterval.Inclusive(10, 20);

        Assert.AreEqual(interval, ValueInterval.Inclusive(10, 20));

        Assert.AreNotEqual(interval, ValueInterval.Exclusive(10, 20));
        Assert.AreNotEqual(interval, ValueInterval.InclusiveExclusive(10, 20));
        Assert.AreNotEqual(interval, ValueInterval.ExclusiveInclusive(10, 20));

        Assert.AreNotEqual(interval, ValueInterval.Inclusive(11, 20));
        Assert.AreNotEqual(interval, ValueInterval.Inclusive(10, 21));
    }
}

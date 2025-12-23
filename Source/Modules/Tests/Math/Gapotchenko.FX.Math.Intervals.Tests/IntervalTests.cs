// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals.Tests;

[TestClass]
public sealed class IntervalTests : IntervalCoreTests
{
    public override IInterval<T> NewInterval<T>(T from, T to) => new Interval<T>(from, to);

    public override IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) => new Interval<T>(from, to);

    public override IInterval<T> InfiniteInterval<T>() => Interval.Infinite<T>();

    public override IInterval<T> EmptyInterval<T>() => Interval.Empty<T>();

    [TestMethod]
    public void Interval_Equality()
    {
        var interval = Interval.Inclusive(10, 20);
        var comparer = IntervalEqualityComparer.Default<int>();

        Assert.AreEqual(interval, Interval.Inclusive(10, 20), comparer);

        Assert.AreNotEqual(interval, Interval.Exclusive(10, 20), comparer);
        Assert.AreNotEqual(interval, Interval.InclusiveExclusive(10, 20), comparer);
        Assert.AreNotEqual(interval, Interval.ExclusiveInclusive(10, 20), comparer);

        Assert.AreNotEqual(interval, Interval.Inclusive(11, 20), comparer);
        Assert.AreNotEqual(interval, Interval.Inclusive(10, 21), comparer);
    }
}

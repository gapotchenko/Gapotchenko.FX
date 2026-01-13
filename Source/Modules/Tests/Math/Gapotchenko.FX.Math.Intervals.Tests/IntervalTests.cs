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
    protected override IInterval<T> NewInterval<T>(T from, T to) => new Interval<T>(from, to);

    protected override IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) => new Interval<T>(from, to);

    protected override IInterval<T> InfiniteInterval<T>() => Interval.Infinite<T>();

    protected override IInterval<T> EmptyInterval<T>() => Interval.Empty<T>();

    #region Parsing

    protected override IInterval<T> Parse<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null) =>
        Interval.Parse<T>(input, provider);

    protected override IInterval<T>? TryParse<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null) =>
        Interval.TryParse<T>(input, provider);

    #endregion

    #region Comparison

    protected override bool LessOperator<T>(IInterval<T>? left, T right) => ToInterval(left) < right;

    protected override bool LessOperator<T>(T left, IInterval<T>? right) => left < ToInterval(right);

    protected override bool GreaterOperator<T>(IInterval<T>? left, T right) => ToInterval(left) > right;

    protected override bool GreaterOperator<T>(T left, IInterval<T>? right) => left > ToInterval(right);

    protected override bool LessOrEqualOperator<T>(IInterval<T>? left, T right) => ToInterval(left) <= right;

    protected override bool LessOrEqualOperator<T>(T left, IInterval<T>? right) => left <= ToInterval(right);

    protected override bool GreaterOrEqualOperator<T>(IInterval<T>? left, T right) => ToInterval(left) >= right;

    protected override bool GreaterOrEqualOperator<T>(T left, IInterval<T>? right) => left >= ToInterval(right);

    #endregion

    static Interval<T>? ToInterval<T>(IInterval<T>? interval) => (Interval<T>?)interval;

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

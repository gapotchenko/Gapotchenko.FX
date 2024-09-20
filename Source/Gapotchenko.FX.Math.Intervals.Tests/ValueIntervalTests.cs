using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Intervals.Tests;

[TestClass]
public sealed class ValueIntervalTests : IntervalCoreTests
{
    public override IInterval<T> NewInterval<T>(T from, T to) => new ValueInterval<T>(from, to);

    public override IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) => new ValueInterval<T>(from, to);

    public override IInterval<T> InfiniteInterval<T>() => ValueInterval<T>.Infinite;

    public override IInterval<T> EmptyInterval<T>() => ValueInterval<T>.Empty;

    [TestMethod]
    public void ValueInterval_Default_1()
    {
        ValueInterval<string?> interval = default;
        Assert.IsTrue(interval.IsEmpty);
        Assert.IsFalse(interval.Contains(default));
    }
}

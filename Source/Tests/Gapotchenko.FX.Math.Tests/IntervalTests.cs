using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Tests;

[TestClass]
public sealed class IntervalTests : IntervalCoreTests
{
    public override IInterval<T> NewInterval<T>(T from, T to) => new Interval<T>(from, to);

    public override IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) => new Interval<T>(from, to);

    public override IInterval<T> InfiniteInterval<T>() => Interval<T>.Infinite;

    public override IInterval<T> EmptyInterval<T>() => Interval<T>.Empty;
}

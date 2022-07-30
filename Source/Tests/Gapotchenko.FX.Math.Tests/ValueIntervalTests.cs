using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Tests;

[TestClass]
public sealed class ValueIntervalTests : IntervalTestsBase
{
    public override IInterval<T> NewInterval<T>(T from, T to) => new ValueInterval<T>(from, to);

    public override IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) => new ValueInterval<T>(from, to);

    [TestMethod]
    public void ValueInterval_Empty_Default()
    {
        ValueInterval<string?> interval = default;
        Assert.IsTrue(interval.IsEmpty);
        Assert.IsFalse(interval.Contains(default));
    }
}

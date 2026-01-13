// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals.Tests;

partial class IntervalCoreTests
{
    [TestMethod]
    public void Interval_Core_ToString_Infinite()
    {
        var interval = NewInterval(ValueInterval.Infinite<int>());

        Assert.AreEqual("(-inf,inf)", interval.ToString());
        Assert.AreEqual("(-∞,∞)", interval.ToString("U", null));
    }

    [TestMethod]
    public void Interval_Core_ToString_Empty()
    {
        var interval = NewInterval(ValueInterval.Empty<int>());

        Assert.AreEqual("{}", interval.ToString());
        Assert.AreEqual("∅", interval.ToString("U", null));
    }
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using System.Globalization;

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

    [TestMethod]
    public void Interval_Core_ToString_CultureSpecific()
    {
        var interval = NewInterval(ValueInterval.Inclusive(10.5f, 20.3f));

        Assert.AreEqual("[10.5,20.3]", interval.ToString(null, CultureInfo.InvariantCulture));
        Assert.AreEqual("[10,5;20,3]", interval.ToString(null, CultureInfo.GetCultureInfo("de-DE")));
    }
}

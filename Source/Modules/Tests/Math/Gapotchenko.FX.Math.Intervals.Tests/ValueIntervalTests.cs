// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

#if TFF_JSON
using System.Text.Json;
#endif

namespace Gapotchenko.FX.Math.Intervals.Tests;

[TestClass]
public sealed class ValueIntervalTests : IntervalCoreTests
{
    protected override IInterval<T> NewInterval<T>(T from, T to) => new ValueInterval<T>(from, to);

    protected override IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) => new ValueInterval<T>(from, to);

    protected override IInterval<T> InfiniteInterval<T>() => ValueInterval.Infinite<T>();

    protected override IInterval<T> EmptyInterval<T>() => ValueInterval.Empty<T>();

    #region Parsing

    protected override IInterval<T> Parse<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null) =>
        ValueInterval.Parse<T>(input, provider);

    protected override IInterval<T>? TryParse<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null) =>
        ValueInterval.TryParse<T>(input, provider);

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

    [TestMethod]
    public void ValueInterval_Default()
    {
        ValueInterval<string?> interval = default;

        Assert.IsTrue(interval.IsEmpty);
        Assert.IsFalse(interval.Contains(default));
    }

    #region Equality

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

    #endregion

    #region Serialization

#if TFF_JSON

    [TestMethod]
    [DataRow("\"[10,20)\"", "[10,20)")]
    public void ValueInterval_Serialization_Json(string json, string? interval)
    {
        var obj = JsonSerializer.Deserialize<ValueInterval<int>>(json);
        Assert.AreEqual(ValueInterval.Parse<int>(interval), obj);
    }

#endif

    #endregion

    static ValueInterval<T> ToInterval<T>(IInterval<T>? interval) where T : IComparable<T>, IEquatable<T> =>
        (ValueInterval<T>?)interval ?? default;
}

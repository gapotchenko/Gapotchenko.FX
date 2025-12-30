// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals.Tests;

partial class IntervalCoreTests
{
    protected abstract bool LessOperator<T>(IInterval<T>? left, T right) where T : IComparable<T>, IEquatable<T>;

    protected abstract bool LessOperator<T>(T left, IInterval<T>? right) where T : IComparable<T>, IEquatable<T>;

    protected abstract bool GreaterOperator<T>(IInterval<T>? left, T right) where T : IComparable<T>, IEquatable<T>;

    protected abstract bool GreaterOperator<T>(T left, IInterval<T>? right) where T : IComparable<T>, IEquatable<T>;

    protected abstract bool LessOrEqualOperator<T>(IInterval<T>? left, T right) where T : IComparable<T>, IEquatable<T>;

    protected abstract bool LessOrEqualOperator<T>(T left, IInterval<T>? right) where T : IComparable<T>, IEquatable<T>;

    protected abstract bool GreaterOrEqualOperator<T>(IInterval<T>? left, T right) where T : IComparable<T>, IEquatable<T>;

    protected abstract bool GreaterOrEqualOperator<T>(T left, IInterval<T>? right) where T : IComparable<T>, IEquatable<T>;

    #region CompareTo

    [TestMethod]
    public void Interval_Core_CompareTo_1()
    {
        var interval = NewInterval(ValueInterval.Inclusive(10, 20));

        Assert.AreEqual(1, CompareTo(interval, 9));
        Assert.AreEqual(0, CompareTo(interval, 10));

        Assert.AreEqual(0, CompareTo(interval, 15));

        Assert.AreEqual(0, CompareTo(interval, 20));
        Assert.AreEqual(-1, CompareTo(interval, 21));
    }

    [TestMethod]
    public void Interval_Core_CompareTo_2()
    {
        var interval = NewInterval(ValueInterval.Exclusive(10, 20));

        Assert.AreEqual(1, CompareTo(interval, 9));
        Assert.AreEqual(1, CompareTo(interval, 10));
        Assert.AreEqual(0, CompareTo(interval, 11));

        Assert.AreEqual(0, CompareTo(interval, 15));

        Assert.AreEqual(0, CompareTo(interval, 19));
        Assert.AreEqual(-1, CompareTo(interval, 20));
        Assert.AreEqual(-1, CompareTo(interval, 21));
    }

    [TestMethod]
    public void Interval_Core_CompareTo_3()
    {
        var interval = InfiniteInterval<int>();

        Assert.AreEqual(0, CompareTo(interval, int.MinValue));
        Assert.AreEqual(0, CompareTo(interval, 0));
        Assert.AreEqual(0, CompareTo(interval, int.MaxValue));
    }

    [TestMethod]
    public void Interval_Core_CompareTo_4()
    {
        var interval = NewInterval(ValueInterval.Exclusive(19, 20));

        Assert.AreEqual(1, CompareTo(interval, 18));
        Assert.AreEqual(1, CompareTo(interval, 19));
        Assert.AreEqual(-1, CompareTo(interval, 20));
        Assert.AreEqual(-1, CompareTo(interval, 21));
    }

    [TestMethod]
    public void Interval_Core_CompareTo_5()
    {
        var interval = NewInterval(ValueInterval.Exclusive(19.0, 20.0));

        Assert.AreEqual(1, CompareTo(interval, 18));
        Assert.AreEqual(1, CompareTo(interval, 19));
        Assert.AreEqual(0, CompareTo(interval, 19.5));
        Assert.AreEqual(-1, CompareTo(interval, 20));
        Assert.AreEqual(-1, CompareTo(interval, 21));
    }

    [TestMethod]
    public void Interval_Core_CompareTo_Empty()
    {
        var interval = EmptyInterval<int>();

        Assert.AreEqual(0, CompareTo(interval, int.MinValue));
        Assert.AreEqual(0, CompareTo(interval, 0));
        Assert.AreEqual(0, CompareTo(interval, int.MaxValue));
    }

    [TestMethod]
    public void Interval_Core_CompareTo_Null()
    {
        IInterval<int>? interval = null;

        Assert.AreEqual(default, CompareTo(interval, int.MinValue));
        Assert.AreEqual(default, CompareTo(interval, 0));
        Assert.AreEqual(default, CompareTo(interval, int.MaxValue));
    }

    int CompareTo<T>(IInterval<T>? interval, T value)
        where T : IComparable<T>, IEquatable<T>
    {
        int? c = interval?.CompareTo(value);
        bool comparisonExists = interval?.IsEmpty == false;

        Assert.AreEqual(LessOperator(interval, value), comparisonExists && c < 0);
        Assert.AreEqual(LessOperator(value, interval), comparisonExists && c > 0);

        Assert.AreEqual(GreaterOperator(interval, value), comparisonExists && c > 0);
        Assert.AreEqual(GreaterOperator(value, interval), comparisonExists && c < 0);

        Assert.AreEqual(LessOrEqualOperator(interval, value), comparisonExists && c <= 0);
        Assert.AreEqual(LessOrEqualOperator(value, interval), comparisonExists && c >= 0);

        Assert.AreEqual(GreaterOrEqualOperator(interval, value), comparisonExists && c >= 0);
        Assert.AreEqual(GreaterOrEqualOperator(value, interval), comparisonExists && c <= 0);

        return c ?? default;
    }

    #endregion
}

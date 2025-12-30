// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals.Tests;

public abstract partial class IntervalCoreTests
{
    protected abstract IInterval<T> NewInterval<T>(T from, T to) where T : IComparable<T>, IEquatable<T>;

    /// <summary>
    /// Creates a new interval of the tested type from the specified template.
    /// </summary>
    protected IInterval<T> NewInterval<T>(IInterval<T> interval) where T : IComparable<T>, IEquatable<T> =>
        NewInterval(interval.From, interval.To);

    protected abstract IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to)
        where T : IComparable<T>, IEquatable<T>;

    protected abstract IInterval<T> InfiniteInterval<T>() where T : IComparable<T>, IEquatable<T>;

    protected abstract IInterval<T> EmptyInterval<T>() where T : IComparable<T>, IEquatable<T>;

    #region Characteristics

    [TestMethod]
    public void Interval_Core_Characteristics_LeftUnbounded()
    {
        var interval = NewInterval(IntervalBoundary.NegativeInfinity<int>(), IntervalBoundary.Inclusive(10));
        Assert.IsFalse(interval.IsBounded);
        Assert.IsTrue(interval.IsHalfBounded);
        Assert.IsTrue(interval.IsClosed);
    }

    [TestMethod]
    public void Interval_Core_Characteristics_RightUnbounded()
    {
        var interval = NewInterval(IntervalBoundary.Inclusive(10), IntervalBoundary.PositiveInfinity<int>());
        Assert.IsFalse(interval.IsBounded);
        Assert.IsTrue(interval.IsHalfBounded);
        Assert.IsTrue(interval.IsClosed);
    }

    [TestMethod]
    public void Interval_Core_Characteristics_Unbounded()
    {
        var interval = NewInterval(IntervalBoundary.NegativeInfinity<int>(), IntervalBoundary.PositiveInfinity<int>());
        Assert.IsFalse(interval.IsBounded);
        Assert.IsFalse(interval.IsHalfBounded);
        Assert.IsTrue(interval.IsClosed);
    }

    [TestMethod]
    public void Interval_Core_Characteristics_HalfOpenBounded()
    {
        var interval = NewInterval(0, 10);
        Assert.IsTrue(interval.IsHalfOpen);
        Assert.IsTrue(interval.IsBounded);
        Assert.IsFalse(interval.IsHalfBounded);
        Assert.IsFalse(interval.IsOpen);
        Assert.IsFalse(interval.IsClosed);
    }

    #endregion

    #region Constructor

    [TestMethod]
    public void Interval_Core_Constructor_1()
    {
        var interval = NewInterval(0, 10);

        Assert.AreEqual(IntervalBoundaryKind.Inclusive, interval.From.Kind);
        Assert.AreEqual(0, interval.From.Value);
        Assert.AreEqual(10, interval.To.Value);
        Assert.AreEqual(IntervalBoundaryKind.Exclusive, interval.To.Kind);
    }

    [TestMethod]
    public void Interval_Core_Constructor_2()
    {
        var interval = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(10));

        Assert.AreEqual(IntervalBoundaryKind.Exclusive, interval.From.Kind);
        Assert.AreEqual(0, interval.From.Value);
        Assert.AreEqual(10, interval.To.Value);
        Assert.AreEqual(IntervalBoundaryKind.Inclusive, interval.To.Kind);
    }

    #endregion

    #region Empty

    [TestMethod]
    public void Interval_Core_Empty_1()
    {
        var interval = EmptyInterval<int>();

        Assert.IsTrue(interval.IsEmpty);
    }

    [TestMethod]
    public void Interval_Core_Empty_2()
    {
        var interval = NewInterval(0, 0);

        Assert.IsTrue(interval.IsEmpty);
    }

    [TestMethod]
    public void Interval_Core_Empty_3()
    {
        var interval = NewInterval(1, 0);

        Assert.IsTrue(interval.IsEmpty);
    }

    [TestMethod]
    public void Interval_Core_Empty_4()
    {
        var a = NewInterval(1, 0);
        var b = EmptyInterval<int>();

        Assert.IsTrue(a.IntervalEquals(b));
    }

    #endregion

    #region Infinite

    [TestMethod]
    public void IntervalCore_Infinite_1()
    {
        var interval = InfiniteInterval<int>();
        Assert.IsTrue(interval.IsInfinite);
    }

    [TestMethod]
    public void Interval_Core_Infinite_2()
    {
        var interval = NewInterval(IntervalBoundary.NegativeInfinity<int>(), IntervalBoundary.PositiveInfinity<int>());
        Assert.IsTrue(interval.IsInfinite);
    }

    [TestMethod]
    public void Interval_Core_Infinite_3()
    {
        var a = NewInterval(IntervalBoundary.NegativeInfinity<int>(), IntervalBoundary.PositiveInfinity<int>());
        var b = InfiniteInterval<int>();

        Assert.IsTrue(a.IntervalEquals(b));
    }

    #endregion

    #region Contains

    [TestMethod]
    public void Interval_Core_Contains_Default()
    {
        var interval = NewInterval(0, 10);

        Assert.IsFalse(interval.Contains(-1));
        Assert.IsTrue(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(9));
        Assert.IsFalse(interval.Contains(10));
        Assert.IsFalse(interval.Contains(11));
    }

    [TestMethod]
    public void Interval_Core_Contains_BoundedRightOpen()
    {
        var interval = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(10));

        Assert.IsFalse(interval.Contains(-1));
        Assert.IsTrue(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(9));
        Assert.IsFalse(interval.Contains(10));
        Assert.IsFalse(interval.Contains(11));
    }

    [TestMethod]
    public void Interval_Core_Contains_BoundedLeftOpen()
    {
        var interval = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(interval.Contains(-1));
        Assert.IsFalse(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(9));
        Assert.IsTrue(interval.Contains(10));
        Assert.IsFalse(interval.Contains(11));
    }

    [TestMethod]
    public void Interval_Core_Contains_BoundedOpen()
    {
        var interval = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Exclusive(10));

        Assert.IsFalse(interval.Contains(-1));
        Assert.IsFalse(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(9));
        Assert.IsFalse(interval.Contains(10));
        Assert.IsFalse(interval.Contains(11));
    }

    [TestMethod]
    public void Interval_Core_Contains_BoundedClosed()
    {
        var interval = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(interval.Contains(-1));
        Assert.IsTrue(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(9));
        Assert.IsTrue(interval.Contains(10));
        Assert.IsFalse(interval.Contains(11));
    }

    [TestMethod]
    public void Interval_Core_Contains_Unbounded()
    {
        var interval = NewInterval(IntervalBoundary.NegativeInfinity<int>(), IntervalBoundary.PositiveInfinity<int>());

        Assert.IsTrue(interval.Contains(int.MinValue));

        Assert.IsTrue(interval.Contains(-1));
        Assert.IsTrue(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(int.MaxValue));
    }

    [TestMethod]
    public void Interval_Core_Contains_RightBoundedOpen()
    {
        var interval = NewInterval(IntervalBoundary.NegativeInfinity<int>(), IntervalBoundary.Exclusive(10));

        Assert.IsTrue(interval.Contains(int.MinValue));

        Assert.IsTrue(interval.Contains(9));
        Assert.IsFalse(interval.Contains(10));
        Assert.IsFalse(interval.Contains(11));
    }

    [TestMethod]
    public void Interval_Core_Contains_RightBoundedClosed()
    {
        var interval = NewInterval(IntervalBoundary.NegativeInfinity<int>(), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(interval.Contains(int.MinValue));

        Assert.IsTrue(interval.Contains(9));
        Assert.IsTrue(interval.Contains(10));
        Assert.IsFalse(interval.Contains(11));
    }

    [TestMethod]
    public void Interval_Core_Contains_LeftBoundedOpen()
    {
        var interval = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.PositiveInfinity<int>());

        Assert.IsFalse(interval.Contains(-1));
        Assert.IsFalse(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(int.MaxValue));
    }

    [TestMethod]
    public void Interval_Core_Contains_LeftBoundedClosed()
    {
        var interval = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.PositiveInfinity<int>());

        Assert.IsFalse(interval.Contains(-1));
        Assert.IsTrue(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(int.MaxValue));
    }

    #endregion

    #region Overlaps

    [TestMethod]
    public void Interval_Core_Overlaps_1()
    {
        var a = NewInterval(2, 10);
        Assert.IsTrue(a.Overlaps(a));

        var b = NewInterval(2, 10);

        Assert.IsTrue(a.Overlaps(b));
        Assert.IsTrue(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Core_Overlaps_2()
    {
        var a = NewInterval(0, 2);
        var b = NewInterval(2, 10);

        Assert.IsFalse(a.Overlaps(b));
        Assert.IsFalse(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Core_Overlaps_3()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(a.Overlaps(b));
        Assert.IsFalse(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Core_Overlaps_4()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Exclusive(2));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsFalse(a.Overlaps(b));
        Assert.IsFalse(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Core_Overlaps_5()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsTrue(a.Overlaps(b));
        Assert.IsTrue(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Core_Overlaps_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(a.Overlaps(b));
        Assert.IsFalse(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Core_Overlaps_7()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(3));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsTrue(a.Overlaps(b));
        Assert.IsTrue(b.Overlaps(a));
    }

    #endregion

    #region Intersect

    [TestMethod]
    public void Interval_Core_Intersect_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10))));
    }

    [TestMethod]
    public void Interval_Core_Intersect_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10))));
    }

    [TestMethod]
    public void Interval_Core_Intersect_3()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(9));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(9))));
    }

    [TestMethod]
    public void Interval_Core_Intersect_4()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Exclusive(9));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(9))));
    }

    [TestMethod]
    public void Interval_Core_Intersect_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IsEmpty);
    }

    [TestMethod]
    public void Interval_Core_Intersect_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(2))));
    }

    [TestMethod]
    public void Interval_Core_Intersect_7()
    {
        var a = InfiniteInterval<int>();
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10))));
    }

    [TestMethod]
    public void Interval_Core_Intersect_8()
    {
        var a = EmptyInterval<int>();
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IsEmpty);
    }

    #endregion

    #region Union

    [TestMethod]
    public void Interval_Core_Union_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        var c = a.Union(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10))));
    }

    [TestMethod]
    public void Interval_Core_Union_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10));

        var c = a.Union(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10))));
    }

    [TestMethod]
    public void Interval_Core_Union_3()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(9));

        var c = a.Union(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(10))));
    }

    [TestMethod]
    public void Interval_Core_Union_4()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Exclusive(9));

        var c = a.Union(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(10))));
    }

    [TestMethod]
    public void Interval_Core_Union_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        var c = a.Union(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(10))));
    }

    [TestMethod]
    public void Interval_Core_Union_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        var c = a.Union(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(10))));
    }

    [TestMethod]
    public void Interval_Core_Union_7()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Inclusive(3), IntervalBoundary.Inclusive(10));

        var c = a.Union(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(10))));
    }

    [TestMethod]
    public void Interval_Core_Union_8()
    {
        var a = InfiniteInterval<int>();
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        var c = a.Union(b);

        Assert.IsTrue(c.IsInfinite);
    }

    [TestMethod]
    public void Interval_Core_Union_9()
    {
        var a = EmptyInterval<int>();
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        var c = a.Union(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10))));
    }

    #endregion

    #region IntervalEquals

    [TestMethod]
    public void Interval_Core_IntervalEquals_1()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        Assert.IsTrue(a.IntervalEquals(a));
        //Assert.IsTrue(a == a);

        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(a.IntervalEquals(b));
        //Assert.IsTrue(a == b);

        Assert.IsTrue(b.IntervalEquals(a));
        //Assert.IsTrue(b == a);
    }

    [TestMethod]
    public void Interval_Core_IntervalEquals_2()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsFalse(a.IntervalEquals(b));
        Assert.IsFalse(b.IntervalEquals(a));

        Assert.IsFalse(a == b);
        Assert.IsFalse(b == a);
    }

    #endregion

    #region IsSubintervalOf

    [TestMethod]
    public void Interval_Core_IsSubintervalOf_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        Assert.IsTrue(a.IsSubintervalOf(a));

        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSubintervalOf(a));
        Assert.IsTrue(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSubintervalOf_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(3), IntervalBoundary.Inclusive(9));

        Assert.IsTrue(b.IsSubintervalOf(a));
        Assert.IsFalse(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSubintervalOf_3()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSubintervalOf(a));
        Assert.IsFalse(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSubintervalOf_4()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSubintervalOf(a));
        Assert.IsTrue(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSubintervalOf_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsTrue(b.IsSubintervalOf(a));
        Assert.IsFalse(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSubintervalOf_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSubintervalOf(a));
        Assert.IsTrue(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSubintervalOf_7()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSubintervalOf(a));
        Assert.IsFalse(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSubintervalOf_8()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSubintervalOf(a));
    }

    [TestMethod]
    public void Interval_Core_IsSubintervalOf_9()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(11));

        Assert.IsFalse(b.IsSubintervalOf(a));
    }

    #endregion

    #region IsSuperintervalOf

    [TestMethod]
    public void Interval_Core_IsSuperintervalOf_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        Assert.IsTrue(a.IsSuperintervalOf(a));

        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSuperintervalOf(a));
        Assert.IsTrue(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSuperintervalOf_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(3), IntervalBoundary.Inclusive(9));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSuperintervalOf(a));
        Assert.IsFalse(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSuperintervalOf_3()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSuperintervalOf(a));
        Assert.IsFalse(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSuperintervalOf_4()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSuperintervalOf(a));
        Assert.IsTrue(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSuperintervalOf_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSuperintervalOf(a));
        Assert.IsFalse(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSuperintervalOf_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsFalse(b.IsSuperintervalOf(a));
        Assert.IsTrue(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSuperintervalOf_7()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));

        Assert.IsFalse(b.IsSuperintervalOf(a));
        Assert.IsFalse(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsSuperintervalOf_8()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSuperintervalOf(a));
    }

    [TestMethod]
    public void Interval_Core_IsSuperintervalOf_9()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(11));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSuperintervalOf(a));
    }

    #endregion

    #region IsProperSubintervalOf

    [TestMethod]
    public void Interval_Core_IsProperSubintervalOf_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        Assert.IsFalse(a.IsProperSubintervalOf(a));

        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
        Assert.IsFalse(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSubintervalOf_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(3), IntervalBoundary.Inclusive(9));

        Assert.IsTrue(b.IsProperSubintervalOf(a));
        Assert.IsFalse(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSubintervalOf_3()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsProperSubintervalOf(a));
        Assert.IsFalse(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSubintervalOf_4()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
        Assert.IsTrue(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSubintervalOf_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsTrue(b.IsProperSubintervalOf(a));
        Assert.IsFalse(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSubintervalOf_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
        Assert.IsTrue(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSubintervalOf_7()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
        Assert.IsFalse(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSubintervalOf_8()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
    }

    [TestMethod]
    public void Interval_Core_IsProperSubintervalOf_9()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(11));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
    }

    #endregion

    #region IsProperSuperintervalOf

    [TestMethod]
    public void Interval_Core_IsProperSuperintervalOf_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        Assert.IsFalse(a.IsProperSuperintervalOf(a));

        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
        Assert.IsFalse(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSuperintervalOf_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(3), IntervalBoundary.Inclusive(9));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsProperSuperintervalOf(a));
        Assert.IsFalse(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSuperintervalOf_3()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsProperSuperintervalOf(a));
        Assert.IsFalse(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSuperintervalOf_4()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
        Assert.IsTrue(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSuperintervalOf_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsProperSuperintervalOf(a));
        Assert.IsFalse(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSuperintervalOf_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
        Assert.IsTrue(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSuperintervalOf_7()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
        Assert.IsFalse(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_Core_IsProperSuperintervalOf_8()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
    }

    [TestMethod]
    public void Interval_Core_IsProperSuperintervalOf_9()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(11));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
    }

    #endregion

    #region Sequence Intersect

    [TestMethod]
    public void Interval_Core_Sequence_Intersect_1()
    {
        var sequence = Enumerable.Range(1, 5);
        var interval = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(4));

        var result = sequence.Intersect(interval);
        Assert.IsTrue(result.SequenceEqual([2, 3, 4]));
    }

    [TestMethod]
    public void Interval_Core_Sequence_Intersect_2()
    {
        var sequence = Enumerable.Range(1, 5);
        var interval = NewInterval(ValueInterval.Empty<int>());

        var result = sequence.Intersect(interval);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public void Interval_Core_Sequence_Intersect_3()
    {
        var sequence = Enumerable.Range(1, 5);
        var interval = NewInterval(ValueInterval.Infinite<int>());

        var result = sequence.Intersect(interval);
        Assert.IsTrue(result.SequenceEqual([1, 2, 3, 4, 5]));
    }

    #endregion

    #region Sequence Except

    [TestMethod]
    public void Interval_Core_Sequence_Except_1()
    {
        var sequence = Enumerable.Range(1, 5);
        var interval = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(4));

        var result = sequence.Except(interval);
        Assert.IsTrue(result.SequenceEqual([1, 5]));
    }

    [TestMethod]
    public void Interval_Core_Sequence_Except_2()
    {
        var sequence = Enumerable.Range(1, 5);
        var interval = NewInterval(ValueInterval.Empty<int>());

        var result = sequence.Except(interval);
        Assert.IsTrue(result.SequenceEqual([1, 2, 3, 4, 5]));
    }

    [TestMethod]
    public void Interval_Core_Sequence_Except_3()
    {
        var sequence = Enumerable.Range(1, 5);
        var interval = NewInterval(ValueInterval.Infinite<int>());

        var result = sequence.Except(interval);
        Assert.AreEqual(0, result.Count());
    }

    #endregion

    #region Set Intersect

    [TestMethod]
    public void Interval_Core_Set_IntersectWith_1()
    {
        var set = new HashSet<int> { 1, 2, 3, 5, 6, 7 };
        var interval = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(6));

        set.IntersectWith(interval);

        Assert.IsTrue(set.SetEquals([2, 3, 5, 6]));
    }

    [TestMethod]
    public void Interval_Core_Set_IntersectWith_2()
    {
        var set = new HashSet<int> { 1, 2, 3, 4, 5 };
        var interval = EmptyInterval<int>();

        set.IntersectWith(interval);

        Assert.AreEqual(0, set.Count);
    }

    [TestMethod]
    public void Interval_Core_Set_IntersectWith_3()
    {
        var set = new HashSet<int> { 1, 2, 3, 4, 5 };
        var interval = InfiniteInterval<int>();

        set.IntersectWith(interval);

        Assert.IsTrue(set.SetEquals([1, 2, 3, 4, 5]));
    }

    #endregion

    #region Set Except

    [TestMethod]
    public void Interval_Core_Set_ExceptWith_1()
    {
        var set = new HashSet<int> { 1, 2, 3, 5, 6, 7 };
        var interval = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(6));

        set.ExceptWith(interval);

        Assert.IsTrue(set.SetEquals([1, 7]));
    }

    [TestMethod]
    public void Interval_Core_Set_ExceptWith_2()
    {
        var set = new HashSet<int> { 1, 2, 3, 4, 5 };
        var interval = EmptyInterval<int>();

        set.ExceptWith(interval);

        Assert.IsTrue(set.SetEquals([1, 2, 3, 4, 5]));
    }

    [TestMethod]
    public void Interval_Core_Set_ExceptWith_3()
    {
        var set = new HashSet<int> { 1, 2, 3, 4, 5 };
        var interval = InfiniteInterval<int>();

        set.ExceptWith(interval);

        Assert.AreEqual(0, set.Count);
    }

    #endregion

    #region Clamp

    [TestMethod]
    public void Interval_Core_Clamp_FloatingPoint_1()
    {
        var interval = NewInterval(ValueInterval.Exclusive(10f, 20f));
        var clamped = interval.Clamp(21f);

        Assert.IsTrue(clamped.HasValue);
        Assert.IsTrue(clamped.Value is > 19 and < 20);
        Assert.IsTrue(interval.Contains(clamped.Value));
    }

    [TestMethod]
    public void Interval_Core_Clamp_FloatingPoint_2()
    {
        var interval = NewInterval(ValueInterval.Exclusive(10f, 20f));
        var clamped = interval.Clamp(9f);

        Assert.IsTrue(clamped.HasValue);
        Assert.IsTrue(clamped.Value is > 10 and < 11);
        Assert.IsTrue(interval.Contains(clamped.Value));
    }

    [TestMethod]
    public void Interval_Core_Clamp_FloatingPoint_3()
    {
        var interval = NewInterval(ValueInterval.Inclusive(10f, 20f));
        var clamped = interval.Clamp(21f);

        Assert.IsTrue(clamped.HasValue);
        Assert.AreEqual(20, clamped.Value);
        Assert.IsTrue(interval.Contains(clamped.Value));
    }

    [TestMethod]
    public void Interval_Core_Clamp_FloatingPoint_4()
    {
        var interval = NewInterval(ValueInterval.Inclusive(10f, 20f));
        var clamped = interval.Clamp(9f);

        Assert.IsTrue(clamped.HasValue);
        Assert.AreEqual(10, clamped.Value);
        Assert.IsTrue(interval.Contains(clamped.Value));
    }

    [TestMethod]
    public void Interval_Core_Clamp_Integer_1()
    {
        var interval = NewInterval(ValueInterval.Exclusive(10, 20));
        var clamped = interval.Clamp(21);

        Assert.IsTrue(clamped.HasValue);
        Assert.AreEqual(19, clamped.Value);
        Assert.IsTrue(interval.Contains(clamped.Value));
    }

    [TestMethod]
    public void Interval_Core_Clamp_Integer_2()
    {
        var interval = NewInterval(ValueInterval.Exclusive(10, 20));
        var clamped = interval.Clamp(9);

        Assert.IsTrue(clamped.HasValue);
        Assert.AreEqual(11, clamped.Value);
        Assert.IsTrue(interval.Contains(clamped.Value));
    }

    [TestMethod]
    public void Interval_Core_Clamp_Integer_3()
    {
        var interval = NewInterval(ValueInterval.Inclusive(10, 20));
        var clamped = interval.Clamp(21);

        Assert.IsTrue(clamped.HasValue);
        Assert.AreEqual(20, clamped.Value);
        Assert.IsTrue(interval.Contains(clamped.Value));
    }

    [TestMethod]
    public void Interval_Core_Clamp_Integer_4()
    {
        var interval = NewInterval(ValueInterval.Inclusive(10, 20));
        var clamped = interval.Clamp(9);

        Assert.IsTrue(clamped.HasValue);
        Assert.AreEqual(10, clamped.Value);
        Assert.IsTrue(interval.Contains(clamped.Value));
    }

    [TestMethod]
    public void Interval_Core_Clamp_Integer_5()
    {
        var interval = NewInterval(ValueInterval.Exclusive(10, 11));
        var clamped = interval.Clamp(21);

        Assert.IsFalse(clamped.HasValue);
    }

    [TestMethod]
    public void Interval_Core_Clamp_Integer_6()
    {
        var interval = NewInterval(ValueInterval.Degenerate(10));
        var clamped = interval.Clamp(21);

        Assert.IsTrue(clamped.HasValue);
        Assert.AreEqual(10, clamped.Value);
    }

    [TestMethod]
    public void Interval_Core_Clamp_Integer_7()
    {
        var interval = NewInterval(ValueInterval.ExclusiveInclusive(int.MaxValue, int.MaxValue));
        var clamped = interval.Clamp(21);

        Assert.IsFalse(clamped.HasValue);
    }

    [TestMethod]
    public void Interval_Core_Clamp_Integer_8()
    {
        var interval = NewInterval(ValueInterval.Exclusive(19, 20));
        var clamped = interval.Clamp(21);

        // The clamped value belonging to (19,20) interval cannot be represented by Int32 type.
        Assert.IsFalse(clamped.HasValue);
    }

    #endregion

    #region Equality

    [TestMethod]
    public void Interval_Core_Equality()
    {
        var interval = NewInterval(ValueInterval.Inclusive(10, 20));
        var comparer = IntervalEqualityComparer.Default<int>();

        Assert.AreEqual(interval, ValueInterval.Inclusive(10, 20), comparer);

        Assert.AreNotEqual(interval, ValueInterval.Exclusive(10, 20), comparer);
        Assert.AreNotEqual(interval, ValueInterval.InclusiveExclusive(10, 20), comparer);
        Assert.AreNotEqual(interval, ValueInterval.ExclusiveInclusive(10, 20), comparer);

        Assert.AreNotEqual(interval, ValueInterval.Inclusive(11, 20), comparer);
        Assert.AreNotEqual(interval, ValueInterval.Inclusive(10, 21), comparer);
    }

    #endregion

    #region ToString

    [TestMethod]
    public void Interval_Core_ToString_Inifinite()
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

    #endregion
}

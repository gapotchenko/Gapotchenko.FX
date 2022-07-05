﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Tests;

public abstract class IntervalTestsBase
{
    public abstract IInterval<T> NewInterval<T>(T from, T to) where T : IComparable<T>, IEquatable<T>;

    public abstract IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) where T : IComparable<T>, IEquatable<T>;

    [TestMethod]
    public void Interval_Characteristics_LeftUnbounded()
    {
        var interval = NewInterval(IntervalBoundary<int>.NegativeInfinity, IntervalBoundary.Inclusive(10));
        Assert.IsFalse(interval.IsBounded);
        Assert.IsTrue(interval.IsHalfBounded);
        Assert.IsTrue(interval.IsClosed);
    }

    [TestMethod]
    public void Interval_Characteristics_RightUnbounded()
    {
        var interval = NewInterval(IntervalBoundary.Inclusive(10), IntervalBoundary<int>.PositiveInfinity);
        Assert.IsFalse(interval.IsBounded);
        Assert.IsTrue(interval.IsHalfBounded);
        Assert.IsTrue(interval.IsClosed);
    }

    [TestMethod]
    public void Interval_Characteristics_Unbounded()
    {
        var interval = NewInterval(IntervalBoundary<int>.NegativeInfinity, IntervalBoundary<int>.PositiveInfinity);
        Assert.IsFalse(interval.IsBounded);
        Assert.IsFalse(interval.IsHalfBounded);
        Assert.IsTrue(interval.IsClosed);
    }

    [TestMethod]
    public void Interval_Characteristics_HalfOpenBounded()
    {
        var interval = NewInterval(0, 10);
        Assert.IsTrue(interval.IsHalfOpen);
        Assert.IsTrue(interval.IsBounded);
        Assert.IsFalse(interval.IsHalfBounded);
        Assert.IsFalse(interval.IsOpen);
        Assert.IsFalse(interval.IsClosed);
    }

    #region Constructor

    [TestMethod]
    public void Interval_Constructor_1()
    {
        var interval = NewInterval(0, 10);

        Assert.AreEqual(IntervalBoundaryKind.Inclusive, interval.From.Kind);
        Assert.AreEqual(0, interval.From.Value);
        Assert.AreEqual(10, interval.To.Value);
        Assert.AreEqual(IntervalBoundaryKind.Exclusive, interval.To.Kind);
    }

    [TestMethod]
    public void Interval_Constructor_2()
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
    public void Interval_Empty_1()
    {
        var interval = NewInterval(0, 0);

        Assert.IsTrue(interval.IsEmpty);
    }

    [TestMethod]
    public void Interval_Empty_2()
    {
        var interval = NewInterval(1, 0);

        Assert.IsTrue(interval.IsEmpty);
    }

    #endregion

    #region Infinite

    [TestMethod]
    public void Interval_Infinite_1()
    {
        var interval = NewInterval(IntervalBoundary<int>.NegativeInfinity, IntervalBoundary<int>.PositiveInfinity);

        Assert.IsTrue(interval.IsInfinite);
    }

    #endregion

    #region Contains

    [TestMethod]
    public void Interval_Contains_Default()
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
    public void Interval_Contains_BoundedRightOpen()
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
    public void Interval_Contains_BoundedLeftOpen()
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
    public void Interval_Contains_BoundedOpen()
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
    public void Interval_Contains_BoundedClosed()
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
    public void Interval_Contains_Unbounded()
    {
        var interval = NewInterval(IntervalBoundary<int>.NegativeInfinity, IntervalBoundary<int>.PositiveInfinity);

        Assert.IsTrue(interval.Contains(int.MinValue));

        Assert.IsTrue(interval.Contains(-1));
        Assert.IsTrue(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(int.MaxValue));
    }

    [TestMethod]
    public void Interval_Contains_RightBoundedOpen()
    {
        var interval = NewInterval(IntervalBoundary<int>.NegativeInfinity, IntervalBoundary.Exclusive(10));

        Assert.IsTrue(interval.Contains(int.MinValue));

        Assert.IsTrue(interval.Contains(9));
        Assert.IsFalse(interval.Contains(10));
        Assert.IsFalse(interval.Contains(11));
    }

    [TestMethod]
    public void Interval_Contains_RightBoundedClosed()
    {
        var interval = NewInterval(IntervalBoundary<int>.NegativeInfinity, IntervalBoundary.Inclusive(10));

        Assert.IsTrue(interval.Contains(int.MinValue));

        Assert.IsTrue(interval.Contains(9));
        Assert.IsTrue(interval.Contains(10));
        Assert.IsFalse(interval.Contains(11));
    }

    [TestMethod]
    public void Interval_Contains_LeftBoundedOpen()
    {
        var interval = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary<int>.PositiveInfinity);

        Assert.IsFalse(interval.Contains(-1));
        Assert.IsFalse(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(int.MaxValue));
    }

    [TestMethod]
    public void Interval_Contains_LeftBoundedClosed()
    {
        var interval = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary<int>.PositiveInfinity);

        Assert.IsFalse(interval.Contains(-1));
        Assert.IsTrue(interval.Contains(0));
        Assert.IsTrue(interval.Contains(1));

        Assert.IsTrue(interval.Contains(int.MaxValue));
    }

    #endregion

    #region Overlaps

    [TestMethod]
    public void Interval_Overlaps_1()
    {
        var a = NewInterval(2, 10);
        Assert.IsTrue(a.Overlaps(a));

        var b = NewInterval(2, 10);

        Assert.IsTrue(a.Overlaps(b));
        Assert.IsTrue(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Overlaps_2()
    {
        var a = NewInterval(0, 2);
        var b = NewInterval(2, 10);

        Assert.IsFalse(a.Overlaps(b));
        Assert.IsFalse(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Overlaps_3()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(a.Overlaps(b));
        Assert.IsFalse(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Overlaps_4()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Exclusive(2));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsFalse(a.Overlaps(b));
        Assert.IsFalse(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Overlaps_5()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsTrue(a.Overlaps(b));
        Assert.IsTrue(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Overlaps_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(a.Overlaps(b));
        Assert.IsFalse(b.Overlaps(a));
    }

    [TestMethod]
    public void Interval_Overlaps_7()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(3));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsTrue(a.Overlaps(b));
        Assert.IsTrue(b.Overlaps(a));
    }

    #endregion

    #region Intersect

    [TestMethod]
    public void Interval_Intersect_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10))));
    }

    [TestMethod]
    public void Interval_Intersect_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10))));
    }

    [TestMethod]
    public void Interval_Intersect_3()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(9));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(9))));
    }

    [TestMethod]
    public void Interval_Intersect_4()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Exclusive(9));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(9))));
    }

    [TestMethod]
    public void Interval_Intersect_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IsEmpty);
    }

    [TestMethod]
    public void Interval_Intersect_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Inclusive(2));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(2))));
    }

    [TestMethod]
    public void Interval_Intersect_7()
    {
        var a = NewInterval(IntervalBoundary<int>.NegativeInfinity, IntervalBoundary<int>.PositiveInfinity);
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        var c = a.Intersect(b);

        Assert.IsTrue(c.IntervalEquals(NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10))));
    }

    #endregion

    #region IntervalEquals

    [TestMethod]
    public void Interval_IntervalEquals_1()
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
    public void Interval_IntervalEquals_2()
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
    public void Interval_IsSubintervalOf_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        Assert.IsTrue(a.IsSubintervalOf(a));

        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSubintervalOf(a));
        Assert.IsTrue(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSubintervalOf_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(3), IntervalBoundary.Inclusive(9));

        Assert.IsTrue(b.IsSubintervalOf(a));
        Assert.IsFalse(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSubintervalOf_3()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSubintervalOf(a));
        Assert.IsFalse(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSubintervalOf_4()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSubintervalOf(a));
        Assert.IsTrue(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSubintervalOf_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsTrue(b.IsSubintervalOf(a));
        Assert.IsFalse(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSubintervalOf_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSubintervalOf(a));
        Assert.IsTrue(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSubintervalOf_7()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSubintervalOf(a));
        Assert.IsFalse(a.IsSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSubintervalOf_8()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSubintervalOf(a));
    }

    [TestMethod]
    public void Interval_IsSubintervalOf_9()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(11));

        Assert.IsFalse(b.IsSubintervalOf(a));
    }

    #endregion

    #region IsSuperintervalOf

    [TestMethod]
    public void Interval_IsSuperintervalOf_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        Assert.IsTrue(a.IsSuperintervalOf(a));

        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSuperintervalOf(a));
        Assert.IsTrue(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSuperintervalOf_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(3), IntervalBoundary.Inclusive(9));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSuperintervalOf(a));
        Assert.IsFalse(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSuperintervalOf_3()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSuperintervalOf(a));
        Assert.IsFalse(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSuperintervalOf_4()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSuperintervalOf(a));
        Assert.IsTrue(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSuperintervalOf_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsSuperintervalOf(a));
        Assert.IsFalse(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSuperintervalOf_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsFalse(b.IsSuperintervalOf(a));
        Assert.IsTrue(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSuperintervalOf_7()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));

        Assert.IsFalse(b.IsSuperintervalOf(a));
        Assert.IsFalse(a.IsSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsSuperintervalOf_8()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSuperintervalOf(a));
    }

    [TestMethod]
    public void Interval_IsSuperintervalOf_9()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(11));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsSuperintervalOf(a));
    }

    #endregion

    #region IsProperSubintervalOf

    [TestMethod]
    public void Interval_IsProperSubintervalOf_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        Assert.IsFalse(a.IsProperSubintervalOf(a));

        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
        Assert.IsFalse(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSubintervalOf_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(3), IntervalBoundary.Inclusive(9));

        Assert.IsTrue(b.IsProperSubintervalOf(a));
        Assert.IsFalse(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSubintervalOf_3()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsProperSubintervalOf(a));
        Assert.IsFalse(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSubintervalOf_4()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
        Assert.IsTrue(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSubintervalOf_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsTrue(b.IsProperSubintervalOf(a));
        Assert.IsFalse(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSubintervalOf_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
        Assert.IsTrue(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSubintervalOf_7()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
        Assert.IsFalse(a.IsProperSubintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSubintervalOf_8()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
    }

    [TestMethod]
    public void Interval_IsProperSubintervalOf_9()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(11));

        Assert.IsFalse(b.IsProperSubintervalOf(a));
    }

    #endregion

    #region IsProperSuperintervalOf

    [TestMethod]
    public void Interval_IsProperSuperintervalOf_1()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        Assert.IsFalse(a.IsProperSuperintervalOf(a));

        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
        Assert.IsFalse(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSuperintervalOf_2()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(3), IntervalBoundary.Inclusive(9));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsProperSuperintervalOf(a));
        Assert.IsFalse(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSuperintervalOf_3()
    {
        var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsProperSuperintervalOf(a));
        Assert.IsFalse(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSuperintervalOf_4()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
        Assert.IsTrue(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSuperintervalOf_5()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsTrue(b.IsProperSuperintervalOf(a));
        Assert.IsFalse(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSuperintervalOf_6()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
        Assert.IsTrue(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSuperintervalOf_7()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
        Assert.IsFalse(a.IsProperSuperintervalOf(b));
    }

    [TestMethod]
    public void Interval_IsProperSuperintervalOf_8()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(1), IntervalBoundary.Inclusive(10));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
    }

    [TestMethod]
    public void Interval_IsProperSuperintervalOf_9()
    {
        var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(11));
        var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

        Assert.IsFalse(b.IsProperSuperintervalOf(a));
    }

    #endregion
}

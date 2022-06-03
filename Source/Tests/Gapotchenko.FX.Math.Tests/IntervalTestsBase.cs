using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Tests
{
    public abstract class IntervalTestsBase
    {
        public abstract IInterval<T> NewInterval<T>(T from, T to, IComparer<T>? comparer = null);

        public abstract IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to, IComparer<T>? comparer = null);

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

        [TestMethod]
        public void Interval_Overlaps_1()
        {
            var a = NewInterval(0, 2);
            var b = NewInterval(2, 10);

            Assert.IsFalse(a.Overlaps(b));
            Assert.IsFalse(b.Overlaps(a));
        }

        [TestMethod]
        public void Interval_Overlaps_2()
        {
            var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(2));
            var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

            Assert.IsFalse(a.Overlaps(b));
            Assert.IsFalse(b.Overlaps(a));
        }

        [TestMethod]
        public void Interval_Overlaps_3()
        {
            var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Exclusive(2));
            var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10));

            Assert.IsFalse(a.Overlaps(b));
            Assert.IsFalse(b.Overlaps(a));
        }

        [TestMethod]
        public void Interval_Overlaps_4()
        {
            var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(2));
            var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Exclusive(10));

            Assert.IsTrue(a.Overlaps(b));
            Assert.IsTrue(b.Overlaps(a));
        }

        [TestMethod]
        public void Interval_Overlaps_5()
        {
            var a = NewInterval(IntervalBoundary.Inclusive(0), IntervalBoundary.Exclusive(2));
            var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

            Assert.IsFalse(a.Overlaps(b));
            Assert.IsFalse(b.Overlaps(a));
        }

        [TestMethod]
        public void Interval_Overlaps_6()
        {
            var a = NewInterval(IntervalBoundary.Exclusive(0), IntervalBoundary.Inclusive(3));
            var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Exclusive(10));

            Assert.IsTrue(a.Overlaps(b));
            Assert.IsTrue(b.Overlaps(a));
        }

        [TestMethod]
        public void Interval_IntervalEquals_1()
        {
            var a = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));
            var b = NewInterval(IntervalBoundary.Exclusive(2), IntervalBoundary.Inclusive(10));

            Assert.IsTrue(a.IntervalEquals(b));
            Assert.IsTrue(b.IntervalEquals(a));

            //Assert.IsTrue(a == b);
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

        [TestMethod]
        public void Interval_IsSubintervalOf_1()
        {
            var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
            var b = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));

            Assert.IsTrue(b.IsSubintervalOf(b));
        }

        [TestMethod]
        public void Interval_IsSubintervalOf_2()
        {
            var a = NewInterval(IntervalBoundary.Inclusive(2), IntervalBoundary.Inclusive(10));
            var b = NewInterval(IntervalBoundary.Inclusive(3), IntervalBoundary.Inclusive(9));

            Assert.IsTrue(b.IsSubintervalOf(b));
        }

    }
}

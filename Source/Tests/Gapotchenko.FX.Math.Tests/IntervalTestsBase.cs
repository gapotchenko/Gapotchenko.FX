using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Tests
{
    public abstract class IntervalTestsBase
    {
        public abstract IInterval<T> NewInterval<T>(T from, T to, IComparer<T>? comparer = null);

        public abstract IInterval<T> NewInterval<T>(IntervalBoundary fromBoundary, T from, T to, IntervalBoundary toBoundary, IComparer<T>? comparer = null);

        [TestMethod]
        public void Interval_Characteristics_LeftUnbounded()
        {
            var interval = NewInterval(IntervalBoundary.Infinite, default, 10, IntervalBoundary.Inclusive);
            Assert.IsFalse(interval.IsBounded);
            Assert.IsTrue(interval.IsHalfBounded);
            Assert.IsTrue(interval.IsClosed);
        }

        [TestMethod]
        public void Interval_Characteristics_RightUnbounded()
        {
            var interval = NewInterval(IntervalBoundary.Inclusive, 10, default, IntervalBoundary.Infinite);
            Assert.IsFalse(interval.IsBounded);
            Assert.IsTrue(interval.IsHalfBounded);
            Assert.IsTrue(interval.IsClosed);
        }

        [TestMethod]
        public void Interval_Characteristics_Unbounded()
        {
            var interval = NewInterval<int>(IntervalBoundary.Infinite, default, default, IntervalBoundary.Infinite);
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

            Assert.AreEqual(IntervalBoundary.Inclusive, interval.FromBoundary);
            Assert.AreEqual(0, interval.From);
            Assert.AreEqual(10, interval.To);
            Assert.AreEqual(IntervalBoundary.Exclusive, interval.ToBoundary);
        }

        [TestMethod]
        public void Interval_Constructor_2()
        {
            var interval = NewInterval(IntervalBoundary.Exclusive, 0, 10, IntervalBoundary.Inclusive);

            Assert.AreEqual(IntervalBoundary.Exclusive, interval.FromBoundary);
            Assert.AreEqual(0, interval.From);
            Assert.AreEqual(10, interval.To);
            Assert.AreEqual(IntervalBoundary.Inclusive, interval.ToBoundary);
        }

        [TestMethod]
        public void Interval_Reverse()
        {
            var interval = NewInterval(IntervalBoundary.Inclusive, 0, 10, IntervalBoundary.Exclusive);
            interval = interval.Reverse();

            Assert.AreEqual(IntervalBoundary.Exclusive, interval.FromBoundary);
            Assert.AreEqual(10, interval.From);
            Assert.AreEqual(0, interval.To);
            Assert.AreEqual(IntervalBoundary.Inclusive, interval.ToBoundary);
        }

        [TestMethod]
        public void Interval_Contains_Default()
        {
            static void Test(IInterval<int> interval)
            {
                Assert.IsFalse(interval.Contains(-1));
                Assert.IsTrue(interval.Contains(0));
                Assert.IsTrue(interval.Contains(1));

                Assert.IsTrue(interval.Contains(9));
                Assert.IsFalse(interval.Contains(10));
                Assert.IsFalse(interval.Contains(11));
            }

            var interval = NewInterval(0, 10);
            Test(interval);
            Test(interval.Reverse());
        }

        [TestMethod]
        public void Interval_Contains_BoundedRightOpen()
        {
            static void Test(IInterval<int> interval)
            {
                Assert.IsFalse(interval.Contains(-1));
                Assert.IsTrue(interval.Contains(0));
                Assert.IsTrue(interval.Contains(1));

                Assert.IsTrue(interval.Contains(9));
                Assert.IsFalse(interval.Contains(10));
                Assert.IsFalse(interval.Contains(11));
            }

            var interval = NewInterval(IntervalBoundary.Inclusive, 0, 10, IntervalBoundary.Exclusive);
            Test(interval);
            Test(interval.Reverse());
        }

        [TestMethod]
        public void Interval_Contains_BoundedLeftOpen()
        {
            static void Test(IInterval<int> interval)
            {
                Assert.IsFalse(interval.Contains(-1));
                Assert.IsFalse(interval.Contains(0));
                Assert.IsTrue(interval.Contains(1));

                Assert.IsTrue(interval.Contains(9));
                Assert.IsTrue(interval.Contains(10));
                Assert.IsFalse(interval.Contains(11));
            }

            var interval = NewInterval(IntervalBoundary.Exclusive, 0, 10, IntervalBoundary.Inclusive);
            Test(interval);
            Test(interval.Reverse());
        }

        [TestMethod]
        public void Interval_Contains_BoundedOpen()
        {
            static void Test(IInterval<int> interval)
            {
                Assert.IsFalse(interval.Contains(-1));
                Assert.IsFalse(interval.Contains(0));
                Assert.IsTrue(interval.Contains(1));

                Assert.IsTrue(interval.Contains(9));
                Assert.IsFalse(interval.Contains(10));
                Assert.IsFalse(interval.Contains(11));
            }

            var interval = NewInterval(IntervalBoundary.Exclusive, 0, 10, IntervalBoundary.Exclusive);
            Test(interval);
            Test(interval.Reverse());
        }

        [TestMethod]
        public void Interval_Contains_BoundedClosed()
        {
            static void Test(IInterval<int> interval)
            {
                Assert.IsFalse(interval.Contains(-1));
                Assert.IsTrue(interval.Contains(0));
                Assert.IsTrue(interval.Contains(1));

                Assert.IsTrue(interval.Contains(9));
                Assert.IsTrue(interval.Contains(10));
                Assert.IsFalse(interval.Contains(11));
            }

            var interval = NewInterval(IntervalBoundary.Inclusive, 0, 10, IntervalBoundary.Inclusive);
            Test(interval);
            Test(interval.Reverse());
        }

        [TestMethod]
        public void Interval_Contains_Unbounded()
        {
            var interval = NewInterval(IntervalBoundary.Infinite, 0, 0, IntervalBoundary.Infinite);

            Assert.IsTrue(interval.Contains(int.MinValue));

            Assert.IsTrue(interval.Contains(-1));
            Assert.IsTrue(interval.Contains(0));
            Assert.IsTrue(interval.Contains(1));

            Assert.IsTrue(interval.Contains(int.MaxValue));
        }
    }
}

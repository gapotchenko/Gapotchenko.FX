using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Tests
{
    [TestClass]
    public class IntervalTests
    {
        [TestMethod]
        public void Interval_LeftUnbounded()
        {
            var interval = new Interval<int>(IntervalBoundary.Infinite, default, 10, IntervalBoundary.Inclusive);
            Assert.IsFalse(interval.IsBounded);
            Assert.IsTrue(interval.IsHalfBounded);
            Assert.IsTrue(interval.IsClosed);
        }

        [TestMethod]
        public void Interval_RightUnbounded()
        {
            var interval = new Interval<int>(IntervalBoundary.Inclusive, 10, default, IntervalBoundary.Infinite);
            Assert.IsFalse(interval.IsBounded);
            Assert.IsTrue(interval.IsHalfBounded);
            Assert.IsTrue(interval.IsClosed);
        }

        [TestMethod]
        public void Interval_Unbounded()
        {
            var interval = new Interval<int>(IntervalBoundary.Infinite, default, default, IntervalBoundary.Infinite);
            Assert.IsFalse(interval.IsBounded);
            Assert.IsFalse(interval.IsHalfBounded);
            Assert.IsTrue(interval.IsClosed);
        }

        [TestMethod]
        public void Interval_HalfOpenBounded()
        {
            var interval = new Interval<int>(0, 10);
            Assert.IsTrue(interval.IsHalfOpen);
            Assert.IsTrue(interval.IsBounded);
            Assert.IsFalse(interval.IsHalfBounded);
            Assert.IsFalse(interval.IsOpen);
            Assert.IsFalse(interval.IsClosed);
        }
    }
}

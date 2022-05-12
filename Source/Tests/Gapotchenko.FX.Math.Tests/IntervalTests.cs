using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Math.Tests
{
    [TestClass]
    public class IntervalTests
    {
        [TestMethod]
        public void Interval_LeftUnboundedIsClosed()
        {
            // If one of the endpoints is ±∞, then the interval still contains all of its limit points, so [a,∞) and (-∞,b] are also closed intervals.
            var interval = new Interval<int>(default, 10, IntervalBoundary.Infinite, IntervalBoundary.Inclusive);
            Assert.IsTrue(interval.IsClosed);
        }

        [TestMethod]
        public void Interval_RightUnboundedIsClosed()
        {
            // If one of the endpoints is ±∞, then the interval still contains all of its limit points, so [a,∞) and (-∞,b] are also closed intervals.
            var interval = new Interval<int>(10, default, IntervalBoundary.Inclusive, IntervalBoundary.Infinite);
            Assert.IsTrue(interval.IsClosed);
        }

        [TestMethod]
        public void Interval_UnboundedIsClosed()
        {
            // If one of the endpoints is ±∞, then the interval still contains all of its limit points, so [a,∞) and (-∞,b] are also closed intervals.
            var interval = new Interval<int>(default, default, IntervalBoundary.Infinite, IntervalBoundary.Infinite);
            Assert.IsTrue(interval.IsClosed);
        }
    }
}

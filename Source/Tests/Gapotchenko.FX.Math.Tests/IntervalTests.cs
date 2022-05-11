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
            var interval = new Interval<int>(default, 10, false, true, false, true);
            Assert.IsTrue(interval.IsClosed);
        }

        [TestMethod]
        public void Interval_RightUnboundedIsClosed()
        {
            // If one of the endpoints is ±∞, then the interval still contains all of its limit points, so [a,∞) and (-∞,b] are also closed intervals.
            var interval = new Interval<int>(10, default, true, false, true, false);
            Assert.IsTrue(interval.IsClosed);
        }
    }
}

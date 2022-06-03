using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Tests
{
    [TestClass]
    public sealed class IntervalTests : IntervalTestsBase
    {
        public override IInterval<T> NewInterval<T>(T from, T to) => new Interval<T>(from, to);

        public override IInterval<T> NewInterval<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) => new Interval<T>(from, to);
    }
}

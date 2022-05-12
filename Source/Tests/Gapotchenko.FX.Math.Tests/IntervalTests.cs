using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Tests
{
    [TestClass]
    public class IntervalTests : IntervalTestsBase
    {
        public override IInterval<T> NewInterval<T>(T from, T to, IComparer<T>? comparer = null) =>
            new Interval<T>(from, to, comparer);

        public override IInterval<T> NewInterval<T>(IntervalBoundary fromBoundary, T from, T to, IntervalBoundary toBoundary, IComparer<T>? comparer = null) =>
            new Interval<T>(fromBoundary, from, to, toBoundary, comparer);
    }
}

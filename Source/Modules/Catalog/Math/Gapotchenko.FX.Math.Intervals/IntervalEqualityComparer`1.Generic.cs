// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

partial class IntervalEqualityComparer<T>
{
    internal sealed class GenericComparer(IEqualityComparer<T>? comparer) : IntervalEqualityComparer<T>
    {
        public override bool Equals(IInterval<T>? x, IInterval<T>? y) =>
            ReferenceEquals(x, y) ||
            x is not null && y is not null &&
            x.From.Equals(y.From, comparer) &&
            x.To.Equals(y.To, comparer);

        public override int GetHashCode(IInterval<T> obj) =>
            HashCode.Combine(
                obj.From.GetHashCode(comparer),
                obj.To.GetHashCode(comparer));
    }
}

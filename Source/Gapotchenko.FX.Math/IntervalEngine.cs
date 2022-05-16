using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.Math
{
    using Math = System.Math;

    static class IntervalEngine
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBounded<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound> =>
            !interval.From.IsInfinity &&
            !interval.To.IsInfinity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfBounded<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound> =>
            interval.From.IsInfinity ^
            interval.To.IsInfinity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOpen<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound> =>
            interval.From.Kind == IntervalBoundaryKind.Exclusive &&
            interval.To.Kind == IntervalBoundaryKind.Exclusive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClosed<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound> =>
            interval.From.Kind != IntervalBoundaryKind.Exclusive &&
            interval.To.Kind != IntervalBoundaryKind.Exclusive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfOpen<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound> =>
            interval.From.Kind == IntervalBoundaryKind.Exclusive ^
            interval.To.Kind == IntervalBoundaryKind.Exclusive;

        public delegate TInterval Constructor<out TInterval, TBound>(
            IntervalBoundary<TBound> from,
            IntervalBoundary<TBound> to)
            where TInterval : IInterval<TBound>;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TInterval WithInclusiveBounds<TInterval, TBound>(
            TInterval interval,
            bool inclusive,
            Constructor<TInterval, TBound> constructor)
            where TInterval : IInterval<TBound>
        {
            static IntervalBoundary<TBound> WithInclusiveBoundary(IntervalBoundary<TBound> boundary, bool inclusive)
            {
                if (boundary.IsInfinity)
                    return boundary;
                else if (inclusive)
                    return IntervalBoundary.Inclusive(boundary.Value);
                else
                    return IntervalBoundary.Exclusive(boundary.Value);
            }

            return constructor(
                WithInclusiveBoundary(interval.From, inclusive),
                WithInclusiveBoundary(interval.To, inclusive));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInterval Interior<TInterval, TBound>(TInterval interval, Constructor<TInterval, TBound> constructor) where TInterval : IInterval<TBound> =>
            IsOpen<TInterval, TBound>(interval) ?
                interval :
                WithInclusiveBounds(interval, false, constructor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInterval Enclosure<TInterval, TBound>(TInterval interval, Constructor<TInterval, TBound> constructor) where TInterval : IInterval<TBound> =>
            IsClosed<TInterval, TBound>(interval) ?
                interval :
                WithInclusiveBounds(interval, true, constructor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<TInterval, TBound>(TInterval interval, IComparer<TBound> comparer) where TInterval : IInterval<TBound> =>
            !IsClosed<TInterval, TBound>(interval) &&
            comparer.Compare(interval.From.GetValueOrDefault(), interval.To.GetValueOrDefault()) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDegenerate<TInterval, TBound>(TInterval interval, IComparer<TBound> comparer) where TInterval : IInterval<TBound> =>
            interval.From.Kind == IntervalBoundaryKind.Inclusive &&
            interval.To.Kind == IntervalBoundaryKind.Inclusive &&
            comparer.Compare(interval.From.Value, interval.To.Value) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TInterval, TBound>(TInterval interval, TBound item, IComparer<TBound> comparer) where TInterval : IInterval<TBound> =>
            CompareBoundaries(interval.From, item, false, comparer) <= 0 &&
            CompareBoundaries(interval.To, item, true, comparer) <= 0;

        static int CompareBoundaries<TBound>(IntervalBoundary<TBound> x, TBound y, bool direction, IComparer<TBound> comparer) =>
            x.Kind switch
            {
                IntervalBoundaryKind.Empty or IntervalBoundaryKind.NegativeInfinity => direction ? 1 : -1,
                IntervalBoundaryKind.Inclusive => direction ? comparer.Compare(y, x.Value) : comparer.Compare(x.Value, y),
                IntervalBoundaryKind.Exclusive => (direction ? comparer.Compare(y, x.Value) : comparer.Compare(x.Value, y)) > -1 ? 1 : -1,
                IntervalBoundaryKind.PositiveInfinity => direction ? -1 : 1
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Overlaps<TInterval, TOther, TBound>(TInterval interval, TOther other, IComparer<TBound> comparer)
            where TInterval : IInterval<TBound>
            where TOther : IInterval<TBound> =>
            CompareBoundaries(interval.From, other.To, false, comparer) <= 0 &&
            CompareBoundaries(interval.To, other.From, true, comparer) >= 0;

        static int CompareBoundaries<TBound>(IntervalBoundary<TBound> x, IntervalBoundary<TBound> y, bool direction, IComparer<TBound> comparer)
        {
            if (x.HasValue && y.HasValue)
            {
                int c = comparer.Compare(x.Value, y.Value);
                if (c == 0)
                {
                    var orderedKindX = GetOrderedBoundaryKind(x.Kind, direction);
                    var orderedKindY = GetOrderedBoundaryKind(y.Kind, !direction);
                    c = orderedKindX.CompareTo(orderedKindY);
                }
                return c;
            }
            else
            {
                return x.Kind.CompareTo(y.Kind);
            }
        }

        enum OrderedBoundaryKind
        {
            Empty,
            NegativeInfinity,
            ToExclusive,
            FromInclusive,
            ToInclusive,
            FromExclusive,
            PositiveInfinity
        }

        static OrderedBoundaryKind GetOrderedBoundaryKind(IntervalBoundaryKind kind, bool direction) =>
            (kind, direction) switch
            {
                (IntervalBoundaryKind.Empty, _) => OrderedBoundaryKind.Empty,
                (IntervalBoundaryKind.NegativeInfinity, _) => OrderedBoundaryKind.NegativeInfinity,
                (IntervalBoundaryKind.Inclusive, false) => OrderedBoundaryKind.FromInclusive,
                (IntervalBoundaryKind.Inclusive, true) => OrderedBoundaryKind.ToInclusive,
                (IntervalBoundaryKind.Exclusive, false) => OrderedBoundaryKind.FromExclusive,
                (IntervalBoundaryKind.Exclusive, true) => OrderedBoundaryKind.ToExclusive,
                (IntervalBoundaryKind.PositiveInfinity, _) => OrderedBoundaryKind.PositiveInfinity
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInterval Intersect<TInterval, TOther, TBound>(
            TInterval interval,
            TOther other,
            IComparer<TBound> comparer,
            Constructor<TInterval, TBound> constructor)
            where TInterval : IInterval<TBound>
            where TOther : IInterval<TBound>
        {
            throw new NotImplementedException();
        }

        public static string ToString<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound>
        {
            var sb = new StringBuilder();

            if (interval.From.Kind == IntervalBoundaryKind.Inclusive)
                sb.Append('[');
            else
                sb.Append('(');

            static void AppendBoundary(StringBuilder sb, IntervalBoundary<TBound> boundary)
            {
                switch (boundary.Kind)
                {
                    case IntervalBoundaryKind.Empty:
                        sb.Append("{}"); // ∅
                        break;
                    case IntervalBoundaryKind.NegativeInfinity:
                        sb.Append("-inf"); // -∞
                        break;
                    case IntervalBoundaryKind.PositiveInfinity:
                        sb.Append("+inf"); // +∞
                        break;
                    default:
                        sb.Append(boundary.Value);
                        break;
                }
            }

            AppendBoundary(sb, interval.From);

            sb.Append(',');

            AppendBoundary(sb, interval.To);

            if (interval.To.Kind == IntervalBoundaryKind.Inclusive)
                sb.Append(']');
            else
                sb.Append(')');

            return sb.ToString();
        }
    }
}

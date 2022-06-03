using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.Math
{
    static class IntervalEngine
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBounded<TInterval, TBound>(TInterval interval) where TInterval : IIntervalOperations<TBound> =>
            !interval.From.IsInfinity &&
            !interval.To.IsInfinity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfBounded<TInterval, TBound>(TInterval interval) where TInterval : IIntervalOperations<TBound> =>
            interval.From.IsInfinity ^
            interval.To.IsInfinity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOpen<TInterval, TBound>(TInterval interval) where TInterval : IIntervalOperations<TBound> =>
            interval.From.Kind == IntervalBoundaryKind.Exclusive &&
            interval.To.Kind == IntervalBoundaryKind.Exclusive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClosed<TInterval, TBound>(TInterval interval) where TInterval : IIntervalOperations<TBound> =>
            interval.From.Kind != IntervalBoundaryKind.Exclusive &&
            interval.To.Kind != IntervalBoundaryKind.Exclusive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfOpen<TInterval, TBound>(TInterval interval) where TInterval : IIntervalOperations<TBound> =>
            interval.From.Kind == IntervalBoundaryKind.Exclusive ^
            interval.To.Kind == IntervalBoundaryKind.Exclusive;

        public delegate TInterval Constructor<out TInterval, TBound>(
            IntervalBoundary<TBound> from,
            IntervalBoundary<TBound> to)
            where TInterval : IIntervalOperations<TBound>;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TInterval WithInclusiveBounds<TInterval, TBound>(
            TInterval interval,
            bool inclusive,
            Constructor<TInterval, TBound> constructor)
            where TInterval : IIntervalOperations<TBound>
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
        public static TInterval Interior<TInterval, TBound>(TInterval interval, Constructor<TInterval, TBound> constructor)
            where TInterval : IIntervalOperations<TBound> =>
            IsOpen<TInterval, TBound>(interval) ?
                interval :
                WithInclusiveBounds(interval, false, constructor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInterval Enclosure<TInterval, TBound>(TInterval interval, Constructor<TInterval, TBound> constructor)
            where TInterval : IIntervalOperations<TBound> =>
            IsClosed<TInterval, TBound>(interval) ?
                interval :
                WithInclusiveBounds(interval, true, constructor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<TInterval, TBound>(TInterval interval, IComparer<TBound> comparer)
            where TInterval : IIntervalOperations<TBound> =>
            !IsClosed<TInterval, TBound>(interval) &&
            comparer.Compare(interval.From.GetValueOrDefault(), interval.To.GetValueOrDefault()) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDegenerate<TInterval, TBound>(TInterval interval, IComparer<TBound> comparer)
            where TInterval : IIntervalOperations<TBound> =>
            interval.From.Kind == IntervalBoundaryKind.Inclusive &&
            interval.To.Kind == IntervalBoundaryKind.Inclusive &&
            comparer.Compare(interval.From.Value, interval.To.Value) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TInterval, TBound>(TInterval interval, TBound item, IComparer<TBound> comparer)
            where TInterval : IIntervalOperations<TBound> =>
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
            where TInterval : IIntervalOperations<TBound>
            where TOther : IIntervalOperations<TBound> =>
            CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.To, other.To, comparer) <= 0 &&
            CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.From, other.From, comparer) >= 0;

        public static bool BoundariesEqual<TBound>(IntervalBoundary<TBound> x, IntervalBoundary<TBound> y, IComparer<TBound> comparer) =>
            x.Kind == y.Kind &&
            comparer.Compare(x.GetValueOrDefault(), y.GetValueOrDefault()) == 0;

        public static bool IntervalsEqual<TInterval, TOther, TBound>(TInterval x, TOther y, IComparer<TBound> comparer)
            where TInterval : IIntervalOperations<TBound>
            where TOther : IIntervalOperations<TBound> =>
            BoundariesEqual(x.From, y.From, comparer) &&
            BoundariesEqual(x.To, y.To, comparer);

        static int CompareBoundaries<TBound>(
            BoundaryDirection directionX,
            IntervalBoundary<TBound> boundaryX,
            BoundaryDirection directionY,
            IntervalBoundary<TBound> boundaryY,
            IComparer<TBound> comparer)
        {
            if (boundaryX.HasValue && boundaryY.HasValue)
            {
                int c = comparer.Compare(boundaryX.Value, boundaryY.Value);
                if (c == 0)
                {
                    var orderedKindX = GetOrderedBoundaryKind(boundaryX.Kind, directionX);
                    var orderedKindY = GetOrderedBoundaryKind(boundaryY.Kind, directionY);
                    c = orderedKindX.CompareTo(orderedKindY);
                }
                return c;
            }
            else
            {
                return boundaryX.Kind.CompareTo(boundaryY.Kind);
            }
        }

        enum BoundaryDirection
        {
            From,
            To
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

        static OrderedBoundaryKind GetOrderedBoundaryKind(IntervalBoundaryKind kind, BoundaryDirection direction) =>
            (kind, direction) switch
            {
                (IntervalBoundaryKind.Empty, _) => OrderedBoundaryKind.Empty,
                (IntervalBoundaryKind.NegativeInfinity, _) => OrderedBoundaryKind.NegativeInfinity,
                (IntervalBoundaryKind.Inclusive, BoundaryDirection.From) => OrderedBoundaryKind.FromInclusive,
                (IntervalBoundaryKind.Inclusive, BoundaryDirection.To) => OrderedBoundaryKind.ToInclusive,
                (IntervalBoundaryKind.Exclusive, BoundaryDirection.From) => OrderedBoundaryKind.FromExclusive,
                (IntervalBoundaryKind.Exclusive, BoundaryDirection.To) => OrderedBoundaryKind.ToExclusive,
                (IntervalBoundaryKind.PositiveInfinity, _) => OrderedBoundaryKind.PositiveInfinity
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInterval Intersect<TInterval, TOther, TBound>(
            TInterval interval,
            TOther other,
            IComparer<TBound> comparer,
            Constructor<TInterval, TBound> constructor)
            where TInterval : IIntervalOperations<TBound>
            where TOther : IIntervalOperations<TBound> =>
            constructor(
                CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) >= 0 ? interval.From : other.From,
                CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) <= 0 ? interval.To : other.To);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSubintervalOf<TInterval, TOther, TBound>(TInterval interval, TOther other, IComparer<TBound> comparer)
            where TInterval : IIntervalOperations<TBound>
            where TOther : IIntervalOperations<TBound> =>
            CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) >= 0 &&
            CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSuperintervalOf<TInterval, TOther, TBound>(TInterval interval, TOther other, IComparer<TBound> comparer)
            where TInterval : IIntervalOperations<TBound>
            where TOther : IIntervalOperations<TBound> =>
            CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) <= 0 &&
            CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsProperSubintervalOf<TInterval, TOther, TBound>(TInterval interval, TOther other, IComparer<TBound> comparer)
            where TInterval : IIntervalOperations<TBound>
            where TOther : IIntervalOperations<TBound>
        {
            int cFrom = CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer);
            if (cFrom < 0)
                return false;

            int cTo = CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer);
            if (cTo > 0)
                return false;

            return cFrom != 0 || cTo != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsProperSuperintervalOf<TInterval, TOther, TBound>(TInterval interval, TOther other, IComparer<TBound> comparer)
            where TInterval : IIntervalOperations<TBound>
            where TOther : IIntervalOperations<TBound>
        {
            int cFrom = CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer);
            if (cFrom > 0)
                return false;

            int cTo = CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer);
            if (cTo < 0)
                return false;

            return cFrom != 0 || cTo != 0;
        }

        public static string ToString<TInterval, TBound>(TInterval interval)
            where TInterval : IIntervalOperations<TBound>
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

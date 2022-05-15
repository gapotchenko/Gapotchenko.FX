using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.Math
{
    static class IntervalHelpers
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
            IsClosed<TInterval, TBound>(interval) &&
            comparer.Compare(interval.From.GetValueOrDefault(), interval.To.GetValueOrDefault()) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TInterval, TBound>(TInterval interval, TBound item, IComparer<TBound> comparer) where TInterval : IInterval<TBound> =>
            CompareBoundaries(interval.From, item, false, comparer) <= 0 &&
            CompareBoundaries(interval.To, item, true, comparer) <= 0;

        static int CompareBoundaries<TBound>(IntervalBoundary<TBound> x, TBound y, bool reverse, IComparer<TBound> comparer) =>
            x.Kind switch
            {
                IntervalBoundaryKind.Empty or IntervalBoundaryKind.NegativeInfinity => reverse ? 1 : -1,
                IntervalBoundaryKind.Inclusive => reverse ? comparer.Compare(y, x.Value) : comparer.Compare(x.Value, y),
                IntervalBoundaryKind.Exclusive => (reverse ? comparer.Compare(y, x.Value) : comparer.Compare(x.Value, y)) > -1 ? 1 : -1,
                IntervalBoundaryKind.PositiveInfinity => reverse ? -1 : 1
            };

        static int CompareBoundaries<TBound>(IntervalBoundary<TBound> x, IntervalBoundary<TBound> y, IComparer<TBound> comparer)
        {
            if (x.HasValue && y.HasValue)
            {
                int c = comparer.Compare(x.Value, y.Value);
                if (c == 0)
                    c = x.Kind.CompareTo(y.Kind);
                return c;
            }
            else
            {
                return x.Kind.CompareTo(y.Kind);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInterval Clamp<TInterval, TLimits, TBound>(
            TInterval interval,
            TLimits limits,
            IComparer<TBound> comparer,
            Constructor<TInterval, TBound> constructor)
            where TInterval : IInterval<TBound>
            where TLimits : IInterval<TBound>
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Overlaps<TInterval, TOther, TBound>(TInterval interval, TOther other)
            where TInterval : IInterval<TBound>
            where TOther : IInterval<TBound> =>
            interval.Contains(other.From.GetValueOrDefault()) || interval.Contains(other.To.GetValueOrDefault()) ||
            other.Contains(interval.From.GetValueOrDefault()) || other.Contains(interval.To.GetValueOrDefault());

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
                    case IntervalBoundaryKind.NegativeInfinity:
                        sb.Append("-inf");
                        break;
                    case IntervalBoundaryKind.PositiveInfinity:
                        sb.Append("inf");
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

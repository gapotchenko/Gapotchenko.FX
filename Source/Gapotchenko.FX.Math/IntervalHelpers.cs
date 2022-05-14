﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.Math
{
    using Math = System.Math;

    static class IntervalHelpers
    {
        public static IntervalFlags SetFlag(IntervalFlags flags, IntervalFlags mask, bool value) => value ? flags | mask : flags & ~mask;

        public static IntervalBoundaryKind GetBoundary(IntervalFlags flags, IntervalFlags boundedFlag, IntervalFlags closedFlag)
        {
            if ((flags & boundedFlag) == 0)
            {
                if ((flags & closedFlag) != 0)
                    return IntervalBoundaryKind.NegativeInfinity;
                else
                    return IntervalBoundaryKind.PositiveInfinity;
            }
            else if ((flags & closedFlag) != 0)
                return IntervalBoundaryKind.Inclusive;
            else
                return IntervalBoundaryKind.Exclusive;
        }

        public static IntervalFlags SetBoundary(IntervalFlags flags, IntervalFlags boundedFlag, IntervalFlags closedFlag, IntervalBoundaryKind boundary) =>
            boundary switch
            {
                IntervalBoundaryKind.Inclusive => flags | boundedFlag | closedFlag,
                IntervalBoundaryKind.Exclusive => (flags & ~closedFlag) | boundedFlag,
                IntervalBoundaryKind.PositiveInfinity => flags & ~(boundedFlag | closedFlag),
                IntervalBoundaryKind.NegativeInfinity => (flags & ~boundedFlag) | closedFlag,
                _ => throw new SwitchExpressionException(boundary)
            };

        static bool IsInfinity(IntervalBoundaryKind boundary) =>
            boundary is IntervalBoundaryKind.PositiveInfinity or IntervalBoundaryKind.NegativeInfinity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBounded<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound> =>
            !IsInfinity(interval.FromBoundary) &&
            !IsInfinity(interval.ToBoundary);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfBounded<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound> =>
            IsInfinity(interval.FromBoundary) ^
            IsInfinity(interval.ToBoundary);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOpen<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound> =>
            interval.FromBoundary == IntervalBoundaryKind.Exclusive &&
            interval.ToBoundary == IntervalBoundaryKind.Exclusive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClosed<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound> =>
            interval.FromBoundary != IntervalBoundaryKind.Exclusive &&
            interval.ToBoundary != IntervalBoundaryKind.Exclusive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfOpen<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound> =>
            interval.FromBoundary == IntervalBoundaryKind.Exclusive ^
            interval.ToBoundary == IntervalBoundaryKind.Exclusive;

        public delegate TInterval Constructor<out TInterval, in TBound>(
            IntervalBoundaryKind fromBoundary,
            TBound from,
            TBound to,
            IntervalBoundaryKind toBoundary)
            where TInterval : IInterval<TBound>;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TInterval WithInclusiveBounds<TInterval, TBound>(
            TInterval interval,
            bool inclusive,
            Constructor<TInterval, TBound> constructor)
            where TInterval : IInterval<TBound>
        {
            static IntervalBoundaryKind WithInclusiveBoundary(IntervalBoundaryKind boundary, bool inclusive)
            {
                if (IsInfinity(boundary))
                    return boundary;
                else if (inclusive)
                    return IntervalBoundaryKind.Inclusive;
                else
                    return IntervalBoundaryKind.Exclusive;
            }

            return constructor(
                WithInclusiveBoundary(interval.FromBoundary, inclusive),
                interval.From,
                interval.To,
                WithInclusiveBoundary(interval.ToBoundary, inclusive));
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
            comparer.Compare(interval.From, interval.To) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDegenerate<TInterval, TBound>(TInterval interval, IComparer<TBound> comparer) where TInterval : IInterval<TBound> =>
            IsClosed<TInterval, TBound>(interval) &&
            comparer.Compare(interval.From, interval.To) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TInterval, TBound>(TInterval interval, TBound item, IComparer<TBound> comparer) where TInterval : IInterval<TBound>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool BoundLimit(int cmp, IntervalBoundaryKind boundary)
            {
                int limit = boundary == IntervalBoundaryKind.Inclusive ? 0 : -1;
                return cmp > limit;
            }

            var fromBoundary = interval.FromBoundary;
            if (!IsInfinity(fromBoundary))
            {
                if (BoundLimit(comparer.Compare(interval.From, item), fromBoundary))
                    return false;
            }

            var toBoundary = interval.ToBoundary;
            if (!IsInfinity(toBoundary))
            {
                if (BoundLimit(comparer.Compare(item, interval.To), toBoundary))
                    return false;
            }

            return true;
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
            interval.Contains(other.From) || interval.Contains(other.To) ||
            other.Contains(interval.From) || other.Contains(interval.To);

        public static string ToString<TInterval, TBound>(TInterval interval) where TInterval : IInterval<TBound>
        {
            var sb = new StringBuilder();

            var fromBoundary = interval.FromBoundary;
            if (fromBoundary == IntervalBoundaryKind.Inclusive)
                sb.Append('[');
            else
                sb.Append('(');

            static void AppendBoundary(StringBuilder sb, IntervalBoundaryKind boundary, TBound bound)
            {
                switch (boundary)
                {
                    case IntervalBoundaryKind.NegativeInfinity:
                        sb.Append("-inf");
                        break;
                    case IntervalBoundaryKind.PositiveInfinity:
                        sb.Append("inf");
                        break;
                    default:
                        sb.Append(bound);
                        break;
                }
            }

            AppendBoundary(sb, fromBoundary, interval.From);

            sb.Append(',');

            var toBoundary = interval.ToBoundary;
            AppendBoundary(sb, toBoundary, interval.To);

            if (toBoundary == IntervalBoundaryKind.Inclusive)
                sb.Append(']');
            else
                sb.Append(')');

            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.Math
{
    static class IntervalHelpers
    {
        public static IntervalFlags SetFlag(IntervalFlags flags, IntervalFlags mask, bool value) => value ? flags | mask : flags & ~mask;

        public static IntervalBoundary GetBoundary(IntervalFlags flags, IntervalFlags closedFlag, IntervalFlags boundedFlag)
        {
            if ((flags & boundedFlag) == 0)
                return IntervalBoundary.Infinite;
            else if ((flags & closedFlag) != 0)
                return IntervalBoundary.Inclusive;
            else
                return IntervalBoundary.Exclusive;
        }

        public static IntervalFlags SetBoundary(IntervalFlags flags, IntervalFlags closedFlag, IntervalFlags boundedFlag, IntervalBoundary boundary) =>
            boundary switch
            {
                IntervalBoundary.Inclusive => flags | boundedFlag | closedFlag,
                IntervalBoundary.Exclusive => (flags & ~closedFlag) | boundedFlag,
                IntervalBoundary.Infinite => (flags & ~boundedFlag) | closedFlag,
                _ => throw new SwitchExpressionException(boundary)
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBounded<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.FromBoundary != IntervalBoundary.Infinite &&
            interval.ToBoundary != IntervalBoundary.Infinite;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfBounded<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.FromBoundary == IntervalBoundary.Infinite ^
            interval.ToBoundary == IntervalBoundary.Infinite;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOpen<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.FromBoundary == IntervalBoundary.Exclusive &&
            interval.ToBoundary == IntervalBoundary.Exclusive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClosed<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.FromBoundary != IntervalBoundary.Exclusive &&
            interval.ToBoundary != IntervalBoundary.Exclusive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfOpen<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.FromBoundary == IntervalBoundary.Exclusive ^
            interval.ToBoundary == IntervalBoundary.Exclusive;

        public delegate TInterval Constructor<out TInterval, in TValue>(
            TValue lowerBound, TValue upperBound,
            IntervalBoundary fromBoundary, IntervalBoundary toBoundary)
            where TInterval : IInterval<TValue>;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TInterval WithInclusiveBounds<TInterval, TValue>(
            TInterval interval,
            bool inclusive,
            Constructor<TInterval, TValue> constructor)
            where TInterval : IInterval<TValue>
        {
            static IntervalBoundary WithInclusiveBoundary(IntervalBoundary boundary, bool inclusive)
            {
                if (boundary == IntervalBoundary.Infinite)
                    return IntervalBoundary.Infinite;
                else if (inclusive)
                    return IntervalBoundary.Inclusive;
                else
                    return IntervalBoundary.Exclusive;
            }

            return constructor(
                interval.From, interval.To,
                WithInclusiveBoundary(interval.FromBoundary, inclusive), WithInclusiveBoundary(interval.ToBoundary, inclusive));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInterval Interior<TInterval, TValue>(TInterval interval, Constructor<TInterval, TValue> constructor) where TInterval : IInterval<TValue> =>
            IsOpen<TInterval, TValue>(interval) ?
                interval :
                WithInclusiveBounds(interval, false, constructor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInterval Enclosure<TInterval, TValue>(TInterval interval, Constructor<TInterval, TValue> constructor) where TInterval : IInterval<TValue> =>
            IsClosed<TInterval, TValue>(interval) ?
                interval :
                WithInclusiveBounds(interval, true, constructor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<TInterval, TValue>(TInterval interval, IComparer<TValue> comparer) where TInterval : IInterval<TValue> =>
            !IsClosed<TInterval, TValue>(interval) &&
            comparer.Compare(interval.From, interval.To) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDegenerate<TInterval, TValue>(TInterval interval, IComparer<TValue> comparer) where TInterval : IInterval<TValue> =>
            IsClosed<TInterval, TValue>(interval) &&
            comparer.Compare(interval.From, interval.To) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int BoundLimit(bool inclusive) => inclusive ? 0 : -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TInterval, TValue>(TInterval interval, TValue item, IComparer<TValue> comparer) where TInterval : IInterval<TValue>
        {
            var fromBoundary = interval.FromBoundary;
            if (fromBoundary != IntervalBoundary.Infinite)
            {
                if (comparer.Compare(interval.From, item) > BoundLimit(fromBoundary == IntervalBoundary.Inclusive))
                    return false;
            }

            var toBoundary = interval.ToBoundary;
            if (toBoundary != IntervalBoundary.Infinite)
            {
                if (comparer.Compare(item, interval.To) > BoundLimit(toBoundary == IntervalBoundary.Inclusive))
                    return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInterval Clamp<TInterval, TLimits, TValue>(
            TInterval interval,
            TLimits limits,
            IComparer<TValue> comparer,
            Constructor<TInterval, TValue> constructor)
            where TInterval : IInterval<TValue>
            where TLimits : IInterval<TValue>
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Overlaps<TInterval, TOther, TValue>(TInterval interval, TOther other, IComparer<TValue> comparer)
            where TInterval : IInterval<TValue>
            where TOther : IInterval<TValue>
        {
            throw new NotImplementedException();
        }

        public static string ToString<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue>
        {
            var sb = new StringBuilder();

            var fromBoundary = interval.FromBoundary;
            if (fromBoundary == IntervalBoundary.Inclusive)
                sb.Append('[');
            else
                sb.Append('(');

            if (fromBoundary == IntervalBoundary.Infinite)
                sb.Append("-inf");
            else
                sb.Append(interval.From);

            sb.Append(',');

            var toBoundary = interval.ToBoundary;
            if (toBoundary == IntervalBoundary.Infinite)
                sb.Append("inf");
            else
                sb.Append(interval.To);

            if (toBoundary == IntervalBoundary.Inclusive)
                sb.Append(']');
            else
                sb.Append(')');

            return sb.ToString();
        }
    }
}

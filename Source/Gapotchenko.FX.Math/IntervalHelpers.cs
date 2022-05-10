using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.Math
{
    static class IntervalHelpers
    {
        public static IntervalFlags SetFlag(IntervalFlags flags, IntervalFlags mask, bool value) => value ? flags | mask : flags & ~mask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBounded<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.IsLeftBounded && interval.IsRightBounded;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfBounded<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.IsLeftBounded ^ interval.IsRightBounded;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOpen<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            !(interval.IsLeftClosed || interval.IsRightClosed);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClosed<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.IsLeftClosed && interval.IsRightClosed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfOpen<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.IsLeftClosed ^ interval.IsRightClosed;

        public delegate TInterval Constructor<out TInterval, in TValue>(
            TValue lowerBound, TValue upperBound,
            bool inclusiveLowerBound, bool inclusiveUpperBound,
            bool hasLowerBound, bool hasUpperBound)
            where TInterval : IInterval<TValue>;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TInterval WithInclusiveBounds<TInterval, TValue>(
            TInterval interval,
            bool inclusive,
            Constructor<TInterval, TValue> constructor)
            where TInterval : IInterval<TValue> =>
            constructor(
                interval.LeftBound, interval.RightBound,
                inclusive, inclusive,
                interval.IsLeftBounded, interval.IsRightBounded);

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
            comparer.Compare(interval.LeftBound, interval.RightBound) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDegenerate<TInterval, TValue>(TInterval interval, IComparer<TValue> comparer) where TInterval : IInterval<TValue> =>
            IsClosed<TInterval, TValue>(interval) &&
            comparer.Compare(interval.LeftBound, interval.RightBound) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int BoundLimit(bool inclusive) => inclusive ? 0 : -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TInterval, TValue>(TInterval interval, TValue item, IComparer<TValue> comparer) where TInterval : IInterval<TValue>
        {
            if (interval.IsLeftBounded)
            {
                if (comparer.Compare(interval.LeftBound, item) > BoundLimit(interval.IsLeftClosed))
                    return false;
            }

            if (interval.IsRightBounded)
            {
                if (comparer.Compare(item, interval.RightBound) > BoundLimit(interval.IsRightClosed))
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
            if (interval.IsLeftClosed)
                sb.Append('[');
            else
                sb.Append('(');

            if (interval.IsLeftBounded)
                sb.Append(interval.LeftBound);
            else
                sb.Append("-inf");

            sb.Append(',');

            if (interval.IsRightBounded)
                sb.Append(interval.RightBound);
            else
                sb.Append("inf");

            if (interval.IsRightClosed)
                sb.Append(']');
            else
                sb.Append(')');

            return sb.ToString();
        }
    }
}

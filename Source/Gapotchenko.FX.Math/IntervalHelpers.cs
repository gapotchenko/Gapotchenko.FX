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
            interval.IsBoundedFrom && interval.IsBoundedTo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfBounded<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.IsBoundedFrom ^ interval.IsBoundedTo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOpen<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            !(interval.IncludesFrom || interval.IncludesTo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClosed<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.IncludesFrom && interval.IncludesTo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHalfOpen<TInterval, TValue>(TInterval interval) where TInterval : IInterval<TValue> =>
            interval.IncludesFrom ^ interval.IncludesTo;

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
                interval.From, interval.To,
                inclusive, inclusive,
                interval.IsBoundedFrom, interval.IsBoundedTo);

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
        static bool IsDegenerate<TInterval, TValue>(TInterval interval, IComparer<TValue> comparer) where TInterval : IInterval<TValue> =>
            comparer.Compare(interval.From, interval.To) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<TInterval, TValue>(TInterval interval, IComparer<TValue> comparer) where TInterval : IInterval<TValue> =>
            !IsClosed<TInterval, TValue>(interval) && IsDegenerate(interval, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSingleton<TInterval, TValue>(TInterval interval, IComparer<TValue> comparer) where TInterval : IInterval<TValue> =>
            IsClosed<TInterval, TValue>(interval) && IsDegenerate(interval, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int BoundLimit(bool inclusive) => inclusive ? 0 : -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TInterval, TValue>(TInterval interval, TValue item, IComparer<TValue> comparer) where TInterval : IInterval<TValue>
        {
            if (interval.IsBoundedFrom)
            {
                if (comparer.Compare(interval.From, item) > BoundLimit(interval.IncludesFrom))
                    return false;
            }

            if (interval.IsBoundedTo)
            {
                if (comparer.Compare(item, interval.To) > BoundLimit(interval.IncludesTo))
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
            if (interval.IncludesFrom)
                sb.Append('[');
            else
                sb.Append('(');

            if (interval.IsBoundedFrom)
                sb.Append(interval.From);
            else
                sb.Append("-inf");

            sb.Append(',');

            if (interval.IsBoundedTo)
                sb.Append(interval.To);
            else
                sb.Append("inf");

            if (interval.IncludesTo)
                sb.Append(']');
            else
                sb.Append(')');

            return sb.ToString();
        }
    }
}

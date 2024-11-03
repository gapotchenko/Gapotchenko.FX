// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.Math.Intervals;

static class IntervalEngine
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBounded<TInterval, TBound>(in TInterval interval) where TInterval : IIntervalOperations<TBound> =>
        !interval.From.IsInfinity &&
        !interval.To.IsInfinity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsHalfBounded<TInterval, TBound>(in TInterval interval) where TInterval : IIntervalOperations<TBound> =>
        interval.From.IsInfinity ^
        interval.To.IsInfinity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOpen<TInterval, TBound>(in TInterval interval) where TInterval : IIntervalOperations<TBound> =>
        interval.From.Kind == IntervalBoundaryKind.Exclusive &&
        interval.To.Kind == IntervalBoundaryKind.Exclusive;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsClosed<TInterval, TBound>(in TInterval interval) where TInterval : IIntervalOperations<TBound> =>
        interval.From.Kind != IntervalBoundaryKind.Exclusive &&
        interval.To.Kind != IntervalBoundaryKind.Exclusive;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsHalfOpen<TInterval, TBound>(in TInterval interval) where TInterval : IIntervalOperations<TBound> =>
        interval.From.Kind == IntervalBoundaryKind.Exclusive ^
        interval.To.Kind == IntervalBoundaryKind.Exclusive;

    public delegate TInterval Constructor<out TInterval, TBound>(
        IntervalBoundary<TBound> from,
        IntervalBoundary<TBound> to)
        where TInterval : IIntervalOperations<TBound>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TInterval Interior<TInterval, TBound>(in TInterval interval, Constructor<TInterval, TBound> constructor)
        where TInterval : IIntervalOperations<TBound> =>
        IsOpen<TInterval, TBound>(interval) ?
            interval :
            WithInclusivity(interval, false, constructor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TInterval Enclosure<TInterval, TBound>(in TInterval interval, Constructor<TInterval, TBound> constructor)
        where TInterval : IIntervalOperations<TBound> =>
        IsClosed<TInterval, TBound>(interval) ?
            interval :
            WithInclusivity(interval, true, constructor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TInterval WithInclusivity<TInterval, TBound>(
        in TInterval interval,
        bool inclusive,
        Constructor<TInterval, TBound> constructor)
        where TInterval : IIntervalOperations<TBound>
    {
        return constructor(
            WithBoundaryInclusivity(interval.From, inclusive),
            WithBoundaryInclusivity(interval.To, inclusive));

        static IntervalBoundary<TBound> WithBoundaryInclusivity(IntervalBoundary<TBound> boundary, bool inclusive)
        {
            if (!boundary.HasValue)
                return boundary;
            else if (inclusive)
                return IntervalBoundary.Inclusive(boundary.Value);
            else
                return IntervalBoundary.Exclusive(boundary.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty<TInterval, TBound>(in TInterval interval, IComparer<TBound> comparer)
        where TInterval : IIntervalOperations<TBound> =>
        CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.To, interval.To, comparer) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInfinite<TInterval, TBound>(in TInterval interval)
        where TInterval : IIntervalOperations<TBound> =>
        interval.From.Kind == IntervalBoundaryKind.NegativeInfinity &&
        interval.To.Kind == IntervalBoundaryKind.PositiveInfinity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDegenerate<TInterval, TBound>(in TInterval interval, IComparer<TBound> comparer)
        where TInterval : IIntervalOperations<TBound> =>
        interval.From.Kind == IntervalBoundaryKind.Inclusive &&
        interval.To.Kind == IntervalBoundaryKind.Inclusive &&
        comparer.Compare(interval.From.Value, interval.To.Value) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDegenerate<TInterval, TBound>(in TInterval interval, IEqualityComparer<TBound> comparer)
        where TInterval : IIntervalOperations<TBound> =>
        interval.From.Kind == IntervalBoundaryKind.Inclusive &&
        interval.To.Kind == IntervalBoundaryKind.Inclusive &&
        comparer.Equals(interval.From.Value, interval.To.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<TInterval, TBound>(in TInterval interval, TBound value, IComparer<TBound> comparer)
        where TInterval : IIntervalOperations<TBound> =>
        CompareBoundaries(BoundaryDirection.From, interval.From, value, comparer) <= 0 &&
        CompareBoundaries(BoundaryDirection.To, interval.To, value, comparer) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Zone<TInterval, TBound>(in TInterval interval, TBound value, IComparer<TBound> comparer)
        where TInterval : IIntervalOperations<TBound>
    {
        if (IsEmpty(interval, comparer))
            return 0; // convention, zone is undefined
        else if (CompareBoundaries(BoundaryDirection.From, interval.From, value, comparer) > 0)
            return -1; // before the left interval boundary
        else if (CompareBoundaries(BoundaryDirection.To, interval.To, value, comparer) < 0)
            return 1; // past the right interval boundary
        else
            return 0; // contained in the interval
    }

    static int CompareBoundaries<TBound>(
        BoundaryDirection direction,
        in IntervalBoundary<TBound> x, TBound y,
        IComparer<TBound> comparer) =>
        (direction, x.Kind) switch
        {
            (_, IntervalBoundaryKind.NegativeInfinity or IntervalBoundaryKind.Empty) => -1,
            (_, IntervalBoundaryKind.Inclusive) => comparer.Compare(x.Value, y),
            (BoundaryDirection.From, IntervalBoundaryKind.Exclusive) => comparer.Compare(x.Value, y) >= 0 ? 1 : -1,
            (BoundaryDirection.To, IntervalBoundaryKind.Exclusive) => comparer.Compare(x.Value, y) <= 0 ? -1 : 1,
            (_, IntervalBoundaryKind.PositiveInfinity) => 1
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Overlaps<TInterval, TOther, TBound>(in TInterval interval, in TOther other, IComparer<TBound> comparer)
        where TInterval : IIntervalOperations<TBound>
        where TOther : IIntervalOperations<TBound> =>
        CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.To, other.To, comparer) <= 0 &&
        CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.From, other.From, comparer) >= 0;

    public static bool IntervalsEqual<TInterval, TOther, TBound>(in TInterval x, in TOther y, IComparer<TBound> comparer)
        where TInterval : IIntervalOperations<TBound>
        where TOther : IIntervalOperations<TBound> =>
        IsEmpty(x, comparer) && IsEmpty(y, comparer) ||
        x.From.Equals(y.From, comparer) && x.To.Equals(y.To, comparer);

    static int CompareBoundaries<TBound>(
        BoundaryDirection directionX, in IntervalBoundary<TBound> boundaryX,
        BoundaryDirection directionY, in IntervalBoundary<TBound> boundaryY,
        IComparer<TBound> comparer)
    {
        int c =
            boundaryX.HasValue && boundaryY.HasValue ?
                comparer.Compare(boundaryX.Value, boundaryY.Value) :
                0;

        if (c == 0)
        {
            var orderedKindX = GetOrderedBoundaryKind(directionX, boundaryX.Kind);
            var orderedKindY = GetOrderedBoundaryKind(directionY, boundaryY.Kind);
            c = orderedKindX.CompareTo(orderedKindY);
        }

        return c;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TInterval Intersect<TInterval, TOther, TBound>(
        in TInterval interval,
        in TOther other,
        IComparer<TBound> comparer,
        Constructor<TInterval, TBound> constructor)
        where TInterval : IIntervalOperations<TBound>
        where TOther : IIntervalOperations<TBound> =>
        constructor(
            CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) >= 0 ? interval.From : other.From,
            CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) <= 0 ? interval.To : other.To);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TInterval Union<TInterval, TOther, TBound>(
        in TInterval interval,
        in TOther other,
        IComparer<TBound> comparer,
        Constructor<TInterval, TBound> constructor)
        where TInterval : IIntervalOperations<TBound>
        where TOther : IIntervalOperations<TBound> =>
        constructor(
            CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) <= 0 ? interval.From : other.From,
            CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) >= 0 ? interval.To : other.To);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSubintervalOf<TInterval, TOther, TBound>(in TInterval interval, in TOther other, IComparer<TBound> comparer)
        where TInterval : IIntervalOperations<TBound>
        where TOther : IIntervalOperations<TBound> =>
        CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) >= 0 &&
        CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) <= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSuperintervalOf<TInterval, TOther, TBound>(in TInterval interval, in TOther other, IComparer<TBound> comparer)
        where TInterval : IIntervalOperations<TBound>
        where TOther : IIntervalOperations<TBound> =>
        CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) <= 0 &&
        CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsProperSubintervalOf<TInterval, TOther, TBound>(in TInterval interval, in TOther other, IComparer<TBound> comparer)
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
    public static bool IsProperSuperintervalOf<TInterval, TOther, TBound>(in TInterval interval, in TOther other, IComparer<TBound> comparer)
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

    static OrderedBoundaryKind GetOrderedBoundaryKind(BoundaryDirection direction, IntervalBoundaryKind kind) =>
        (direction, kind) switch
        {
            (BoundaryDirection.To, IntervalBoundaryKind.Empty) => OrderedBoundaryKind.ToEmpty,
            (_, IntervalBoundaryKind.NegativeInfinity) => OrderedBoundaryKind.NegativeInfinity,
            (BoundaryDirection.To, IntervalBoundaryKind.Exclusive) => OrderedBoundaryKind.ToExclusive,
            (BoundaryDirection.From, IntervalBoundaryKind.Inclusive) => OrderedBoundaryKind.FromInclusive,
            (BoundaryDirection.To, IntervalBoundaryKind.Inclusive) => OrderedBoundaryKind.ToInclusive,
            (BoundaryDirection.From, IntervalBoundaryKind.Exclusive) => OrderedBoundaryKind.FromExclusive,
            (_, IntervalBoundaryKind.PositiveInfinity) => OrderedBoundaryKind.PositiveInfinity,
            (BoundaryDirection.From, IntervalBoundaryKind.Empty) => OrderedBoundaryKind.FromEmpty
        };

    enum BoundaryDirection
    {
        From,
        To
    }

    enum OrderedBoundaryKind
    {
        ToEmpty,
        NegativeInfinity,
        ToExclusive,
        FromInclusive,
        ToInclusive,
        FromExclusive,
        PositiveInfinity,
        FromEmpty
    }

    public static string ToString<TInterval, TBound>(in TInterval interval)
        where TInterval : IIntervalOperations<TBound>
    {
        var sb = new StringBuilder();

        if (interval.From.Kind == IntervalBoundaryKind.Inclusive)
            sb.Append('[');
        else
            sb.Append('(');

        AppendBoundary(sb, interval.From);
        sb.Append(',');
        AppendBoundary(sb, interval.To);

        if (interval.To.Kind == IntervalBoundaryKind.Inclusive)
            sb.Append(']');
        else
            sb.Append(')');

        return sb.ToString();

        static void AppendBoundary(StringBuilder sb, in IntervalBoundary<TBound> boundary)
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
    }

    public static Optional<TValue> Clamp<TValue, TInterval>(
        in TInterval interval,
        TValue value,
        Func<TValue, TValue> nextUp,
        Func<TValue, TValue> nextDown)
        where TInterval : IInterval<TValue>
    {
        if (interval.IsInfinite)
            return value;
        if (interval.IsEmpty)
            return default;

        var comparer = interval.Comparer;
        var allowedMinimum = Optional<TValue>.None;

        switch (interval.From.Kind)
        {
            case IntervalBoundaryKind.Inclusive:
                {
                    var limit = interval.From.Value;
                    if (comparer.Compare(value, limit) < 0)
                        value = limit;
                }
                break;

            case IntervalBoundaryKind.Exclusive:
                {
                    var limit = interval.From.Value;
                    allowedMinimum = nextUp(limit);
                    if (comparer.Compare(value, limit) <= 0)
                        value = allowedMinimum.Value;
                }
                break;
        }

        switch (interval.To.Kind)
        {
            case IntervalBoundaryKind.Inclusive:
                {
                    var limit = interval.To.Value;
                    if (comparer.Compare(value, limit) > 0)
                        value = limit;
                }
                break;

            case IntervalBoundaryKind.Exclusive:
                {
                    var limit = interval.To.Value;
                    if (comparer.Compare(value, limit) >= 0)
                    {
                        value = nextDown(limit);
                        if (allowedMinimum.HasValue && comparer.Compare(value, allowedMinimum.Value) < 0)
                        {
                            // Convergence is impossible.
                            // The clamped value cannot be represented by TValue type.
                            return default;
                        }
                    }
                }
                break;
        }

        return value;
    }
}

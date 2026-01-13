// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using Gapotchenko.FX.Math.Intervals.Properties;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.Math.Intervals;

static class IntervalEngine
{
    /// <exception cref="ArgumentException">If one interval boundary is empty, another should be empty too.</exception>
    [StackTraceHidden]
    public static void ValidateBoundaryArguments<T>(
        in IntervalBoundary<T> from,
        in IntervalBoundary<T> to,
        [CallerArgumentExpression(nameof(from))] string? fromParamName = null,
        [CallerArgumentExpression(nameof(to))] string? toParamName = null)
    {
        var (message, paramName) = VerifyBoundaries(from, to, true, fromParamName, toParamName);
        if (message != null)
            throw new ArgumentException(message, paramName);
    }

    public static (string? Message, string? ParamName) VerifyBoundaries<T>(
        in IntervalBoundary<T> left,
        in IntervalBoundary<T> right,
        bool message,
        [CallerArgumentExpression(nameof(left))] string? leftParamName = null,
        [CallerArgumentExpression(nameof(right))] string? rightParamName = null)
    {
        // Presence mismatch.
        if (left.Kind is IntervalBoundaryKind.Empty != right.Kind is IntervalBoundaryKind.Empty)
            return (message ? Resources.OneEmptyBoundaryRequiresAnother : string.Empty, null);

        // Infinity sign violation.
        if (left.Kind is IntervalBoundaryKind.PositiveInfinity)
            return (message ? "The left interval boundary cannot be a positive infinity." : string.Empty, leftParamName);
        if (right.Kind is IntervalBoundaryKind.NegativeInfinity)
            return (message ? "The right interval boundary cannot be a negative infinity." : string.Empty, rightParamName);

        // No errors.
        return (null, null);
    }

    public static bool DoesEmptyBoundaryMatchAnother<T>(in IntervalBoundary<T> from, in IntervalBoundary<T> to) =>
        from.Kind is IntervalBoundaryKind.Empty == to.Kind is IntervalBoundaryKind.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBounded<TInterval, TValue>(in TInterval interval) where TInterval : IIntervalModel<TValue> =>
        !interval.From.IsInfinity &&
        !interval.To.IsInfinity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsHalfBounded<TInterval, TValue>(in TInterval interval) where TInterval : IIntervalModel<TValue> =>
        interval.From.IsInfinity ^
        interval.To.IsInfinity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOpen<TInterval, TValue>(in TInterval interval) where TInterval : IIntervalModel<TValue> =>
        interval.From.Kind == IntervalBoundaryKind.Exclusive &&
        interval.To.Kind == IntervalBoundaryKind.Exclusive;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsClosed<TInterval, TValue>(in TInterval interval) where TInterval : IIntervalModel<TValue> =>
        interval.From.Kind != IntervalBoundaryKind.Exclusive &&
        interval.To.Kind != IntervalBoundaryKind.Exclusive;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsHalfOpen<TInterval, TValue>(in TInterval interval) where TInterval : IIntervalModel<TValue> =>
        interval.From.Kind == IntervalBoundaryKind.Exclusive ^
        interval.To.Kind == IntervalBoundaryKind.Exclusive;

    public delegate TInterval Constructor<out TInterval, TValue>(
        in IntervalBoundary<TValue> from,
        in IntervalBoundary<TValue> to)
        where TInterval : IIntervalModel<TValue>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TInterval Interior<TInterval, TValue>(in TInterval interval, Constructor<TInterval, TValue> constructor)
        where TInterval : IIntervalModel<TValue> =>
        IsOpen<TInterval, TValue>(interval) ?
            interval :
            WithInclusivity(interval, false, constructor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TInterval Enclosure<TInterval, TValue>(in TInterval interval, Constructor<TInterval, TValue> constructor)
        where TInterval : IIntervalModel<TValue> =>
        IsClosed<TInterval, TValue>(interval) ?
            interval :
            WithInclusivity(interval, true, constructor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static TInterval WithInclusivity<TInterval, TValue>(
        in TInterval interval,
        bool inclusive,
        Constructor<TInterval, TValue> constructor)
        where TInterval : IIntervalModel<TValue>
    {
        return constructor(
            WithBoundaryInclusivity(interval.From, inclusive),
            WithBoundaryInclusivity(interval.To, inclusive));

        static IntervalBoundary<TValue> WithBoundaryInclusivity(IntervalBoundary<TValue> boundary, bool inclusive)
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
    public static bool IsEmpty<TInterval, TValue>(in TInterval interval, IComparer<TValue> comparer)
        where TInterval : IIntervalModel<TValue> =>
        CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.To, interval.To, comparer) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInfinite<TInterval, TValue>(in TInterval interval)
        where TInterval : IIntervalModel<TValue> =>
        interval.From.Kind == IntervalBoundaryKind.NegativeInfinity &&
        interval.To.Kind == IntervalBoundaryKind.PositiveInfinity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDegenerate<TInterval, TValue>(in TInterval interval, IComparer<TValue> comparer)
        where TInterval : IIntervalModel<TValue> =>
        interval.From.Kind == IntervalBoundaryKind.Inclusive &&
        interval.To.Kind == IntervalBoundaryKind.Inclusive &&
        comparer.Compare(interval.From.Value, interval.To.Value) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<TInterval, TValue>(in TInterval interval, TValue value, IComparer<TValue> comparer)
        where TInterval : IIntervalModel<TValue> =>
        CompareBoundaries(BoundaryDirection.From, interval.From, value, comparer) <= 0 &&
        CompareBoundaries(BoundaryDirection.To, interval.To, value, comparer) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CompareTo<TInterval, TValue>(in TInterval interval, TValue? value, IComparer<TValue> comparer)
        where TInterval : IIntervalModel<TValue>
    {
        if (IsEmpty(interval, comparer))
            return 0; // convention, zone is undefined
        else if (CompareBoundaries(BoundaryDirection.From, interval.From, value, comparer) > 0)
            return 1; // before the left interval boundary
        else if (CompareBoundaries(BoundaryDirection.To, interval.To, value, comparer) < 0)
            return -1; // past the right interval boundary
        else
            return 0; // contained in the interval
    }

    static int CompareBoundaries<TValue>(
        BoundaryDirection direction,
        in IntervalBoundary<TValue> x, TValue? y,
        IComparer<TValue> comparer) =>
        (direction, x.Kind) switch
        {
            (_, IntervalBoundaryKind.NegativeInfinity or IntervalBoundaryKind.Empty) => -1,
            (_, IntervalBoundaryKind.Inclusive) => comparer.Compare(x.Value, y),
            (BoundaryDirection.From, IntervalBoundaryKind.Exclusive) => comparer.Compare(x.Value, y) >= 0 ? 1 : -1,
            (BoundaryDirection.To, IntervalBoundaryKind.Exclusive) => comparer.Compare(x.Value, y) <= 0 ? -1 : 1,
            (_, IntervalBoundaryKind.PositiveInfinity) => 1
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Overlaps<TInterval, TOther, TValue>(in TInterval interval, in TOther other, IComparer<TValue> comparer)
        where TInterval : IIntervalModel<TValue>
        where TOther : IIntervalModel<TValue> =>
        CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.To, other.To, comparer) <= 0 &&
        CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.From, other.From, comparer) >= 0;

    public static bool IntervalsEqual<TInterval, TOther, TValue>(in TInterval x, in TOther? y, IComparer<TValue> comparer)
        where TInterval : IIntervalModel<TValue>
        where TOther : IIntervalModel<TValue> =>
        y is not null &&
        (IsEmpty(x, comparer) && IsEmpty(y, comparer) ||
        x.From.Equals(y.From, comparer) && x.To.Equals(y.To, comparer));

    static int CompareBoundaries<TValue>(
        BoundaryDirection directionX, in IntervalBoundary<TValue> boundaryX,
        BoundaryDirection directionY, in IntervalBoundary<TValue> boundaryY,
        IComparer<TValue> comparer)
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
    public static TInterval Intersect<TInterval, TOther, TValue>(
        in TInterval interval,
        in TOther other,
        IComparer<TValue> comparer,
        Constructor<TInterval, TValue> constructor)
        where TInterval : IIntervalModel<TValue>
        where TOther : IIntervalModel<TValue> =>
        constructor(
            CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) >= 0 ? interval.From : other.From,
            CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) <= 0 ? interval.To : other.To);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TInterval Union<TInterval, TOther, TValue>(
        in TInterval interval,
        in TOther other,
        IComparer<TValue> comparer,
        Constructor<TInterval, TValue> constructor)
        where TInterval : IIntervalModel<TValue>
        where TOther : IIntervalModel<TValue> =>
        constructor(
            CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) <= 0 ? interval.From : other.From,
            CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) >= 0 ? interval.To : other.To);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSubintervalOf<TInterval, TOther, TValue>(in TInterval interval, in TOther other, IComparer<TValue> comparer)
        where TInterval : IIntervalModel<TValue>
        where TOther : IIntervalModel<TValue> =>
        CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) >= 0 &&
        CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) <= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSuperintervalOf<TInterval, TOther, TValue>(in TInterval interval, in TOther other, IComparer<TValue> comparer)
        where TInterval : IIntervalModel<TValue>
        where TOther : IIntervalModel<TValue> =>
        CompareBoundaries(BoundaryDirection.From, interval.From, BoundaryDirection.From, other.From, comparer) <= 0 &&
        CompareBoundaries(BoundaryDirection.To, interval.To, BoundaryDirection.To, other.To, comparer) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsProperSubintervalOf<TInterval, TOther, TValue>(in TInterval interval, in TOther other, IComparer<TValue> comparer)
        where TInterval : IIntervalModel<TValue>
        where TOther : IIntervalModel<TValue>
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
    public static bool IsProperSuperintervalOf<TInterval, TOther, TValue>(in TInterval interval, in TOther other, IComparer<TValue> comparer)
        where TInterval : IIntervalModel<TValue>
        where TOther : IIntervalModel<TValue>
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

    public static string ToString<TInterval, TValue>(in TInterval interval, string? format, IFormatProvider? formatProvider)
        where TInterval : IIntervalOperations<TValue> =>
        format switch
        {
            "G" or "" or null => ToString<TInterval, TValue>(interval, formatProvider),
            "U" => ToStringCore<TInterval, TValue>(interval, "∅", "∞", formatProvider),
            _ => throw new FormatException()
        };

    public static string ToString<TInterval, TValue>(in TInterval interval, IFormatProvider? formatProvider = null)
        where TInterval : IIntervalOperations<TValue> =>
        ToStringCore<TInterval, TValue>(interval, "{}", "inf", formatProvider);

    static string ToStringCore<TInterval, TValue>(in TInterval interval, string emptySymbol, string infinitySymbol, IFormatProvider? formatProvider)
        where TInterval : IIntervalOperations<TValue>
    {
        if (interval.IsEmpty)
            return emptySymbol;

        var sb = new StringBuilder();

        if (interval.From.Kind == IntervalBoundaryKind.Inclusive)
            sb.Append('[');
        else
            sb.Append('(');

        AppendBoundary(sb, interval.From, emptySymbol, infinitySymbol, formatProvider);
        sb.Append(',');
        AppendBoundary(sb, interval.To, emptySymbol, infinitySymbol, formatProvider);

        if (interval.To.Kind == IntervalBoundaryKind.Inclusive)
            sb.Append(']');
        else
            sb.Append(')');

        return sb.ToString();
    }

    public static string ToString<T>(in IntervalBoundary<T> boundary, string? format, IFormatProvider? formatProvider) =>
        format switch
        {
            "G" or "" or null => ToStringCore(boundary, "{}", "inf", formatProvider),
            "U" => ToStringCore(boundary, "∅", "∞", formatProvider),
            _ => throw new FormatException()
        };

    static string ToStringCore<T>(in IntervalBoundary<T> boundary, string emptySymbol, string infinitySymbol, IFormatProvider? formatProvider)
    {
        var sb = new StringBuilder();
        AppendBoundary(sb, boundary, emptySymbol, infinitySymbol, formatProvider);

        string? description = boundary.Kind switch
        {
            IntervalBoundaryKind.Inclusive => "inclusive",
            IntervalBoundaryKind.Exclusive => "exclusive",
            _ => null
        };

        if (description != null)
            sb.AppendFormat(" ({0})", description);

        return sb.ToString();
    }

    static void AppendBoundary<T>(
        StringBuilder sb,
        in IntervalBoundary<T> boundary,
        string emptySymbol,
        string infinitySymbol,
        IFormatProvider? formatProvider)
    {
        switch (boundary.Kind)
        {
            case IntervalBoundaryKind.Empty:
                sb.Append(emptySymbol); // ∅
                break;

            case IntervalBoundaryKind.NegativeInfinity:
                sb.Append('-').Append(infinitySymbol); // -∞
                break;

            case IntervalBoundaryKind.PositiveInfinity:
                sb.Append(infinitySymbol); // ∞
                break;

            default:
                var value = boundary.Value;
                if (formatProvider is not null && value is IFormattable formattableValue)
                    sb.Append(formattableValue.ToString(null, formatProvider));
                else
                    sb.Append(value?.ToString());
                break;
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
        var allowedMinimum = Optional.None<TValue>();

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
                            // The clamped value cannot be represented by the TValue type.
                            return default;
                        }
                    }
                }
                break;
        }

        return value;
    }
}

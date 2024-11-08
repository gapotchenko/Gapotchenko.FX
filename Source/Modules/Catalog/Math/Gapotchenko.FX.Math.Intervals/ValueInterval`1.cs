// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using System.Diagnostics;

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Represents a continuous value interval.
/// </summary>
/// <typeparam name="T">The type of interval value.</typeparam>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct ValueInterval<T> : IInterval<T>, IEquatable<ValueInterval<T>>, IEmptiable<ValueInterval<T>>
    where T : IEquatable<T>?, IComparable<T>?
{
    /// <summary>
    /// Initializes a new <see cref="ValueInterval{T}"/> instance with the specified inclusive left and exclusive right bounds:
    /// <c>[from,to)</c>.
    /// </summary>
    /// <param name="from">
    /// The left bound of the interval.
    /// Represents a value the interval starts with.
    /// The corresponding limit point is included in the interval.
    /// </param>
    /// <param name="to">
    /// The right bound of the interval.
    /// Represents a value the interval ends with.
    /// The corresponding limit point is not included in the interval.
    /// </param>
    public ValueInterval(T from, T to) :
        this(IntervalBoundary.Inclusive(from), IntervalBoundary.Exclusive(to))
    {
    }

    /// <summary>
    /// Initializes a new <see cref="ValueInterval{T}"/> instance with the specified boundaries.
    /// </summary>
    /// <param name="from">
    /// The left boundary of the interval.
    /// Represents a boundary the interval starts with.
    /// </param>
    /// <param name="to">
    /// The right boundary of the interval.
    /// Represents a boundary the interval ends with.
    /// </param>
    /// <exception cref="ArgumentException">If one interval boundary is empty, another should be empty too.</exception>
    public ValueInterval(IntervalBoundary<T> from, IntervalBoundary<T> to)
    {
        IntervalEngine.ValidateBoundaries(from, to);

        From = from;
        To = to;
    }

#pragma warning disable CA1000 // Do not declare static members on generic types
    /// <inheritdoc cref="ValueInterval.Empty{T}"/>
    [Obsolete(
        "Use ValueInterval.Empty<T>() method instead because this method is a part of Gapotchenko.FX infrastructure and should not be used directly."
#if NET5_0_OR_GREATER
        , DiagnosticId = "GPFX0001"
#endif
        )]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ValueInterval<T> Empty { get; } = new(IntervalBoundary<T>.Empty, IntervalBoundary<T>.Empty);
#pragma warning restore CA1000 // Do not declare static members on generic types

    internal static ValueInterval<T> Infinite { get; } = new(IntervalBoundary<T>.NegativeInfinity, IntervalBoundary<T>.PositiveInfinity);

    /// <summary>
    /// The left boundary of the interval.
    /// Represents a boundary the interval starts with.
    /// </summary>
    public IntervalBoundary<T> From { get; init; }

    /// <summary>
    /// The right boundary of the interval.
    /// Represents a boundary the interval ends with.
    /// </summary>
    public IntervalBoundary<T> To { get; init; }

    IComparer<T> IIntervalOperations<T>.Comparer => Comparer<T>.Default;

    /// <inheritdoc/>
    public bool IsBounded => IntervalEngine.IsBounded<ValueInterval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsHalfBounded => IntervalEngine.IsHalfBounded<ValueInterval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsOpen => IntervalEngine.IsOpen<ValueInterval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsClosed => IntervalEngine.IsClosed<ValueInterval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsHalfOpen => IntervalEngine.IsHalfOpen<ValueInterval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsEmpty => IntervalEngine.IsEmpty(this, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IsInfinite => IntervalEngine.IsInfinite<ValueInterval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsDegenerate => IntervalEngine.IsDegenerate(this, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool Contains(T value) => IntervalEngine.Contains(this, value, Comparer<T>.Default);

    /// <inheritdoc/>
    public int Zone(T value) => IntervalEngine.Zone(this, value, Comparer<T>.Default);

    /// <summary>
    /// <para>
    /// Gets the interval interior.
    /// </para>
    /// <para>
    /// The interior of an interval <c>I</c> is the largest open interval that is contained in <c>I</c>;
    /// it is also the set of points in <c>I</c> which are not the endpoints of <c>I</c>.
    /// </para>
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ValueInterval<T> Interior => IntervalEngine.Interior<ValueInterval<T>, T>(this, Construct);

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IInterval<T> IIntervalOperations<T>.Interior => Interior;

    /// <summary>
    /// <para>
    /// Gets the interval enclosure.
    /// </para>
    /// <para>
    /// The enclosure of an interval <c>I</c> is the smallest closed interval that contains <c>I</c>;
    /// which is also the set <c>I</c> augmented with its finite endpoints.
    /// </para>
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ValueInterval<T> Enclosure => IntervalEngine.Enclosure<ValueInterval<T>, T>(this, Construct);

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IInterval<T> IIntervalOperations<T>.Enclosure => Enclosure;

    /// <summary>
    /// Produces the intersection of the current and specified intervals.
    /// </summary>
    /// <param name="other">The interval to produce the intersection with.</param>
    /// <returns>A new interval representing an intersection of the current and specified intervals.</returns>
    public ValueInterval<T> Intersect(IInterval<T> other) => Intersect<IIntervalOperations<T>>(other);

    /// <summary>
    /// Produces the intersection of the current and specified intervals.
    /// </summary>
    /// <param name="other">The interval to produce the intersection with.</param>
    /// <returns>A new interval representing an intersection of the current and specified intervals.</returns>
    /// <typeparam name="TOther">Type of the other interval to produce the intersection with.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ValueInterval<T> Intersect<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.Intersect(
            this,
            other ?? throw new ArgumentNullException(nameof(other)),
            Comparer<T>.Default,
            Construct);

    IInterval<T> IIntervalOperations<T>.Intersect(IInterval<T> other) => Intersect<IIntervalOperations<T>>(other);

    /// <summary>
    /// Produces the union of the current and specified intervals.
    /// </summary>
    /// <param name="other">The interval to produce the union with.</param>
    /// <returns>A new interval representing a union of the current and specified intervals.</returns>
    public ValueInterval<T> Union(IInterval<T> other) => Union<IIntervalOperations<T>>(other);

    /// <summary>
    /// Produces the union of the current and specified intervals.
    /// </summary>
    /// <param name="other">The interval to produce the union with.</param>
    /// <returns>A new interval representing a union of the current and specified intervals.</returns>
    /// <typeparam name="TOther">Type of the other interval to produce the union with.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ValueInterval<T> Union<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.Union(
            this,
            other ?? throw new ArgumentNullException(nameof(other)),
            Comparer<T>.Default,
            Construct);

    IInterval<T> IIntervalOperations<T>.Union(IInterval<T> other) => Union<IIntervalOperations<T>>(other);

    static ValueInterval<T> Construct(in IntervalBoundary<T> from, in IntervalBoundary<T> to) => new(from, to);

    /// <inheritdoc/>
    public bool Overlaps(IInterval<T> other) => Overlaps<IInterval<T>>(other);

    /// <summary>
    /// Determines whether this and the specified intervals overlap.
    /// </summary>
    /// <param name="other">The interval to check for overlapping.</param>
    /// <returns><see langword="true"/> if this interval and <paramref name="other"/> overlap; otherwise, <see langword="false"/>.</returns>
    /// <typeparam name="TOther">Type of the interval to check for overlapping.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Overlaps<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.Overlaps(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IsSubintervalOf(IInterval<T> other) => IsSubintervalOf<IInterval<T>>(other);

    /// <summary>
    /// Determines whether the current interval is a subinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a subinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsSubintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.IsSubintervalOf(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IsSuperintervalOf(IInterval<T> other) => IsSuperintervalOf<IInterval<T>>(other);

    /// <summary>
    /// Determines whether the current interval is a superinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a superinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsSuperintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.IsSuperintervalOf(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IsProperSubintervalOf(IInterval<T> other) => IsProperSubintervalOf<IInterval<T>>(other);

    /// <summary>
    /// Determines whether the current interval is a proper subinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a proper subinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsProperSubintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.IsProperSubintervalOf(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IsProperSuperintervalOf(IInterval<T> other) => IsProperSuperintervalOf<IInterval<T>>(other);

    /// <summary>
    /// Determines whether the current interval is a proper superinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a proper superinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsProperSuperintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.IsProperSuperintervalOf(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IntervalEquals(IInterval<T> other) => IntervalEquals<IIntervalOperations<T>>(other);

    /// <summary>
    /// Determines whether this and the specified intervals are equal.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if this interval and <paramref name="other"/> are equal; otherwise, <see langword="false"/>.</returns>
    /// <typeparam name="TOther">Type of the interval to compare.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IntervalEquals<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.IntervalsEqual(this, other, Comparer<T>.Default);

    /// <summary>
    /// Determines whether the specified intervals are equal.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns><see langword="true"/> if the specified intervals are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(in ValueInterval<T> x, IInterval<T>? y) => EqualityOperator(x, y);

    /// <summary>
    /// Determines whether the specified intervals are not equal.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns><see langword="true"/> if the specified intervals are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(in ValueInterval<T> x, IInterval<T>? y) => !EqualityOperator(x, y);

    /// <summary>
    /// Determines whether the specified intervals are equal.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns><see langword="true"/> if the specified intervals are equal; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(in ValueInterval<T> x, Interval<T>? y) => EqualityOperator(x, y);

    /// <summary>
    /// Determines whether the specified intervals are not equal.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns><see langword="true"/> if the specified intervals are not equal; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator !=(in ValueInterval<T> x, Interval<T>? y) => !EqualityOperator(x, y);

    static bool EqualityOperator<TOther>(in ValueInterval<T> x, TOther? y)
        where TOther : IIntervalOperations<T> =>
        y is not null &&
        x.IntervalEquals(y);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ValueInterval<T> other &&
        Equals(other);

    /// <inheritdoc/>
    public bool Equals(ValueInterval<T> other) => IntervalEngine.IntervalsEqual(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(From.GetHashCode(), To.GetHashCode());

    /// <inheritdoc/>
    public override string ToString() => IntervalEngine.ToString<ValueInterval<T>, T>(this);

    /// <summary>
    /// Converts a specified <see cref="Interval{T}"/> instance to <see cref="ValueInterval{T}"/>.
    /// </summary>
    /// <param name="interval">The <see cref="Interval{T}"/> instance to convert.</param>
    public static implicit operator ValueInterval<T>(Interval<T> interval) =>
        interval is null ?
            default :
            new(interval.From, interval.To);

    /// <summary>
    /// Converts a specified <see cref="ValueInterval{T}"/> instance to <see cref="Interval{T}"/>.
    /// </summary>
    /// <param name="interval">The <see cref="ValueInterval{T}"/> instance to convert.</param>
    public static implicit operator Interval<T>(ValueInterval<T> interval) => new(interval.From, interval.To);
}

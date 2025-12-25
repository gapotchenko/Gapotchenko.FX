// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Represents a continuous value interval.
/// </summary>
/// <typeparam name="T">The type of interval values.</typeparam>
[DebuggerDisplay("{ToString(),nq}")]
public readonly partial struct ValueInterval<T> : IConstructibleInterval<T, ValueInterval<T>>, IEquatable<ValueInterval<T>>
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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal static ValueInterval<T> Empty { get; } = new(IntervalBoundary<T>.Empty, IntervalBoundary<T>.Empty);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal static ValueInterval<T> Infinite { get; } = new(IntervalBoundary<T>.NegativeInfinity, IntervalBoundary<T>.PositiveInfinity);

    /// <inheritdoc/>
    public IntervalBoundary<T> From { get; init; }

    /// <inheritdoc/>
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

    /// <inheritdoc cref="Interval{T}.IsEmpty"/>
    public bool IsEmpty => IntervalEngine.IsEmpty(this, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IsInfinite => IntervalEngine.IsInfinite<ValueInterval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsDegenerate => IntervalEngine.IsDegenerate(this, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool Contains(T value) => IntervalEngine.Contains(this, value, Comparer<T>.Default);

    /// <inheritdoc/>
    public int Zone(T value) => IntervalEngine.Zone(this, value, Comparer<T>.Default);

    /// <inheritdoc cref="IIntervalOperations{T}.Interior"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ValueInterval<T> Interior => IntervalEngine.Interior<ValueInterval<T>, T>(this, Construct);

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IInterval<T> IIntervalOperations<T>.Interior => Interior;

    /// <inheritdoc cref="IIntervalOperations{T}.Enclosure"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ValueInterval<T> Enclosure => IntervalEngine.Enclosure<ValueInterval<T>, T>(this, Construct);

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IInterval<T> IIntervalOperations<T>.Enclosure => Enclosure;

    /// <inheritdoc cref="IIntervalOperations{T}.Intersect(IInterval{T})"/>
    public ValueInterval<T> Intersect(IInterval<T> other) => Intersect<IIntervalOperations<T>>(other);

    /// <inheritdoc cref="Intersect(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval to produce the intersection with.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ValueInterval<T> Intersect<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.Intersect(
            this,
            other ?? throw new ArgumentNullException(nameof(other)),
            Comparer<T>.Default,
            Construct);

    IInterval<T> IIntervalOperations<T>.Intersect(IInterval<T> other) => Intersect<IIntervalOperations<T>>(other);

    /// <inheritdoc cref="IIntervalOperations{T}.Union(IInterval{T})"/>
    public ValueInterval<T> Union(IInterval<T> other) => Union<IIntervalOperations<T>>(other);

    /// <inheritdoc cref="Union(IInterval{T})"/>
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

    /// <inheritdoc cref="Overlaps(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the interval to check for overlapping.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Overlaps<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.Overlaps(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IsSubintervalOf(IInterval<T> other) => IsSubintervalOf<IInterval<T>>(other);

    /// <inheritdoc cref="IsSubintervalOf(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsSubintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.IsSubintervalOf(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IsSuperintervalOf(IInterval<T> other) => IsSuperintervalOf<IInterval<T>>(other);

    /// <inheritdoc cref="IsSuperintervalOf(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsSuperintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.IsSuperintervalOf(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IsProperSubintervalOf(IInterval<T> other) => IsProperSubintervalOf<IInterval<T>>(other);

    /// <inheritdoc cref="IsProperSubintervalOf(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsProperSubintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.IsProperSubintervalOf(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IsProperSuperintervalOf(IInterval<T> other) => IsProperSuperintervalOf<IInterval<T>>(other);

    /// <inheritdoc cref="IsProperSuperintervalOf(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsProperSuperintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.IsProperSuperintervalOf(this, other, Comparer<T>.Default);

    /// <inheritdoc/>
    public bool IntervalEquals(IInterval<T> other) => IntervalEquals<IIntervalOperations<T>>(other);

    /// <inheritdoc cref="IntervalEquals(IInterval{T})"/>
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

    /// <inheritdoc cref="operator ==(in ValueInterval{T}, IInterval{T}?)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool operator ==(in ValueInterval<T> x, Interval<T>? y) => EqualityOperator(x, y);

    /// <inheritdoc cref="operator !=(in ValueInterval{T}, IInterval{T}?)"/>
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

    /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
    public string ToString(string? format) => ToString(format, null);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        IntervalEngine.ToString<ValueInterval<T>, T>(this, format, formatProvider);

    #region IConstructibleInterval

#if TFF_STATIC_INTERFACE

    static ValueInterval<T> IEmptiable<ValueInterval<T>>.Empty => Empty;

    static ValueInterval<T> IConstructibleInterval<T, ValueInterval<T>>.Infinite => Infinite;

    static ValueInterval<T> IConstructibleInterval<T, ValueInterval<T>>.Create(IntervalBoundary<T> from, IntervalBoundary<T> to, IComparer<T>? comparer)
    {
        ValidateComparer(comparer);

        return new(from, to);
    }

    static void ValidateComparer(
        IComparer<T>? comparer,
        [CallerArgumentExpression(nameof(comparer))] string? paramName = null)
    {
        if (FX.Empty.Nullify(comparer) is not null)
        {
            throw new ArgumentException(
                string.Format(
                    "The specified comparer is not compatible with the comparer used by {0}.",
                    typeof(ValueInterval<T>)),
                paramName);
        }
    }

#endif

    #endregion
}

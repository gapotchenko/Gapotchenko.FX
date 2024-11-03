// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using Gapotchenko.FX.Runtime.CompilerServices;
using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Represents a continuous interval.
/// </summary>
/// <typeparam name="T">The type of interval value.</typeparam>
[DebuggerDisplay("{ToString(),nq}")]
public sealed record Interval<T> : IInterval<T>, IEmptiable<Interval<T>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Interval{T}"/> class with the specified inclusive left and exclusive right bounds:
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
    /// <param name="comparer">
    /// The <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
    /// or <see langword="null"/> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
    /// </param>
    public Interval(T from, T to, IComparer<T>? comparer = null) :
        this(IntervalBoundary.Inclusive(from), IntervalBoundary.Exclusive(to), comparer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Interval{T}"/> class with the specified boundaries.
    /// </summary>
    /// <param name="from">
    /// The left boundary of the interval.
    /// Represents a boundary the interval starts with.
    /// </param>
    /// <param name="to">
    /// The right boundary of the interval.
    /// Represents a boundary the interval ends with.
    /// </param>
    /// <param name="comparer">
    /// The <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
    /// or <see langword="null"/> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
    /// </param>
    public Interval(IntervalBoundary<T> from, IntervalBoundary<T> to, IComparer<T>? comparer = null)
    {
        From = from;
        To = to;

        Comparer = comparer;
    }

#pragma warning disable CA1000 // Do not declare static members on generic types
    /// <inheritdoc cref="Interval.Empty{T}"/>
    [Obsolete(
        "Use Interval.Empty<T>() method instead because this method is a part of Gapotchenko.FX infrastructure and should not be used directly."
#if NET5_0_OR_GREATER
        , DiagnosticId = "GPFX0001"
#endif
        )]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Interval<T> Empty { get; } = new(IntervalBoundary<T>.Empty, IntervalBoundary<T>.Empty);
#pragma warning restore CA1000 // Do not declare static members on generic types

    internal static Interval<T> Infinite { get; } = new(IntervalBoundary<T>.NegativeInfinity, IntervalBoundary<T>.PositiveInfinity);

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

    /// <summary>
    /// The <see cref="IComparer{T}"/> object that is used to compare the values in the interval.
    /// </summary>
    [AllowNull]
    public IComparer<T> Comparer
    {
        get => m_Comparer;

        [MemberNotNull(nameof(m_Comparer))]
        init => m_Comparer = value ?? Comparer<T>.Default;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IComparer<T> m_Comparer;

    /// <inheritdoc/>
    public bool IsBounded => IntervalEngine.IsBounded<Interval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsHalfBounded => IntervalEngine.IsHalfBounded<Interval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsOpen => IntervalEngine.IsOpen<Interval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsClosed => IntervalEngine.IsClosed<Interval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsHalfOpen => IntervalEngine.IsHalfOpen<Interval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsEmpty => IntervalEngine.IsEmpty(this, m_Comparer);

    /// <inheritdoc/>
    public bool IsInfinite => IntervalEngine.IsInfinite<Interval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsDegenerate => IntervalEngine.IsDegenerate(this, m_Comparer);

    /// <inheritdoc/>
    public bool Contains(T value) => IntervalEngine.Contains(this, value, m_Comparer);

    /// <inheritdoc/>
    public int Sign(T value) => IntervalEngine.Sign(this, value, m_Comparer);

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
    public Interval<T> Interior => IntervalEngine.Interior<Interval<T>, T>(this, Construct);

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
    public Interval<T> Enclosure => IntervalEngine.Enclosure<Interval<T>, T>(this, Construct);

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IInterval<T> IIntervalOperations<T>.Enclosure => Enclosure;

    /// <summary>
    /// Produces the intersection of the current and specified intervals.
    /// </summary>
    /// <param name="other">The interval to produce the intersection with.</param>
    /// <returns>A new interval representing an intersection of the current and specified intervals.</returns>
    public Interval<T> Intersect(IInterval<T> other) => Intersect<IIntervalOperations<T>>(other);

    /// <summary>
    /// Produces the intersection of the current and specified intervals.
    /// </summary>
    /// <param name="other">The interval to produce the intersection with.</param>
    /// <returns>A new interval representing an intersection of the current and specified intervals.</returns>
    /// <typeparam name="TOther">Type of the other interval to produce the intersection with.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Interval<T> Intersect<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.Intersect(
            this,
            other ?? throw new ArgumentNullException(nameof(other)),
            m_Comparer,
            Construct);

    IInterval<T> IIntervalOperations<T>.Intersect(IInterval<T> other) => Intersect<IIntervalOperations<T>>(other);

    /// <summary>
    /// Produces the union of the current and specified intervals.
    /// </summary>
    /// <param name="other">The interval to produce the union with.</param>
    /// <returns>A new interval representing a union of the current and specified intervals.</returns>
    public Interval<T> Union(IInterval<T> other) => Union<IIntervalOperations<T>>(other);

    /// <summary>
    /// Produces the union of the current and specified intervals.
    /// </summary>
    /// <param name="other">The interval to produce the union with.</param>
    /// <returns>A new interval representing a union of the current and specified intervals.</returns>
    /// <typeparam name="TOther">Type of the other interval to produce the union with.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Interval<T> Union<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.Union(
            this,
            other ?? throw new ArgumentNullException(nameof(other)),
            m_Comparer,
            Construct);

    IInterval<T> IIntervalOperations<T>.Union(IInterval<T> other) => Union<IIntervalOperations<T>>(other);

    Interval<T> Construct(IntervalBoundary<T> from, IntervalBoundary<T> to) => new(from, to, m_Comparer);

    bool IsThis<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        !TypeTraits<TOther>.IsValueType &&
        ReferenceEquals(other, this);

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
        IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
        IntervalEngine.Overlaps(this, other, m_Comparer);

    /// <inheritdoc/>
    public bool IsSubintervalOf(IInterval<T> other) => IsSubintervalOf<IInterval<T>>(other);

    /// <summary>
    /// Determines whether the current interval is a subinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a subinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsSubintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
        IntervalEngine.IsSubintervalOf(this, other, m_Comparer);

    /// <inheritdoc/>
    public bool IsSuperintervalOf(IInterval<T> other) => IsSuperintervalOf<IInterval<T>>(other);

    /// <summary>
    /// Determines whether the current interval is a superinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a superinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsSuperintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
        IntervalEngine.IsSuperintervalOf(this, other, m_Comparer);

    /// <inheritdoc/>
    public bool IsProperSubintervalOf(IInterval<T> other) => IsProperSubintervalOf<IInterval<T>>(other);

    /// <summary>
    /// Determines whether the current interval is a proper subinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a proper subinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsProperSubintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        !IsThis(other ?? throw new ArgumentNullException(nameof(other))) &&
        IntervalEngine.IsProperSubintervalOf(this, other, m_Comparer);

    /// <inheritdoc/>
    public bool IsProperSuperintervalOf(IInterval<T> other) => IsProperSuperintervalOf<IInterval<T>>(other);

    /// <summary>
    /// Determines whether the current interval is a proper superinterval of the specified interval.
    /// </summary>
    /// <param name="other">The interval to compare to the current interval.</param>
    /// <returns><see langword="true"/> if the current interval is a proper superinterval of <paramref name="other"/>; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsProperSuperintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        !IsThis(other ?? throw new ArgumentNullException(nameof(other))) &&
        IntervalEngine.IsProperSuperintervalOf(this, other, m_Comparer);

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
        IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
        IntervalEngine.IntervalsEqual(this, other, m_Comparer);

    /// <summary>
    /// Determines whether the specified intervals are equal.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns><see langword="true"/> if the specified intervals are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(Interval<T>? x, IInterval<T>? y) => EqualityOperator(x, y);

    /// <summary>
    /// Determines whether the specified intervals are not equal.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns><see langword="true"/> if the specified intervals are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(Interval<T>? x, IInterval<T>? y) => !EqualityOperator(x, y);

    static bool EqualityOperator(Interval<T>? x, IInterval<T>? y) =>
        ReferenceEquals(x, y) ||
        x is not null &&
        y is not null &&
        x.IntervalEquals(y);

    /// <inheritdoc/>
    public override string ToString() => IntervalEngine.ToString<Interval<T>, T>(this);

    // Minify unused record method.
    bool PrintMembers(StringBuilder _) => false;
}

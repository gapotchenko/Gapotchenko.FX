// Gapotchenko.FX
//
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
/// <typeparam name="T">The type of interval values.</typeparam>
[ImmutableObject(true)]
[DebuggerDisplay("{ToString(),nq}")]
public sealed partial record Interval<T> : IConstructibleInterval<T, Interval<T>>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal static Interval<T> Empty { get; } = new(IntervalBoundary<T>.Empty, IntervalBoundary<T>.Empty);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal static Interval<T> Infinite { get; } = new(IntervalBoundary<T>.NegativeInfinity, IntervalBoundary<T>.PositiveInfinity);

    /// <inheritdoc/>
    public IntervalBoundary<T> From { get; init; }

    /// <inheritdoc/>
    public IntervalBoundary<T> To { get; init; }

    IIntervalBoundary IIntervalModel.From => From;

    IIntervalBoundary IIntervalModel.To => To;

    /// <summary>
    /// Gets or initializes the <see cref="IComparer{T}"/> object that is used to compare the values in the interval.
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

    /// <summary>
    /// Gets a value indicating whether the interval is empty.
    /// </summary>
    public bool IsEmpty => IntervalEngine.IsEmpty(this, m_Comparer);

    /// <inheritdoc/>
    public bool IsInfinite => IntervalEngine.IsInfinite<Interval<T>, T>(this);

    /// <inheritdoc/>
    public bool IsDegenerate => IntervalEngine.IsDegenerate(this, m_Comparer);

    /// <inheritdoc/>
    public bool Contains(T value) => IntervalEngine.Contains(this, value, m_Comparer);

#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY // 2025
    /// <inheritdoc/>
    [Obsolete("Use a negated value returned by CompareTo(value) method, or one of the relation operators: >, >=, <, <=.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int Zone(T value) => -CompareTo(value);
#endif

    /// <inheritdoc cref="IIntervalOperations{T}.Interior"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Interval<T> Interior => IntervalEngine.Interior<Interval<T>, T>(this, Construct);

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IInterval<T> IIntervalOperations<T>.Interior => Interior;

    /// <inheritdoc cref="IIntervalOperations{T}.Enclosure"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Interval<T> Enclosure => IntervalEngine.Enclosure<Interval<T>, T>(this, Construct);

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IInterval<T> IIntervalOperations<T>.Enclosure => Enclosure;

    /// <inheritdoc cref="IIntervalOperations{T}.Intersect(IInterval{T})"/>
    public Interval<T> Intersect(IInterval<T> other) => Intersect<IIntervalOperations<T>>(other);

    /// <inheritdoc cref="Intersect(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval to produce the intersection with.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Interval<T> Intersect<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.Intersect(
            this,
            other ?? throw new ArgumentNullException(nameof(other)),
            m_Comparer,
            Construct);

    IInterval<T> IIntervalOperations<T>.Intersect(IInterval<T> other) => Intersect<IIntervalOperations<T>>(other);

    /// <inheritdoc cref="IIntervalOperations{T}.Union(IInterval{T})"/>
    public Interval<T> Union(IInterval<T> other) => Union<IIntervalOperations<T>>(other);

    /// <inheritdoc cref="Union(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval to produce the union with.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Interval<T> Union<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IntervalEngine.Union(
            this,
            other ?? throw new ArgumentNullException(nameof(other)),
            m_Comparer,
            Construct);

    IInterval<T> IIntervalOperations<T>.Union(IInterval<T> other) => Union<IIntervalOperations<T>>(other);

    Interval<T> Construct(in IntervalBoundary<T> from, in IntervalBoundary<T> to) => new(from, to, m_Comparer);

    bool IsThis<TOther>(in TOther? other) =>
        !TypeTraits<TOther>.IsValueType &&
        ReferenceEquals(other, this);

    /// <inheritdoc/>
    public bool Overlaps(IInterval<T> other) => Overlaps<IInterval<T>>(other);

    /// <inheritdoc cref="Overlaps(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the interval to check for overlapping.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Overlaps<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
        IntervalEngine.Overlaps(this, other, m_Comparer);

    /// <inheritdoc/>
    public bool IsSubintervalOf(IInterval<T> other) => IsSubintervalOf<IInterval<T>>(other);

    /// <inheritdoc cref="IsSubintervalOf(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsSubintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
        IntervalEngine.IsSubintervalOf(this, other, m_Comparer);

    /// <inheritdoc/>
    public bool IsSuperintervalOf(IInterval<T> other) => IsSuperintervalOf<IInterval<T>>(other);

    /// <inheritdoc cref="IsSuperintervalOf(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsSuperintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        IsThis(other ?? throw new ArgumentNullException(nameof(other))) ||
        IntervalEngine.IsSuperintervalOf(this, other, m_Comparer);

    /// <inheritdoc/>
    public bool IsProperSubintervalOf(IInterval<T> other) => IsProperSubintervalOf<IInterval<T>>(other);

    /// <inheritdoc cref="IsProperSubintervalOf(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsProperSubintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        !IsThis(other ?? throw new ArgumentNullException(nameof(other))) &&
        IntervalEngine.IsProperSubintervalOf(this, other, m_Comparer);

    /// <inheritdoc/>
    public bool IsProperSuperintervalOf(IInterval<T> other) => IsProperSuperintervalOf<IInterval<T>>(other);

    /// <inheritdoc cref="IsProperSuperintervalOf(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the other interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsProperSuperintervalOf<TOther>(in TOther other) where TOther : IIntervalOperations<T> =>
        !IsThis(other ?? throw new ArgumentNullException(nameof(other))) &&
        IntervalEngine.IsProperSuperintervalOf(this, other, m_Comparer);

    /// <inheritdoc/>
    public bool IntervalEquals([NotNullWhen(true)] IInterval? other) =>
        other is IInterval<T> typed &&
        IntervalEquals(typed);

    /// <inheritdoc/>
    public bool IntervalEquals([NotNullWhen(true)] IInterval<T>? other) => IntervalEquals<IIntervalModel<T>>(other);

    /// <inheritdoc cref="IntervalEquals(IInterval{T})"/>
    /// <typeparam name="TOther">Type of the interval to compare.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IntervalEquals<TOther>([NotNullWhen(true)] in TOther? other) where TOther : IIntervalModel<T> =>
        IsThis(other) ||
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
        x is not null && x.IntervalEquals(y);

    /// <inheritdoc/>
    public override string ToString() => IntervalEngine.ToString<Interval<T>, T>(this);

    /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
    public string ToString(string? format) => ToString(format, null);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        IntervalEngine.ToString<Interval<T>, T>(this, format, formatProvider);

    // Minify unused record method.
    bool PrintMembers(StringBuilder _) => false;

    #region IConstructibleInterval

#if TFF_STATIC_INTERFACE

    static Interval<T> IEmptiable<Interval<T>>.Empty => Empty;

    static Interval<T> IConstructibleInterval<T, Interval<T>>.Infinite => Infinite;

    static Interval<T> IConstructibleInterval<T, Interval<T>>.Create(IntervalBoundary<T> from, IntervalBoundary<T> to, IComparer<T>? comparer) =>
        new(from, to, comparer);

#endif

    #endregion
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using Gapotchenko.FX.Math.Intervals.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Represents an interval boundary.
/// </summary>
/// <typeparam name="T">The type of a value of the bound limit point.</typeparam>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct IntervalBoundary<T> : IEquatable<IntervalBoundary<T>>
{
    internal IntervalBoundary(IntervalBoundaryKind kind, T value)
    {
        Kind = kind;
        m_Value = value;
    }

    /// <summary>
    /// Gets the value of a bound limit point.
    /// </summary>
    /// <exception cref="InvalidOperationException">Interval boundary has no bound limit point.</exception>
    public T Value
    {
        get
        {
            if (!HasValue)
                ThrowNoValue();
            return m_Value;

            [DoesNotReturn, StackTraceHidden]
            static void ThrowNoValue() =>
                throw new InvalidOperationException("Interval boundary has no bound limit point.");
        }
    }

    /// <summary>
    /// Gets the value of a bound limit point, or the <see langword="default"/> value of the underlying type <typeparamref name="T"/> when the boundary is unbounded.
    /// </summary>
    /// <returns>The value of a bound limit point, or the <see langword="default"/> value of the underlying type <typeparamref name="T"/> when the boundary is unbounded.</returns>
    public T GetValueOrDefault() => GetValueOrDefault(default!);

    /// <summary>
    /// Gets the value of a bound limit point, or the specified default value when the boundary is unbounded.
    /// </summary>
    /// <param name="defaultValue">A value to return if the boundary is infinite.</param>
    /// <returns>The value of a bound limit point, or the specified default value when the boundary is unbounded.</returns>
    public T GetValueOrDefault(T defaultValue) => HasValue ? m_Value : defaultValue;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly T m_Value;

    /// <summary>
    /// Gets a value indicating whether the current interval boundary has a bound limit point, e.g. is bounded.
    /// </summary>
    public bool HasValue => !(IsInfinity || Kind is IntervalBoundaryKind.Empty);

    /// <summary>
    /// Gets a value indicating whether the boundary represents either a negative or a positive infinity.
    /// </summary>
    public bool IsInfinity => Kind is IntervalBoundaryKind.NegativeInfinity or IntervalBoundaryKind.PositiveInfinity;

    /// <summary>
    /// Gets interval boundary kind.
    /// </summary>
    public IntervalBoundaryKind Kind { get; }

    /// <inheritdoc/>
    public override string ToString() => IntervalEngine.ToString(this, null, null);

    internal static IntervalBoundary<T> Empty { get; } = new(IntervalBoundaryKind.Empty, default!);

    internal static IntervalBoundary<T> NegativeInfinity { get; } = new(IntervalBoundaryKind.NegativeInfinity, default!);

    internal static IntervalBoundary<T> PositiveInfinity { get; } = new(IntervalBoundaryKind.PositiveInfinity, default!);

    string DebuggerDisplay =>
        Kind switch
        {
            IntervalBoundaryKind.Empty => "{Empty, ∅}",
            IntervalBoundaryKind.NegativeInfinity => "{Negative infinity, -∞}",
            IntervalBoundaryKind.PositiveInfinity => "{Positive infinity, +∞}",
            IntervalBoundaryKind.Exclusive => $"{{Exclusive, {Value}}}",
            IntervalBoundaryKind.Inclusive => $"{{Inclusive, {Value}}}"
        };

    /// <summary>
    /// Projects a value of type <typeparamref name="T"/> of the current boundary into a new boundary of the same kind but with a new value of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="selector">
    /// A transform function to apply to the boundary value.
    /// The function is invoked only when the boundary has a bound limit point, i.e. has a value.
    /// </param>
    /// <returns>An <see cref="IntervalBoundary{T}"/> whose value is the result of invoking the transform function on the current boundary value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is null.</exception>
    public IntervalBoundary<TResult> SelectValue<TResult>(Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return new(Kind, HasValue ? selector(m_Value) : default!);
    }

    #region Equality

    /// <summary>
    /// Determines whether the specified interval boundaries are equal.
    /// </summary>
    /// <param name="x">The first interval boundary.</param>
    /// <param name="y">The second interval boundary.</param>
    /// <returns>
    /// <see langword="true"/> if the specified interval boundaries are equal;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(IntervalBoundary<T> x, IntervalBoundary<T> y) => x.EqualsCore(y);

    /// <summary>
    /// Determines whether the specified interval boundaries are not equal.
    /// </summary>
    /// <param name="x">The first interval boundary.</param>
    /// <param name="y">The second interval boundary.</param>
    /// <returns>
    /// <see langword="true"/> if the specified interval boundaries are not equal;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(IntervalBoundary<T> x, IntervalBoundary<T> y) => !x.EqualsCore(y);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is IntervalBoundary<T> other &&
        Equals(other);

    /// <inheritdoc/>
    public bool Equals(IntervalBoundary<T> other) => EqualsCore(other);

    bool EqualsCore(in IntervalBoundary<T> other) => Equals(other, (IEqualityComparer<T>?)null);

    /// <summary>
    /// Determines whether the current interval boundary is equal to another.
    /// </summary>
    /// <param name="other">The interval boundary to compare with.</param>
    /// <param name="comparer">The comparer to use for comparing values of type <typeparamref name="T"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the current interval boundary is equal to the <paramref name="other"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(in IntervalBoundary<T> other, IEqualityComparer<T>? comparer) =>
        EqualsCore(other, new LazyEqualityComparer<T>(comparer));

    /// <inheritdoc cref="Equals(in IntervalBoundary{T}, IEqualityComparer{T}?)"/>
    public bool Equals(in IntervalBoundary<T> other, IComparer<T>? comparer) =>
        EqualsCore(other, new EqualityComparerFromComparer<T>(comparer));

    bool EqualsCore<TEqualityComparer>(in IntervalBoundary<T> other, TEqualityComparer comparer)
        where TEqualityComparer : IEqualityComparer<T>
    {
        if (Kind != other.Kind)
        {
            return false;
        }
        else if (HasValue)
        {
            if (other.HasValue)
                return comparer.Equals(m_Value, other.m_Value);
            else
                return false;
        }
        else
        {
            return !other.HasValue;
        }
    }

    /// <inheritdoc/>
    public override int GetHashCode() => GetHashCode(null);

    /// <inheritdoc cref="GetHashCode()"/>
    /// <param name="comparer">The comparer to use for hashing values of type <typeparamref name="T"/>.</param>
    public int GetHashCode(IEqualityComparer<T>? comparer)
    {
        var hc = new HashCode();
        hc.Add(Kind.GetHashCode());
        if (HasValue && m_Value is not null and var value)
            hc.Add(comparer != null ? comparer.GetHashCode(value) : value.GetHashCode());
        return hc.ToHashCode();
    }

    #endregion
}

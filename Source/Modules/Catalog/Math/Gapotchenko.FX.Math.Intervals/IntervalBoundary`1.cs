// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using System.Diagnostics;

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Represents an interval boundary.
/// </summary>
/// <typeparam name="T">The type of a value of the bound limit point.</typeparam>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct IntervalBoundary<T>
{
    internal IntervalBoundary(IntervalBoundaryKind kind, T value)
    {
        Kind = kind;
        m_Value = value;
    }

    /// <summary>
    /// Gets the value of a bound limit point.
    /// </summary>
    public T Value
    {
        get
        {
            if (!HasValue)
                throw new InvalidOperationException("Interval boundary has no bound limit point.");
            return m_Value;
        }
    }

    /// <summary>
    /// Gets the value of a bound limit point, or the default value of the underlying type <typeparamref name="T"/> when the boundary is unbounded.
    /// </summary>
    /// <returns>The value of a bound limit point, or the default value of the underlying type <typeparamref name="T"/> when the boundary is unbounded.</returns>
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
        if (selector == null)
            throw new ArgumentNullException(nameof(selector));

        return new(Kind, HasValue ? selector(m_Value) : default!);
    }
}

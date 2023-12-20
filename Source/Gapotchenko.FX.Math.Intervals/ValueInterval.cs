namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Provides static methods for creating value intervals.
/// </summary>
public static class ValueInterval
{
    /// <summary>
    /// Creates a new <see cref="ValueInterval{T}"/> instance with the specified inclusive left and exclusive right bounds.
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
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    public static ValueInterval<T> Create<T>(T from, T to) where T : IEquatable<T>, IComparable<T> =>
        new(from, to);

    /// <summary>
    /// Creates a new <see cref="ValueInterval{T}"/> instance with the specified boundaries.
    /// </summary>
    /// <param name="from">
    /// The left boundary of the interval.
    /// Represents a boundary the interval starts with.
    /// </param>
    /// <param name="to">
    /// The right boundary of the interval.
    /// Represents a boundary the interval ends with.
    /// </param>
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    public static ValueInterval<T> Create<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) where T : IEquatable<T>, IComparable<T> =>
        new(from, to);

    /// <summary>
    /// Creates a new <see cref="ValueInterval{T}"/> instance with the specified inclusive boundaries.
    /// </summary>
    /// <param name="from">
    /// The value of the left boundary.
    /// Left boundary is a boundary the interval starts with.
    /// If the specified value is <see langword="null"/> then the left boundary is set to the negative infinity.
    /// </param>
    /// <param name="to">
    /// The value of the right boundary.
    /// Right boundary is a boundary the interval ends with.
    /// If the specified value is <see langword="null"/> then the right boundary is set to the positive infinity.
    /// </param>
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    public static ValueInterval<T> Inclusive<T>(T? from, T? to) where T : IEquatable<T>, IComparable<T> =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Inclusive(from),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Inclusive(to));

    /// <summary>
    /// Creates a new <see cref="ValueInterval{T}"/> instance with the specified inclusive boundaries.
    /// </summary>
    /// <param name="from">
    /// The value of the left boundary.
    /// Left boundary is a boundary the interval starts with.
    /// If the specified value is <see langword="null"/> then the left boundary is set to the negative infinity.
    /// </param>
    /// <param name="to">
    /// The value of the right boundary.
    /// Right boundary is a boundary the interval ends with.
    /// If the specified value is <see langword="null"/> then the right boundary is set to the positive infinity.
    /// </param>
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ValueInterval<T> Inclusive<T>(T? from, T? to) where T : struct, IEquatable<T>, IComparable<T> =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Inclusive(from.Value),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Inclusive(to.Value));

    /// <summary>
    /// Creates a new <see cref="ValueInterval{T}"/> instance with the specified exclusive boundaries.
    /// </summary>
    /// <param name="from">
    /// The value of the left boundary.
    /// Left boundary is a boundary the interval starts with.
    /// If the specified value is <see langword="null"/> then the left boundary is set to the negative infinity.
    /// </param>
    /// <param name="to">
    /// The value of the right boundary.
    /// Right boundary is a boundary the interval ends with.
    /// If the specified value is <see langword="null"/> then the right boundary is set to the positive infinity.
    /// </param>
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    public static ValueInterval<T> Exclusive<T>(T? from, T? to) where T : IEquatable<T>, IComparable<T> =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Exclusive(from),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Exclusive(to));

    /// <summary>
    /// Creates a new <see cref="ValueInterval{T}"/> instance with the specified exclusive boundaries.
    /// </summary>
    /// <param name="from">
    /// The value of the left boundary.
    /// Left boundary is a boundary the interval starts with.
    /// If the specified value is <see langword="null"/> then the left boundary is set to the negative infinity.
    /// </param>
    /// <param name="to">
    /// The value of the right boundary.
    /// Right boundary is a boundary the interval ends with.
    /// If the specified value is <see langword="null"/> then the right boundary is set to the positive infinity.
    /// </param>
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ValueInterval<T> Exclusive<T>(T? from, T? to) where T : struct, IEquatable<T>, IComparable<T> =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Exclusive(from.Value),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Exclusive(to.Value));

    /// <summary>
    /// Creates a new <see cref="ValueInterval{T}"/> instance with the specified left inclusive and right exclusive boundaries.
    /// </summary>
    /// <param name="from">
    /// The value of the left boundary.
    /// Left boundary is a boundary the interval starts with.
    /// If the specified value is <see langword="null"/> then the left boundary is set to the negative infinity.
    /// </param>
    /// <param name="to">
    /// The value of the right boundary.
    /// Right boundary is a boundary the interval ends with.
    /// If the specified value is <see langword="null"/> then the right boundary is set to the positive infinity.
    /// </param>
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    public static ValueInterval<T> InclusiveExclusive<T>(T? from, T? to) where T : IEquatable<T>, IComparable<T> =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Inclusive(from),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Exclusive(to));

    /// <summary>
    /// Creates a new <see cref="ValueInterval{T}"/> instance with the specified left inclusive and right exclusive boundaries.
    /// </summary>
    /// <param name="from">
    /// The value of the left boundary.
    /// Left boundary is a boundary the interval starts with.
    /// If the specified value is <see langword="null"/> then the left boundary is set to the negative infinity.
    /// </param>
    /// <param name="to">
    /// The value of the right boundary.
    /// Right boundary is a boundary the interval ends with.
    /// If the specified value is <see langword="null"/> then the right boundary is set to the positive infinity.
    /// </param>
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ValueInterval<T> InclusiveExclusive<T>(T? from, T? to) where T : struct, IEquatable<T>, IComparable<T> =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Inclusive(from.Value),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Exclusive(to.Value));

    /// <summary>
    /// Creates a new <see cref="ValueInterval{T}"/> instance with the specified left exclusive and right inclusive boundaries.
    /// </summary>
    /// <param name="from">
    /// The value of the left boundary.
    /// Left boundary is a boundary the interval starts with.
    /// If the specified value is <see langword="null"/> then the left boundary is set to the negative infinity.
    /// </param>
    /// <param name="to">
    /// The value of the right boundary.
    /// Right boundary is a boundary the interval ends with.
    /// If the specified value is <see langword="null"/> then the right boundary is set to the positive infinity.
    /// </param>
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    public static ValueInterval<T> ExclusiveInclusive<T>(T? from, T? to) where T : IEquatable<T>, IComparable<T> =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Exclusive(from),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Inclusive(to));

    /// <summary>
    /// Creates a new <see cref="ValueInterval{T}"/> instance with the specified left exclusive and right inclusive boundaries.
    /// </summary>
    /// <param name="from">
    /// The value of the left boundary.
    /// Left boundary is a boundary the interval starts with.
    /// If the specified value is <see langword="null"/> then the left boundary is set to the negative infinity.
    /// </param>
    /// <param name="to">
    /// The value of the right boundary.
    /// Right boundary is a boundary the interval ends with.
    /// If the specified value is <see langword="null"/> then the right boundary is set to the positive infinity.
    /// </param>
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ValueInterval<T> ExclusiveInclusive<T>(T? from, T? to) where T : struct, IEquatable<T>, IComparable<T> =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Exclusive(from.Value),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Inclusive(to.Value));

    /// <summary>
    /// <para>
    /// Creates a new degenerate <see cref="ValueInterval{T}"/> instance with the specified value of its inclusive boundaries.
    /// </para>
    /// <para>
    /// A degenerate interval <c>[x,x]</c> represents a set of exactly one element <c>{x}</c>.
    /// </para>
    /// </summary>
    /// <param name="value">The value of the inclusive boundaries.</param>
    /// <returns>The new <see cref="ValueInterval{T}"/> instance.</returns>
    public static ValueInterval<T> Degenerate<T>(T value) where T : IEquatable<T>, IComparable<T> =>
        new(
            IntervalBoundary.Inclusive(value),
            IntervalBoundary.Inclusive(value));
}

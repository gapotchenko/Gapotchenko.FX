﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Provides static methods for creating intervals.
/// </summary>
public static class Interval
{
    /// <summary>
    /// Returns an empty <see cref="Interval{T}"/>:
    /// <c>∅</c>.
    /// </summary>
    public static Interval<T> Empty<T>() =>
#pragma warning disable CS0618 // Type or member is obsolete
        Interval<T>.Empty;
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified inclusive left and exclusive right bounds:
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
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> Create<T>(T from, T to) => new(from, to);

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified boundaries.
    /// </summary>
    /// <param name="from">
    /// The left boundary of the interval.
    /// Represents a boundary the interval starts with.
    /// </param>
    /// <param name="to">
    /// The right boundary of the interval.
    /// Represents a boundary the interval ends with.
    /// </param>
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> Create<T>(IntervalBoundary<T> from, IntervalBoundary<T> to) => new(from, to);

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified inclusive boundaries:
    /// <c>[from,to]</c>.
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
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> Inclusive<T>(T? from, T? to) =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Inclusive(from),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Inclusive(to));

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified inclusive boundaries:
    /// <c>[from,to]</c>.
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
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Interval<T> Inclusive<T>(T? from, T? to) where T : struct =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Inclusive(from.Value),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Inclusive(to.Value));

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified exclusive boundaries:
    /// <c>(from,to)</c>.
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
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> Exclusive<T>(T? from, T? to) =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Exclusive(from),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Exclusive(to));

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified exclusive boundaries:
    /// <c>(from,to)</c>.
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
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Interval<T> Exclusive<T>(T? from, T? to) where T : struct =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Exclusive(from.Value),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Exclusive(to.Value));

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified left inclusive and right exclusive boundaries:
    /// <c>[from,to)</c>.
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
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> InclusiveExclusive<T>(T? from, T? to) =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Inclusive(from),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Exclusive(to));

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified left inclusive and right exclusive boundaries:
    /// <c>[from,to)</c>.
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
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Interval<T> InclusiveExclusive<T>(T? from, T? to)
        where T : struct
        => new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Inclusive(from.Value),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Exclusive(to.Value));

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified left exclusive and right inclusive boundaries:
    /// <c>(from,to]</c>.
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
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> ExclusiveInclusive<T>(T? from, T? to) =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Exclusive(from),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Inclusive(to));

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified left exclusive and right inclusive boundaries.
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
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Interval<T> ExclusiveInclusive<T>(T? from, T? to) where T : struct =>
        new(
            from is null ? IntervalBoundary<T>.NegativeInfinity : IntervalBoundary.Exclusive(from.Value),
            to is null ? IntervalBoundary<T>.PositiveInfinity : IntervalBoundary.Inclusive(to.Value));

    /// <summary>
    /// <para>
    /// Creates a new degenerate <see cref="Interval{T}"/> instance with the specified value of its inclusive bounds.
    /// </para>
    /// <para>
    /// A degenerate interval <c>[x,x]</c> represents a set of exactly one element <c>{x}</c>.
    /// </para>
    /// </summary>
    /// <param name="value">The value of the inclusive bounds.</param>
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> Degenerate<T>(T value) =>
        new(
            IntervalBoundary.Inclusive(value),
            IntervalBoundary.Inclusive(value));

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified left inclusive bound and positive infinity as the right boundary:
    /// <c>[value,∞)</c>.
    /// </summary>
    /// <param name="value">
    /// The left bound of the interval.
    /// Represents a value the interval starts with.
    /// The corresponding limit point is included in the interval.
    /// </param>
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> FromInclusive<T>(T value) =>
        new(
            IntervalBoundary.Inclusive(value),
            IntervalBoundary<T>.PositiveInfinity);

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with the specified left exclusive bound and positive infinity as the right boundary:
    /// <c>(value,∞)</c>.
    /// </summary>
    /// <param name="value">
    /// The left bound of the interval.
    /// Represents a value the interval starts with.
    /// The corresponding limit point is not included in the interval.
    /// </param>
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> FromExclusive<T>(T value) =>
        new(
            IntervalBoundary.Exclusive(value),
            IntervalBoundary<T>.PositiveInfinity);

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with negative infinity as the left boundary and the specified right inclusive bound:
    /// <c>(-∞,value]</c>.
    /// </summary>
    /// <param name="value">
    /// The right bound of the interval.
    /// Represents a value the interval ends with.
    /// The corresponding limit point is included in the interval.
    /// </param>
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> ToInclusive<T>(T value) =>
        new(
            IntervalBoundary<T>.NegativeInfinity,
            IntervalBoundary.Inclusive(value));

    /// <summary>
    /// Creates a new <see cref="Interval{T}"/> instance with negative infinity as the left boundary and the specified right inclusive bound:
    /// <c>(-∞,value)</c>.
    /// </summary>
    /// <param name="value">
    /// The right bound of the interval.
    /// Represents a value the interval ends with.
    /// The corresponding limit point is not included in the interval.
    /// </param>
    /// <returns>The new <see cref="Interval{T}"/> instance.</returns>
    public static Interval<T> ToExclusive<T>(T value) =>
        new(
            IntervalBoundary<T>.NegativeInfinity,
            IntervalBoundary.Exclusive(value));

    /// <summary>
    /// Returns an infinite <see cref="Interval{T}"/>:
    /// <c>(-∞,∞)</c>.
    /// </summary>
    public static Interval<T> Infinite<T>() => Interval<T>.Infinite;
}

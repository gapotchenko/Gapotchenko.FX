// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Defines the interval operations for <see cref="IInterval"/>.
/// This interface is not intended to be used directly, use <see cref="IInterval"/> instead.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IIntervalOperations : IIntervalModel, IEmptiable
{
    /// <summary>
    /// <para>
    /// Gets a value indicating whether the interval is bounded.
    /// </para>
    /// <para>
    /// A bounded interval is both left- and right-bounded.
    /// </para>
    /// </summary>
    bool IsBounded { get; }

    /// <summary>
    /// <para>
    /// Gets a value indicating whether the interval is half-bounded.
    /// </para>
    /// <para>
    /// A half-bounded interval is either left- or right-bounded.
    /// </para>
    /// </summary>
    bool IsHalfBounded { get; }

    /// <summary>
    /// <para>
    /// Gets a value indicating whether the interval is open.
    /// </para>
    /// <para>
    /// An open interval does not include its endpoints.
    /// </para>
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// <para>
    /// Gets a value indicating whether the interval is closed.
    /// </para>
    /// <para>
    /// A closed interval includes all its limit points.
    /// </para>
    /// </summary>
    bool IsClosed { get; }

    /// <summary>
    /// <para>
    /// Gets a value indicating whether the interval is half-open.
    /// </para>
    /// <para>
    /// A half-open interval includes only one of its endpoints.
    /// </para>
    /// </summary>
    bool IsHalfOpen { get; }

    /// <summary>
    /// Gets a value indicating whether the interval is infinite.
    /// </summary>
    bool IsInfinite { get; }

    /// <summary>
    /// <para>
    /// Gets a value indicating whether the interval is a degenerate.
    /// </para>
    /// <para>
    /// A degenerate interval <c>[x,x]</c> represents a set of exactly one element <c>{x}</c>.
    /// </para>
    /// </summary>
    bool IsDegenerate { get; }

    /// <summary>
    /// Determines whether this and the specified intervals are equal.
    /// </summary>
    /// <param name="other">The interval to check for equality.</param>
    /// <returns><see langword="true"/> if this and <paramref name="other"/> intervals are equal; otherwise, <see langword="false"/>.</returns>
    bool IntervalEquals([NotNullWhen(true)] IInterval? other);
}

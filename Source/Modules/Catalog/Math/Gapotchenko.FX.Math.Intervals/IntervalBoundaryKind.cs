// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Specifies the kinds of interval boundaries.
/// </summary>
public enum IntervalBoundaryKind : byte
{
    /// <summary>
    /// Empty boundary, <c>∅</c>.
    /// </summary>
    Empty,

    /// <summary>
    /// Negative infinity, <c>-∞</c>.
    /// </summary>
    NegativeInfinity,

    /// <summary>
    /// <para>
    /// Represents a finite boundary whose bound value is included in the interval.
    /// </para>
    /// <para>
    /// An inclusive boundary is also called a closed boundary.
    /// </para>
    /// </summary>
    Inclusive,

    /// <summary>
    /// <para>
    /// Represents a finite boundary whose bound value is not included in the interval.
    /// </para>
    /// <para>
    /// An exclusive boundary is also called an open boundary.
    /// </para>
    /// </summary>
    Exclusive,

    /// <summary>
    /// Positive infinity, <c>+∞</c>.
    /// </summary>
    PositiveInfinity
}

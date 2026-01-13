// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Defines the interface of an untyped interval boundary.
/// </summary>
public interface IIntervalBoundary
{
    /// <summary>
    /// Gets interval boundary kind.
    /// </summary>
    IntervalBoundaryKind Kind { get; }

    /// <summary>
    /// Gets the value of a bound limit point.
    /// </summary>
    /// <exception cref="InvalidOperationException">Interval boundary has no bound limit point.</exception>
    object? Value { get; }

    /// <summary>
    /// Gets a value indicating whether the current interval boundary has a bound limit point, e.g. is bounded.
    /// </summary>
    bool HasValue { get; }

    /// <summary>
    /// Gets a value indicating whether the boundary represents either a negative or a positive infinity.
    /// </summary>
    bool IsInfinity { get; }
}

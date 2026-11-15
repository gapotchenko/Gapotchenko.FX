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
    /// Gets the kind of the interval boundary.
    /// </summary>
    IntervalBoundaryKind Kind { get; }

    /// <summary>
    /// Gets the value of the finite bound represented by the current boundary.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The current boundary does not represent a finite bound.
    /// </exception>
    object? Value { get; }

    /// <summary>
    /// Gets a value indicating whether the current boundary represents a finite bound.
    /// </summary>
    bool HasValue { get; }

    /// <summary>
    /// Gets a value indicating whether the current boundary represents
    /// negative or positive infinity.
    /// </summary>
    bool IsInfinity { get; }
}

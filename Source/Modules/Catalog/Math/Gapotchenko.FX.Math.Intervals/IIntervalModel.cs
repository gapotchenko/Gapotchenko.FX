// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Defines the object model of an interval.
/// This interface is not intended to be used directly, use <see cref="IInterval{T}"/> instead.
/// </summary>
/// <typeparam name="T">The type of interval values.</typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IIntervalModel<T>
{
    /// <summary>
    /// Gets the left boundary of the interval.
    /// Represents a boundary the interval starts with.
    /// </summary>
    IntervalBoundary<T> From { get; }

    /// <summary>
    /// Gets the right boundary of the interval.
    /// Represents a boundary the interval ends with.
    /// </summary>
    IntervalBoundary<T> To { get; }
}

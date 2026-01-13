// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Defines the object model of an untyped interval.
/// This interface is not intended to be used directly, use <see cref="IInterval{T}"/> instead.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IIntervalModel
{
    /// <summary>
    /// Gets the left boundary of the interval.
    /// Represents a boundary the interval starts with.
    /// </summary>
    IIntervalBoundary From { get; }

    /// <summary>
    /// Gets the right boundary of the interval.
    /// Represents a boundary the interval ends with.
    /// </summary>
    IIntervalBoundary To { get; }
}

/// <summary>
/// Defines the object model of an interval.
/// This interface is not intended to be used directly, use <see cref="IInterval{T}"/> instead.
/// </summary>
/// <typeparam name="T">The type of interval values.</typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IIntervalModel<T> : IIntervalModel
{
    /// <inheritdoc cref="IIntervalModel.From"/>
    new IntervalBoundary<T> From { get; }

    /// <inheritdoc cref="IIntervalModel.To"/>
    new IntervalBoundary<T> To { get; }
}

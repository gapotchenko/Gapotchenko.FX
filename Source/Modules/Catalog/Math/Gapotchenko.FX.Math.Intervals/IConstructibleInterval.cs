// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Provides the abstraction interface for a constructable continuous interval.
/// </summary>
/// <typeparam name="TValue">The type of interval value.</typeparam>
/// <typeparam name="TInterval">The type of interval that can be constructed.</typeparam>
public interface IConstructibleInterval<TValue, out TInterval> : IInterval<TValue>, IEmptiable<TInterval>
    where TInterval : IConstructibleInterval<TValue, TInterval>
{
#if TFF_STATIC_INTERFACE
    /// <summary>
    /// Gets the infinite <typeparamref name="TInterval"/> instance.
    /// </summary>
    static abstract TInterval Infinite { get; }

    /// <summary>
    /// Creates a new <typeparamref name="TInterval"/> instance with the specified boundaries.
    /// </summary>
    /// <param name="from">
    /// The left boundary of the interval.
    /// Represents a boundary the interval starts with.
    /// </param>
    /// <param name="to">
    /// The right boundary of the interval.
    /// Represents a boundary the interval ends with.
    /// </param>
    /// <param name="comparer">
    /// The <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
    /// or <see langword="null"/> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="TValue"/>.
    /// </param>
    /// <returns>The new <typeparamref name="TInterval"/> instance.</returns>
    /// <exception cref="ArgumentException">If one interval boundary is empty, another should be empty too.</exception>
    /// <exception cref="ArgumentException">The specified <paramref name="comparer"/> is not compatible with the comparer used by <typeparamref name="TInterval"/>.</exception>
    static abstract TInterval Create(IntervalBoundary<TValue> from, IntervalBoundary<TValue> to, IComparer<TValue>? comparer = null);
#endif
}

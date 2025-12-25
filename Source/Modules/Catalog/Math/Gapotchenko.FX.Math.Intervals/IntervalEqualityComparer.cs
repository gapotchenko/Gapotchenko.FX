// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Equality comparer for contiguous intervals of values represented by <see cref="IInterval{T}"/> interface.
/// </summary>
public static class IntervalEqualityComparer
{
    /// <summary>
    /// Creates an equality comparer for contiguous intervals of values
    /// with the specified equality comparer for interval limit points.
    /// </summary>
    /// <param name="valueComparer">
    /// The <see cref="IEqualityComparer{T}"/> to compare interval limit point values.
    /// </param>
    /// <typeparam name="T">The type of interval limit point values.</typeparam>
    /// <returns>The equality comparer for contiguous intervals of values of type <typeparamref name="T"/>.</returns>
    public static IntervalEqualityComparer<T> Create<T>(IEqualityComparer<T>? valueComparer)
    {
        valueComparer = Empty.Nullify(valueComparer);
        if (valueComparer is null)
            return Default<T>();
        else
            return new IntervalEqualityComparer<T>.GenericComparer(valueComparer);
    }

    /// <summary>
    /// Gets the default equality comparer for contiguous intervals of values
    /// that uses the default equality comparer for interval limit points.
    /// </summary>
    /// <returns>The default equality comparer for contiguous intervals of values of type <typeparamref name="T"/>.</returns>
    public static IntervalEqualityComparer<T> Default<T>() => IntervalEqualityComparer<T>.DefaultComparer.Instance;
}

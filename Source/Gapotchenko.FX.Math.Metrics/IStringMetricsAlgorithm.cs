// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Math.Metrics;

/// <summary>
/// Defines the interface of a string metrics algorithm.
/// </summary>
public interface IStringMetricsAlgorithm
{
    /// <summary>
    /// Calculates a measure between the two specified sequences of elements.
    /// </summary>
    /// <typeparam name="T">The type of string elements.</typeparam>
    /// <param name="a">The first sequence of elements.</param>
    /// <param name="b">The second sequence of elements.</param>
    /// <param name="equalityComparer">The equality comparer for elements.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The calculated measure between the specified sequences of elements.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="a"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="b"/> is <see langword="null"/>.</exception>
    object Measure<T>(
        IEnumerable<T> a,
        IEnumerable<T> b,
        IEqualityComparer<T>? equalityComparer = null,
        CancellationToken cancellationToken = default);
}

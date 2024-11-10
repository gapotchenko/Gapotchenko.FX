// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Math.Intervals;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Metrics;

/// <summary>
/// The base class for <see cref="IStringMetricAlgorithm{TMeasure}"/> implementations.
/// </summary>
/// <typeparam name="TMeasure"><inheritdoc cref="IStringMetricAlgorithm{TMeasure}"/></typeparam>
public abstract class StringMetricAlgorithm<TMeasure> :
    IStringMetricAlgorithm<TMeasure>
    where TMeasure :
        notnull,
        IEquatable<TMeasure>?,
        IComparable<TMeasure>?
{
    /// <inheritdoc/>
    public abstract StringMetricTraits MetricTraits { get; }

    /// <inheritdoc/>
    public TMeasure Calculate<TElement>(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        IEqualityComparer<TElement>? equalityComparer = null,
        CancellationToken cancellationToken = default) =>
        Calculate(a, b, ValueInterval.Infinite<TMeasure>(), equalityComparer, cancellationToken);

    object IStringMetricAlgorithm.Calculate<TElement>(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        IEqualityComparer<TElement>? equalityComparer,
        CancellationToken cancellationToken) =>
        Calculate(a, b, equalityComparer, cancellationToken);

    /// <inheritdoc/>
    public abstract TMeasure Calculate<TElement>(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        ValueInterval<TMeasure> range,
        IEqualityComparer<TElement>? equalityComparer = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the specified arguments.
    /// </summary>
    /// <typeparam name="TElement">The type of sequence elements.</typeparam>
    /// <param name="a">The first sequence of elements.</param>
    /// <param name="b">The second sequence of elements.</param>
    /// <param name="range">The range of measures.</param>
    /// <exception cref="ArgumentNullException"><paramref name="a"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="b"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="range"/> cannot be empty.</exception>
    [StackTraceHidden]
    protected static void ValidateArguments<TElement>(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        in ValueInterval<TMeasure> range)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));
        if (b == null)
            throw new ArgumentNullException(nameof(b));
        if (range.IsEmpty)
            throw new ArgumentOutOfRangeException(nameof(range), "The range of measures cannot be empty.");
    }
}

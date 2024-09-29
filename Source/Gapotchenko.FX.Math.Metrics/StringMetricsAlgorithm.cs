// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics;

/// <summary>
/// The base class for <see cref="IStringMetricsAlgorithm{TElement, TMeasure}"/> implementations.
/// </summary>
/// <typeparam name="TElement">The type of string elements.</typeparam>
/// <typeparam name="TMeasure">The type of the measure.</typeparam>
public abstract class StringMetricsAlgorithm<TElement, TMeasure> : IStringMetricsAlgorithm<TElement, TMeasure>
    where TMeasure : notnull, IEquatable<TMeasure>?, IComparable<TMeasure>?
{
    /// <inheritdoc/>
    public TMeasure Measure(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        IEqualityComparer<TElement>? equalityComparer = null,
        CancellationToken cancellationToken = default) =>
        Measure(a, b, ValueInterval<TMeasure>.Infinite, equalityComparer, cancellationToken);

    /// <inheritdoc/>
    public abstract TMeasure Measure(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        ValueInterval<TMeasure> range,
        IEqualityComparer<TElement>? equalityComparer = null,
        CancellationToken cancellationToken = default);

    object IStringMetricsAlgorithm<TElement>.Measure(IEnumerable<TElement> a, IEnumerable<TElement> b, IEqualityComparer<TElement>? equalityComparer, CancellationToken cancellationToken) =>
        Measure(a, b, equalityComparer, cancellationToken);
}

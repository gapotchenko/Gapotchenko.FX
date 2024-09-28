// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Math.Metrics;

/// <summary>
/// Defines the interface of a strongly-typed string metrics algorithm.
/// </summary>
/// <typeparam name="TElement"><inheritdoc/></typeparam>
/// <typeparam name="TMeasure">The type of the measure.</typeparam>
public interface IStringMetricsAlgorithm<TElement, TMeasure> : IStringMetricsAlgorithm<TElement>
{
    /// <inheritdoc cref="IStringMetricsAlgorithm{TElement}.Measure(IEnumerable{TElement}, IEnumerable{TElement}, IEqualityComparer{TElement}?, CancellationToken)"/>
    new object Measure(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        IEqualityComparer<TElement>? equalityComparer = null,
        CancellationToken cancellationToken = default);
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics;

/// <summary>
/// Defines the interface of a strongly-typed string metrics algorithm.
/// </summary>
/// <typeparam name="TElement"><inheritdoc/></typeparam>
/// <typeparam name="TMeasure">The type of the measure.</typeparam>
public interface IStringMetricsAlgorithm<TElement, TMeasure> : IStringMetricsAlgorithm<TElement>
    where TMeasure : notnull, IEquatable<TMeasure>?, IComparable<TMeasure>?
{
    /// <inheritdoc cref="IStringMetricsAlgorithm{TElement}.Measure(IEnumerable{TElement}, IEnumerable{TElement}, IEqualityComparer{TElement}?, CancellationToken)"/>
    new TMeasure Measure(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        IEqualityComparer<TElement>? equalityComparer = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="Measure(IEnumerable{TElement}, IEnumerable{TElement}, IEqualityComparer{TElement}?, CancellationToken)"/>
    /// <param name="a"><inheritdoc/></param>
    /// <param name="b"><inheritdoc/></param>
    /// <param name="range">
    /// Specifies the range in which a calculated measure must reside.
    /// The algorithm uses the range to optimize calculation for narrower ranges.
    /// If the specified range is empty, then the <see langword="default"/> <typeparamref name="TMeasure"/> value is returned.
    /// </param>
    /// <param name="equalityComparer"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    TMeasure Measure(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        ValueInterval<TMeasure> range,
        IEqualityComparer<TElement>? equalityComparer = null,
        CancellationToken cancellationToken = default);
}

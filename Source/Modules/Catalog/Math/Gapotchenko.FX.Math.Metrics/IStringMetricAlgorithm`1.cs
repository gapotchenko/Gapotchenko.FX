﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics;

/// <summary>
/// Defines the interface of a strongly-typed string metric algorithm.
/// </summary>
/// <typeparam name="TMeasure">The type of the measure.</typeparam>
public interface IStringMetricAlgorithm<TMeasure> :
    IStringMetricAlgorithm
    where TMeasure :
        notnull,
        IEquatable<TMeasure>?,
        IComparable<TMeasure>?
{
    /// <inheritdoc cref="IStringMetricAlgorithm.Calculate{TElement}(IEnumerable{TElement}, IEnumerable{TElement}, IEqualityComparer{TElement}?, CancellationToken)"/>
    /// <typeparam name="TElement">The type of sequence elements.</typeparam>
    new TMeasure Calculate<TElement>(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        IEqualityComparer<TElement>? equalityComparer = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="Calculate{TElement}(IEnumerable{TElement}, IEnumerable{TElement}, IEqualityComparer{TElement}?, CancellationToken)"/>
    /// <param name="a"><inheritdoc/></param>
    /// <param name="b"><inheritdoc/></param>
    /// <param name="range">
    /// Specifies the range in which a calculated measure must reside.
    /// The algorithm may use the range of measures to optimize the calculation.
    /// </param>
    /// <param name="equalityComparer"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="range"/> cannot be empty.</exception>
    TMeasure Calculate<TElement>(
        IEnumerable<TElement> a,
        IEnumerable<TElement> b,
        ValueInterval<TMeasure> range,
        IEqualityComparer<TElement>? equalityComparer = null,
        CancellationToken cancellationToken = default);
}

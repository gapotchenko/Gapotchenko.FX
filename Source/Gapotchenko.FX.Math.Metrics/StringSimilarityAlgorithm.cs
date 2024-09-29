// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Math.Metrics;

/// <summary>
/// The base class for <see cref="IStringSimilarityAlgorithm{TElement}"/> implementations.
/// </summary>
/// <typeparam name="TElement">The type of string elements.</typeparam>
public abstract class StringSimilarityAlgorithm<TElement> :
    StringMetricsAlgorithm<TElement, double>,
    IStringSimilarityAlgorithm<TElement>
{
}

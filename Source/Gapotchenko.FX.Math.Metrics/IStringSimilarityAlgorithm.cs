// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Math.Metrics;

/// <summary>
/// Defines the interface of a string similarity algorithm.
/// </summary>
/// <typeparam name="TElement">The type of string elements.</typeparam>
public interface IStringSimilarityAlgorithm<TElement> : IStringMetricsAlgorithm<TElement, double>
{
}

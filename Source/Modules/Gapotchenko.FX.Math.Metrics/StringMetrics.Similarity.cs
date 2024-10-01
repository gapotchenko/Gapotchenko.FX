// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Math.Metrics.StringSimilarityAlgorithms;

namespace Gapotchenko.FX.Math.Metrics;

partial class StringMetrics
{
    /// <summary>
    /// Provides string similarity algorithms.
    /// </summary>
    public static class Similarity
    {
        /// <summary>
        /// Gets Jaro string similarity algorithm.
        /// </summary>
        public static StringSimilarityAlgorithm Jaro => JaroAlgorithm.Instance;
    }
}

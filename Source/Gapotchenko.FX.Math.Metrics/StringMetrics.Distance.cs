// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Math.Metrics.StringDistanceAlgorithms;

namespace Gapotchenko.FX.Math.Metrics;

partial class StringMetrics
{
    /// <summary>
    /// Provides string distance algorithms.
    /// </summary>
    public static class Distance
    {
        /// <summary>
        /// Gets Damerau–Levenshtein string distance algorithm.
        /// </summary>
        public static StringDistanceAlgorithm DamerauLevenshtein => DamerauLevenshteinAlgorithm.Instance;

        /// <summary>
        /// Gets Hamming string distance algorithm.
        /// </summary>
        /// <remarks>
        /// Hamming distance algorithm can be applied to sequences of the same length only.
        /// </remarks>
        public static StringDistanceAlgorithm Hamming => HammingAlgorithm.Instance;

        /// <summary>
        /// Gets Longest Common Subsequence (LCS) string distance algorithm.
        /// </summary>
        public static StringDistanceAlgorithm Lcs => LcsAlgorithm.Instance;

        /// <summary>
        /// Gets Levenshtein string distance algorithm.
        /// </summary>
        public static StringDistanceAlgorithm Levenshtein => LevenshteinAlgorithm.Instance;

        /// <summary>
        /// Gets Optimal String Alignment (OSA) distance algorithm.
        /// </summary>
        public static StringDistanceAlgorithm Osa => OsaAlgorithm.Instance;
    }
}

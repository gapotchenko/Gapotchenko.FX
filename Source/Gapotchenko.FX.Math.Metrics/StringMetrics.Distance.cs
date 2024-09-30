using Gapotchenko.FX.Math.Metrics.StringDistanceAlgorithms;

namespace Gapotchenko.FX.Math.Metrics;

partial class StringMetrics
{
    /// <summary>
    /// Provides string distance measuring algorithms.
    /// </summary>
    public static class Distance
    {
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

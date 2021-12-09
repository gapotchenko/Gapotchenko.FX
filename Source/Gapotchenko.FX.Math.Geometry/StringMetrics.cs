using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Geometry
{
    /// <summary>
    /// String metrics functions.
    /// </summary>
    public static class StringMetrics
    {
        /// <summary>
        /// Calculates Levenshtein distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <returns>The Levenshtein distance.</returns>
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b) =>
            LevenshteinDistance(a, b, null, null);

        /// <summary>
        /// Calculates Levenshtein distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <returns>The Levenshtein distance.</returns>
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance) =>
            LevenshteinDistance(a, b, maxDistance, null);

        /// <summary>
        /// Calculates Levenshtein distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Levenshtein distance.</returns>
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? equalityComparer) =>
            LevenshteinDistance(a, b, null, equalityComparer);

        /// <summary>
        /// Calculates Levenshtein distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Levenshtein distance.</returns>
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance, IEqualityComparer<T>? equalityComparer)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));
            if (maxDistance < 0)
                throw new ArgumentOutOfRangeException(nameof(maxDistance));

            if (maxDistance == 0)
                return 0;
            if (ReferenceEquals(a, b))
                return 0;

            var sRow = b.AsReadOnly();
            var sCol = a.AsReadOnly();

            int rowLen = sRow.Count;
            int colLen = sCol.Count;

            if (rowLen == 0)
                return colLen;
            if (colLen == 0)
                return rowLen;

            equalityComparer ??= EqualityComparer<T>.Default;

            // Although the algorithm is typically described using an colLen x rowLen
            // array, only one row plus one element are used at a time, so this
            // implementation just keeps one vector for the row.
            // To update one entry, only the entries to the left, top, and top-left
            // are needed. The left entry is in row[rowIdx - 1], the top entry is what's in
            // row[rowIdx] from the last iteration, and the top-left entry is stored
            // in topLeftDistance.

            // Create and initialize the row vector.
            var row = new int[rowLen + 1];
            for (int rowIdx = 1; rowIdx <= rowLen; rowIdx++)
                row[rowIdx] = rowIdx;

            // For each column.
            for (int colIdx = 1; colIdx <= colLen; colIdx++)
            {
                // Set the first element to the column number.
                row[0] = colIdx;

                var col_j = sCol[colIdx - 1];
                int bestAtRow = colIdx;
                int topLeftDistance = colIdx - 1;

                // For each row.
                for (int rowIdx = 1; rowIdx <= rowLen; rowIdx++)
                {
                    var row_i = sRow[rowIdx - 1];
                    var sameValue = equalityComparer.Equals(row_i, col_j);

                    int topDistance = row[rowIdx];
                    int leftDistance = row[rowIdx - 1];

                    var replacementCost = sameValue ? 0 : 1;

                    // Find minimum.
                    var currentDistance = MathEx.Min(
                        topDistance + 1,
                        leftDistance + 1,
                        topLeftDistance + replacementCost);

                    row[rowIdx] = currentDistance;

                    bestAtRow = System.Math.Min(bestAtRow, currentDistance);

                    topLeftDistance = topDistance;
                }

                if (bestAtRow >= maxDistance)
                    return maxDistance.Value;
            }

            return row[rowLen];
        }
    }
}

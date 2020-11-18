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
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b) => LevenshteinDistance(a, b, null);

        /// <summary>
        /// Calculates Levenshtein distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Levenshtein distance.</returns>
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? equalityComparer)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

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

            if (equalityComparer == null)
                equalityComparer = EqualityComparer<T>.Default;

            // Create the two vectors.
            var v0 = new int[rowLen + 1];
            var v1 = new int[rowLen + 1];

            // Initialize the first vector.
            for (int rowIdx = 1; rowIdx <= rowLen; rowIdx++)
                v0[rowIdx] = rowIdx;

            // For each column.
            for (int colIdx = 1; colIdx <= colLen; colIdx++)
            {
                // Set the 0'th element to the column number.
                v1[0] = colIdx;

                var col_j = sCol[colIdx - 1];

                // For each row.
                for (int rowIdx = 1; rowIdx <= rowLen; rowIdx++)
                {
                    var row_i = sRow[rowIdx - 1];

                    int cost;
                    if (equalityComparer.Equals(row_i, col_j))
                        cost = 0;
                    else
                        cost = 1;

                    // Find minimum.
                    v1[rowIdx] = MathEx.Min(
                        v0[rowIdx] + 1,
                        v1[rowIdx - 1] + 1,
                        v0[rowIdx - 1] + cost);
                }

                // Swap the vectors.
                MathEx.Swap(ref v0, ref v1);
            }

            return v0[rowLen];
        }
    }
}

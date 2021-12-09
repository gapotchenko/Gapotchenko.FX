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
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance, IEqualityComparer<T>? equalityComparer) =>
            InsertDeleteReplaceDistance(a, b, maxDistance, allowReplacements: true, equalityComparer);

        /// <summary>
        /// Calculates longest common subsequence distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <returns>The longest common subsequence distance.</returns>
        public static int LongestCommonSubsequenceDistance<T>(IEnumerable<T> a, IEnumerable<T> b) =>
            LongestCommonSubsequenceDistance(a, b, null, null);

        /// <summary>
        /// Calculates longest common subsequence distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <returns>The longest common subsequence distance.</returns>
        public static int LongestCommonSubsequenceDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance) =>
            LongestCommonSubsequenceDistance(a, b, maxDistance, null);

        /// <summary>
        /// Calculates longest common subsequence distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The longest common subsequence distance.</returns>
        public static int LongestCommonSubsequenceDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? equalityComparer) =>
            LongestCommonSubsequenceDistance(a, b, null, equalityComparer);

        /// <summary>
        /// Calculates longest common subsequence distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The longest common subsequence distance.</returns>
        public static int LongestCommonSubsequenceDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance, IEqualityComparer<T>? equalityComparer) =>
            InsertDeleteReplaceDistance(a, b, maxDistance, allowReplacements: false, equalityComparer);

        static int InsertDeleteReplaceDistance<T>(
            IEnumerable<T> a,
            IEnumerable<T> b,
            int? maxDistance,
            bool allowReplacements,
            IEqualityComparer<T>? equalityComparer)
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

                    int currentDistance;
                    int topDistance = row[rowIdx];
                    int leftDistance = row[rowIdx - 1];

                    // Find minimum.
                    if (allowReplacements)
                    {
                        var replacementCost = sameValue ? 0 : 1;

                        currentDistance = MathEx.Min(
                            topDistance + 1,
                            leftDistance + 1,
                            topLeftDistance + replacementCost);
                    }
                    else
                    {
                        if (sameValue)
                            currentDistance = topLeftDistance;
                        else
                            currentDistance = System.Math.Min(leftDistance, topDistance) + 1;
                    }

                    row[rowIdx] = currentDistance;

                    bestAtRow = System.Math.Min(bestAtRow, currentDistance);

                    topLeftDistance = topDistance;
                }

                if (bestAtRow >= maxDistance)
                    return maxDistance.Value;
            }

            var distance = row[rowLen];
            if (distance >= maxDistance)
                return maxDistance.Value;
            return distance;
        }

        /// <summary>
        /// Calculates Hamming distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <returns>The Hamming distance.</returns>
        public static int HammingDistance<T>(IEnumerable<T> a, IEnumerable<T> b) =>
            HammingDistance(a, b, null, null);

        /// <summary>
        /// Calculates Hamming distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <returns>The Hamming distance.</returns>
        public static int HammingDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance) =>
            HammingDistance(a, b, maxDistance, null);

        /// <summary>
        /// Calculates Hamming distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Hamming distance.</returns>
        public static int HammingDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? equalityComparer) =>
            HammingDistance(a, b, null, equalityComparer);

        /// <summary>
        /// Calculates Hamming distance between two sequences.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence.</param>
        /// <param name="b">The second sequence.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Hamming distance.</returns>
        public static int HammingDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance, IEqualityComparer<T>? equalityComparer)
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

            equalityComparer ??= EqualityComparer<T>.Default;

            int distance = 0;

            using (var aEnumerator = a.GetEnumerator())
            using (var bEnumerator = b.GetEnumerator())
            {
                while (true)
                {
                    var aNext = aEnumerator.MoveNext();
                    var bNext = bEnumerator.MoveNext();

                    if (aNext && bNext)
                    {
                        if (!equalityComparer.Equals(aEnumerator.Current, bEnumerator.Current))
                        {
                            distance++;

                            if (distance >= maxDistance)
                                return maxDistance.Value;
                        }
                    }
                    else if (aNext || bNext)
                    {
                        throw new ArgumentException("Hamming distance applies to sequences of the same length only.");
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return distance;
        }
    }
}

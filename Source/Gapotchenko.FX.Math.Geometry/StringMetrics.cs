using Gapotchenko.FX.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Geometry
{
    /// <summary>
    /// String metrics functions.
    /// </summary>
    public static class StringMetrics
    {
        /// <summary>
        /// Calculates Levenshtein distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <returns>The Levenshtein distance between the specified sequences of elements.</returns>
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b) =>
            LevenshteinDistance(a, b, null, null);

        /// <summary>
        /// Calculates Levenshtein distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <returns>The Levenshtein distance between the specified sequences of elements.</returns>
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance) =>
            LevenshteinDistance(a, b, maxDistance, null);

        /// <summary>
        /// Calculates Levenshtein distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Levenshtein distance between the specified sequences of elements.</returns>
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? equalityComparer) =>
            LevenshteinDistance(a, b, null, equalityComparer);

        /// <summary>
        /// Calculates Levenshtein distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Levenshtein distance between the specified sequences of elements.</returns>
        public static int LevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance, IEqualityComparer<T>? equalityComparer) =>
            OsaDistance(a, b, maxDistance, allowReplacements: true, allowTranspositions: false, equalityComparer);

        /// <summary>
        /// Calculates longest common subsequence distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <returns>The longest common subsequence distance between the specified sequences of elements.</returns>
        public static int LcsDistance<T>(IEnumerable<T> a, IEnumerable<T> b) =>
            LcsDistance(a, b, null, null);

        /// <summary>
        /// Calculates longest common subsequence distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <returns>The longest common subsequence distance between the specified sequences of elements.</returns>
        public static int LcsDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance) =>
            LcsDistance(a, b, maxDistance, null);

        /// <summary>
        /// Calculates longest common subsequence distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The longest common subsequence distance between the specified sequences of elements.</returns>
        public static int LcsDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? equalityComparer) =>
            LcsDistance(a, b, null, equalityComparer);

        /// <summary>
        /// Calculates longest common subsequence distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The longest common subsequence distance between the specified sequences of elements.</returns>
        public static int LcsDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance, IEqualityComparer<T>? equalityComparer) =>
            OsaDistance(a, b, maxDistance, allowReplacements: false, allowTranspositions: false, equalityComparer);

        /// <summary>
        /// Calculates optimal string alignment distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <returns>The optimal string alignment distance between the specified sequences of elements.</returns>
        public static int OsaDistance<T>(IEnumerable<T> a, IEnumerable<T> b) =>
            OsaDistance(a, b, null, null);

        /// <summary>
        /// Calculates optimal string alignment distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <returns>The optimal string alignment distance between the specified sequences of elements.</returns>
        public static int OsaDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance) =>
            OsaDistance(a, b, maxDistance, null);

        /// <summary>
        /// Calculates optimal string alignment distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The optimal string alignment distance between the specified sequences of elements.</returns>
        public static int OsaDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? equalityComparer) =>
            OsaDistance(a, b, null, equalityComparer);

        /// <summary>
        /// Calculates optimal string alignment distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The optimal string alignment distance between the specified sequences of elements.</returns>
        public static int OsaDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance, IEqualityComparer<T>? equalityComparer) =>
            OsaDistance(a, b, maxDistance, allowReplacements: true, allowTranspositions: true, equalityComparer);

        static int OsaDistance<T>(
            IEnumerable<T> a,
            IEnumerable<T> b,
            int? maxDistance,
            bool allowReplacements,
            bool allowTranspositions,
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

            var sRow = b.AsReadOnlyList();
            var sCol = a.AsReadOnlyList();

            int rowLen = sRow.Count;
            int colLen = sCol.Count;

            if (rowLen == 0)
                return colLen;
            if (colLen == 0)
                return rowLen;

            equalityComparer ??= EqualityComparer<T>.Default;

            // Create and initialize the row vector.
            var row = new int[rowLen + 1];
            var preRow = new int[rowLen + 1];
            int[]? prePreRow = null;
            if (allowTranspositions)
                prePreRow = new int[rowLen + 1];

            for (int rowIdx = 1; rowIdx <= rowLen; rowIdx++)
                preRow[rowIdx] = rowIdx;

            // For each column.
            for (int colIdx = 1; colIdx <= colLen; colIdx++)
            {
                // Set the first element to the column number.
                row[0] = colIdx;

                var col_j = sCol[colIdx - 1];
                int bestAtRow = colIdx;

                // For each row.
                for (int rowIdx = 1; rowIdx <= rowLen; rowIdx++)
                {
                    var row_i = sRow[rowIdx - 1];

                    int currentDistance;
                    if (equalityComparer.Equals(row_i, col_j))
                    {
                        currentDistance = preRow[rowIdx - 1];
                    }
                    else
                    {
                        // Find minimum.
                        currentDistance = System.Math.Min(preRow[rowIdx] + 1, row[rowIdx - 1] + 1);

                        if (allowReplacements)
                        {
                            currentDistance = System.Math.Min(currentDistance, preRow[rowIdx - 1] + 1);
                        }

                        if (allowTranspositions &&
                            rowIdx > 1 && colIdx > 1 &&
                            equalityComparer.Equals(row_i, sCol[colIdx - 2]) &&
                            equalityComparer.Equals(sRow[rowIdx - 2], col_j))
                        {
                            currentDistance = System.Math.Min(currentDistance, prePreRow![rowIdx - 2] + 1);
                        }
                    }

                    row[rowIdx] = currentDistance;

                    bestAtRow = System.Math.Min(bestAtRow, currentDistance);
                }

                if (bestAtRow >= maxDistance)
                    return maxDistance.Value;

                // Swap the vectors.
                MathEx.Swap(ref preRow, ref row);

                if (allowTranspositions)
                    MathEx.Swap(ref prePreRow!, ref row);
            }

            var distance = preRow[rowLen];
            if (distance >= maxDistance)
                return maxDistance.Value;
            return distance;
        }

        /// <summary>
        /// Calculates Hamming distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <returns>The Hamming distance between the specified sequences of elements.</returns>
        public static int HammingDistance<T>(IEnumerable<T> a, IEnumerable<T> b) =>
            HammingDistance(a, b, null, null);

        /// <summary>
        /// Calculates Hamming distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <returns>The Hamming distance between the specified sequences of elements.</returns>
        public static int HammingDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance) =>
            HammingDistance(a, b, maxDistance, null);

        /// <summary>
        /// Calculates Hamming distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Hamming distance between the specified sequences of elements.</returns>
        public static int HammingDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? equalityComparer) =>
            HammingDistance(a, b, null, equalityComparer);

        /// <summary>
        /// Calculates Hamming distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Hamming distance between the specified sequences of elements.</returns>
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

        /// <summary>
        /// Calculates Jaro distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <returns>The Jaro distance between the specified sequences of elements.</returns>
        public static double JaroDistance<T>(IEnumerable<T> a, IEnumerable<T> b) =>
            JaroDistance(a, b, null);

        /// <summary>
        /// Calculates Jaro distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Jaro distance between the specified sequences of elements.</returns>
        public static double JaroDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? equalityComparer)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (ReferenceEquals(a, b))
                return 0;

            var aList = a.AsReadOnlyList();
            var bList = b.AsReadOnlyList();

            if (aList.Count == 0)
            {
                if (bList.Count == 0)
                    return 0;
                return 1;
            }

            equalityComparer ??= EqualityComparer<T>.Default;

            if (bList.Count > aList.Count)
                MathEx.Swap(ref aList, ref bList);

            int matchingRange = System.Math.Max(aList.Count / 2 - 1, 0);

            // Determine the count of matches.
            var machedElements = new List<T>();
            var aMatches = new BitArray(aList.Count);
            for (int bIdx = 0; bIdx < bList.Count; bIdx++)
            {
                var aWindowStart = System.Math.Max(bIdx - matchingRange, 0);
                var aWindowEnd = System.Math.Min(bIdx + matchingRange + 1, aList.Count);
                var bElement = bList[bIdx];
                for (int aIdx = aWindowStart; aIdx < aWindowEnd; aIdx++)
                {
                    if (!aMatches[aIdx] &&
                        equalityComparer.Equals(bElement, aList[aIdx]))
                    {
                        machedElements.Add(bElement);
                        aMatches[aIdx] = true;
                        break;
                    }
                }
            }

            if (machedElements.Count == 0)
                return 1;

            // Determine the count of transpositions.
            int transpositions = 0;
            for (int aIdx = 0, matchIdx = 0; matchIdx < machedElements.Count; aIdx++)
            {
                if (aMatches[aIdx])
                {
                    if (!equalityComparer.Equals(aList[aIdx], machedElements[matchIdx]))
                        transpositions += 1;
                    matchIdx += 1;
                }
            }

            transpositions /= 2;

            double matches = machedElements.Count;
            double jaroSimilarity =
                (matches / aList.Count + matches / bList.Count + (matches - transpositions) / matches)
                / 3;

            return 1 - jaroSimilarity;
        }

        /// <summary>
        /// Calculates Damerau–Levenshtein distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <returns>The Damerau–Levenshtein distance between the specified sequences of elements.</returns>
        public static int DamerauLevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b) where T : notnull =>
            DamerauLevenshteinDistance(a, b, null, null);

        /// <summary>
        /// Calculates Damerau–Levenshtein distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <returns>The Damerau–Levenshtein distance between the specified sequences of elements.</returns>
        public static int DamerauLevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance) where T : notnull =>
            DamerauLevenshteinDistance(a, b, maxDistance, null);

        /// <summary>
        /// Calculates Damerau–Levenshtein distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Damerau–Levenshtein distance between the specified sequences of elements.</returns>
        public static int DamerauLevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, IEqualityComparer<T>? equalityComparer) where T : notnull =>
            DamerauLevenshteinDistance(a, b, null, equalityComparer);

        /// <summary>
        /// Calculates Damerau–Levenshtein distance between two specified sequences of elements.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="a">The first sequence of elements.</param>
        /// <param name="b">The second sequence of elements.</param>
        /// <param name="maxDistance">The inclusive upped bound of the edit distance.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The Damerau–Levenshtein distance between the specified sequences of elements.</returns>
        public static int DamerauLevenshteinDistance<T>(IEnumerable<T> a, IEnumerable<T> b, int? maxDistance, IEqualityComparer<T>? equalityComparer)
            where T : notnull
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

            var aList = a.AsReadOnlyList();
            var bList = b.AsReadOnlyList();

            int aLength = aList.Count;
            int bLength = bList.Count;

            if (bLength == 0)
                return aLength;
            if (aLength == 0)
                return bLength;

            equalityComparer ??= EqualityComparer<T>.Default;

            // The max possible distance.
            int maxDist = bList.Count + aList.Count;

            // Sequence elements map.
            var da = new Dictionary<T, int>(equalityComparer);

            // Create the distance matrix d[0 .. a.Length + 1][0 .. b.Length + 1].
            int[,] d = new int[aLength + 2, bLength + 2];

            // Initialize the left and top edges of d.
            for (int i = 0; i <= aLength; i++)
            {
                d[i + 1, 0] = maxDist;
                d[i + 1, 1] = i;
            }

            for (int j = 0; j <= bLength; j++)
            {
                d[0, j + 1] = maxDist;
                d[1, j + 1] = j;
            }

            // Fill in the distance matrix d.
            for (int aIdx = 1; aIdx <= aLength; aIdx++)
            {
                int db = 0;
                var aElement = aList[aIdx - 1];

                var best = aIdx;

                for (int bIdx = 1; bIdx <= bLength; bIdx++)
                {
                    int k, l;

                    var bElement = bList[bIdx - 1];
                    if (!da.TryGetValue(bElement, out k))
                        k = 0;
                    l = db;

                    int substitutionCost = 1;
                    if (equalityComparer.Equals(aElement, bElement))
                    {
                        substitutionCost = 0;
                        db = bIdx;
                    }

                    var currentDistance = Min(
                        d[aIdx, bIdx] + substitutionCost,             // Substitution
                        d[aIdx + 1, bIdx] + 1,                        // Insertion
                        d[aIdx, bIdx + 1] + 1,                        // Deletion
                        d[k, l] + (aIdx - k - 1) + 1 + (bIdx - l - 1) // Transposition
                    );

                    d[aIdx + 1, bIdx + 1] = currentDistance;

                    best = System.Math.Min(best, currentDistance);
                }

                if (best >= maxDistance)
                    return maxDistance.Value;

                da[aList[aIdx - 1]] = aIdx;
            }

            var distance = d[aLength + 1, bLength + 1];
            if (distance >= maxDistance)
                return maxDistance.Value;
            return distance;
        }

        static int Min(int val1, int val2, int val3, int val4) =>
            System.Math.Min(val1, System.Math.Min(val2, System.Math.Min(val3, val4)));
    }
}

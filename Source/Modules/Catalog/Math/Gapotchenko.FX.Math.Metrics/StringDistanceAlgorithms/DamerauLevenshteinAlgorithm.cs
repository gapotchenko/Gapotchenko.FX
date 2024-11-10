// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Kirill Rode
// Year of introduction: 2021

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.StringDistanceAlgorithms;

using Math = System.Math;

/// <summary>
/// Calculates Damerau–Levenshtein distance between two sequences of elements.
/// </summary>
sealed class DamerauLevenshteinAlgorithm : StringDistanceAlgorithm
{
    public static DamerauLevenshteinAlgorithm Instance { get; } = new();

    public override StringMetricTraits MetricTraits =>
        StringMetricTraits.Insertion |
        StringMetricTraits.Deletion |
        StringMetricTraits.Substitution |
        StringMetricTraits.Transposition;

    public override int Calculate<T>(
        IEnumerable<T> a,
        IEnumerable<T> b,
        ValueInterval<int> range,
        IEqualityComparer<T>? equalityComparer = null,
        CancellationToken cancellationToken = default)
    {
        ValidateArguments(a, b, range);

        return range
            .Clamp(CalculateDistance(range.Intersect(ValueInterval.FromInclusive(0))))
            .Value;

        int CalculateDistance(in ValueInterval<int> range)
        {
            if (range.IsEmpty)
                return default;

            if (ReferenceEquals(a, b))
                return 0;

            var aList = a.ReifyList();
            var bList = b.ReifyList();

            int aLength = aList.Count;
            int bLength = bList.Count;

            if (bLength == 0)
                return aLength;
            if (aLength == 0)
                return bLength;

            equalityComparer ??= EqualityComparer<T>.Default;

            // The max possible distance.
            int maxDist = aLength + bLength;

            // Sequence elements map.
            var da = new AssociativeArray<T, int>(equalityComparer);

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
                    var bElement = bList[bIdx - 1];
                    if (!da.TryGetValue(bElement, out var k))
                        k = 0;
                    var l = db;

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

                    best = Math.Min(best, currentDistance);
                }

                cancellationToken.ThrowIfCancellationRequested();

                if (range.Zone(best) > 0)
                    return best;

                da[aList[aIdx - 1]] = aIdx;
            }

            return d[aLength + 1, bLength + 1];
        }
    }

    static int Min(int val1, int val2, int val3, int val4) =>
        Math.Min(val1, Math.Min(val2, Math.Min(val3, val4)));
}

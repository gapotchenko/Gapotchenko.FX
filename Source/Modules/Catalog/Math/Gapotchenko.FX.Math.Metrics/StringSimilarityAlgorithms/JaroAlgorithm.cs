// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Kirill Rode
// Year of introduction: 2021

using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math.Intervals;
using System.Collections;

namespace Gapotchenko.FX.Math.Metrics.StringSimilarityAlgorithms;

using Math = System.Math;

sealed class JaroAlgorithm : StringSimilarityAlgorithm
{
    public static JaroAlgorithm Instance { get; } = new();

    public override double Calculate<T>(
        IEnumerable<T> a,
        IEnumerable<T> b,
        ValueInterval<double> range,
        IEqualityComparer<T>? equalityComparer = null,
        CancellationToken cancellationToken = default)
    {
        ValidateArguments(a, b, range);

        return range.Clamp(
            CalculateSimilarity(
                ValueInterval.Inclusive(0.0, 1.0)
                .Intersect(range)))
            .Value;

        double CalculateSimilarity(in ValueInterval<double> range)
        {
            if (range.IsEmpty)
                return default;

            if (ReferenceEquals(a, b))
                return 1;

            var aList = a.ReifyList();
            var bList = b.ReifyList();

            if (aList.Count == 0)
                return bList.Count == 0 ? 1 : 0;
            else if (bList.Count == 0)
                return 0;

            equalityComparer ??= EqualityComparer<T>.Default;

            if (bList.Count > aList.Count)
                (aList, bList) = (bList, aList);

            int matchingRange = Math.Max(aList.Count / 2 - 1, 0);

            // Determine the count of matches.
            var machedElements = new List<T>();
            var aMatches = new BitArray(aList.Count);
            for (int bIdx = 0; bIdx < bList.Count; bIdx++)
            {
                var aWindowStart = Math.Max(bIdx - matchingRange, 0);
                var aWindowEnd = Math.Min(bIdx + matchingRange + 1, aList.Count);
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
                cancellationToken.ThrowIfCancellationRequested();
            }

            int matchCount = machedElements.Count;
            if (matchCount == 0)
                return 0;

            // Determine the count of transpositions.
            int transpositions = 0;
            for (int aIdx = 0, matchIdx = 0; matchIdx < matchCount; aIdx++)
            {
                if (aMatches[aIdx])
                {
                    if (!equalityComparer.Equals(aList[aIdx], machedElements[matchIdx]))
                        ++transpositions;
                    matchIdx += 1;
                }
                cancellationToken.ThrowIfCancellationRequested();
            }

            double matches = matchCount;
            double similarity =
                (matches / aList.Count +
                matches / bList.Count +
                (matches - transpositions / 2) / matches)
                / 3;

            return similarity;
        }
    }
}

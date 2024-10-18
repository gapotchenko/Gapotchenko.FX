// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Kirill Rode
// Year of introduction: 2021

using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.StringDistanceAlgorithms;

/// <summary>
/// Calculates Hamming distance between two sequences of elements.
/// </summary>
sealed class HammingAlgorithm : StringDistanceAlgorithm
{
    public static HammingAlgorithm Instance { get; } = new();

    public override StringMetricTraits MetricTraits =>
        StringMetricTraits.Substitution;

    public override int Calculate<T>(
        IEnumerable<T> a,
        IEnumerable<T> b,
        ValueInterval<int> range,
        IEqualityComparer<T>? equalityComparer = null,
        CancellationToken cancellationToken = default)
    {
        ValidateArguments(a, b, range);

        return range.Clamp(
            CalculateDistance(
                ValueInterval.Inclusive<int>(0, null)
                .Intersect(range)))
            .Value;

        int CalculateDistance(in ValueInterval<int> range)
        {
            if (range.IsEmpty)
                return default;

            if (ReferenceEquals(a, b))
                return 0;

            equalityComparer ??= EqualityComparer<T>.Default;

            int distance = 0;
            using var aEnumerator = a.GetEnumerator();
            using var bEnumerator = b.GetEnumerator();

            for (; ; )
            {
                var aNext = aEnumerator.MoveNext();
                var bNext = bEnumerator.MoveNext();

                if (aNext && bNext)
                {
                    if (!equalityComparer.Equals(aEnumerator.Current, bEnumerator.Current))
                    {
                        ++distance;

                        cancellationToken.ThrowIfCancellationRequested();
                        if (range.Sign(distance) > 0)
                            return distance;
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

            return distance;
        }
    }
}

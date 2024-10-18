// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Kirill Rode
// Year of introduction: 2021

using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.StringDistanceAlgorithms;

/// <summary>
/// Calculates longest common subsequence (LCS) distance between two sequences of elements.
/// </summary>
sealed class LcsAlgorithm : OsaBaseAlgorithm
{
    public static LcsAlgorithm Instance { get; } = new();

    public override StringMetricAlgorithmCapabilities Capabilities =>
        StringMetricAlgorithmCapabilities.Insertion |
        StringMetricAlgorithmCapabilities.Deletion;

    public override int Calculate<T>(
        IEnumerable<T> a,
        IEnumerable<T> b,
        ValueInterval<int> range,
        IEqualityComparer<T>? equalityComparer = null,
        CancellationToken cancellationToken = default) =>
        CalculateCore(a, b, range, false, false, equalityComparer, cancellationToken);
}

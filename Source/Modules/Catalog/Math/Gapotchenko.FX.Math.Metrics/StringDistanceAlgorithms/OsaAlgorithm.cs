// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Kirill Rode
// Year of introduction: 2021

using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.StringDistanceAlgorithms;

/// <summary>
/// Calculates optimal string alignment (OSA) distance between two sequences of elements.
/// </summary>
sealed class OsaAlgorithm : OsaBaseAlgorithm
{
    public static OsaAlgorithm Instance { get; } = new();

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
        CancellationToken cancellationToken = default) =>
        CalculateCore(a, b, range, true, true, equalityComparer, cancellationToken);
}

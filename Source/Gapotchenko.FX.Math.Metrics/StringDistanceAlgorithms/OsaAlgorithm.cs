using Gapotchenko.FX.Math.Intervals;

namespace Gapotchenko.FX.Math.Metrics.StringDistanceAlgorithms;

sealed class OsaAlgorithm : OsaBaseAlgorithm
{
    public static OsaAlgorithm Instance { get; } = new();

    public override int Measure<T>(
        IEnumerable<T> a,
        IEnumerable<T> b,
        ValueInterval<int> range,
        IEqualityComparer<T>? equalityComparer = null,
        CancellationToken cancellationToken = default) =>
        MeasureCore(a, b, range, true, true, equalityComparer, cancellationToken);
}

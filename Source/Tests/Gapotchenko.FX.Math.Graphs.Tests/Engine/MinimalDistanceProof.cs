using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math.Geometry;

namespace Gapotchenko.FX.Math.Graphs.Tests.Engine;

static class MinimalDistanceProof
{
    public static bool Verify<T>(
        IEnumerable<T> source,
        IEnumerable<T> destination,
        Func<T, T, bool> df)
    {
        var s = source.ReifyList();
        var d = destination.ReifyList();

        int actualDistance = StringMetrics.LevenshteinDistance(s, d);
        if (actualDistance == 0)
            return true;

        foreach (var candidateOrder in TopologicalOrderProof.AllOrdersOf(s, df))
        {
            int possibleDistance = StringMetrics.LevenshteinDistance(s, candidateOrder);
            if (possibleDistance < actualDistance)
                return false;
        }

        return true;
    }
}

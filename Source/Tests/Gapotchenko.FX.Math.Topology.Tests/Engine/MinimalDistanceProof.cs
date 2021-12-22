using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Math.Geometry;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology.Tests.Engine
{
    static class MinimalDistanceProof
    {
        public static bool Verify<T>(
            IEnumerable<T> source,
            IEnumerable<T> destination,
            DependencyFunction<T> df)
        {
            var s = source.AsReadOnlyList();
            var d = destination.AsReadOnlyList();

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
}

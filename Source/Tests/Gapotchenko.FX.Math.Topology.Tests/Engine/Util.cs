using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology.Tests.Engine
{
    static class Util
    {
        static IEnumerable<GraphEdge<T>> Conv<T>(IEnumerable<(T, T)> other) => other.Select(x => (GraphEdge<T>)x);

        public static bool SetEquals<T>(this ISet<GraphEdge<T>> edgeSet, IEnumerable<(T From, T To)> other)
        {
            if (edgeSet == null)
                throw new ArgumentNullException(nameof(edgeSet));
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return edgeSet.SetEquals(Conv(other));
        }
    }
}

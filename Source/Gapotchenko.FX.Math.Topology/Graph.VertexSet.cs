using Gapotchenko.FX.Collections.Generic.Kit;
using Gapotchenko.FX.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        [DebuggerDisplay("Count = {Count}")]
        sealed class VertexSet : SetBase<T>
        {
            internal VertexSet(Graph<T> graph)
            {
                m_Graph = graph;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Graph<T> m_Graph;

            public override IEqualityComparer<T> Comparer => m_Graph.Comparer;

            public override int Count => m_Graph.m_CachedOrder ??= GetEnumerator().Rest().Count();

            public override bool Add(T vertex)
            {
                if (Contains(vertex))
                    return false;
                m_Graph.m_AdjacencyList.Add(vertex, null);
                ++m_Graph.m_CachedOrder;
                return true;
            }

            public override void Clear() => m_Graph.Clear();

            public override bool Contains(T vertex)
            {
                var adjacencyList = m_Graph.m_AdjacencyList;
                return
                    adjacencyList.ContainsKey(vertex) ||
                    adjacencyList.Any(x => x.Value?.Contains(vertex) ?? false);
            }

            public override bool Remove(T vertex)
            {
                bool hit = false;
                var adjacencyList = m_Graph.m_AdjacencyList;

                hit |= adjacencyList.Remove(vertex);

                foreach (var i in adjacencyList)
                {
                    var adjacencyRow = i.Value;
                    if (adjacencyRow != null)
                        hit |= adjacencyRow.Remove(vertex);
                }

                if (hit)
                    --m_Graph.m_CachedOrder;

                return hit;
            }

            public override IEnumerator<T> GetEnumerator() =>
                m_Graph.m_AdjacencyList
                .SelectMany(x => (x.Value ?? Enumerable.Empty<T>()).Prepend(x.Key))
                .Distinct(m_Graph.Comparer)
                .GetEnumerator();
        }
    }
}

using Gapotchenko.FX.Collections.Generic.Kit;
using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        sealed class VertexSet : SetBase<TVertex>
        {
            public VertexSet(Graph<TVertex> graph)
            {
                m_Graph = graph;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Graph<TVertex> m_Graph;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public override IEqualityComparer<TVertex> Comparer => m_Graph.VertexComparer;

            public override int Count => m_Graph.m_CachedOrder ??= GetEnumerator().Rest().Count();

            public override bool Add(TVertex vertex)
            {
                if (Contains(vertex))
                    return false;
                m_Graph.m_AdjacencyList.Add(vertex, null);
                ++m_Graph.m_CachedOrder;
                m_Graph.IncrementVersion();
                return true;
            }

            public override bool Remove(TVertex vertex)
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
                {
                    --m_Graph.m_CachedOrder;

#if !(TFF_DICTIONARY_ENUMERATION_REMOVE_ALLOWED && TFF_HASHSET_ENUMERATION_REMOVE_ALLOWED)
                    m_Graph.IncrementVersion();
#endif
                }

                return hit;
            }

            public override void Clear() => m_Graph.Clear();

            public override bool Contains(TVertex vertex)
            {
                var adjacencyList = m_Graph.m_AdjacencyList;
                return
                    adjacencyList.ContainsKey(vertex) ||
                    adjacencyList.Any(x => x.Value?.Contains(vertex) ?? false);
            }

            public override IEnumerator<TVertex> GetEnumerator()
            {
                var version = m_Graph.m_Version;

                var query = m_Graph.m_AdjacencyList
                    .SelectMany(x => (x.Value ?? Enumerable.Empty<TVertex>()).Prepend(x.Key))
                    .Distinct(m_Graph.VertexComparer);

                foreach (var i in query)
                {
                    if (m_Graph.m_Version != version)
                        ModificationGuard.Throw();

                    yield return i;
                }
            }
        }
    }
}

using Gapotchenko.FX.Collections.Generic.Kit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        sealed class EdgeSet : SetBase<GraphEdge<T>>
        {
            internal EdgeSet(Graph<T> graph)
            {
                m_Graph = graph;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Graph<T> m_Graph;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IEqualityComparer<GraphEdge<T>>? m_Comparer;

            public override IEqualityComparer<GraphEdge<T>> Comparer => m_Comparer ??= GraphEdge<T>.CreateEqualityComparer(m_Graph.Comparer);

            public override int Count => m_Graph.m_CachedSize ??= m_Graph.m_AdjacencyList.Select(x => x.Value?.Count ?? 0).Sum();

            public override bool Add(GraphEdge<T> edge)
            {
                var from = edge.From;

                var adjList = m_Graph.m_AdjacencyList;

                if (!adjList.TryGetValue(from, out var adjRow))
                {
                    adjRow = m_Graph.NewAdjacencyRow();
                    adjList.Add(from, adjRow);
                }
                else if (adjRow == null)
                {
                    adjRow = m_Graph.NewAdjacencyRow();
                    adjList[from] = adjRow;
                }

                if (adjRow.Add(edge.To))
                {
                    ++m_Graph.m_CachedSize;
                    m_Graph.m_CachedOrder = null;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override bool Remove(GraphEdge<T> item)
            {
                throw new NotImplementedException();
            }

            public override void Clear()
            {
                foreach (var i in m_Graph.AdjacencyList)
                    i.Value?.Clear();
                m_Graph.m_CachedSize = 0;
            }

            public override bool Contains(GraphEdge<T> edge) =>
                m_Graph.m_AdjacencyList.TryGetValue(edge.From, out var adjRow) &&
                adjRow != null &&
                adjRow.Contains(edge.To);

            public override IEnumerator<GraphEdge<T>> GetEnumerator()
            {
                foreach (var i in m_Graph.m_AdjacencyList)
                {
                    var adjacencyRow = i.Value;
                    if (adjacencyRow != null)
                    {
                        var from = i.Key;
                        foreach (var to in adjacencyRow)
                            yield return new GraphEdge<T>(from, to);
                    }
                }
            }
        }
    }
}

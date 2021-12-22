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
            public EdgeSet(Graph<T> graph)
            {
                m_Graph = graph;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Graph<T> m_Graph;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IEqualityComparer<GraphEdge<T>>? m_Comparer;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public override IEqualityComparer<GraphEdge<T>> Comparer => m_Comparer ??= GraphEdge.CreateComparer(m_Graph.Comparer);

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
                    m_Graph.IncrementVersion();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override bool Remove(GraphEdge<T> item)
            {
                var adjList = m_Graph.m_AdjacencyList;

                if (!adjList.TryGetValue(item.From, out var adjRow))
                    return false;

                if (adjRow == null)
                    return false;

                var v = item.To;

                // Remove the adjacent vertex.
                if (!adjRow.Remove(v))
                    return false;

                // Preserve the vertex in vertices collection.
                if (!adjList.ContainsKey(v))
                    adjList.Add(v, null);

                --m_Graph.m_CachedSize;
                m_Graph.IncrementVersion();

                return true;
            }

            public override void Clear()
            {
                HashSet<T>? verticesToKeep = null;

                var adjList = m_Graph.AdjacencyList;
                foreach (var i in adjList)
                {
                    var adjRow = i.Value;
                    if (adjRow == null)
                        continue;

                    foreach (var j in adjRow)
                    {
                        // Preserve the vertex in vertices collection.
                        if (!adjList.ContainsKey(j))
                        {
                            verticesToKeep ??= new HashSet<T>(m_Graph.Comparer);
                            verticesToKeep.Add(j);
                        }
                    }

                    // Clear the adjacency row.
                    adjRow.Clear();
                }

                if (verticesToKeep != null)
                {
                    // Keep the list of vertices.
                    foreach (var i in verticesToKeep)
                        adjList.Add(i, null);
                }

                m_Graph.m_CachedSize = 0;
                m_Graph.IncrementVersion();
            }

            public override bool Contains(GraphEdge<T> edge) =>
                m_Graph.m_AdjacencyList.TryGetValue(edge.From, out var adjRow) &&
                adjRow != null &&
                adjRow.Contains(edge.To);

            public override IEnumerator<GraphEdge<T>> GetEnumerator()
            {
                var version = m_Graph.m_Version;

                foreach (var i in m_Graph.m_AdjacencyList)
                {
                    var adjacencyRow = i.Value;
                    if (adjacencyRow != null)
                    {
                        var from = i.Key;
                        foreach (var to in adjacencyRow)
                        {
                            if (m_Graph.m_Version != version)
                                ModificationGuard.Throw();

                            yield return GraphEdge.Create(from, to);
                        }
                    }
                }
            }
        }
    }
}

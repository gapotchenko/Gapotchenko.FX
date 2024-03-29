﻿using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Generic.Kit;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        /// <summary>
        /// Represents a set of graph vertices.
        /// </summary>
        public sealed class EdgeSet : SetBase<GraphEdge<TVertex>>
        {
            internal EdgeSet(Graph<TVertex> graph)
            {
                m_Graph = graph;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Graph<TVertex> m_Graph;

            /// <inheritdoc/>
            public override IEqualityComparer<GraphEdge<TVertex>> Comparer => m_Graph.EdgeComparer;

            /// <inheritdoc/>
            public override int Count => m_Graph.m_CachedSize ??= m_Graph.m_AdjacencyList.Select(x => x.Value?.Count ?? 0).Sum();

            /// <inheritdoc/>
            public override bool Add(GraphEdge<TVertex> edge)
            {
                if (!m_Graph.IsDirected && Contains(edge))
                    return false;

                static bool AddToAdjacencyList(Graph<TVertex> graph, AssociativeArray<TVertex, AdjacencyRow?> adjacencyList, TVertex from, TVertex to)
                {
                    if (!adjacencyList.TryGetValue(from, out var adjacencyRow))
                    {
                        adjacencyRow = graph.NewAdjacencyRow();
                        adjacencyList.Add(from, adjacencyRow);
                    }
                    else if (adjacencyRow == null)
                    {
                        adjacencyRow = graph.NewAdjacencyRow();
                        adjacencyList[from] = adjacencyRow;
                    }

                    return adjacencyRow.Add(to);
                }

                if (!AddToAdjacencyList(m_Graph, m_Graph.m_AdjacencyList, edge.From, edge.To))
                    return false;

                var reverseAdjacencyList = m_Graph.m_ReverseAdjacencyList;
                if (reverseAdjacencyList != null)
                {
                    bool hit = AddToAdjacencyList(m_Graph, reverseAdjacencyList, edge.To, edge.From);
                    Debug.Assert(hit);
                }

                ++m_Graph.m_CachedSize;
                m_Graph.m_CachedOrder = null;
                m_Graph.InvalidateCachedRelations();
                m_Graph.IncrementVersion();

                return true;
            }

            /// <inheritdoc/>
            public override bool Remove(GraphEdge<TVertex> item)
            {
                static bool RemoveFromAdjacencyList(AssociativeArray<TVertex, AdjacencyRow?> adjacencyList, TVertex from, TVertex to)
                {
                    if (!adjacencyList.TryGetValue(from, out var adjacencyRow))
                        return false;

                    if (adjacencyRow == null)
                        return false;

                    // Remove the adjacent vertex.
                    if (!adjacencyRow.Remove(to))
                        return false;

                    // Preserve the vertex in vertices collection.
                    if (!adjacencyList.ContainsKey(to))
                        adjacencyList.Add(to, null);

                    return true;
                }

                if (m_Graph.IsDirected)
                {
                    if (!RemoveFromAdjacencyList(m_Graph.m_AdjacencyList, item.From, item.To))
                        return false;

                    var reverseAdjacencyList = m_Graph.m_ReverseAdjacencyList;
                    if (reverseAdjacencyList != null)
                    {
                        bool hit = RemoveFromAdjacencyList(reverseAdjacencyList, item.To, item.From);
                        Debug.Assert(hit);
                    }
                }
                else
                {
                    var adjacencyList = m_Graph.m_AdjacencyList;
                    bool hit =
                        RemoveFromAdjacencyList(adjacencyList, item.From, item.To) ||
                        RemoveFromAdjacencyList(adjacencyList, item.To, item.From);
                    if (!hit)
                        return false;
                }

                --m_Graph.m_CachedSize;
                m_Graph.InvalidateCachedRelations();
                m_Graph.IncrementVersion();

                return true;
            }

            /// <inheritdoc/>
            public override void Clear()
            {
                HashSet<TVertex>? verticesToKeep = null;

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
                            verticesToKeep ??= new HashSet<TVertex>(m_Graph.VertexComparer);
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
                m_Graph.m_ReverseAdjacencyList = null;
                m_Graph.InvalidateCachedRelations();
                m_Graph.IncrementVersion();
            }

            /// <inheritdoc/>
            public override bool Contains(GraphEdge<TVertex> edge)
            {
                bool ContainsCore(TVertex from, TVertex to) =>
                    m_Graph.m_AdjacencyList.TryGetValue(from, out var adjRow) &&
                    adjRow != null &&
                    adjRow.Contains(to);

                if (m_Graph.IsDirected)
                    return ContainsCore(edge.From, edge.To);
                else
                    return ContainsCore(edge.From, edge.To) || ContainsCore(edge.To, edge.From);
            }

            /// <inheritdoc/>
            public override IEnumerator<GraphEdge<TVertex>> GetEnumerator()
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

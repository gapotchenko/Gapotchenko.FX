﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Generic.Kits;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc cref="IGraph{TVertex}.Edges"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public EdgeSet Edges => m_CachedEdges ??= new(this);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    EdgeSet? m_CachedEdges;

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ISet<GraphEdge<TVertex>> IGraph<TVertex>.Edges => Edges;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlySet<GraphEdge<TVertex>> IReadOnlyGraph<TVertex>.Edges => Edges;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<GraphEdge<TVertex>> EdgeComparer =>
        m_EdgeComparer ??=
        GraphEdge.EqualityComparer.Create(VertexComparer, IsDirected);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<GraphEdge<TVertex>>? m_EdgeComparer;

    /// <summary>
    /// Represents a set of graph vertices.
    /// </summary>
    public sealed class EdgeSet : SetKit<GraphEdge<TVertex>>
    {
        internal EdgeSet(Graph<TVertex> graph)
        {
            m_Graph = graph;
        }

        /// <inheritdoc/>
        public override IEqualityComparer<GraphEdge<TVertex>> Comparer => m_Graph.EdgeComparer;

        /// <inheritdoc/>
        public override int Count =>
            m_Graph.m_CachedSize ??=
            m_Graph.m_AdjacencyList.Select(x => x.Value?.Count ?? 0).Sum();

        /// <inheritdoc/>
        public override bool Add(GraphEdge<TVertex> edge)
        {
            var graph = m_Graph;

            if (!graph.IsDirected && Contains(edge))
                return false;

            if (!AddToAdjacencyList(graph, graph.m_AdjacencyList, edge.From, edge.To))
                return false;

            var reverseAdjacencyList = graph.m_ReverseAdjacencyList;
            if (reverseAdjacencyList != null)
            {
                bool hit = AddToAdjacencyList(graph, reverseAdjacencyList, edge.To, edge.From);
                Debug.Assert(hit);
            }

            ++graph.m_CachedSize;
            graph.m_CachedOrder = null;
            graph.InvalidateCachedRelations();
            graph.IncrementVersion();

            return true;

            static bool AddToAdjacencyList(
                Graph<TVertex> graph,
                AssociativeArray<TVertex, AdjacencyRow?> adjacencyList,
                TVertex from,
                TVertex to)
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
        }

        /// <inheritdoc/>
        public override bool Remove(GraphEdge<TVertex> item)
        {
            var graph = m_Graph;

            if (graph.IsDirected)
            {
                if (!RemoveFromAdjacencyList(graph.m_AdjacencyList, item.From, item.To))
                    return false;

                var reverseAdjacencyList = graph.m_ReverseAdjacencyList;
                if (reverseAdjacencyList != null)
                {
                    bool hit = RemoveFromAdjacencyList(reverseAdjacencyList, item.To, item.From);
                    Debug.Assert(hit);
                }
            }
            else
            {
                var adjacencyList = graph.m_AdjacencyList;
                bool hit =
                    RemoveFromAdjacencyList(adjacencyList, item.From, item.To) ||
                    RemoveFromAdjacencyList(adjacencyList, item.To, item.From);
                if (!hit)
                    return false;
            }

            --graph.m_CachedSize;
            graph.InvalidateCachedRelations();
            graph.IncrementVersion();

            return true;

            static bool RemoveFromAdjacencyList(
                AssociativeArray<TVertex, AdjacencyRow?> adjacencyList,
                TVertex from,
                TVertex to)
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
            if (m_Graph.IsDirected)
                return ContainsCore(edge);
            else
                return ContainsCore(edge) || ContainsCore(edge.Reverse());

            bool ContainsCore(in GraphEdge<TVertex> edge) =>
                m_Graph.m_AdjacencyList.TryGetValue(edge.From, out var adjRow) &&
                adjRow != null &&
                adjRow.Contains(edge.To);
        }

        /// <inheritdoc/>
        public override IEnumerator<GraphEdge<TVertex>> GetEnumerator()
        {
            var modificationGuard = new ModificationGuard(m_Graph);

            foreach (var i in m_Graph.m_AdjacencyList)
            {
                var adjacencyRow = i.Value;
                if (adjacencyRow != null)
                {
                    var from = i.Key;
                    foreach (var to in adjacencyRow)
                    {
                        modificationGuard.Checkpoint();
                        yield return GraphEdge.Create(from, to);
                    }
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Graph<TVertex> m_Graph;
    }
}

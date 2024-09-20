// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Generic.Kit;
using Gapotchenko.FX.Linq;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    VertexSet? m_CachedVertices;

    /// <inheritdoc cref="IGraph{TVertex}.Vertices"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public VertexSet Vertices => m_CachedVertices ??= new(this);

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ISet<TVertex> IGraph<TVertex>.Vertices => Vertices;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlySet<TVertex> IReadOnlyGraph<TVertex>.Vertices => Vertices;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<TVertex> VertexComparer => m_AdjacencyList.Comparer;

    /// <summary>
    /// Represents a set of graph vertices.
    /// </summary>
    public sealed class VertexSet : SetBase<TVertex>
    {
        internal VertexSet(Graph<TVertex> graph)
        {
            m_Graph = graph;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Graph<TVertex> m_Graph;

        /// <inheritdoc/>
        public override IEqualityComparer<TVertex> Comparer => m_Graph.VertexComparer;

        /// <inheritdoc/>
        public override int Count => m_Graph.m_CachedOrder ??= this.Stream().Count();

        /// <inheritdoc/>
        public override bool Add(TVertex vertex)
        {
            if (Contains(vertex))
                return false;

            var graph = m_Graph;

            graph.m_AdjacencyList.Add(vertex, null);
            graph.m_ReverseAdjacencyList?.Add(vertex, null);

            ++graph.m_CachedOrder;
            graph.InvalidateCachedConnectivity();
            graph.IncrementVersion();

            return true;
        }

        /// <inheritdoc/>
        public override bool Remove(TVertex vertex)
        {
            var graph = m_Graph;

            if (!RemoveFromAdjacencyList(graph.m_AdjacencyList, vertex))
                return false;

            var reverseAdjacencyList = graph.m_ReverseAdjacencyList;
            if (reverseAdjacencyList != null)
            {
                bool hit = RemoveFromAdjacencyList(reverseAdjacencyList, vertex);
                Debug.Assert(hit);
            }

            --graph.m_CachedOrder;
            graph.m_CachedSize = null;
            graph.InvalidateCachedRelations();
            graph.IncrementVersion();

            return true;

            static bool RemoveFromAdjacencyList(AssociativeArray<TVertex, AdjacencyRow?> adjacencyList, TVertex vertex)
            {
                bool hit = adjacencyList.Remove(vertex);

                foreach (var i in adjacencyList)
                {
                    var adjacencyRow = i.Value;
                    if (adjacencyRow != null)
                        hit |= adjacencyRow.Remove(vertex);
                }

                return hit;
            }
        }

        /// <inheritdoc/>
        public override void Clear() => m_Graph.Clear();

        /// <inheritdoc/>
        public override bool Contains(TVertex vertex)
        {
            var adjacencyList = m_Graph.m_AdjacencyList;
            if (adjacencyList.ContainsKey(vertex))
                return true;

            var reverseAdjacencyList = m_Graph.m_ReverseAdjacencyList;
            if (reverseAdjacencyList != null)
                return reverseAdjacencyList.ContainsKey(vertex);
            else
                return adjacencyList.Any(x => x.Value?.Contains(vertex) ?? false);
        }

        /// <inheritdoc/>
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

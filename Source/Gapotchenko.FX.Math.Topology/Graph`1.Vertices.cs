using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Generic.Kit;
using Gapotchenko.FX.Linq;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology;

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
        public override int Count => m_Graph.m_CachedOrder ??= GetEnumerator().Rest().Count();

        /// <inheritdoc/>
        public override bool Add(TVertex vertex)
        {
            if (Contains(vertex))
                return false;

            m_Graph.m_AdjacencyList.Add(vertex, null);
            m_Graph.m_ReverseAdjacencyList?.Add(vertex, null);

            ++m_Graph.m_CachedOrder;
            m_Graph.InvalidateCachedConnectivity();
            m_Graph.IncrementVersion();

            return true;
        }

        /// <inheritdoc/>
        public override bool Remove(TVertex vertex)
        {
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

            if (!RemoveFromAdjacencyList(m_Graph.m_AdjacencyList, vertex))
                return false;

            var reverseAdjacencyList = m_Graph.m_ReverseAdjacencyList;
            if (reverseAdjacencyList != null)
            {
                bool hit = RemoveFromAdjacencyList(reverseAdjacencyList, vertex);
                Debug.Assert(hit);
            }

            --m_Graph.m_CachedOrder;
            m_Graph.m_CachedSize = null;
            m_Graph.InvalidateCachedRelations();
            m_Graph.IncrementVersion();

            return true;
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

using Gapotchenko.FX.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <summary>
        /// Represents a set of graph edges.
        /// </summary>
        [DebuggerDisplay("Count = {Count}")]
        public sealed class EdgeSet : ISet<GraphEdge<T>>, IReadOnlySet<GraphEdge<T>>
        {
            internal EdgeSet(Graph<T> graph)
            {
                m_Graph = graph;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Graph<T> m_Graph;

            /// <summary>
            /// <para>
            /// Gets the size of the graph.
            /// </para>
            /// <para>
            /// The size of a graph is |E|, the number of its edges.
            /// </para>
            /// </summary>
            public int Count => m_Graph.m_CachedSize ??= m_Graph.m_AdjacencyList.Select(x => x.Value?.Count ?? 0).Sum();

            bool ICollection<GraphEdge<T>>.IsReadOnly => false;

            /// <summary>
            /// Adds the specified edge to the graph.
            /// </summary>
            /// <param name="edge">The edge.</param>
            /// <returns><c>true</c> if the edge is added to the graph; <c>false</c> if the edge is already present.</returns>
            public bool Add(GraphEdge<T> edge)
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

            /// <summary>
            /// Removes all edges from the graph.
            /// </summary>
            public void Clear()
            {
                foreach (var i in m_Graph.AdjacencyList)
                    i.Value?.Clear();
                m_Graph.m_CachedSize = 0;
            }

            /// <summary>
            /// <para>
            /// Determines whether the graph contains a specified edge.
            /// </para>
            /// <para>
            /// The presence of an edge in the graph signifies that corresponding vertices are adjacent.
            /// </para>
            /// <para>
            /// Adjacent vertices are those connected by one edge without intermediary vertices.
            /// </para>
            /// </summary>
            /// <param name="edge">The edge.</param>
            /// <returns><c>true</c> when the graph contains the specified edge; otherwise, <c>false</c>.</returns>
            public bool Contains(GraphEdge<T> edge) =>
                m_Graph.m_AdjacencyList.TryGetValue(edge.From, out var adjRow) &&
                adjRow != null &&
                adjRow.Contains(edge.To);

            /// <inheritdoc/>
            public void CopyTo(GraphEdge<T>[] array, int arrayIndex) => SetImplUtil.CopyTo(this, array, arrayIndex);

            /// <inheritdoc/>
            public void ExceptWith(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public void IntersectWith(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool IsProperSubsetOf(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool IsProperSupersetOf(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool IsSubsetOf(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool IsSupersetOf(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool Overlaps(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool Remove(GraphEdge<T> item)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool SetEquals(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public void SymmetricExceptWith(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public void UnionWith(IEnumerable<GraphEdge<T>> other) => SetImplUtil.UnionWith(this, other);

            void ICollection<GraphEdge<T>>.Add(GraphEdge<T> item) => Add(item);

            /// <inheritdoc/>
            public IEnumerator<GraphEdge<T>> GetEnumerator()
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

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}

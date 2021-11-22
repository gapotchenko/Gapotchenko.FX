using Gapotchenko.FX.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <summary>
        /// Represents a set of graph vertices.
        /// </summary>
        [DebuggerDisplay("Count = {Count}")]
        public struct VertexSet : ISet<T>, IReadOnlySet<T>
        {
            internal VertexSet(Graph<T> graph)
            {
                m_Graph = graph;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Graph<T> m_Graph;

            /// <summary>
            /// <para>
            /// Gets the order of the graph.
            /// </para>
            /// <para>
            /// The order of a graph is |V|, the number of its vertices.
            /// </para>
            /// </summary>
            public int Count => GetEnumerator().Rest().Count();

            bool ICollection<T>.IsReadOnly => false;

            /// <summary>
            /// Adds the specified vertex to the graph.
            /// </summary>
            /// <param name="vertex">The vertex.</param>
            /// <returns><c>true</c> if the vertex is added to the graph; <c>false</c> if the vertex is already present.</returns>
            public bool Add(T vertex)
            {
                if (Contains(vertex))
                    return false;
                m_Graph.AdjacencyList.Add(vertex, null);
                return true;
            }

            /// <inheritdoc/>
            public void UnionWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public void IntersectWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public void ExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public void SymmetricExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool IsSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool IsSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool IsProperSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool IsProperSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool Overlaps(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public bool SetEquals(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            void ICollection<T>.Add(T item) => Add(item);

            /// <summary>
            /// Removes all vertices from the graph.
            /// </summary>
            public void Clear() => m_Graph.Clear();

            /// <summary>
            /// Determines whether the graph contains a specified vertex.
            /// </summary>
            /// <param name="vertex">The vertex.</param>
            /// <returns><c>true</c> when the graph contains a specified vertex; otherwise, <c>false</c>.</returns>
            public bool Contains(T vertex)
            {
                var adjacencyList = m_Graph.m_AdjacencyList;
                return
                    adjacencyList.ContainsKey(vertex) ||
                    adjacencyList.Any(x => x.Value?.Contains(vertex) ?? false);
            }

            /// <inheritdoc/>
            public void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Removes the specified vertex from the graph.
            /// </summary>
            /// <param name="vertex">The vertex.</param>
            /// <returns><c>true</c> if the vertex was removed from the graph; otherwise, <c>false</c>.</returns>
            public bool Remove(T vertex)
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

                return hit;
            }

            /// <inheritdoc/>
            public IEnumerator<T> GetEnumerator() =>
                m_Graph.m_AdjacencyList
                .SelectMany(x => (x.Value ?? Enumerable.Empty<T>()).Append(x.Key))
                .Distinct(m_Graph.Comparer)
                .GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}

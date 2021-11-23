using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        [DebuggerDisplay("Count = {Count}")]
        sealed class EdgeSet : ISet<GraphEdge<T>>, IReadOnlySet<GraphEdge<T>>
        {
            internal EdgeSet(Graph<T> graph)
            {
                m_Graph = graph;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Graph<T> m_Graph;

            public int Count => m_Graph.m_CachedSize ??= m_Graph.m_AdjacencyList.Select(x => x.Value?.Count ?? 0).Sum();

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            bool ICollection<GraphEdge<T>>.IsReadOnly => false;

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

            public void Clear()
            {
                foreach (var i in m_Graph.AdjacencyList)
                    i.Value?.Clear();
                m_Graph.m_CachedSize = 0;
            }

            public bool Contains(GraphEdge<T> edge) =>
                m_Graph.m_AdjacencyList.TryGetValue(edge.From, out var adjRow) &&
                adjRow != null &&
                adjRow.Contains(edge.To);

            public void CopyTo(GraphEdge<T>[] array, int arrayIndex) => SetImplUtil.CopyTo(this, array, arrayIndex);

            public void ExceptWith(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            public void IntersectWith(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSubsetOf(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSupersetOf(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSubsetOf(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSupersetOf(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            public bool Overlaps(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            public bool Remove(GraphEdge<T> item)
            {
                throw new NotImplementedException();
            }

            public bool SetEquals(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            public void SymmetricExceptWith(IEnumerable<GraphEdge<T>> other)
            {
                throw new NotImplementedException();
            }

            public void UnionWith(IEnumerable<GraphEdge<T>> other) => SetImplUtil.UnionWith(this, other);

            void ICollection<GraphEdge<T>>.Add(GraphEdge<T> item) => Add(item);

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

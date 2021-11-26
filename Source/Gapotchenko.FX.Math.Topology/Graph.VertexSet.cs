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
        [DebuggerDisplay("Count = {Count}")]
        sealed class VertexSet : ISet<T>, IReadOnlySet<T>
        {
            internal VertexSet(Graph<T> graph)
            {
                m_Graph = graph;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Graph<T> m_Graph;

            public int Count => m_Graph.m_CachedOrder ??= GetEnumerator().Rest().Count();

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            bool ICollection<T>.IsReadOnly => false;

            public bool Add(T vertex)
            {
                if (Contains(vertex))
                    return false;
                m_Graph.m_AdjacencyList.Add(vertex, null);
                ++m_Graph.m_CachedOrder;
                return true;
            }

            public void UnionWith(IEnumerable<T> other) => SetImplUtil.UnionWith(this, other);

            public void IntersectWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void ExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void SymmetricExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool Overlaps(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool SetEquals(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            void ICollection<T>.Add(T item) => Add(item);

            public void Clear() => m_Graph.Clear();

            public bool Contains(T vertex)
            {
                var adjacencyList = m_Graph.m_AdjacencyList;
                return
                    adjacencyList.ContainsKey(vertex) ||
                    adjacencyList.Any(x => x.Value?.Contains(vertex) ?? false);
            }

            public void CopyTo(T[] array, int arrayIndex) => SetImplUtil.CopyTo(this, array, arrayIndex);

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

                if (hit)
                    --m_Graph.m_CachedOrder;

                return hit;
            }

            public IEnumerator<T> GetEnumerator() =>
                m_Graph.m_AdjacencyList
                .SelectMany(x => (x.Value ?? Enumerable.Empty<T>()).Prepend(x.Key))
                .Distinct(m_Graph.Comparer)
                .GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}

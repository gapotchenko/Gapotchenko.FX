using Gapotchenko.FX.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        /// <inheritdoc />
        public IOrderedEnumerable<TVertex> OrderTopologically() => OrderTopologically(false);

        /// <inheritdoc />
        public IOrderedEnumerable<TVertex> OrderTopologicallyInReverse() => OrderTopologically(true);

        IOrderedEnumerable<TVertex> OrderTopologically(bool reverse) => new TopologicallyOrderedEnumerable(this, reverse);

        IEnumerator<TVertex> OrderTopologicallyCore()
        {
            var queue = new Queue<TVertex>();
            var graph = GetTransposition();

            foreach (var vertex in graph.Vertices)
            {
                // Find the nodes with no outgoing edges.
                if (graph.GetVertexOutdegree(vertex) is 0)
                    queue.Enqueue(vertex);
            }

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                yield return vertex;

                foreach (var adjacentVertex in DestinationVerticesAdjacentTo(vertex))
                {
                    graph.Edges.Remove(adjacentVertex, vertex);

                    if (graph.GetVertexOutdegree(adjacentVertex) is 0)
                        queue.Enqueue(adjacentVertex);
                }
            }

            if (graph.Edges.Count is > 0)
                throw new CircularDependencyException();
        }

        IEnumerator<TVertex> OrderTopologicallyCore(IComparer<TVertex> comparer)
        {
            var queue = new PriorityQueue<TVertex, TVertex>(comparer);
            var graph = GetTransposition();

            foreach (var vertex in graph.Vertices)
            {
                // Find the nodes with no outgoing edges.
                if (graph.GetVertexOutdegree(vertex) is 0)
                    queue.Enqueue(vertex, vertex);
            }

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                yield return vertex;

                foreach (var adjacentVertex in DestinationVerticesAdjacentTo(vertex))
                {
                    graph.Edges.Remove(adjacentVertex, vertex);

                    if (graph.GetVertexOutdegree(adjacentVertex) is 0)
                        queue.Enqueue(adjacentVertex, adjacentVertex);
                }
            }

            if (graph.Edges.Count is > 0)
                throw new CircularDependencyException();
        }

        interface ITopologicallyOrderedEnumerable : IOrderedEnumerable<TVertex>
        {
        }

        sealed class TopologicallyOrderedEnumerable : ITopologicallyOrderedEnumerable
        {
            public TopologicallyOrderedEnumerable(Graph<TVertex> source, bool reverse)
            {
                Source = source ?? throw new ArgumentNullException(nameof(source));
                Reverse = reverse;
            }

            public Graph<TVertex> Source { get; }

            public bool Reverse { get; }

            public IEnumerator<TVertex> GetEnumerator()
            {
                var g = Source;
                if (Reverse)
                    g = g.GetTransposition();

                return g.OrderTopologicallyCore();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            IOrderedEnumerable<TVertex> IOrderedEnumerable<TVertex>.CreateOrderedEnumerable<TKey>(
                Func<TVertex, TKey> keySelector,
                IComparer<TKey>? comparer,
                bool descending)
                => new TopologicallyOrderedEnumerable<TKey>(keySelector, comparer, descending, parent: this);
        }

        sealed class TopologicallyOrderedEnumerable<TKey> : ITopologicallyOrderedEnumerable
        {
            readonly ITopologicallyOrderedEnumerable m_Parent;
            readonly Func<TVertex, TKey> m_KeySelector;
            readonly IComparer<TKey>? m_Comparer;
            readonly bool m_Descending;

            public TopologicallyOrderedEnumerable(
                Func<TVertex, TKey> keySelector,
                IComparer<TKey>? comparer,
                bool descending,
                ITopologicallyOrderedEnumerable parent)
            {
                m_KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
                m_Comparer = comparer;
                m_Descending = descending;
                m_Parent = parent;
            }

            IComparer<TVertex>? GetTopologicalComparer(IComparer<TVertex>? nextComparer, IEqualityComparer<TVertex> vertexEqualityComparer) =>
                new TopologicalComparer<TKey>(m_KeySelector, m_Comparer, m_Descending, nextComparer, vertexEqualityComparer);

            public IEnumerator<TVertex> GetEnumerator()
            {
                var list = new List<TopologicallyOrderedEnumerable<TKey>>();
                TopologicallyOrderedEnumerable root;

                for (TopologicallyOrderedEnumerable<TKey> i = this; ;)
                {
                    list.Add(i);

                    var parent = i.m_Parent;
                    if (parent is TopologicallyOrderedEnumerable<TKey> subsequent)
                    {
                        i = subsequent;
                    }
                    else
                    {
                        root = (TopologicallyOrderedEnumerable)parent;
                        break;
                    }
                }

                var source = root.Source;
                var vertexComparer = source.VertexComparer;

                IComparer<TVertex>? comparer = null;
                foreach (var i in list)
                    comparer = i.GetTopologicalComparer(comparer, vertexComparer);

                var g = source;
                if (root.Reverse)
                    g = g.GetTransposition();

                return g.OrderTopologicallyCore(comparer!);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            IOrderedEnumerable<TVertex> IOrderedEnumerable<TVertex>.CreateOrderedEnumerable<TSubsequentKey>(
                Func<TVertex, TSubsequentKey> keySelector,
                IComparer<TSubsequentKey>? comparer,
                bool descending)
                => new TopologicallyOrderedEnumerable<TSubsequentKey>(keySelector, comparer, descending, parent: this);
        }

        sealed class TopologicalComparer<TKey> : IComparer<TVertex>
        {
            readonly Func<TVertex, TKey> m_KeySelector;
            readonly IComparer<TKey> m_KeyComparer;
            readonly bool m_Descending;
            readonly IComparer<TVertex>? m_NextComparer;
            readonly AssociativeArray<TVertex, TKey> m_Keys;

            public TopologicalComparer(
                Func<TVertex, TKey> keySelector,
                IComparer<TKey>? keyComparer,
                bool descending,
                IComparer<TVertex>? nextComparer,
                IEqualityComparer<TVertex> vertexEqualityComparer)
            {
                m_KeySelector = keySelector;
                m_KeyComparer = keyComparer ?? Comparer<TKey>.Default;
                m_Descending = descending;
                m_NextComparer = nextComparer;
                m_Keys = new(vertexEqualityComparer);
            }

            public int Compare(TVertex? x, TVertex? y)
            {
                if (!m_Keys.TryGetValue(x!, out var xKey))
                    m_Keys[x!] = xKey = m_KeySelector(x!);
                if (!m_Keys.TryGetValue(y!, out var yKey))
                    m_Keys[y!] = yKey = m_KeySelector(y!);

                var c = m_KeyComparer.Compare(xKey, yKey);
                if (c == 0 && m_NextComparer is not null)
                    return m_NextComparer.Compare(x, y);
                return m_Descending ? -c : c;
            }
        }
    }
}

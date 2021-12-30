using Gapotchenko.FX.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        IEnumerator<TVertex> OrderTopologicallyCore()
        {
            var queue = new Queue<TVertex>();
            var transposition = GetTransposition();

            foreach (var vertex in transposition.Vertices)
            {
                // Find the nodes with no outcoming edges.
                if (transposition.GetVertexOutdegree(vertex) is 0)
                    queue.Enqueue(vertex);
            }

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                yield return vertex;

                foreach (var adjacentVertex in DestinationVerticesAdjacentTo(vertex))
                {
                    transposition.Edges.Remove(adjacentVertex, vertex);

                    if (transposition.GetVertexOutdegree(adjacentVertex) is 0)
                        queue.Enqueue(adjacentVertex);
                }
            }

            if (transposition.Edges.Count is > 0)
                throw new CircularDependencyException();
        }

        IEnumerator<TVertex> OrderTopologicallyByCore(IComparer<TVertex> comparer)
        {
            var queue = new PriorityQueue<TVertex, TVertex>(comparer);
            var graphTransposition = GetTransposition();

            foreach (var vertex in graphTransposition.Vertices)
            {
                // Find the nodes with no outcoming edges.
                if (graphTransposition.GetVertexOutdegree(vertex) is 0)
                    queue.Enqueue(vertex, vertex);
            }

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                yield return vertex;

                foreach (var adjacentVertex in DestinationVerticesAdjacentTo(vertex))
                {
                    graphTransposition.Edges.Remove(adjacentVertex, vertex);

                    if (graphTransposition.GetVertexOutdegree(adjacentVertex) is 0)
                        queue.Enqueue(adjacentVertex, adjacentVertex);
                }
            }

            if (graphTransposition.Edges.Count is > 0)
                throw new CircularDependencyException();
        }

        /// <inheritdoc />
        public IOrderedEnumerable<TVertex> OrderTopologically() =>
            new TopologicallyOrderedEnumerable(this);

        class TopologicallyOrderedEnumerable : IOrderedEnumerable<TVertex>
        {
            protected readonly Graph<TVertex> m_Source;

            public TopologicallyOrderedEnumerable(Graph<TVertex> source)
            {
                m_Source = source ?? throw new ArgumentNullException(nameof(source));
            }

            public IEnumerator<TVertex> GetEnumerator()
            {
                var comparer = GetTopologicalComparer(nextComparer: null);

                if (comparer is not null)
                    return m_Source.OrderTopologicallyByCore(comparer);
                else
                    return m_Source.OrderTopologicallyCore();
            }

            public virtual IComparer<TVertex>? GetTopologicalComparer(IComparer<TVertex>? nextComparer)
                => nextComparer;

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();

            IOrderedEnumerable<TVertex> IOrderedEnumerable<TVertex>.CreateOrderedEnumerable<TKey>(Func<TVertex, TKey> keySelector, IComparer<TKey>? comparer, bool descending) =>
                new TopologicallyOrderedEnumerable<TKey>(m_Source, keySelector, comparer, descending, parent: this);
        }

        sealed class TopologicallyOrderedEnumerable<TKey> : TopologicallyOrderedEnumerable
        {
            readonly TopologicallyOrderedEnumerable? m_Parent;
            readonly Func<TVertex, TKey> m_KeySelector;
            readonly IComparer<TKey> m_Comparer;
            readonly bool m_Descending;

            public TopologicallyOrderedEnumerable(
                Graph<TVertex> source,
                Func<TVertex, TKey> keySelector,
                IComparer<TKey>? comparer,
                bool descending,
                TopologicallyOrderedEnumerable? parent = default)
                : base(source)
            {
                m_KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
                m_Comparer = comparer ?? Comparer<TKey>.Default;
                m_Descending = descending;
                m_Parent = parent;
            }

            public override IComparer<TVertex>? GetTopologicalComparer(IComparer<TVertex>? nextComparer)
            {
                IComparer<TVertex>? comparer = new TopologicalComparer<TKey>(m_KeySelector, m_Comparer, m_Descending, nextComparer, m_Source.VertexComparer);
                if (m_Parent != null)
                {
                    comparer = m_Parent.GetTopologicalComparer(comparer);
                }
                return comparer;
            }
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
                IComparer<TKey> keyComparer,
                bool descending,
                IComparer<TVertex>? nextComparer,
                IEqualityComparer<TVertex> vertexEqualityComparer)
            {
                m_KeySelector = keySelector;
                m_KeyComparer = keyComparer;
                m_Descending = descending;
                m_NextComparer = nextComparer;
                m_Keys = new(vertexEqualityComparer);
            }

            public int Compare(TVertex? x, TVertex? y)
            {
                if (x is null)
                    throw new ArgumentNullException(nameof(x));
                if (y is null)
                    throw new ArgumentNullException(nameof(y));

                if (!m_Keys.TryGetValue(x, out var xKey))
                    m_Keys[x] = xKey = m_KeySelector(x);
                if (!m_Keys.TryGetValue(y, out var yKey))
                    m_Keys[y] = yKey = m_KeySelector(y);

                var c = m_KeyComparer.Compare(xKey, yKey);
                if (c == 0 && m_NextComparer is not null)
                    return m_NextComparer.Compare(x, y);
                return m_Descending ? -c : c;
            }
        }
    }
}

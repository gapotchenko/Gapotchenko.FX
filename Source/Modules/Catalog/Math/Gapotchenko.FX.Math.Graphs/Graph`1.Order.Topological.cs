// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc />
    public ITopologicallyOrderedEnumerable<TVertex> OrderTopologically() => new TopologicallyOrderedEnumerable(this);

    sealed class TopologicallyOrderedEnumerable(Graph<TVertex> source) : ITopologicallyOrderedEnumerable<TVertex>
    {
        public ITopologicallyOrderedEnumerable<TVertex> Reverse() =>
            new TopologicallyOrderedEnumerable(Source)
            {
                Reversed = !Reversed
            };

        public IEnumerator<TVertex> GetEnumerator()
        {
            var g = Source;
            if (Reversed)
                g = g.GetTransposition();
            return g.DoOrderTopologically();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Graph<TVertex> Source { get; } = source;

        public bool Reversed { get; private init; }

        IOrderedEnumerable<TVertex> IOrderedEnumerable<TVertex>.CreateOrderedEnumerable<TKey>(
            Func<TVertex, TKey> keySelector,
            IComparer<TKey>? comparer,
            bool descending) =>
            new NestedTopologicallyOrderedEnumerable<TKey>(this, keySelector, comparer, descending);
    }

    IEnumerator<TVertex> DoOrderTopologically()
    {
        var queue = new Queue<TVertex>();
        var graph = GetTransposition();

        foreach (var vertex in graph.Vertices)
        {
            // Find the nodes with no outgoing edges.
            if (graph.GetVertexOutdegree(vertex) == 0)
                queue.Enqueue(vertex);
        }

        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();

            yield return vertex;

            foreach (var adjacentVertex in OutgoingVerticesAdjacentTo(vertex))
            {
                graph.Edges.Remove(adjacentVertex, vertex);

                if (graph.GetVertexOutdegree(adjacentVertex) == 0)
                    queue.Enqueue(adjacentVertex);
            }
        }

        if (graph.Edges.Count > 0)
            throw new GraphCircularReferenceException();
    }

    sealed class NestedTopologicallyOrderedEnumerable<TKey>(
        IOrderedEnumerable<TVertex> parent,
        Func<TVertex, TKey> keySelector,
        IComparer<TKey>? comparer,
        bool descending) :
        IOrderedEnumerable<TVertex>
    {
        public IEnumerator<TVertex> GetEnumerator()
        {
            TopologicallyOrderedEnumerable root;
            var list = new List<NestedTopologicallyOrderedEnumerable<TKey>>();

            for (var i = this; ;)
            {
                list.Add(i);

                var parent = i.m_Parent;
                if (parent is NestedTopologicallyOrderedEnumerable<TKey> nested)
                {
                    i = nested;
                }
                else
                {
                    root = (TopologicallyOrderedEnumerable)parent;
                    break;
                }
            }

            var source = root.Source;

            IComparer<TVertex>? comparer = null;
            var vertexComparer = source.VertexComparer;
            foreach (var i in list)
                comparer = i.GetTopologicalComparer(comparer, vertexComparer);
            Debug.Assert(comparer != null);

            var g = source;
            if (root.Reversed)
                g = g.GetTransposition();

            return g.DoOrderTopologically(comparer);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IComparer<TVertex>? GetTopologicalComparer(IComparer<TVertex>? nextComparer, IEqualityComparer<TVertex> vertexEqualityComparer) =>
            new TopologicalComparer<TKey>(m_KeySelector, comparer, descending, nextComparer, vertexEqualityComparer);

        readonly Func<TVertex, TKey> m_KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        readonly IOrderedEnumerable<TVertex> m_Parent = parent;

        IOrderedEnumerable<TVertex> IOrderedEnumerable<TVertex>.CreateOrderedEnumerable<TSubsequentKey>(
            Func<TVertex, TSubsequentKey> keySelector,
            IComparer<TSubsequentKey>? comparer,
            bool descending) =>
            new NestedTopologicallyOrderedEnumerable<TSubsequentKey>(this, keySelector, comparer, descending);
    }

    IEnumerator<TVertex> DoOrderTopologically(IComparer<TVertex> comparer)
    {
        var queue = new PriorityQueue<TVertex, TVertex>(comparer);
        var graph = GetTransposition();

        foreach (var vertex in graph.Vertices)
        {
            // Find the nodes with no outgoing edges.
            if (graph.GetVertexOutdegree(vertex) == 0)
                queue.Enqueue(vertex, vertex);
        }

        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();

            yield return vertex;

            foreach (var adjacentVertex in OutgoingVerticesAdjacentTo(vertex))
            {
                graph.Edges.Remove(adjacentVertex, vertex);

                if (graph.GetVertexOutdegree(adjacentVertex) == 0)
                    queue.Enqueue(adjacentVertex, adjacentVertex);
            }
        }

        if (graph.Edges.Count > 0)
            throw new GraphCircularReferenceException();
    }

    sealed class TopologicalComparer<TKey>(
        Func<TVertex, TKey> keySelector,
        IComparer<TKey>? keyComparer,
        bool descending,
        IComparer<TVertex>? nextComparer,
        IEqualityComparer<TVertex> vertexEqualityComparer) : IComparer<TVertex>
    {
        public int Compare(TVertex? x, TVertex? y)
        {
            if (!m_Keys.TryGetValue(x!, out var xKey))
                m_Keys[x!] = xKey = keySelector(x!);
            if (!m_Keys.TryGetValue(y!, out var yKey))
                m_Keys[y!] = yKey = keySelector(y!);

            var c = m_KeyComparer.Compare(xKey, yKey);
            if (c == 0 && nextComparer is not null)
                return nextComparer.Compare(x, y);

            return descending ? -c : c;
        }

        readonly IComparer<TKey> m_KeyComparer = keyComparer ?? Comparer<TKey>.Default;
        readonly AssociativeArray<TVertex, TKey> m_Keys = new(vertexEqualityComparer);
    }
}

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
    public ITopologicallyOrderedEnumerable<TVertex> OrderTopologically() => new PrimaryTopologicallyOrderedEnumerable(this);

    #region Topologically ordered enumerable

    sealed class PrimaryTopologicallyOrderedEnumerable(Graph<TVertex> source) : ITopologicallyOrderedEnumerable<TVertex>
    {
        public ITopologicallyOrderedEnumerable<TVertex> Reverse() =>
            new PrimaryTopologicallyOrderedEnumerable(Source)
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
            new SubsequentTopologicallyOrderedEnumerable<TKey>(this, keySelector, comparer, descending);
    }

    sealed class SubsequentTopologicallyOrderedEnumerable<TKey>(
        IOrderedEnumerable<TVertex> parent,
        Func<TVertex, TKey> keySelector,
        IComparer<TKey>? comparer,
        bool descending) :
        IOrderedEnumerable<TVertex>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TVertex> GetEnumerator()
        {
            PrimaryTopologicallyOrderedEnumerable primary;
            var subsequentList = new List<SubsequentTopologicallyOrderedEnumerable<TKey>>();

            for (var i = this; ;)
            {
                subsequentList.Add(i);

                var parent = i.m_Parent;
                if (parent is SubsequentTopologicallyOrderedEnumerable<TKey> subsequent)
                {
                    i = subsequent;
                }
                else
                {
                    primary = (PrimaryTopologicallyOrderedEnumerable)parent;
                    break;
                }
            }

            var g = primary.Source;

            // Apply subsequent orders.
            IComparer<TVertex>? comparer = null;
            var vertexComparer = g.VertexComparer;
            foreach (var subsequent in subsequentList)
                comparer = subsequent.GetTopologicalComparer(comparer, vertexComparer);
            Debug.Assert(comparer != null);

            // Apply primary order.
            if (primary.Reversed)
                g = g.GetTransposition();

            return g.DoOrderTopologically(comparer);
        }

        IComparer<TVertex>? GetTopologicalComparer(IComparer<TVertex>? subsequentComparer, IEqualityComparer<TVertex> vertexEqualityComparer) =>
            new TopologicalComparer(m_KeySelector, comparer, descending, subsequentComparer, vertexEqualityComparer);

        readonly Func<TVertex, TKey> m_KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        readonly IOrderedEnumerable<TVertex> m_Parent = parent;

        IOrderedEnumerable<TVertex> IOrderedEnumerable<TVertex>.CreateOrderedEnumerable<TSubsequentKey>(
            Func<TVertex, TSubsequentKey> keySelector,
            IComparer<TSubsequentKey>? comparer,
            bool descending) =>
            new SubsequentTopologicallyOrderedEnumerable<TSubsequentKey>(this, keySelector, comparer, descending);

        sealed class TopologicalComparer(
            Func<TVertex, TKey> keySelector,
            IComparer<TKey>? keyComparer,
            bool descending,
            IComparer<TVertex>? subsequentComparer,
            IEqualityComparer<TVertex> vertexEqualityComparer) :
            IComparer<TVertex>
        {
            public int Compare(TVertex? x, TVertex? y)
            {
                if (!m_Keys.TryGetValue(x!, out var xKey))
                    m_Keys[x!] = xKey = keySelector(x!);
                if (!m_Keys.TryGetValue(y!, out var yKey))
                    m_Keys[y!] = yKey = keySelector(y!);

                int c = m_KeyComparer.Compare(xKey, yKey);
                if (c == 0 && subsequentComparer is not null)
                    return subsequentComparer.Compare(x, y);

                return descending ? -c : c;
            }

            readonly IComparer<TKey> m_KeyComparer = keyComparer ?? Comparer<TKey>.Default;
            readonly AssociativeArray<TVertex, TKey> m_Keys = new(vertexEqualityComparer);
        }
    }

    #endregion

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

            // We use the former expression to avoid taking a snapshot of the graph being modified:
            // this.OutgoingVerticesAdjacentTo(vertex) = graph.IncomingVerticesAdjacentTo(vertex)
            // given that "graph" is a transposition of "this" graph.
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

    IEnumerator<TVertex> DoOrderTopologically(IComparer<TVertex>? comparer)
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

            // We use the former expression to avoid taking a snapshot of the graph being modified:
            // this.OutgoingVerticesAdjacentTo(vertex) = graph.IncomingVerticesAdjacentTo(vertex)
            // given that "graph" is a transposition of "this" graph.
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
}

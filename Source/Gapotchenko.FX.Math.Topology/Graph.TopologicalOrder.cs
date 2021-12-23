using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <inheritdoc />
        public IEnumerable<T> TopologicalOrder()
        {
            var queue = new Queue<T>();
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

                foreach (var adjacentVertex in VerticesAdjacentTo(vertex))
                {
                    transposition.Edges.Remove(adjacentVertex, vertex);

                    if (transposition.GetVertexOutdegree(adjacentVertex) is 0)
                        queue.Enqueue(adjacentVertex);
                }
            }

            if (transposition.Edges.Count is > 0)
                throw new CircularDependencyException();
        }

        IEnumerator<T> TopologicalOrderBy(IComparer<T> comparer)
        {
            var queue = new PriorityQueue<T, T>(comparer);
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

                foreach (var adjacentVertex in VerticesAdjacentTo(vertex))
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
        public IOrderedEnumerable<T> TopologicalOrderBy<TKey>(Func<T, TKey> keySelector, IComparer<TKey>? comparer = default) =>
            new TopologicalOrderedEnumerable<TKey>(this, keySelector, comparer, descending: false);

        /// <inheritdoc />
        public IOrderedEnumerable<T> TopologicalOrderByDescending<TKey>(Func<T, TKey> keySelector, IComparer<TKey>? comparer = default) =>
            new TopologicalOrderedEnumerable<TKey>(this, keySelector, comparer, descending: true);

        abstract class TopologicalOrderedEnumerable : IOrderedEnumerable<T>
        {
            protected readonly Graph<T> _source;

            protected TopologicalOrderedEnumerable(Graph<T> source)
            {
                _source = source ?? throw new ArgumentNullException(nameof(source));
            }

            public IEnumerator<T> GetEnumerator()
            {
                var comparer = GetTopologicalComparer(nextComparer: null);
                return _source.TopologicalOrderBy(comparer);
            }

            public abstract IComparer<T> GetTopologicalComparer(IComparer<T>? nextComparer);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            IOrderedEnumerable<T> IOrderedEnumerable<T>.CreateOrderedEnumerable<TKey>(Func<T, TKey> keySelector, IComparer<TKey>? comparer, bool descending) =>
                new TopologicalOrderedEnumerable<TKey>(_source, keySelector, comparer, descending, parent: this);
        }

        sealed class TopologicalOrderedEnumerable<TKey> : TopologicalOrderedEnumerable
        {
            readonly TopologicalOrderedEnumerable? _parent;
            readonly Func<T, TKey> _keySelector;
            readonly IComparer<TKey> _comparer;
            readonly bool _descending;

            public TopologicalOrderedEnumerable(
                Graph<T> source,
                Func<T, TKey> keySelector,
                IComparer<TKey>? comparer,
                bool descending,
                TopologicalOrderedEnumerable? parent = default)
                : base(source)
            {
                _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
                _comparer = comparer ?? Comparer<TKey>.Default;
                _descending = descending;
                _parent = parent;
            }

            public override IComparer<T> GetTopologicalComparer(IComparer<T>? nextComparer)
            {
                IComparer<T> comparer = new TopologicalComparer<TKey>(_keySelector, _comparer, _descending, nextComparer, _source.Comparer);
                if (_parent != null)
                {
                    comparer = _parent.GetTopologicalComparer(comparer);
                }
                return comparer;
            }
        }

        sealed class TopologicalComparer<TKey> : IComparer<T>
        {
            readonly Func<T, TKey> _keySelector;
            readonly IComparer<TKey> _keyComparer;
            readonly bool _descending;
            readonly IComparer<T>? _nextComparer;
            readonly Dictionary<T, TKey> _keys;

            public TopologicalComparer(
                Func<T, TKey> keySelector,
                IComparer<TKey> keyComparer,
                bool descending,
                IComparer<T>? nextComparer,
                IEqualityComparer<T> vertexEqualityComparer)
            {
                _keySelector = keySelector;
                _keyComparer = keyComparer;
                _descending = descending;
                _nextComparer = nextComparer;
                _keys = new(vertexEqualityComparer);
            }

            public int Compare(T? x, T? y)
            {
                if (x is null)
                    throw new ArgumentNullException(nameof(x));
                if (y is null)
                    throw new ArgumentNullException(nameof(y));

                if (!_keys.TryGetValue(x, out var xKey))
                    _keys[x] = xKey = _keySelector(x);
                if (!_keys.TryGetValue(y, out var yKey))
                    _keys[y] = yKey = _keySelector(y);

                var c = _keyComparer.Compare(xKey, yKey);
                if (c == 0 && _nextComparer is not null)
                    return _nextComparer.Compare(x, y);
                return _descending ? -c : c;
            }
        }
    }
}

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
            new TopologicalOrderedEnumerable(this);

        class TopologicalOrderedEnumerable : IOrderedEnumerable<TVertex>
        {
            protected readonly Graph<TVertex> _source;

            public TopologicalOrderedEnumerable(Graph<TVertex> source)
            {
                _source = source ?? throw new ArgumentNullException(nameof(source));
            }

            public IEnumerator<TVertex> GetEnumerator()
            {
                var comparer = GetTopologicalComparer(nextComparer: null);

                if (comparer is not null)
                    return _source.OrderTopologicallyByCore(comparer);
                else
                    return _source.OrderTopologicallyCore();
            }

            public virtual IComparer<TVertex>? GetTopologicalComparer(IComparer<TVertex>? nextComparer)
                => nextComparer;

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();

            IOrderedEnumerable<TVertex> IOrderedEnumerable<TVertex>.CreateOrderedEnumerable<TKey>(Func<TVertex, TKey> keySelector, IComparer<TKey>? comparer, bool descending) =>
                new TopologicalOrderedEnumerable<TKey>(_source, keySelector, comparer, descending, parent: this);
        }

        sealed class TopologicalOrderedEnumerable<TKey> : TopologicalOrderedEnumerable
        {
            readonly TopologicalOrderedEnumerable? _parent;
            readonly Func<TVertex, TKey> _keySelector;
            readonly IComparer<TKey> _comparer;
            readonly bool _descending;

            public TopologicalOrderedEnumerable(
                Graph<TVertex> source,
                Func<TVertex, TKey> keySelector,
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

            public override IComparer<TVertex>? GetTopologicalComparer(IComparer<TVertex>? nextComparer)
            {
                IComparer<TVertex>? comparer = new TopologicalComparer<TKey>(_keySelector, _comparer, _descending, nextComparer, _source.VertexComparer);
                if (_parent != null)
                {
                    comparer = _parent.GetTopologicalComparer(comparer);
                }
                return comparer;
            }
        }

        sealed class TopologicalComparer<TKey> : IComparer<TVertex>
        {
            readonly Func<TVertex, TKey> _keySelector;
            readonly IComparer<TKey> _keyComparer;
            readonly bool _descending;
            readonly IComparer<TVertex>? _nextComparer;
            readonly AssociativeArray<TVertex, TKey> _keys;

            public TopologicalComparer(
                Func<TVertex, TKey> keySelector,
                IComparer<TKey> keyComparer,
                bool descending,
                IComparer<TVertex>? nextComparer,
                IEqualityComparer<TVertex> vertexEqualityComparer)
            {
                _keySelector = keySelector;
                _keyComparer = keyComparer;
                _descending = descending;
                _nextComparer = nextComparer;
                _keys = new(vertexEqualityComparer);
            }

            public int Compare(TVertex? x, TVertex? y)
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

using System;
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
            var order = new List<T>();
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

                order.Add(vertex);

                foreach (var adjacentVertex in VerticesAdjacentTo(vertex))
                {
                    transposition.Edges.Remove(adjacentVertex, vertex);

                    if (transposition.GetVertexOutdegree(adjacentVertex) is 0)
                        queue.Enqueue(adjacentVertex);
                }
            }

            if (transposition.Edges.Count is > 0)
                throw new CircularDependencyException();

            return order;
        }

        /// <inheritdoc />
        public IEnumerable<T> TopologicalOrderBy<TKey>(Func<T, TKey> keySelector, IComparer<TKey>? comparer = default)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var queue = new PriorityQueue<T, TKey>(comparer);
            var order = new List<T>();
            var transposition = GetTransposition();

            var keys = Vertices
                .ToDictionary(v => v, v => keySelector(v), Comparer);

            foreach (var vertex in transposition.Vertices)
            {
                // Find the nodes with no outcoming edges.
                if (transposition.GetVertexOutdegree(vertex) is 0)
                    queue.Enqueue(vertex, keys[vertex]);
            }

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                order.Add(vertex);

                foreach (var adjacentVertex in VerticesAdjacentTo(vertex))
                {
                    transposition.Edges.Remove(adjacentVertex, vertex);

                    if (transposition.GetVertexOutdegree(adjacentVertex) is 0)
                        queue.Enqueue(adjacentVertex, keys[adjacentVertex]);
                }
            }

            if (transposition.Edges.Count is > 0)
                throw new CircularDependencyException();

            return order;
        }
    }
}

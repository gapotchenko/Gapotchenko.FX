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
        public IEnumerable<T> TopologicalOrder(IComparer<T> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            var queue = new PriorityQueue<T, T>(comparer);
            var order = new List<T>();
            var transposition = GetTransposition();

            foreach (var vertex in transposition.Vertices)
            {
                // Find the nodes with no outcoming edges.
                if (transposition.GetVertexOutdegree(vertex) is 0)
                    queue.Enqueue(vertex, vertex);
            }

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                order.Add(vertex);

                foreach (var adjacentVertex in VerticesAdjacentTo(vertex))
                {
                    transposition.Edges.Remove(adjacentVertex, vertex);

                    if (transposition.GetVertexOutdegree(adjacentVertex) is 0)
                        queue.Enqueue(adjacentVertex, adjacentVertex);
                }
            }

            if (transposition.Edges.Count is > 0)
                throw new CircularDependencyException();

            return order;
        }
    }
}

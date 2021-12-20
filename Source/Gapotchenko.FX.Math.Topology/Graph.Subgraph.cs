using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <summary>
        /// Gets a vertex-induced subgraph of the current graph.
        /// </summary>
        /// <param name="vertices">The vertices to induce the subgraph from.</param>
        /// <returns>The vertex-induced subgraph of the current graph.</returns>
        public Graph<T> GetSubgraph(IEnumerable<T> vertices)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));

            var subgraph = NewGraph();

            foreach (var vertex in vertices)
            {
                if (Vertices.Contains(vertex))
                {
                    subgraph.Vertices.Add(vertex);
                }
            }

            foreach (var vertex in subgraph.Vertices)
            {
                var adjacentTo = VerticesAdjacentTo(vertex);
                foreach (var adjacentVertex in adjacentTo)
                {
                    if (subgraph.Vertices.Contains(adjacentVertex))
                    {
                        subgraph.Edges.Add((vertex, adjacentVertex));
                    }
                }
            }

            return subgraph;
        }

        IGraph<T> IGraph<T>.GetSubgraph(IEnumerable<T> vertices) => GetSubgraph(vertices);

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetSubgraph(IEnumerable<T> vertices) => GetSubgraph(vertices);

        /// <summary>
        /// Gets an edge-induced subgraph of the current graph.
        /// </summary>
        /// <param name="edges">The edges to induce the subgraph from.</param>
        /// <returns>The edge-induced subgraph of the current graph.</returns>
        public Graph<T> GetSubgraph(IEnumerable<GraphEdge<T>> edges)
        {
            if (edges == null)
                throw new ArgumentNullException(nameof(edges));

            var subgraph = NewGraph();

            foreach (var edge in edges)
            {
                if (Edges.Contains(edge))
                {
                    subgraph.Edges.Add(edge);
                }
            }

            return subgraph;
        }

        IGraph<T> IGraph<T>.GetSubgraph(IEnumerable<GraphEdge<T>> edges) => GetSubgraph(edges);

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetSubgraph(IEnumerable<GraphEdge<T>> edges) => GetSubgraph(edges);
    }
}

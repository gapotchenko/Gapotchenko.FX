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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        IGraph<T> IGraph<T>.GetSubgraph(IEnumerable<GraphEdge<T>> edges) => GetSubgraph(edges);

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetSubgraph(IEnumerable<GraphEdge<T>> edges) => GetSubgraph(edges);

        /// <summary>
        /// Gets an induced subgraph of the current graph.
        /// </summary>
        /// <param name="vertices">The vertices to induce the subgraph from.</param>
        /// <param name="edges">The edges to induce the subgraph from.</param>
        /// <returns>The induced subgraph of the current graph.</returns>
        public Graph<T> GetSubgraph(IEnumerable<T> vertices, IEnumerable<GraphEdge<T>> edges)
        {
            throw new NotImplementedException();
        }

        IGraph<T> IGraph<T>.GetSubgraph(IEnumerable<T> vertices, IEnumerable<GraphEdge<T>> edges) => GetSubgraph(vertices, edges);

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetSubgraph(IEnumerable<T> vertices, IEnumerable<GraphEdge<T>> edges) => GetSubgraph(vertices, edges);
    }
}

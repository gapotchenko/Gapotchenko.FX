using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        /// <inheritdoc/>
        public void Subgraph(IEnumerable<TVertex> vertices)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a vertex-induced subgraph of the current graph.
        /// </summary>
        /// <param name="vertices">The vertices to induce the subgraph from.</param>
        /// <returns>The vertex-induced subgraph of the current graph.</returns>
        public Graph<TVertex> GetSubgraph(IEnumerable<TVertex> vertices)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));

            var subgraph = NewGraph();

            var subgraphVertices = subgraph.Vertices;

            foreach (var vertex in vertices)
            {
                if (Vertices.Contains(vertex))
                    subgraphVertices.Add(vertex);
            }

            var edgesToAdd = new List<GraphEdge<TVertex>>();

            foreach (var vertex in subgraphVertices)
            {
                foreach (var adjacentVertex in DestinationVerticesAdjacentTo(vertex))
                {
                    if (subgraphVertices.Contains(adjacentVertex))
                        edgesToAdd.Add(GraphEdge.Create(vertex, adjacentVertex));
                }
            }

            subgraph.Edges.UnionWith(edgesToAdd);

            return subgraph;
        }

        IGraph<TVertex> IGraph<TVertex>.GetSubgraph(IEnumerable<TVertex> vertices) => GetSubgraph(vertices);

        IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.GetSubgraph(IEnumerable<TVertex> vertices) => GetSubgraph(vertices);

        /// <inheritdoc/>
        public void Subgraph(IEnumerable<GraphEdge<TVertex>> edges)
        {
            if (edges == null)
                throw new ArgumentNullException(nameof(edges));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an edge-induced subgraph of the current graph.
        /// </summary>
        /// <param name="edges">The edges to induce the subgraph from.</param>
        /// <returns>The edge-induced subgraph of the current graph.</returns>
        public Graph<TVertex> GetSubgraph(IEnumerable<GraphEdge<TVertex>> edges)
        {
            if (edges == null)
                throw new ArgumentNullException(nameof(edges));

            var subgraph = NewGraph();

            foreach (var edge in edges)
                if (Edges.Contains(edge))
                    subgraph.Edges.Add(edge);

            return subgraph;
        }

        IGraph<TVertex> IGraph<TVertex>.GetSubgraph(IEnumerable<GraphEdge<TVertex>> edges) => GetSubgraph(edges);

        IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.GetSubgraph(IEnumerable<GraphEdge<TVertex>> edges) => GetSubgraph(edges);
    }
}

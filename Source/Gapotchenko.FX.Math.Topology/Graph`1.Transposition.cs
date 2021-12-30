using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        /// <inheritdoc/>
        public void Transpose()
        {
            var edges = Edges.ToList();
            var vertices = Vertices.ToList();

            Clear();
            TransposeCore(this, edges, vertices);
        }

        /// <summary>
        /// Gets a graph transposition by reversing its edge directions.
        /// </summary>
        /// <returns>The transposed graph.</returns>
        public Graph<TVertex> GetTransposition()
        {
            var graph = NewGraph();
            TransposeCore(graph, Edges, Vertices);
            return graph;
        }

        static void TransposeCore(Graph<TVertex> graph, IEnumerable<GraphEdge<TVertex>> edges, IEnumerable<TVertex> vertices)
        {
            foreach (var edge in edges)
                graph.Edges.Add(edge.Reverse());

            foreach (var vertex in vertices)
                graph.Vertices.Add(vertex);
        }

        IGraph<TVertex> IGraph<TVertex>.GetTransposition() => GetTransposition();

        IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.GetTransposition() => GetTransposition();
    }
}

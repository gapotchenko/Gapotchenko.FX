using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
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
        public Graph<T> GetTransposition()
        {
            var graph = NewGraph();
            TransposeCore(graph, Edges, Vertices);
            return graph;
        }

        static void TransposeCore(Graph<T> graph, IEnumerable<GraphEdge<T>> edges, IEnumerable<T> vertices)
        {
            foreach (var edge in edges)
                graph.AddEdge(edge.To, edge.From);

            foreach (var vertex in vertices)
                graph.Vertices.Add(vertex);
        }

        IGraph<T> IGraph<T>.GetTransposition() => GetTransposition();

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetTransposition() => GetTransposition();
    }
}

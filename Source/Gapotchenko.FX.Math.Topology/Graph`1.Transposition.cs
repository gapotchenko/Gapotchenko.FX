namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public void Transpose()
    {
        if (!IsDirected)
            return;

        if (HasReverseAdjacencyList)
        {
            MathEx.Swap(ref m_AdjacencyList, ref m_ReverseAdjacencyList);
        }
        else
        {
            var edges = Edges.ToList();
            var vertices = Vertices.ToList();

            Clear();
            TransposeCore(this, edges, vertices);
        }
    }

    /// <summary>
    /// Gets a graph transposition by reversing its edge directions.
    /// </summary>
    /// <returns>The transposed graph.</returns>
    public Graph<TVertex> GetTransposition()
    {
        var graph = NewGraph();
        if (IsDirected)
            TransposeCore(graph, Edges, Vertices);
        return graph;
    }

    static void TransposeCore(Graph<TVertex> graph, IEnumerable<GraphEdge<TVertex>> edges, IEnumerable<TVertex> vertices)
    {
        graph.Edges.UnionWith(edges.Select(x => x.Reverse()));
        graph.Vertices.UnionWith(vertices);
    }

    IGraph<TVertex> IGraph<TVertex>.GetTransposition() => GetTransposition();

    IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.GetTransposition() => GetTransposition();
}

namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public void Transpose()
    {
        if (!IsDirected)
            return;

        var t = m_AdjacencyList;
        m_AdjacencyList = ReverseAdjacencyListCore;
        m_ReverseAdjacencyList = t;
    }

    /// <inheritdoc cref="IGraph{TVertex}.GetTransposition"/>
    public Graph<TVertex> GetTransposition()
    {
        var graph = NewGraph();
        if (IsDirected)
        {
            graph.Edges.UnionWith(Edges.Select(x => x.Reverse()));
            graph.Vertices.UnionWith(Vertices);
        }
        return graph;
    }

    IGraph<TVertex> IGraph<TVertex>.GetTransposition() => GetTransposition();

    IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.GetTransposition() => GetTransposition();
}

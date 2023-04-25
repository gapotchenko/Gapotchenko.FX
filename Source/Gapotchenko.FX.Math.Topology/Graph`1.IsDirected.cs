using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <summary>
    /// Gets or initializes a value indicating whether the current graph is directed.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsDirected
    {
        get => !m_Flags[F_IsUndirected];
        init
        {
            if (m_Flags[F_IsDirected_Initialized])
                throw new InvalidOperationException("Graph direction is already initialized.");

            if (!value)
                ConvertToUndirected();

            m_Flags[F_IsDirected_Initialized] = true;
        }
    }

    void ConvertToUndirected()
    {
        if (m_EdgeComparer != null)
            throw new InvalidOperationException("Graph direction cannot be changed at this stage.");

        var edges = Edges;
        var savedEdges = edges.ToList();

        m_Flags[F_IsUndirected] = true;
        m_ReverseAdjacencyList = null;

        edges.Clear();
        edges.UnionWith(savedEdges);
    }
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <summary>
    /// Gets or initializes a value indicating whether the current graph is directed.
    /// </summary>
    /// <exception cref="InvalidOperationException">The graph direction is already initialized.</exception>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsDirected
    {
        get => !m_Flags[F_IsUndirected];
        init
        {
            if (m_Flags[F_IsUndirected_Initialized])
                throw new InvalidOperationException("The graph direction is already initialized.");

            if (!value)
                ConvertToUndirected();

            m_Flags[F_IsUndirected_Initialized] = true;
        }
    }

    void ConvertToUndirected()
    {
        if (m_EdgeComparer != null)
            throw new InvalidOperationException("The graph direction can no longer be changed.");

        var edges = Edges;
        var savedEdges = edges.ToList();

        m_Flags[F_IsUndirected] = true;

        edges.Clear();
        edges.UnionWith(savedEdges);
    }
}

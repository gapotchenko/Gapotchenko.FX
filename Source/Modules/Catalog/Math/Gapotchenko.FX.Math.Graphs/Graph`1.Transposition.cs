﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public void Transpose()
    {
        if (IsDirected)
            (m_AdjacencyList, m_ReverseAdjacencyList) = (ReverseAdjacencyListCore, m_AdjacencyList);
    }

    /// <inheritdoc cref="IGraph{TVertex}.GetTransposition"/>
    public Graph<TVertex> GetTransposition()
    {
        if (IsDirected)
        {
            var graph = NewGraph();
            graph.Edges.UnionWith(Edges.Select(x => x.Reverse()));
            graph.Vertices.UnionWith(Vertices);
            graph.CopyCacheFrom(this);
            return graph;
        }
        else
        {
            return Clone();
        }
    }

    IGraph<TVertex> IGraph<TVertex>.GetTransposition() => GetTransposition();

    IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.GetTransposition() => GetTransposition();
}

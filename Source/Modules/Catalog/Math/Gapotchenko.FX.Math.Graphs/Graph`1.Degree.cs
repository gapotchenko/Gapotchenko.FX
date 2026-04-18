// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Collections.Generic;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc />
    public int GetVertexIndegree(TVertex vertex)
    {
        if (IsDirected)
            return GetVertexIndegreeCore(vertex);
        else
            return GetVertexDegree(vertex);
    }

    /// <inheritdoc />
    public int GetVertexOutdegree(TVertex vertex)
    {
        if (IsDirected)
            return GetVertexOutdegreeCore(vertex);
        else
            return GetVertexDegree(vertex);
    }

    /// <inheritdoc />
    public int GetVertexDegree(TVertex vertex) =>
        GetVertexIndegreeCore(vertex) + GetVertexOutdegreeCore(vertex);

    int GetVertexIndegreeCore(TVertex vertex)
    {
        if (m_ReverseAdjacencyList != null)
            return GetVertexOutdegreeCore(m_ReverseAdjacencyList, vertex);
        else
            return m_AdjacencyList.Count(kv => kv.Value?.Contains(vertex) == true);
    }

    int GetVertexOutdegreeCore(TVertex vertex) => GetVertexOutdegreeCore(m_AdjacencyList, vertex);

    static int GetVertexOutdegreeCore(AssociativeArray<TVertex, AdjacencyRow?> adjacencyList, TVertex vertex) =>
        adjacencyList.TryGetValue(vertex, out var adjacencyRow) ?
            adjacencyRow?.Count ?? 0 :
            0;
}

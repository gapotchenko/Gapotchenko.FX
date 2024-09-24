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
        if (m_ReverseAdjacencyList != null)
        {
            return GetVertexOutdegreeCore(m_ReverseAdjacencyList, vertex);
        }
        else
        {
            return m_AdjacencyList
                .Where(kv => kv.Value?.Contains(vertex) == true)
                .Count();
        }
    }

    /// <inheritdoc />
    public int GetVertexOutdegree(TVertex vertex) => GetVertexOutdegreeCore(m_AdjacencyList, vertex);

    static int GetVertexOutdegreeCore(AssociativeArray<TVertex, AdjacencyRow?> adjacencyList, TVertex vertex) =>
        adjacencyList.TryGetValue(vertex, out var adjacencyRow) ?
            adjacencyRow?.Count ?? 0 :
            0;

    /// <inheritdoc />
    public int GetVertexDegree(TVertex vertex) =>
        GetVertexIndegree(vertex) + GetVertexOutdegree(vertex);
}

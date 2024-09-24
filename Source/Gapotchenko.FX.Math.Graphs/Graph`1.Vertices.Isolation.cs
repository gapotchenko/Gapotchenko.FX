// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Collections.Generic;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public bool IsVertexIsolated(TVertex vertex)
    {
        static bool IsAssociatedKeyVertex(AssociativeArray<TVertex, AdjacencyRow?> adjList, TVertex vertex) =>
            adjList.TryGetValue(vertex, out var adjRow) &&
            adjRow?.Count > 0;

        var adjList = m_AdjacencyList;

        if (IsAssociatedKeyVertex(adjList, vertex))
            return false;

        var revAdjList = m_ReverseAdjacencyList;
        if (revAdjList != null)
        {
            if (IsAssociatedKeyVertex(revAdjList, vertex))
                return false;
        }
        else
        {
            foreach (var i in adjList)
            {
                var adjRow = i.Value;
                if (adjRow == null)
                    continue;

                if (adjRow.Contains(vertex))
                    return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public IEnumerable<TVertex> IsolatedVertices
    {
        get
        {
            var map = new AssociativeArray<TVertex, bool>(VertexComparer);

            foreach (var (v, adjRow) in m_AdjacencyList)
            {
                if (adjRow?.Count > 0)
                {
                    foreach (var u in adjRow)
                        map[u] = false;
                    map[v] = false;
                }
                else if (!map.ContainsKey(v))
                {
                    map.Add(v, true);
                }
            }

            return map.Where(x => x.Value).Select(x => x.Key);
        }
    }
}

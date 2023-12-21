// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public void ReduceReflexes()
    {
        bool hasChanges = false;

        foreach (var i in m_AdjacencyList)
        {
            var adjRow = i.Value;
            if (adjRow == null)
                continue;

            hasChanges |= adjRow.Remove(i.Key);
        }

        if (hasChanges)
            InvalidateCache();
    }

    /// <inheritdoc cref="IGraph{TVertex}.GetReflexiveReduction"/>
    public Graph<TVertex> GetReflexiveReduction()
    {
        var graph = Clone();
        graph.ReduceReflexes();
        return graph;
    }

    IGraph<TVertex> IGraph<TVertex>.GetReflexiveReduction() => GetReflexiveReduction();

    IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.GetReflexiveReduction() => GetReflexiveReduction();
}

﻿namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public void ReduceTransitions()
    {
        bool hasChanges = false;

        foreach (var i in m_AdjacencyList)
        {
            var adjRow = i.Value;
            if (adjRow == null)
                continue;

            var from = i.Key;

            List<TVertex>? removeList = null;

            foreach (var to in adjRow)
            {
                if (HasTransitivePath(from, to))
                {
                    removeList ??= new List<TVertex>();
                    removeList.Add(to);
                }
            }

            if (removeList != null)
            {
                adjRow.ExceptWith(removeList);
                hasChanges = true;
            }
        }

        if (hasChanges)
            InvalidateCache();
    }

    /// <inheritdoc cref="IGraph{TVertex}.GetTransitiveReduction"/>
    public Graph<TVertex> GetTransitiveReduction()
    {
        var graph = Clone();
        graph.ReduceTransitions();
        return graph;
    }

    IGraph<TVertex> IGraph<TVertex>.GetTransitiveReduction() => GetTransitiveReduction();

    IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.GetTransitiveReduction() => GetTransitiveReduction();
}

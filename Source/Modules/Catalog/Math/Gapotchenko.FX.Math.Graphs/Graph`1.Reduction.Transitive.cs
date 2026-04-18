// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public void ReduceTransitions()
    {
        if (IsDirected)
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
                        (removeList ??= []).Add(to);
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
        else
        {
            var edges = Edges;

            foreach (var edge in edges.ToList())
            {
                // In undirected graphs, multiple edges can appear redundant
                // against the same original state while depending on each other
                // collectively to preserve connectivity. Remove them one by one.
                if (!edges.Remove(edge))
                    continue;

                if (!HasPath(edge.From, edge.To))
                    edges.Add(edge);
            }
        }
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

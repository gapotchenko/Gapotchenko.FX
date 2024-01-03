// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology;

/// <summary>
/// Represents a strongly-typed directional graph of objects.
/// </summary>
/// <inheritdoc cref="IGraph{TVertex}"/>
public partial class Graph<TVertex> : IGraph<TVertex>
{
    // This class is partial. Please take a look at the neighboring source files.

    /// <inheritdoc/>
    public void Clear()
    {
        if (m_AdjacencyList.Count == 0)
            return;

        m_AdjacencyList.Clear();
        InvalidateCacheCore();

#if !TFF_DICTIONARY_ENUMERATION_REMOVE_ALLOWED
        IncrementVersion();
#endif
    }

    /// <inheritdoc cref="IGraph{TVertex}.ConnectedComponents"/>
    public IEnumerable<Graph<TVertex>> ConnectedComponents
    {
        get
        {
            var vertices = Vertices;

            var comparer = vertices.Comparer;
            var seen = new HashSet<TVertex>(comparer);

            foreach (var v in vertices)
            {
                if (seen.Contains(v))
                    continue;

                var connectedVertices = new HashSet<TVertex>(comparer) { v };

                var queue = new Queue<TVertex>();
                queue.Enqueue(v);
                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    foreach (var adjacent in VerticesAdjacentTo(current))
                        if (connectedVertices.Add(adjacent))
                            queue.Enqueue(adjacent);
                }

                seen.AddRange(connectedVertices);

                var connectedSubgraph = GetSubgraph(connectedVertices);
                yield return connectedSubgraph;
            }
        }
    }

    IEnumerable<IGraph<TVertex>> IGraph<TVertex>.ConnectedComponents => ConnectedComponents;

    IEnumerable<IReadOnlyGraph<TVertex>> IReadOnlyGraph<TVertex>.ConnectedComponents => ConnectedComponents;
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

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
}

using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology;

/// <summary>
/// Represents a strongly-typed directional graph of objects.
/// </summary>
/// <inheritdoc cref="IGraph{TVertex}"/>
[DebuggerDisplay($"Order = {{{nameof(Vertices)}.Count}}, Size = {{{nameof(Edges)}.Count}}")]
[DebuggerTypeProxy(typeof(GraphDebugView<>))]
public partial class Graph<TVertex> : IGraph<TVertex>
{
    // This class is partial. Please take a look at the neighboring source files.

    /// <summary>
    /// Creates a new graph instance inheriting parent object settings such as comparer and edge direction awareness,
    /// but without inheriting its contents (vertices and edges).
    /// </summary>
    /// <returns>A new graph instance with inherited parent object settings.</returns>
    protected Graph<TVertex> NewGraph() =>
        new(VertexComparer)
        {
            IsDirected = IsDirected
        };

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

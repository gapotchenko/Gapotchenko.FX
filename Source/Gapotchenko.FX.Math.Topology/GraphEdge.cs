using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology;

/// <summary>
/// Provides static methods for creating <see cref="GraphEdge{TVertex}"/> objects.
/// </summary>
public static class GraphEdge
{
    /// <summary>
    /// Creates a new instance of <see cref="GraphEdge{TVertex}"/> object with source and destination vertices.
    /// </summary>
    /// <param name="from">The source vertex of the edge.</param>
    /// <param name="to">The destination vertex of the edge.</param>
    /// <returns>The new instance of <see cref="GraphEdge{TVertex}"/> object with specified source and destination vertices.</returns>
    public static GraphEdge<TVertex> Create<TVertex>(TVertex from, TVertex to) => new(from, to);

    /// <summary>
    /// Creates graph edge equality comparer with specified vertex comparer and direction awareness.
    /// </summary>
    /// <param name="vertexComparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the edge,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <param name="directed">
    /// The direction awareness.
    /// Indicates whether the equality comparer is for directed edges.
    /// </param>
    /// <returns>A new instance of graph edge equality comparer.</returns>
    public static IEqualityComparer<GraphEdge<TVertex>> CreateComparer<TVertex>(IEqualityComparer<TVertex> vertexComparer, bool directed) =>
        directed ?
            new GraphEdgeEqualityComparer<TVertex>.Directed(vertexComparer) :
            new GraphEdgeEqualityComparer<TVertex>.Undirected(vertexComparer);
}

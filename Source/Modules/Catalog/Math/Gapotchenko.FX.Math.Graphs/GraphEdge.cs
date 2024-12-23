﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

/// <summary>
/// Provides static methods for creating <see cref="GraphEdge{TVertex}"/> objects and working with them.
/// </summary>
public static class GraphEdge
{
    /// <summary>
    /// Creates a new instance of <see cref="GraphEdge{TVertex}"/> object with source and destination vertices.
    /// </summary>
    /// <param name="from">The source vertex of the edge.</param>
    /// <param name="to">The destination vertex of the edge.</param>
    /// <returns>A new instance of <see cref="GraphEdge{TVertex}"/> object with specified source and destination vertices.</returns>
    public static GraphEdge<TVertex> Create<TVertex>(TVertex from, TVertex to) => new(from, to);

    /// <summary>
    /// Provides <see cref="IEqualityComparer{T}"/> services for <see cref="GraphEdge{TVertex}"/>.
    /// </summary>
    public static class EqualityComparer
    {
        /// <summary>
        /// Creates an equality comparer for <see cref="GraphEdge{TVertex}"/> with the specified vertex comparer and direction awareness.
        /// </summary>
        /// <param name="vertexComparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the edge,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        /// <param name="directed">
        /// The direction awareness.
        /// Indicates whether the equality comparer is for directed edges.
        /// </param>
        /// <returns>A new <see cref="IEqualityComparer{T}"/> instance for <see cref="GraphEdge{TVertex}"/>.</returns>
        public static IEqualityComparer<GraphEdge<TVertex>> Create<TVertex>(IEqualityComparer<TVertex>? vertexComparer, bool directed)
        {
            vertexComparer ??= EqualityComparer<TVertex>.Default;
            return directed ?
                new GraphEdgeEqualityComparer.Directed<TVertex>(vertexComparer) :
                new GraphEdgeEqualityComparer.Undirected<TVertex>(vertexComparer);
        }
    }
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

/// <summary>
/// Extensions for <see cref="GraphEdge{TVertex}"/> structure.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class GraphEdgeExtensions
{
    /// <summary>
    /// Adds the specified edge to the graph.
    /// </summary>
    /// <param name="edges">The set of graph edges.</param>
    /// <param name="from">The source vertex of the edge.</param>
    /// <param name="to">The destination vertex of the edge.</param>
    /// <returns>
    /// <see langword="true"/> if the edge is added to the graph;
    /// <see langword="false"/> if the edge is already present.</returns>
    public static bool Add<TVertex>(this ISet<GraphEdge<TVertex>> edges, TVertex from, TVertex to) =>
        (edges ?? throw new ArgumentNullException(nameof(edges)))
        .Add(new(from, to));

    /// <summary>
    /// Removes the specified edge from the graph.
    /// </summary>
    /// <param name="edges">The set of graph edges.</param>
    /// <param name="from">The source vertex of the edge.</param>
    /// <param name="to">The destination vertex of the edge.</param>
    /// <returns>
    /// <see langword="true"/> if the edge was removed from the graph;
    /// <see langword="false"/> if the edge is not found.
    /// </returns>
    public static bool Remove<TVertex>(this ISet<GraphEdge<TVertex>> edges, TVertex from, TVertex to) =>
        (edges ?? throw new ArgumentNullException(nameof(edges)))
        .Remove(new(from, to));

    /// <summary>
    /// <para>
    /// Determines whether the graph contains a specified edge.
    /// </para>
    /// <para>
    /// The presence of an edge in the graph signifies that corresponding vertices are adjacent.
    /// </para>
    /// <para>
    /// Adjacent vertices are those connected by one edge without intermediary vertices.
    /// </para>
    /// </summary>
    /// <param name="edges">The set of graph edges.</param>
    /// <param name="from">The source vertex of the edge.</param>
    /// <param name="to">The destination vertex of the edge.</param>
    /// <returns>
    /// <see langword="true"/> when a specified vertex <paramref name="from">A</paramref> is adjacent to vertex <paramref name="to">B</paramref>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Contains<TVertex>(this IReadOnlySet<GraphEdge<TVertex>> edges, TVertex from, TVertex to) =>
        (edges ?? throw new ArgumentNullException(nameof(edges)))
        .Contains(new(from, to));

#if BINARY_COMPATIBILITY
    /// <summary>
    /// <para>
    /// Determines whether the graph contains a specified edge.
    /// </para>
    /// <para>
    /// The presence of an edge in the graph signifies that corresponding vertices are adjacent.
    /// </para>
    /// <para>
    /// Adjacent vertices are those connected by one edge without intermediary vertices.
    /// </para>
    /// </summary>
    /// <param name="edges">The set of graph edges.</param>
    /// <param name="from">The source vertex of the edge.</param>
    /// <param name="to">The destination vertex of the edge.</param>
    /// <returns>
    /// <see langword="true"/> when a specified vertex <paramref name="from">A</paramref> is adjacent to vertex <paramref name="to">B</paramref>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public
#endif
    static bool Contains<TVertex>(ISet<GraphEdge<TVertex>> edges, TVertex from, TVertex to) =>
        (edges ?? throw new ArgumentNullException(nameof(edges)))
        .Contains(new(from, to));

    /// <summary>
    /// <para>
    /// Determines whether the graph contains a specified edge.
    /// </para>
    /// <para>
    /// The presence of an edge in the graph signifies that corresponding vertices are adjacent.
    /// </para>
    /// <para>
    /// Adjacent vertices are those connected by one edge without intermediary vertices.
    /// </para>
    /// </summary>
    /// <param name="edges">The set of graph edges.</param>
    /// <param name="from">The source vertex of the edge.</param>
    /// <param name="to">The destination vertex of the edge.</param>
    /// <param name="reserved">The reserved parameter used for method signature resolution.</param>
    /// <returns>
    /// <see langword="true"/> when a specified vertex <paramref name="from">A</paramref> is adjacent to vertex <paramref name="to">B</paramref>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool Contains<TVertex>(this ISet<GraphEdge<TVertex>> edges, TVertex from, TVertex to, int reserved = default) =>
        Contains(edges, from, to);

    /// <summary>
    /// Adds the specified edge to the collection.
    /// </summary>
    /// <param name="collection">The collection of graph edges.</param>
    /// <param name="from">The source vertex of the edge.</param>
    /// <param name="to">The destination vertex of the edge.</param>
    /// <returns>
    /// <see langword="true"/> if the edge is added to the graph;
    /// <see langword="false"/> if the edge is already present.</returns>
    public static void Add<TVertex>(this ICollection<GraphEdge<TVertex>> collection, TVertex from, TVertex to) =>
        (collection ?? throw new ArgumentNullException(nameof(collection)))
        .Add(new(from, to));

    /// <summary>
    /// Removes the specified edge from the collection.
    /// </summary>
    /// <param name="collection">The collection of graph edges.</param>
    /// <param name="from">The source vertex of the edge.</param>
    /// <param name="to">The destination vertex of the edge.</param>
    /// <returns>
    /// <see langword="true"/> if the edge was removed from the collection;
    /// <see langword="false"/> if the edge is not found.
    /// </returns>
    public static bool Remove<TVertex>(this ICollection<GraphEdge<TVertex>> collection, TVertex from, TVertex to) =>
        (collection ?? throw new ArgumentNullException(nameof(collection)))
        .Remove(new(from, to));
}

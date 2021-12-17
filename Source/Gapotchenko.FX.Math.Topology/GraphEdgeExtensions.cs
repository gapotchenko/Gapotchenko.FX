using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Extensions for <see cref="GraphEdge{T}"/> struct.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class GraphEdgeExtensions
    {
        /// <summary>
        /// Adds the specified edge to the graph.
        /// </summary>
        /// <param name="edgeSet">The set of graph edges.</param>
        /// <param name="from">The source vertex of the edge.</param>
        /// <param name="to">The destination vertex of the edge.</param>
        /// <returns><c>true</c> if the edge is added to the graph; <c>false</c> if the edge is already present.</returns>
        public static bool Add<T>(this ISet<GraphEdge<T>> edgeSet, T from, T to) =>
            (edgeSet ?? throw new ArgumentNullException(nameof(edgeSet))).Add(new GraphEdge<T>(from, to));

        /// <summary>
        /// Removes the specified edge from the graph.
        /// </summary>
        /// <param name="edgeSet">The set of graph edges.</param>
        /// <param name="from">The source vertex of the edge.</param>
        /// <param name="to">The destination vertex of the edge.</param>
        /// <returns><c>true</c> if the edge was removed from the graph; <c>false</c> if the edge is was not found.</returns>
        public static bool Remove<T>(this ISet<GraphEdge<T>> edgeSet, T from, T to) =>
            (edgeSet ?? throw new ArgumentNullException(nameof(edgeSet))).Remove(new GraphEdge<T>(from, to));

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
        /// <param name="edgeSet">The set of graph edges.</param>
        /// <param name="from">The source vertex of the edge.</param>
        /// <param name="to">The destination vertex of the edge.</param>
        /// <returns><c>true</c> when a specified vertex <paramref name="from">A</paramref> is adjacent to vertex <paramref name="to">B</paramref>; otherwise, <c>false</c>.</returns>
        public static bool Contains<T>(this ISet<GraphEdge<T>> edgeSet, T from, T to) =>
            (edgeSet ?? throw new ArgumentNullException(nameof(edgeSet))).Contains(new GraphEdge<T>(from, to));
    }
}

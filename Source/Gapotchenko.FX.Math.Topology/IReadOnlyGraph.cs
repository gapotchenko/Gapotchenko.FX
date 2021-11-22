using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// <para>
    /// Defines an interface for a read-only graph.
    /// </para>
    /// <para>
    /// Graph is a set of vertices and edges.
    /// Vertices represent the objects and edges represents the relations between them.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Vertex type.</typeparam>
    public interface IReadOnlyGraph<T> : ICloneable<IReadOnlyGraph<T>>
    {
        /// <summary>
        /// Gets a set containing the vertices of the graph.
        /// </summary>
        IReadOnlySet<T> Vertices { get; }

        /// <summary>
        /// Gets edges of the graph.
        /// </summary>
        IEnumerable<GraphEdge<T>> Edges { get; }

        /// <summary>
        /// <para>
        /// Gets the size of the graph.
        /// </para>
        /// <para>
        /// The size of a graph is |E|, the number of its edges.
        /// </para>
        /// </summary>
        int Size { get; }

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
        /// <param name="from">The source vertex of the edge.</param>
        /// <param name="to">The destination vertex of the edge.</param>
        /// <returns><c>true</c> when a specified vertex <paramref name="from">A</paramref> is adjacent to vertex <paramref name="to">B</paramref>; otherwise, <c>false</c>.</returns>
        bool ContainsEdge(T from, T to);

        /// <summary>
        /// <para>
        /// Gets a value indicating whether there is a path from a specified source vertex to a destination.
        /// </para>
        /// <para>
        /// A path consists of one or more edges with or without intermediate vertices.
        /// </para>
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The target vertex.</param>
        /// <returns><c>true</c> when the specified source vertex can reach the target; otherwise, <c>false</c>.</returns>
        bool HasPath(T from, T to);

        /// <summary>
        /// <para>
        /// Gets a value indicating whether there is a transitive path from a specified source vertex to a destination.
        /// </para>
        /// <para>
        /// A transitive path consists of two or more edges with at least one intermediate vertex.
        /// </para>
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The target vertex.</param>
        /// <returns><c>true</c> when the specified source vertex can reach the target via one or more intermediate vertices; otherwise, <c>false</c>.</returns>
        bool HasTransitivePath(T from, T to);

        /// <summary>
        /// Gets the vertices adjacent to a specified vertex.
        /// </summary>
        /// <param name="vertex">The vertex to find the adjacent vertices for.</param>
        /// <returns>Sequence of vertices adjacent to vertex <paramref name="vertex"/>.</returns>
        IEnumerable<T> VerticesAdjacentTo(T vertex);

        /// <summary>
        /// Gets a value indicating whether the graph contains a cycle.
        /// </summary>
        bool IsCyclic { get; }

        /// <summary>
        /// Gets a graph transposition by reversing its edge directions.
        /// </summary>
        /// <returns>The transposed graph.</returns>
        IReadOnlyGraph<T> GetTransposition();

        /// <summary>
        /// Gets a transitively reduced graph.
        /// </summary>
        /// <returns>The transitively reduced graph.</returns>
        IReadOnlyGraph<T> GetTransitiveReduction();

        /// <summary>
        /// Gets a reflexively reduced graph.
        /// </summary>
        /// <returns>The reflexively reduced graph.</returns>
        IReadOnlyGraph<T> GetReflexiveReduction();
    }
}

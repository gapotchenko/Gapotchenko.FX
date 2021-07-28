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
        /// Gets vertices.
        /// </summary>
        IEnumerable<T> Vertices { get; }

        /// <summary>
        /// Gets edges.
        /// </summary>
        IEnumerable<(T A, T B)> Edges { get; }

        /// <summary>
        /// <para>
        /// Gets the order of the graph.
        /// </para>
        /// <para>
        /// The order of a graph is |V|, the number of its vertices.
        /// </para>
        /// </summary>
        int Order { get; }

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
        /// Determines whether the graph contains a specified vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns><c>true</c> when the graph contains a specified vertex; otherwise, <c>false</c>.</returns>
        bool ContainsVertex(T v);

        /// <summary>
        /// <para>
        /// Gets a value indicating whether specified vertices are adjacent.
        /// </para>
        /// <para>
        /// Adjacent vertices are those connected by the edge without intermediary vertices.
        /// </para>
        /// </summary>
        /// <param name="a">The vertex A.</param>
        /// <param name="b">The vertex B.</param>
        /// <returns><c>true</c> when a specified vertex <paramref name="a">A</paramref> is adjacent to vertex <paramref name="b">B</paramref>; otherwise, <c>false</c>.</returns>
        bool AreAdjacent(T a, T b);

        /// <summary>
        /// <para>
        /// Gets a value indicating whether specified vertices are transitive.
        /// </para>
        /// <para>
        /// Transitive vertices are those connected by two or more edges with at least one intermediate vertex.
        /// </para>
        /// </summary>
        /// <param name="a">The vertex A.</param>
        /// <param name="b">The vertex B.</param>
        /// <returns><c>true</c> when a specified vertex <paramref name="a">A</paramref> can reach vertex <paramref name="b">B</paramref> via one or more intermediate vertices; otherwise, <c>false</c>.</returns>
        bool AreTransitive(T a, T b);

        /// <summary>
        /// <para>
        /// Gets a value indicating whether specified vertices are reachable.
        /// </para>
        /// <para>
        /// Reachable vertices are those connected by one or more edges with or without intermediate vertices.
        /// </para>
        /// </summary>
        /// <param name="a">The vertex A.</param>
        /// <param name="b">The vertex B.</param>
        /// <returns><c>true</c> when a specified vertex <paramref name="a">A</paramref> can reach vertex <paramref name="b">B</paramref>; otherwise, <c>false</c>.</returns>
        bool AreReachable(T a, T b);

        /// <summary>
        /// Gets the vertices adjacent to a specified vertex.
        /// </summary>
        /// <param name="v">The vertex to find the adjacent vertices for.</param>
        /// <returns>Sequence of vertices adjacent to vertex <paramref name="v"/>.</returns>
        IEnumerable<T> VerticesAdjacentTo(T v);

        /// <summary>
        /// Gets a graph transposition by reversing edge directions.
        /// </summary>
        /// <returns>The transposed graph.</returns>
        IReadOnlyGraph<T> GetTransposition();

        /// <summary>
        /// Gets a transitively reduced graph.
        /// </summary>
        /// <returns>The transitively reduced graph.</returns>
        IReadOnlyGraph<T> GetReduction();
    }
}

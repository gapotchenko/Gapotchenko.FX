using System;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// <para>
    /// Defines an interface for a graph.
    /// </para>
    /// <para>
    /// Graph is a set of vertices and edges.
    /// Vertices represent the objects and edges represent the relations between them.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of graph vertices.</typeparam>
    public interface IGraph<T> : IReadOnlyGraph<T>, ICloneable<IGraph<T>>
    {
        /// <summary>
        /// Adds the specified vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns><c>true</c> if the vertex is added to the graph; <c>false</c> if the vertex is already present.</returns>
        bool AddVertex(T v);

        /// <summary>
        /// Removes the specified vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns><c>true</c> if the vertex was removed from the graph; otherwise, <c>false</c>.</returns>
        bool RemoveVertex(T v);

        /// <summary>
        /// Adds the specified edge.
        /// </summary>
        /// <param name="a">The A vertex of the edge.</param>
        /// <param name="b">The B vertex of the edge.</param>
        /// <returns><c>true</c> if the edge is added to the graph; <c>false</c> if the edge is already present.</returns>
        bool AddEdge(T a, T b);

        /// <summary>
        /// Clears the graph.
        /// </summary>
        void Clear();

        /// <summary>
        /// Transposes the graph by reversing edge directions.
        /// </summary>
        void Transpose();

        /// <summary>
        /// Gets a graph transposition by reversing edge directions.
        /// </summary>
        /// <returns>The transposed graph.</returns>
        new IGraph<T> GetTransposition();

        /// <summary>
        /// Performs a transitive reduction of the graph.
        /// </summary>
        void Reduce();

        /// <summary>
        /// Gets a transitively reduced graph.
        /// </summary>
        /// <returns>The transitively reduced graph.</returns>
        new IGraph<T> GetReduction();
    }
}

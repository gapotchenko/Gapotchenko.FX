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
        /// <param name="vertex">The vertex.</param>
        /// <returns><c>true</c> if the vertex is added to the graph; <c>false</c> if the vertex is already present.</returns>
        bool AddVertex(T vertex);

        /// <summary>
        /// Removes the specified vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns><c>true</c> if the vertex was removed from the graph; otherwise, <c>false</c>.</returns>
        bool RemoveVertex(T vertex);

        /// <summary>
        /// Adds the specified edge.
        /// </summary>
        /// <param name="from">The source vertex of the edge.</param>
        /// <param name="to">The destination vertex of the edge.</param>
        /// <returns><c>true</c> if the edge is added to the graph; <c>false</c> if the edge is already present.</returns>
        bool AddEdge(T from, T to);

        /// <summary>
        /// Clears the graph.
        /// </summary>
        void Clear();

        /// <summary>
        /// Transposes the current graph by reversing its edge directions in place.
        /// </summary>
        void Transpose();

        /// <summary>
        /// Gets a graph transposition by reversing its edge directions.
        /// </summary>
        /// <returns>The transposed graph.</returns>
        new IGraph<T> GetTransposition();

        /// <summary>
        /// <para>
        /// Performs in-place transitive reduction of the current graph.
        /// </para>
        /// <para>
        /// Transitive reduction prunes the transitive relations that have shorter paths.
        /// </para>
        /// </summary>
        void ReduceTransitions();

        /// <summary>
        /// <para>
        /// Gets a transitively reduced graph.
        /// </para>
        /// <para>
        /// Transitive reduction prunes the transitive relations that have shorter paths.
        /// </para>
        /// </summary>
        /// <returns>The transitively reduced graph.</returns>
        new IGraph<T> GetTransitiveReduction();

        /// <summary>
        /// <para>
        /// Performs in-place reflexive reduction of the current graph.
        /// </para>
        /// <para>
        /// Reflexive reduction prunes the reflexive relations.
        /// </para>
        /// </summary>
        void ReduceReflexes();

        /// <summary>
        /// <para>
        /// Gets a reflexively reduced graph.
        /// </para>
        /// <para>
        /// Reflexive reduction prunes the reflexive relations.
        /// </para>
        /// </summary>
        /// <returns>The reflexively reduced graph.</returns>
        new IGraph<T> GetReflexiveReduction();
    }
}

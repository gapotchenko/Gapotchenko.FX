using System;
using System.Collections.Generic;

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
        /// Gets a set containing the vertices of the graph.
        /// </summary>
        new ISet<T> Vertices { get; }

        /// <summary>
        /// Gets a set containing the edges of the graph.
        /// </summary>
        new ISet<GraphEdge<T>> Edges { get; }

        /// <summary>
        /// Removes all vertices and edges from the graph.
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

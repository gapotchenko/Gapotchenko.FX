using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// <para>
    /// Provides the base interface for the abstraction of graphs.
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
        /// Gets a vertex-induced subgraph of the current graph.
        /// </summary>
        /// <param name="vertices">The vertices to induce the subgraph from.</param>
        /// <returns>The vertex-induced subgraph of the current graph.</returns>
        new IGraph<T> GetSubgraph(IEnumerable<T> vertices);

        /// <summary>
        /// Gets an edge-induced subgraph of the current graph.
        /// </summary>
        /// <param name="edges">The edges to induce the subgraph from.</param>
        /// <returns>The edge-induced subgraph of the current graph.</returns>
        new IGraph<T> GetSubgraph(IEnumerable<GraphEdge<T>> edges);

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
        /// Reflexive relation is caused by a vertex that has a connection (edge) to itself.
        /// The removal of such connections prunes the reflexive relations, making a graph reflexively reduced.
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

        /// <summary>
        /// Removes all vertices and edges in the specified graph from the current graph.
        /// </summary>
        /// <param name="other">The graph whose vertices and edges should be removed from the current graph.</param>
        void ExceptWith(IReadOnlyGraph<T> other);

        /// <summary>
        /// Modifies the current graph so that it contains only vertices and edges that are also present in a specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        void IntersectWith(IReadOnlyGraph<T> other);

        /// <summary>
        /// Gets a graph containing vertices and edges that are present in both the current and a specified graphs.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns>The graph containing vertices and edges that are present in both the current and a specified graphs.</returns>
        new IGraph<T> Intersect(IReadOnlyGraph<T> other);

        /// <summary>
        /// Modifies the current graph so that it contains all vertices and edges that are present in the current graph, in the specified graph, or in both.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        void UnionWith(IReadOnlyGraph<T> other);

        /// <summary>
        /// Gets a graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns>The graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both.</returns>
        new IGraph<T> Union(IReadOnlyGraph<T> other);
    }
}

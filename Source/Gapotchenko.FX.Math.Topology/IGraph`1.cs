﻿using System;
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
    /// <typeparam name="TVertex">Type of graph vertices.</typeparam>
    public interface IGraph<TVertex> : IReadOnlyGraph<TVertex>, ICloneable<IGraph<TVertex>>
    {
        /// <summary>
        /// Gets a set containing the vertices of the graph.
        /// </summary>
        new ISet<TVertex> Vertices { get; }

        /// <summary>
        /// Gets a set containing the edges of the graph.
        /// </summary>
        new ISet<GraphEdge<TVertex>> Edges { get; }

        /// <summary>
        /// Removes all vertices and edges from the graph.
        /// </summary>
        void Clear();

        /// <summary>
        /// Makes the current graph a vertex-induced subgraph from specified vertices.
        /// </summary>
        /// <param name="vertices">The vertices to induce the subgraph from.</param>
        void Subgraph(IEnumerable<TVertex> vertices);

        /// <summary>
        /// Gets a vertex-induced subgraph of the current graph.
        /// </summary>
        /// <param name="vertices">The vertices to induce the subgraph from.</param>
        /// <returns>The vertex-induced subgraph of the current graph.</returns>
        new IGraph<TVertex> GetSubgraph(IEnumerable<TVertex> vertices);

        /// <summary>
        /// Makes the current graph an edge-induced subgraph from specified edges.
        /// </summary>
        /// <param name="edges">The edges to induce the subgraph from.</param>
        void Subgraph(IEnumerable<GraphEdge<TVertex>> edges);

        /// <summary>
        /// Gets an edge-induced subgraph of the current graph.
        /// </summary>
        /// <param name="edges">The edges to induce the subgraph from.</param>
        /// <returns>The edge-induced subgraph of the current graph.</returns>
        new IGraph<TVertex> GetSubgraph(IEnumerable<GraphEdge<TVertex>> edges);

        /// <summary>
        /// Transposes the current graph by reversing its edge directions in place.
        /// </summary>
        void Transpose();

        /// <summary>
        /// Gets a graph transposition by reversing its edge directions.
        /// </summary>
        /// <returns>The transposed graph.</returns>
        new IGraph<TVertex> GetTransposition();

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
        new IGraph<TVertex> GetTransitiveReduction();

        /// <summary>
        /// <para>
        /// Performs in-place reflexive reduction of the current graph.
        /// </para>
        /// <para>
        /// Reflexive reduction removes the loops (also called self-loops or buckles) from the graph.
        /// </para>
        /// </summary>
        void ReduceReflexes();

        /// <summary>
        /// <para>
        /// Gets a reflexively reduced graph.
        /// </para>
        /// <para>
        /// Reflexive reduction removes the loops (also called self-loops or buckles) from the graph.
        /// </para>
        /// </summary>
        /// <returns>The graph without the loops.</returns>
        new IGraph<TVertex> GetReflexiveReduction();

        /// <summary>
        /// Removes all vertices and edges in the specified graph from the current graph.
        /// </summary>
        /// <param name="other">The graph whose vertices and edges should be removed from the current graph.</param>
        void ExceptWith(IReadOnlyGraph<TVertex> other);

        /// <summary>
        /// Gets a graph containing vertices and edges that are present in the current graph but not in the specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns>The graph containing vertices and edges that are present in the current graph but not in the specified graph.</returns>
        new IGraph<TVertex> Except(IReadOnlyGraph<TVertex> other);

        /// <summary>
        /// Modifies the current graph so that it contains only vertices and edges that are also present in a specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        void IntersectWith(IReadOnlyGraph<TVertex> other);

        /// <summary>
        /// Gets a graph containing vertices and edges that are present in both the current and a specified graphs.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns>The graph containing vertices and edges that are present in both the current and a specified graphs.</returns>
        new IGraph<TVertex> Intersect(IReadOnlyGraph<TVertex> other);

        /// <summary>
        /// Modifies the current graph so that it contains all vertices and edges that are present in the current graph, in the specified graph, or in both.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        void UnionWith(IReadOnlyGraph<TVertex> other);

        /// <summary>
        /// Gets a graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns>The graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both.</returns>
        new IGraph<TVertex> Union(IReadOnlyGraph<TVertex> other);
    }
}

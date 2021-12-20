﻿using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// <para>
    /// Provides a read-only abstraction of a graph.
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
        /// Gets a set containing the edges of the graph.
        /// </summary>
        IReadOnlySet<GraphEdge<T>> Edges { get; }

        /// <summary>
        /// Gets the vertices adjacent to a specified vertex.
        /// </summary>
        /// <param name="vertex">The vertex to find the adjacent vertices for.</param>
        /// <returns>Sequence of vertices adjacent to the specified <paramref name="vertex"/>.</returns>
        IEnumerable<T> VerticesAdjacentTo(T vertex);

        /// <summary>
        /// Determines whether the current graph contains a cycle.
        /// </summary>
        bool IsCyclic { get; }

        /// <summary>
        /// <para>
        /// Gets a value indicating whether there is a path from a specified source vertex to a destination.
        /// </para>
        /// <para>
        /// A path consists of one or more edges with or without intermediate vertices.
        /// </para>
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The destination vertex.</param>
        /// <returns><see langword="true"/> when the specified source vertex can reach the target; otherwise, <see langword="false"/>.</returns>
        bool HasPath(T from, T to);

        /// <summary>
        /// <para>
        /// Determines whether the specified vertex is isolated, that is, not connected by an edge to another vertex.
        /// </para>
        /// <para>
        /// An isolated vertex is a vertex with degree zero; that is, a vertex that is not an endpoint of any edge.
        /// </para>
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns><see langword="true"/> when the specified vertex is isolated; otherwise, <see langword="false"/>.</returns>
        bool IsVertexIsolated(T vertex);

        /// <summary>
        /// <para>
        /// Gets the indegree of a specified vertex.
        /// </para>
        /// <para>
        /// Indegree of a vertex is the number of head ends adjacent to the vertex.
        /// </para>
        /// </summary>
        /// <param name="vertex">The vertex to find the indegree of.</param>
        /// <returns>The indegree of a vertex.</returns>
        int GetVertexIndegree(T vertex);

        /// <summary>
        /// <para>
        /// Gets the outdegree of a specified vertex.
        /// </para>
        /// <para>
        /// Outdegree of a vertex is the number of tail ends adjacent to the vertex.
        /// </para>
        /// </summary>
        /// <param name="vertex">The vertex to find the outdegree of.</param>
        /// <returns>The outdegree of a vertex.</returns>
        int GetVertexOutdegree(T vertex);

        /// <summary>
        /// <para>
        /// Gets the degree of a specified vertex.
        /// </para>
        /// <para>
        /// Degree of a vertex is the number of head and tail ends adjacent to the vertex.
        /// </para>
        /// </summary>
        /// <param name="vertex">The vertex to find the degree of.</param>
        /// <returns>The degree of a vertex.</returns>
        int GetVertexDegree(T vertex);

        /// <summary>
        /// Gets a vertex-induced subgraph of the current graph.
        /// </summary>
        /// <param name="vertices">The vertices to induce the subgraph from.</param>
        /// <returns>The vertex-induced subgraph of the current graph.</returns>
        IReadOnlyGraph<T> GetSubgraph(IEnumerable<T> vertices);

        /// <summary>
        /// Gets an edge-induced subgraph of the current graph.
        /// </summary>
        /// <param name="edges">The edges to induce the subgraph from.</param>
        /// <returns>The edge-induced subgraph of the current graph.</returns>
        IReadOnlyGraph<T> GetSubgraph(IEnumerable<GraphEdge<T>> edges);

        /// <summary>
        /// Gets a transposition of the current graph by reversing its edge directions.
        /// </summary>
        /// <returns>The transposed graph.</returns>
        IReadOnlyGraph<T> GetTransposition();

        /// <summary>
        /// Gets a transitive reduction of the current graph.
        /// </summary>
        /// <returns>The transitively reduced graph.</returns>
        IReadOnlyGraph<T> GetTransitiveReduction();

        /// <summary>
        /// Gets a reflexive reduction of the current graph.
        /// </summary>
        /// <returns>The reflexively reduced graph.</returns>
        IReadOnlyGraph<T> GetReflexiveReduction();

        /// <summary>
        /// Determines whether the current graph and the specified one contain the same vertices and edges.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns><see langword="true"/> if the current graph is equal to other; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
        bool GraphEquals(IReadOnlyGraph<T> other);

        /// <summary>
        /// Determines whether the current graph is an edge-induced subgraph of a specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns><see langword="true" /> if the current graph is an edge-induced subgraph of other; otherwise <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
        bool IsEdgeInducedSubgraphOf(IReadOnlyGraph<T> other);

        /// <summary>
        /// Determines whether the current graph is an edge-induced supergraph of a specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns><see langword="true" /> if the current graph is an edge-induced supergraph of other; otherwise <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
        bool IsEdgeInducedSupergraphOf(IReadOnlyGraph<T> other);

        /// <summary>
        /// Determines whether the current graph is a proper (strict) subgraph of a specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns><see langword="true" /> if the current graph is a proper subgraph of other; otherwise <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
        bool IsProperSubgraphOf(IReadOnlyGraph<T> other);

        /// <summary>
        /// Determines whether the current graph is a proper (strict) supergraph of a specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns><see langword="true" /> if the current graph is a proper supergraph of other; otherwise <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
        bool IsProperSupergraphOf(IReadOnlyGraph<T> other);

        /// <summary>
        /// Determines whether the current graph is a subgraph of a specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns><see langword="true" /> if the current graph is a subgraph of other; otherwise <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
        bool IsSubgraphOf(IReadOnlyGraph<T> other);

        /// <summary>
        /// Determines whether the current graph is a supergraph of a specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns><see langword="true" /> if the current graph is a supergraph of other; otherwise <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
        bool IsSupergraphOf(IReadOnlyGraph<T> other);

        /// <summary>
        /// Determines whether the current graph is a vertex-induced subgraph of a specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns><see langword="true" /> if the current graph is a vertex-induced subgraph of other; otherwise <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
        bool IsVertexInducedSubgraphOf(IReadOnlyGraph<T> other);

        /// <summary>
        /// Determines whether the current graph is a vertex-induced supergraph of a specified graph.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns><see langword="true" /> if the current graph is a vertex-induced supergraph of other; otherwise <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
        bool IsVertexInducedSupergraphOf(IReadOnlyGraph<T> other);
    }
}

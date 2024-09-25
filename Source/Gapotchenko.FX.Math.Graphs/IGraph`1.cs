// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

/// <summary>
/// Provides a graph abstraction.
/// </summary>
/// <inheritdoc cref="IReadOnlyGraph{TVertex}"/>
public interface IGraph<TVertex> : IReadOnlyGraph<TVertex>, ICloneable<IGraph<TVertex>>
{
    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.Vertices"/>
    new ISet<TVertex> Vertices { get; }

    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.Edges"/>
    new ISet<GraphEdge<TVertex>> Edges { get; }

    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.ConnectedComponents"/>
    new IEnumerable<IGraph<TVertex>> ConnectedComponents { get; }

    /// <summary>
    /// Removes all vertices and edges from the graph.
    /// </summary>
    void Clear();

    /// <summary>
    /// Makes the current graph a vertex-induced subgraph from the specified vertices.
    /// </summary>
    /// <param name="vertices"><inheritdoc cref="GetSubgraph(IEnumerable{TVertex})"/></param>
    void Subgraph(IEnumerable<TVertex> vertices);

    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.GetSubgraph(IEnumerable{TVertex})"/>
    new IGraph<TVertex> GetSubgraph(IEnumerable<TVertex> vertices);

    /// <summary>
    /// Makes the current graph an edge-induced subgraph from the specified edges.
    /// </summary>
    /// <param name="edges"><inheritdoc cref="GetSubgraph(IEnumerable{GraphEdge{TVertex}})"/></param>
    void Subgraph(IEnumerable<GraphEdge<TVertex>> edges);

    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.GetSubgraph(IEnumerable{GraphEdge{TVertex}})"/>
    new IGraph<TVertex> GetSubgraph(IEnumerable<GraphEdge<TVertex>> edges);

    /// <summary>
    /// Transposes the current graph by reversing its edge directions in place.
    /// </summary>
    void Transpose();

    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.GetTransposition"/>
    new IGraph<TVertex> GetTransposition();

    /// <summary>
    /// Performs in-place transitive reduction of the current graph.
    /// </summary>
    /// <remarks><inheritdoc cref="GetTransitiveReduction"/></remarks>
    void ReduceTransitions();

    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.GetTransitiveReduction"/>
    new IGraph<TVertex> GetTransitiveReduction();

    /// <summary>
    /// Performs in-place reflexive reduction of the current graph.
    /// </summary>
    /// <remarks><inheritdoc cref="GetReflexiveReduction"/></remarks>
    void ReduceReflexes();

    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.GetReflexiveReduction"/>
    new IGraph<TVertex> GetReflexiveReduction();

    /// <summary>
    /// Removes all vertices and edges in the specified graph from the current graph.
    /// </summary>
    /// <param name="other"><inheritdoc cref="Except(IReadOnlyGraph{TVertex})"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
    void ExceptWith(IReadOnlyGraph<TVertex> other);

    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.Except(IReadOnlyGraph{TVertex})"/>
    new IGraph<TVertex> Except(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Modifies the current graph so that it contains only vertices and edges that are also present in a specified graph.
    /// </summary>
    /// <param name="other"><inheritdoc cref="Intersect(IReadOnlyGraph{TVertex})"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
    void IntersectWith(IReadOnlyGraph<TVertex> other);

    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.Intersect(IReadOnlyGraph{TVertex})"/>
    new IGraph<TVertex> Intersect(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Modifies the current graph so that it contains all vertices and edges that are present in the current graph, in the specified graph, or in both.
    /// </summary>
    /// <param name="other"><inheritdoc cref="Union(IReadOnlyGraph{TVertex})"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null" />.</exception>
    void UnionWith(IReadOnlyGraph<TVertex> other);

    /// <inheritdoc cref="IReadOnlyGraph{TVertex}.Union(IReadOnlyGraph{TVertex})"/>
    new IGraph<TVertex> Union(IReadOnlyGraph<TVertex> other);
}

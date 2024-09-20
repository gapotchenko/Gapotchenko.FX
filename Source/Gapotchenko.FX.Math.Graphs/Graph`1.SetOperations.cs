// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public bool GraphEquals(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return
            other == this ||
            Vertices.SetEquals(other.Vertices) &&
            Edges.SetEquals(other.Edges);
    }

    /// <inheritdoc/>
    public bool IsEdgeInducedSubgraphOf(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return
            other == this ||
            Vertices.IsSubsetOf(other.Vertices) &&
            Edges.IsSubsetOf(GetInducedSubgraphEdges(this, other)) &&
            Vertices.All(v => !IsVertexIsolated(v));
    }

    /// <inheritdoc/>
    public bool IsEdgeInducedSupergraphOf(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return
            other == this ||
            Vertices.IsSupersetOf(other.Vertices) &&
            Edges.IsSupersetOf(GetInducedSubgraphEdges(this, other)) &&
            other.Vertices.All(v => !other.IsVertexIsolated(v));
    }

    static IEnumerable<GraphEdge<TVertex>> GetInducedSubgraphEdges(IReadOnlyGraph<TVertex> a, IReadOnlyGraph<TVertex> b)
    {
        var aVertices = a.Vertices;
        return b.Edges.Where(e => aVertices.Contains(e.From) && aVertices.Contains(e.To));
    }

    /// <inheritdoc/>
    public bool IsProperSubgraphOf(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return
            !GraphEquals(other) &&
            IsSubgraphOf(other);
    }

    /// <inheritdoc/>
    public bool IsProperSupergraphOf(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return
            !GraphEquals(other) &&
            IsSupergraphOf(other);
    }

    /// <inheritdoc/>
    public bool IsSubgraphOf(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return
            other == this ||
            Vertices.IsSubsetOf(other.Vertices) &&
            Edges.IsSubsetOf(other.Edges);
    }

    /// <inheritdoc/>
    public bool IsSupergraphOf(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return
            other == this ||
            Vertices.IsSupersetOf(other.Vertices) &&
            Edges.IsSupersetOf(other.Edges);
    }

    /// <inheritdoc/>
    public bool IsVertexInducedSubgraphOf(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return
            other == this ||
            Vertices.IsSubsetOf(other.Vertices) &&
            VertexInducedSubgraphEdgesEqual(this, other);
    }

    /// <inheritdoc/>
    public bool IsVertexInducedSupergraphOf(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return
            other == this ||
            Vertices.IsSupersetOf(other.Vertices) &&
            VertexInducedSubgraphEdgesEqual(other, this);
    }

    static bool VertexInducedSubgraphEdgesEqual(IReadOnlyGraph<TVertex> a, IReadOnlyGraph<TVertex> b) =>
        a.Edges.SetEquals(GetInducedSubgraphEdges(a, b));

    /// <inheritdoc/>
    public void IntersectWith(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        IntersectWithCore(other);
    }

    void IntersectWithCore(IReadOnlyGraph<TVertex> other)
    {
        Vertices.IntersectWith(other.Vertices);
        Edges.IntersectWith(other.Edges);
    }

    /// <inheritdoc cref="IGraph{TVertex}.Intersect(IReadOnlyGraph{TVertex})"/>
    public Graph<TVertex> Intersect(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        var graph = Clone();
        graph.IntersectWithCore(other);
        return graph;
    }

    /// <inheritdoc/>
    public void ExceptWith(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        ExceptWithCore(other);
    }

    void ExceptWithCore(IReadOnlyGraph<TVertex> other)
    {
        Vertices.ExceptWith(other.Vertices);
        Edges.ExceptWith(other.Edges);
    }

    IGraph<TVertex> IGraph<TVertex>.Intersect(IReadOnlyGraph<TVertex> other) => Intersect(other);

    IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.Intersect(IReadOnlyGraph<TVertex> other) => Intersect(other);

    /// <inheritdoc cref="IGraph{TVertex}.Except(IReadOnlyGraph{TVertex})"/>
    public Graph<TVertex> Except(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        var graph = Clone();
        graph.ExceptWithCore(other);
        return graph;
    }

    IGraph<TVertex> IGraph<TVertex>.Except(IReadOnlyGraph<TVertex> other) => Except(other);

    IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.Except(IReadOnlyGraph<TVertex> other) => Except(other);

    /// <inheritdoc/>
    public void UnionWith(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        UnionWithCore(other);
    }

    void UnionWithCore(IReadOnlyGraph<TVertex> other)
    {
        Edges.UnionWith(other.Edges);
        Vertices.UnionWith(other.Vertices);
    }

    /// <inheritdoc cref="IGraph{TVertex}.Union(IReadOnlyGraph{TVertex})"/>
    public Graph<TVertex> Union(IReadOnlyGraph<TVertex> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        var graph = Clone();
        graph.UnionWithCore(other);
        return graph;
    }

    IGraph<TVertex> IGraph<TVertex>.Union(IReadOnlyGraph<TVertex> other) => Union(other);

    IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.Union(IReadOnlyGraph<TVertex> other) => Union(other);
}

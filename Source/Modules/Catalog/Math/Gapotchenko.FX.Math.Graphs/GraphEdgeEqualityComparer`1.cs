// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

static class GraphEdgeEqualityComparer
{
    public sealed class Directed<TVertex>(IEqualityComparer<TVertex> vertexComparer) :
        IEqualityComparer<GraphEdge<TVertex>>
    {
        public bool Equals(GraphEdge<TVertex> x, GraphEdge<TVertex> y) =>
            m_VertexComparer.Equals(x.From, y.From) &&
            m_VertexComparer.Equals(x.To, y.To);

        public int GetHashCode([DisallowNull] GraphEdge<TVertex> obj) =>
            HashCode.Combine(
                obj.From is { } from ? m_VertexComparer.GetHashCode(from) : 0,
                obj.To is { } to ? m_VertexComparer.GetHashCode(to) : 0);

        public override bool Equals(object? obj) =>
            obj is Directed<TVertex> other &&
            m_VertexComparer.Equals(other.m_VertexComparer);

        public override int GetHashCode() => m_VertexComparer.GetHashCode() ^ 17;

        // Needed for a debug view.
        public IEqualityComparer<TVertex> VertexComparer => m_VertexComparer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEqualityComparer<TVertex> m_VertexComparer = vertexComparer;
    }

    public sealed class Undirected<TVertex>(IEqualityComparer<TVertex> vertexComparer) :
        IEqualityComparer<GraphEdge<TVertex>>
    {
        public bool Equals(GraphEdge<TVertex> x, GraphEdge<TVertex> y) =>
            m_VertexComparer.Equals(x.From, y.From) && m_VertexComparer.Equals(x.To, y.To) ||   // direct (from, to) == (from, to)
            m_VertexComparer.Equals(x.To, y.From) && m_VertexComparer.Equals(x.From, y.To);     // reverse (from, to) == (to, from)

        public int GetHashCode([DisallowNull] GraphEdge<TVertex> obj) =>
            // An associative combining operation should be used here
            // because From and To values can be reversed.
            (obj.From is { } from ? m_VertexComparer.GetHashCode(from) : 0) ^
            (obj.To is { } to ? m_VertexComparer.GetHashCode(to) : 0);

        public override bool Equals(object? obj) =>
            obj is Undirected<TVertex> other &&
            m_VertexComparer.Equals(other.m_VertexComparer);

        public override int GetHashCode() => m_VertexComparer.GetHashCode() ^ 37;

        // Needed for a debug view.
        public IEqualityComparer<TVertex> VertexComparer => m_VertexComparer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEqualityComparer<TVertex> m_VertexComparer = vertexComparer;
    }
}

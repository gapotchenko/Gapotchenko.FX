// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Math.Graphs;

static class GraphEdgeEqualityComparer<TVertex>
{
    public sealed class Directed : IEqualityComparer<GraphEdge<TVertex>>
    {
        public Directed(IEqualityComparer<TVertex> vertexComparer)
        {
            if (vertexComparer == null)
                throw new ArgumentNullException(nameof(vertexComparer));

            m_VertexComparer = vertexComparer;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEqualityComparer<TVertex> m_VertexComparer;

        // Needed for a debug view.
        public IEqualityComparer<TVertex> VertexComparer => m_VertexComparer;

        public bool Equals(GraphEdge<TVertex> x, GraphEdge<TVertex> y) =>
            m_VertexComparer.Equals(x.From, y.From) &&
            m_VertexComparer.Equals(x.To, y.To);

        public int GetHashCode([DisallowNull] GraphEdge<TVertex> obj)
        {
            var from = obj.From;
            var to = obj.To;

            return HashCode.Combine(
                from != null ? m_VertexComparer.GetHashCode(from) : 0,
                to != null ? m_VertexComparer.GetHashCode(to) : 0);
        }

        public override bool Equals(object? obj) =>
            obj is GraphEdgeEqualityComparer<TVertex>.Directed other &&
            m_VertexComparer.Equals(other.m_VertexComparer);

        public override int GetHashCode() => m_VertexComparer.GetHashCode() ^ 17;
    }

    public sealed class Undirected : IEqualityComparer<GraphEdge<TVertex>>
    {
        public Undirected(IEqualityComparer<TVertex> vertexComparer)
        {
            if (vertexComparer == null)
                throw new ArgumentNullException(nameof(vertexComparer));

            m_VertexComparer = vertexComparer;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEqualityComparer<TVertex> m_VertexComparer;

        // Needed for a debug view.
        public IEqualityComparer<TVertex> VertexComparer => m_VertexComparer;

        public bool Equals(GraphEdge<TVertex> x, GraphEdge<TVertex> y) =>
            m_VertexComparer.Equals(x.From, y.From) && m_VertexComparer.Equals(x.To, y.To) ||   // direct (from, to) == (from, to)
            m_VertexComparer.Equals(x.To, y.From) && m_VertexComparer.Equals(x.From, y.To);     // reverse (from, to) == (to, from)

        public int GetHashCode([DisallowNull] GraphEdge<TVertex> obj)
        {
            var from = obj.From;
            var to = obj.To;

            return
                (from != null ? m_VertexComparer.GetHashCode(from) : 0) ^
                (to != null ? m_VertexComparer.GetHashCode(to) : 0);
        }

        public override bool Equals(object? obj) =>
            obj is GraphEdgeEqualityComparer<TVertex>.Undirected other &&
            m_VertexComparer.Equals(other.m_VertexComparer);

        public override int GetHashCode() => m_VertexComparer.GetHashCode() ^ 37;
    }
}

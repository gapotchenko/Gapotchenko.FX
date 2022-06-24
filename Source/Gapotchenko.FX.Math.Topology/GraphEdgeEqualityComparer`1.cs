using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Math.Topology;

static class GraphEdgeEqualityComparer<TVertex>
{
    public sealed class Directed : IEqualityComparer<GraphEdge<TVertex>>
    {
        public Directed(IEqualityComparer<TVertex> vertexComparer)
        {
            if (vertexComparer == null)
                throw new ArgumentNullException(nameof(vertexComparer));

            m_vertexComparer = vertexComparer;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEqualityComparer<TVertex> m_vertexComparer;

        // Needed for debug view.
        public IEqualityComparer<TVertex> VertexComparer => m_vertexComparer;

        public bool Equals(GraphEdge<TVertex> x, GraphEdge<TVertex> y) =>
            m_vertexComparer.Equals(x.From, y.From) &&
            m_vertexComparer.Equals(x.To, y.To);

        public int GetHashCode([DisallowNull] GraphEdge<TVertex> obj)
        {
            var from = obj.From;
            var to = obj.To;

            return HashCode.Combine(
                from != null ? m_vertexComparer.GetHashCode(from) : 0,
                to != null ? m_vertexComparer.GetHashCode(to) : 0);
        }

        public override bool Equals(object? obj) =>
            obj is GraphEdgeEqualityComparer<TVertex>.Directed other &&
            m_vertexComparer.Equals(other.m_vertexComparer);

        public override int GetHashCode() => m_vertexComparer.GetHashCode() ^ 17;
    }

    public sealed class Undirected : IEqualityComparer<GraphEdge<TVertex>>
    {
        public Undirected(IEqualityComparer<TVertex> vertexComparer)
        {
            if (vertexComparer == null)
                throw new ArgumentNullException(nameof(vertexComparer));

            m_vertexComparer = vertexComparer;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEqualityComparer<TVertex> m_vertexComparer;

        // Needed for debug view.
        public IEqualityComparer<TVertex> VertexComparer => m_vertexComparer;

        public bool Equals(GraphEdge<TVertex> x, GraphEdge<TVertex> y) =>
            m_vertexComparer.Equals(x.From, y.From) && m_vertexComparer.Equals(x.To, y.To) ||
            m_vertexComparer.Equals(x.To, y.From) && m_vertexComparer.Equals(x.From, y.To);

        public int GetHashCode([DisallowNull] GraphEdge<TVertex> obj)
        {
            var from = obj.From;
            var to = obj.To;

            return
                (from != null ? m_vertexComparer.GetHashCode(from) : 0) ^
                (to != null ? m_vertexComparer.GetHashCode(to) : 0);
        }

        public override bool Equals(object? obj) =>
            obj is GraphEdgeEqualityComparer<TVertex>.Undirected other &&
            m_vertexComparer.Equals(other.m_vertexComparer);

        public override int GetHashCode() => m_vertexComparer.GetHashCode() ^ 37;
    }
}

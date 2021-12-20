using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <inheritdoc/>
        public bool GraphEquals(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.SetEquals(other.Vertices) &&
                Edges.SetEquals(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsEdgeInducedSubgraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSubsetOf(other.Vertices) &&
                Edges.IsSubsetOf(GetInducedSubgraphEdges(this, other)) &&
                Vertices.All(IsVertexConnected);
        }

        /// <inheritdoc/>
        public bool IsEdgeInducedSupergraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSupersetOf(other.Vertices) &&
                Edges.IsSupersetOf(GetInducedSubgraphEdges(this, other)) &&
                other.Vertices.All(other.IsVertexConnected);
        }

        static IEnumerable<GraphEdge<T>> GetInducedSubgraphEdges(IReadOnlyGraph<T> a, IReadOnlyGraph<T> b)
        {
            var aVertices = a.Vertices;
            return b.Edges.Where(e => aVertices.Contains(e.From) && aVertices.Contains(e.To));
        }

        /// <inheritdoc/>
        public bool IsProperSubgraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                !GraphEquals(other) &&
                IsSubgraphOf(other);
        }

        /// <inheritdoc/>
        public bool IsProperSupergraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                !GraphEquals(other) &&
                IsSupergraphOf(other);
        }

        /// <inheritdoc/>
        public bool IsSubgraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSubsetOf(other.Vertices) &&
                Edges.IsSubsetOf(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsSupergraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSupersetOf(other.Vertices) &&
                Edges.IsSupersetOf(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsVertexInducedSubgraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSubsetOf(other.Vertices) &&
                VertexInducedSubgraphEdgesEqual(this, other);
        }

        /// <inheritdoc/>
        public bool IsVertexInducedSupergraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSupersetOf(other.Vertices) &&
                VertexInducedSubgraphEdgesEqual(other, this);
        }

        static bool VertexInducedSubgraphEdgesEqual(IReadOnlyGraph<T> a, IReadOnlyGraph<T> b) =>
            a.Edges.SetEquals(GetInducedSubgraphEdges(a, b));
    }
}

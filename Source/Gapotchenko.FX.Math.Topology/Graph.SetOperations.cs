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
        public bool IsVertexInducedSubgraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                Vertices.IsSubsetOf(other.Vertices) &&
                InducedEdgesEqual(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsVertexInducedSupergraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                Vertices.IsSupersetOf(other.Vertices) &&
                InducedEdgesEqual(other.Edges);
        }

        bool InducedEdgesEqual(IEnumerable<GraphEdge<T>> others)
        {
            var vertices = Vertices;
            return Edges.SetEquals(others.Where(e => vertices.Contains(e.From) && vertices.Contains(e.To)));
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
    }
}

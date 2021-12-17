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
                VertexInducedEdgesEqual(this, other);
        }

        /// <inheritdoc/>
        public bool IsVertexInducedSupergraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSupersetOf(other.Vertices) &&
                VertexInducedEdgesEqual(other, this);
        }

        static bool VertexInducedEdgesEqual(IReadOnlyGraph<T> a, IReadOnlyGraph<T> b)
        {
            var aVertices = a.Vertices;
            return a.Edges.SetEquals(b.Edges.Where(e => aVertices.Contains(e.From) && aVertices.Contains(e.To)));
        }
    }
}

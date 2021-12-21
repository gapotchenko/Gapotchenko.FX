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
                Vertices.All(v => !IsVertexIsolated(v));
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
                other.Vertices.All(v => !other.IsVertexIsolated(v));
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

        /// <inheritdoc/>
        public void IntersectWith(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            IntersectWithCore(other);
        }

        void IntersectWithCore(IReadOnlyGraph<T> other)
        {
            Vertices.IntersectWith(other.Vertices);
            Edges.IntersectWith(other.Edges);
        }

        /// <summary>
        /// Gets a graph containing vertices and edges that are present in both the current and a specified graphs.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns>The graph containing vertices and edges that are present in both the current and a specified graphs.</returns>
        public Graph<T> Intersect(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var graph = Clone();
            graph.IntersectWithCore(other);
            return graph;
        }

        /// <inheritdoc/>
        public void ExceptWith(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            ExceptWithCore(other);
        }

        void ExceptWithCore(IReadOnlyGraph<T> other)
        {
            Vertices.ExceptWith(other.Vertices);
            Edges.ExceptWith(other.Edges);
        }

        IGraph<T> IGraph<T>.Intersect(IReadOnlyGraph<T> other) => Intersect(other);

        IReadOnlyGraph<T> IReadOnlyGraph<T>.Intersect(IReadOnlyGraph<T> other) => Intersect(other);

        /// <inheritdoc/>
        public void UnionWith(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            UnionWithCore(other);
        }

        void UnionWithCore(IReadOnlyGraph<T> other)
        {
            Vertices.UnionWith(other.Vertices);
            Edges.UnionWith(other.Edges);
        }

        /// <summary>
        /// Gets a graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns>The graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both.</returns>
        public Graph<T> Union(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var graph = Clone();
            graph.UnionWithCore(other);
            return graph;
        }

        IGraph<T> IGraph<T>.Union(IReadOnlyGraph<T> other) => Union(other);

        IReadOnlyGraph<T> IReadOnlyGraph<T>.Union(IReadOnlyGraph<T> other) => Union(other);
    }
}

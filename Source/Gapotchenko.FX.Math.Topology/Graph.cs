using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// <para>
    /// Represents a strongly-typed directional graph of objects.
    /// </para>
    /// <para>
    /// Graph is a set of vertices and edges.
    /// Vertices represent the objects, and edges represent the relations between them.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of vertices in the graph.</typeparam>
    [DebuggerDisplay("Order = {Vertices.Count}, Size = {Edges.Count}")]
    [DebuggerTypeProxy(typeof(GraphDebugView<>))]
    public partial class Graph<T> : IGraph<T>
        where T : notnull
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that is empty and uses the default equality comparer for graph vertices.
        /// </summary>
        public Graph() :
            this((IEqualityComparer<T>?)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that is empty and uses the specified equality comparer for graph vertices.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IEqualityComparer<T>? comparer)
        {
            m_AdjacencyList = new(comparer);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the default equality comparer for vertices
        /// and contains vertices and edges copied from the specified <see cref="IReadOnlyGraph{T}"/>.
        /// </summary>
        /// <param name="graph">The <see cref="IReadOnlyGraph{T}"/> whose vertices and edges are copied to the new <see cref="Graph{T}"/>.</param>
        public Graph(IReadOnlyGraph<T> graph) :
            this(graph, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the specified equality comparer for vertices
        /// and contains vertices and edges copied from the specified <see cref="IReadOnlyGraph{T}"/>.
        /// </summary>
        /// <param name="graph">The <see cref="IReadOnlyGraph{T}"/> whose vertices and edges are copied to the new <see cref="Graph{T}"/>.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IReadOnlyGraph<T> graph, IEqualityComparer<T>? comparer) :
            this(comparer)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            Edges.UnionWith(graph.Edges);
            Vertices.UnionWith(graph.Vertices);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the default equality comparer for vertices,
        /// contains vertices copied from the specified collection
        /// and edges defined by the specified incidence function.
        /// </summary>
        /// <param name="vertices">The collection of graph vertices.</param>
        /// <param name="incidence">The graph incidence function.</param>
        public Graph(IEnumerable<T> vertices, GraphIncidenceFunction<T> incidence) :
            this(vertices, incidence, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the specified equality comparer for vertices,
        /// contains vertices copied from the specified collection
        /// and edges defined by the specified incidence function.
        /// </summary>
        /// <param name="vertices">The collection of graph vertices.</param>
        /// <param name="incidence">The graph incidence function.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IEnumerable<T> vertices, GraphIncidenceFunction<T> incidence, IEqualityComparer<T>? comparer) :
            this(comparer)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));
            if (incidence == null)
                throw new ArgumentNullException(nameof(incidence));

            var list = vertices.AsReadOnlyList();
            int count = list.Count;

            for (int i = 0; i < count; ++i)
            {
                var from = list[i];

                bool edge = false;
                for (int j = 0; j < count; ++j)
                {
                    var to = list[j];

                    if (incidence(from, to))
                    {
                        Edges.Add(from, to);
                        edge = true;
                    }
                }

                if (!edge)
                    Vertices.Add(from);
            }
        }

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> that is used to determine equality of vertices for the graph.
        /// </summary>
        public IEqualityComparer<T> Comparer => m_AdjacencyList.Comparer;

        /// <summary>
        /// Graph adjacency row represents a set of vertices that relate to another vertex.
        /// </summary>
        protected internal sealed class AdjacencyRow : HashSet<T>
        {
            /// <summary>
            /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the specified equality comparer for vertices.
            /// </summary>
            /// <param name="comparer">The comparer.</param>
            public AdjacencyRow(IEqualityComparer<T>? comparer) :
                base(comparer)
            {
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append("{ ");

                bool first = true;
                foreach (var i in this)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");

                    sb.Append(i);
                }

                sb.Append(" }");
                return sb.ToString();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Dictionary<T, AdjacencyRow?> m_AdjacencyList;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        VertexSet? m_CachedVertices;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        VertexSet VerticesCore => m_CachedVertices ??= new(this);

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISet<T> Vertices => VerticesCore;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlySet<T> IReadOnlyGraph<T>.Vertices => VerticesCore;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        EdgeSet? m_CachedEdges;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        EdgeSet EdgesCore => m_CachedEdges ??= new(this);

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISet<GraphEdge<T>> Edges => EdgesCore;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlySet<GraphEdge<T>> IReadOnlyGraph<T>.Edges => EdgesCore;

        /// <summary>
        /// Cached number of vertices.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int? m_CachedOrder = 0;

        /// <summary>
        /// Cached number of edges.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int? m_CachedSize = 0;

        struct ReachibilityTraverser
        {
            public ReachibilityTraverser(Graph<T> graph, T destination, bool adjacent)
            {
                m_Graph = graph;
                m_Destination = destination;

                m_VisitedNodes = new HashSet<T>(graph.Comparer);
                m_Adjacent = adjacent;
            }

            readonly Graph<T> m_Graph;
            readonly T m_Destination;

            readonly HashSet<T> m_VisitedNodes;
            bool m_Adjacent;

            public bool CanBeReachedFrom(T source)
            {
                if (!m_VisitedNodes.Add(source))
                    return false;

                if (m_Graph.m_AdjacencyList.TryGetValue(source, out var adjRow) &&
                    adjRow != null)
                {
                    if (m_Adjacent)
                    {
                        if (adjRow.Contains(m_Destination))
                            return true;
                    }
                    else
                    {
                        m_Adjacent = true;
                    }

                    foreach (var i in adjRow)
                    {
                        if (CanBeReachedFrom(i))
                            return true;
                    }
                }

                return false;
            }
        }

        /// <inheritdoc/>
        public bool HasTransitivePath(T from, T to) => new ReachibilityTraverser(this, to, false).CanBeReachedFrom(from);

        /// <inheritdoc/>
        public bool ContainsPath(T from, T to) => Edges.Contains(from, to) || HasTransitivePath(from, to);

        /// <inheritdoc/>
        public void Clear()
        {
            m_AdjacencyList.Clear();
            m_CachedOrder = 0;
            m_CachedSize = 0;
        }

        /// <inheritdoc/>
        public IEnumerable<T> VerticesAdjacentTo(T vertex)
        {
            m_AdjacencyList.TryGetValue(vertex, out var adjRow);
            return adjRow ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Creates a new graph instance inheriting parent class settings such as comparer.
        /// </summary>
        /// <returns>The new graph instance.</returns>
        protected Graph<T> NewGraph() => new(Comparer);

        /// <summary>
        /// <para>
        /// Gets the graph adjacency list.
        /// </para>
        /// <para>
        /// The list consists of a number of rows, each of them representing a set of vertices that relate to another vertex.
        /// </para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected internal IDictionary<T, AdjacencyRow?> AdjacencyList => m_AdjacencyList;

        /// <summary>
        /// Creates a new adjacency row instance inheriting parent class settings such as comparer.
        /// </summary>
        /// <returns>The new adjacency row instance.</returns>
        protected AdjacencyRow NewAdjacencyRow() => new(Comparer);

        /// <summary>
        /// Invalidates the cache.
        /// This method should be called if <see cref="AdjacencyList"/> is manipulated directly.
        /// </summary>
        protected void InvalidateCache()
        {
            m_CachedOrder = null;
            m_CachedSize = null;
        }
    }
}

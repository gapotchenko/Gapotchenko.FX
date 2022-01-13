using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
    /// <typeparam name="TVertex">The type of vertices in the graph.</typeparam>
    [DebuggerDisplay("Order = {Vertices.Count}, Size = {Edges.Count}")]
    [DebuggerTypeProxy(typeof(GraphDebugView<>))]
    public partial class Graph<TVertex> : IGraph<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Graph{TVertex}"/> class that uses the default equality comparer for graph vertices.
        /// </summary>
        public Graph() :
            this((IEqualityComparer<TVertex>?)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{TVertex}"/> class that uses the specified equality comparer for graph vertices.
        /// </summary>
        /// <param name="vertexComparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IEqualityComparer<TVertex>? vertexComparer)
        {
            m_AdjacencyList = new(vertexComparer);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{TVertex}"/> class that uses the default equality comparer for vertices
        /// and contains vertices and edges copied from the specified <see cref="IReadOnlyGraph{TVertex}"/>.
        /// </summary>
        /// <param name="graph">The <see cref="IReadOnlyGraph{TVertex}"/> whose vertices and edges are copied to the new <see cref="Graph{TVertex}"/>.</param>
        public Graph(IReadOnlyGraph<TVertex> graph) :
            this(graph, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{TVertex}"/> class that uses the specified equality comparer for vertices
        /// and contains vertices and edges copied from the specified <see cref="IReadOnlyGraph{TVertex}"/>.
        /// </summary>
        /// <param name="graph">The <see cref="IReadOnlyGraph{TVertex}"/> whose vertices and edges are copied to the new <see cref="Graph{TVertex}"/>.</param>
        /// <param name="vertexComparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IReadOnlyGraph<TVertex> graph, IEqualityComparer<TVertex>? vertexComparer) :
            this(vertexComparer)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            UnionWithCore(graph);

            if (graph is Graph<TVertex> other && VertexComparer.Equals(other.VertexComparer))
                CopyCacheFrom(other);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{TVertex}"/> class that uses the default equality comparer for vertices
        /// and contains vertices copied from the specified collection
        /// and edges defined by the specified incidence function.
        /// </summary>
        /// <param name="vertices">The collection of graph vertices.</param>
        /// <param name="incidenceFunction">The graph incidence function.</param>
        public Graph(IEnumerable<TVertex> vertices, GraphIncidenceFunction<TVertex> incidenceFunction) :
            this(vertices, incidenceFunction, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{TVertex}"/> class that uses the specified equality comparer for vertices
        /// and contains vertices copied from the specified collection
        /// and edges defined by the specified incidence function.
        /// </summary>
        /// <param name="vertices">The collection of graph vertices.</param>
        /// <param name="incidenceFunction">The graph incidence function.</param>
        /// <param name="vertexComparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IEnumerable<TVertex> vertices, GraphIncidenceFunction<TVertex> incidenceFunction, IEqualityComparer<TVertex>? vertexComparer) :
            this(vertices, incidenceFunction, vertexComparer, GraphIncidenceOptions.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{TVertex}"/> class that uses the specified equality comparer for vertices
        /// and contains vertices copied from the specified collection
        /// and edges defined by the specified incidence function
        /// with the given options.
        /// </summary>
        /// <param name="vertices">The collection of graph vertices.</param>
        /// <param name="incidenceFunction">The graph incidence function.</param>
        /// <param name="vertexComparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        /// <param name="options">The graph incidence options.</param>
        public Graph(IEnumerable<TVertex> vertices, GraphIncidenceFunction<TVertex> incidenceFunction, IEqualityComparer<TVertex>? vertexComparer, GraphIncidenceOptions options) :
            this(vertexComparer)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));
            if (incidenceFunction == null)
                throw new ArgumentNullException(nameof(incidenceFunction));

            var list = vertices.AsReadOnlyList();
            int count = list.Count;

            bool reflexiveReducion = (options & GraphIncidenceOptions.ReflexiveReduction) != 0;
            bool storeIsolatedVertices =
                count == 1 || // a singleton graph is always connected by definition
                (options & GraphIncidenceOptions.Connected) == 0;

            for (int i = 0; i < count; ++i)
            {
                var from = list[i];

                bool storeIsolatedVertex = storeIsolatedVertices;

                for (int j = 0; j < count; ++j)
                {
                    if (reflexiveReducion && i == j)
                        continue;

                    var to = list[j];

                    if (incidenceFunction(from, to))
                    {
                        Edges.Add(from, to);
                        storeIsolatedVertex = false;
                    }
                }

                if (storeIsolatedVertex)
                    Vertices.Add(from);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IEqualityComparer<TVertex> VertexComparer => m_AdjacencyList.Comparer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        VertexSet? m_CachedVertices;

        /// <summary>
        /// Gets a set containing the vertices of the graph.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public VertexSet Vertices => m_CachedVertices ??= new(this);

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ISet<TVertex> IGraph<TVertex>.Vertices => Vertices;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlySet<TVertex> IReadOnlyGraph<TVertex>.Vertices => Vertices;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        EdgeSet? m_CachedEdges;

        /// <summary>
        /// Gets a set containing the edges of the graph.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public EdgeSet Edges => m_CachedEdges ??= new(this);

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ISet<GraphEdge<TVertex>> IGraph<TVertex>.Edges => Edges;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlySet<GraphEdge<TVertex>> IReadOnlyGraph<TVertex>.Edges => Edges;

        struct ReachibilityTraverser
        {
            public ReachibilityTraverser(Graph<TVertex> graph, TVertex destination, bool adjacent)
            {
                m_Graph = graph;
                m_Destination = destination;

                m_VisitedNodes = new HashSet<TVertex>(graph.VertexComparer);
                m_Adjacent = adjacent;
            }

            readonly Graph<TVertex> m_Graph;
            readonly TVertex m_Destination;

            readonly HashSet<TVertex> m_VisitedNodes;
            bool m_Adjacent;

            public bool CanBeReachedFrom(TVertex source)
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

        /// <summary>
        /// <para>
        /// Gets a value indicating whether there is a transitive path from a specified source vertex to a destination.
        /// </para>
        /// <para>
        /// A transitive path consists of two or more edges with at least one intermediate vertex.
        /// </para>
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The target vertex.</param>
        /// <returns><c>true</c> when the specified source vertex can reach the target via one or more intermediate vertices; otherwise, <c>false</c>.</returns>
        bool HasTransitivePath(TVertex from, TVertex to) => new ReachibilityTraverser(this, to, false).CanBeReachedFrom(from);

        /// <inheritdoc/>
        public bool HasPath(TVertex from, TVertex to) => Edges.Contains(from, to) || HasTransitivePath(from, to);

        /// <inheritdoc/>
        public void Clear()
        {
            if (m_AdjacencyList.Count == 0)
                return;

            m_AdjacencyList.Clear();
            m_CachedOrder = 0;
            m_CachedSize = 0;
            InvalidateCachedRelations();

#if !TFF_DICTIONARY_ENUMERATION_REMOVE_ALLOWED
            IncrementVersion();
#endif
        }

        /// <inheritdoc/>
        public IEnumerable<TVertex> DestinationVerticesAdjacentTo(TVertex vertex)
        {
            var mg = new ModificationGuard(this);
            m_AdjacencyList.TryGetValue(vertex, out var adjRow);
            return mg.Protect(adjRow) ?? Enumerable.Empty<TVertex>();
        }

        /// <inheritdoc/>
        public IEnumerable<GraphEdge<TVertex>> OutgoingEdgesIncidentWith(TVertex vertex) =>
            DestinationVerticesAdjacentTo(vertex)
            .Select(x => GraphEdge.Create(vertex, x));

        /// <summary>
        /// Creates a new graph instance inheriting parent class settings such as comparer.
        /// </summary>
        /// <returns>The new graph instance.</returns>
        protected Graph<TVertex> NewGraph() => new(VertexComparer);
    }
}

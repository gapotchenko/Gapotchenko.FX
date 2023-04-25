using Gapotchenko.FX.Linq;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology;

/// <summary>
/// Represents a strongly-typed directional graph of objects.
/// </summary>
/// <inheritdoc cref="IGraph{TVertex}"/>
[DebuggerDisplay($"Order = {nameof(Vertices)}.Count, Size = {nameof(Edges)}.Count")]
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
    IEqualityComparer<GraphEdge<TVertex>>? m_EdgeComparer;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEqualityComparer<GraphEdge<TVertex>> EdgeComparer => m_EdgeComparer ??= GraphEdge.CreateComparer(VertexComparer, IsDirected);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    VertexSet? m_CachedVertices;

    /// <inheritdoc cref="IGraph{TVertex}.Vertices"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public VertexSet Vertices => m_CachedVertices ??= new(this);

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ISet<TVertex> IGraph<TVertex>.Vertices => Vertices;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlySet<TVertex> IReadOnlyGraph<TVertex>.Vertices => Vertices;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    EdgeSet? m_CachedEdges;

    /// <inheritdoc cref="IGraph{TVertex}.Edges"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public EdgeSet Edges => m_CachedEdges ??= new(this);

    /// <inheritdoc/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ISet<GraphEdge<TVertex>> IGraph<TVertex>.Edges => Edges;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlySet<GraphEdge<TVertex>> IReadOnlyGraph<TVertex>.Edges => Edges;

    /// <inheritdoc/>
    public void Clear()
    {
        if (m_AdjacencyList.Count == 0)
            return;

        m_AdjacencyList.Clear();
        m_CachedOrder = 0;
        m_CachedSize = 0;
        m_ReverseAdjacencyList = null;
        InvalidateCachedRelations();

#if !TFF_DICTIONARY_ENUMERATION_REMOVE_ALLOWED
        IncrementVersion();
#endif
    }

    /// <inheritdoc/>
    public IEnumerable<TVertex> SourceVerticesAdjacentTo(TVertex vertex)
    {
        var mg = new ModificationGuard(this);
        ReverseAdjacencyList.TryGetValue(vertex, out var adjRow);
        return mg.Protect(adjRow) ?? Enumerable.Empty<TVertex>();
    }

    /// <inheritdoc/>
    public IEnumerable<TVertex> DestinationVerticesAdjacentTo(TVertex vertex)
    {
        var mg = new ModificationGuard(this);
        m_AdjacencyList.TryGetValue(vertex, out var adjRow);
        return mg.Protect(adjRow) ?? Enumerable.Empty<TVertex>();
    }

    /// <inheritdoc/>
    public IEnumerable<TVertex> VerticesAdjacentTo(TVertex vertex) =>
        SourceVerticesAdjacentTo(vertex)
        .Concat(DestinationVerticesAdjacentTo(vertex))
        .Distinct(VertexComparer);

    /// <inheritdoc/>
    public IEnumerable<GraphEdge<TVertex>> IncomingEdgesIncidentWith(TVertex vertex) =>
        SourceVerticesAdjacentTo(vertex)
        .Select(x => GraphEdge.Create(x, vertex));

    /// <inheritdoc/>
    public IEnumerable<GraphEdge<TVertex>> OutgoingEdgesIncidentWith(TVertex vertex) =>
        DestinationVerticesAdjacentTo(vertex)
        .Select(x => GraphEdge.Create(vertex, x));

    /// <inheritdoc/>
    public IEnumerable<GraphEdge<TVertex>> EdgesIncidentWith(TVertex vertex) =>
        DistinctSelfLoop(
            IncomingEdgesIncidentWith(vertex).Concat(OutgoingEdgesIncidentWith(vertex)),
            VertexComparer);

    static IEnumerable<GraphEdge<TVertex>> DistinctSelfLoop(
        IEnumerable<GraphEdge<TVertex>> source,
        IEqualityComparer<TVertex> vertexComparer)
    {
        bool hasSelfLoop = false;

        foreach (var i in source)
        {
            if (vertexComparer.Equals(i.From, i.To))
            {
                if (hasSelfLoop)
                    continue;
                hasSelfLoop = true;
            }

            yield return i;
        }
    }

    /// <summary>
    /// Creates a new graph instance inheriting parent object settings such as comparer and edge direction awareness,
    /// but without inheriting vertices and edges.
    /// </summary>
    /// <returns>A new graph instance with inherited parent object settings.</returns>
    protected Graph<TVertex> NewGraph() =>
        new(VertexComparer)
        {
            IsDirected = IsDirected
        };
}

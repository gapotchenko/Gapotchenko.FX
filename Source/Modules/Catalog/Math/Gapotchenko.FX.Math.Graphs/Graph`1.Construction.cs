// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Linq;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
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
        ArgumentNullException.ThrowIfNull(graph);

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
        ArgumentNullException.ThrowIfNull(vertices);
        ArgumentNullException.ThrowIfNull(incidenceFunction);

        var list = vertices.ReifyList();
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

    /// <summary>
    /// Creates a new graph instance inheriting parent object settings such as comparer and edge direction awareness,
    /// but without inheriting its contents (vertices and edges).
    /// </summary>
    /// <returns>A new graph instance with inherited parent object settings.</returns>
    protected Graph<TVertex> NewGraph() =>
        new(VertexComparer)
        {
            IsDirected = IsDirected
        };
}

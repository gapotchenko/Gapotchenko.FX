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
    [DebuggerDisplay("Order = {Vertices.Count}, Size = {Size}")]
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
        /// or <c>null</c> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IEqualityComparer<T>? comparer)
        {
            m_AdjacencyList = new(comparer);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the default equality comparer for vertices
        /// and contains vertices and edges copied from the specified <see cref="IReadOnlyGraph{T}"/>.
        /// </summary>
        /// <param name="graph">The <see cref="IReadOnlyGraph{T}"/> whole vertices and edges are copied to the new <see cref="Graph{T}"/>.</param>
        public Graph(IReadOnlyGraph<T> graph) :
            this(graph, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the specified equality comparer for vertices
        /// and contains vertices and edges copied from the specified <see cref="IReadOnlyGraph{T}"/>.
        /// </summary>
        /// <param name="graph">The <see cref="IReadOnlyGraph{T}"/> whole vertices and edges are copied to the new <see cref="Graph{T}"/>.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <c>null</c> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IReadOnlyGraph<T> graph, IEqualityComparer<T>? comparer) :
            this(comparer)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            foreach (var edge in graph.Edges)
                AddEdge(edge.From, edge.To);

            foreach (var vertex in graph.Vertices)
                Vertices.Add(vertex);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the default equality comparer for vertices,
        /// contains vertices copied from the specified collection
        /// and edges representing the relations between the vertices defined by the specified dependency function.
        /// </summary>
        public Graph(IEnumerable<T> collection, DependencyFunction<T> df) :
            this(collection, df, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the specified equality comparer for vertices,
        /// contains vertices copied from the specified collection
        /// and edges representing the relations between the vertices defined by the specified dependency function.
        /// </summary>
        public Graph(IEnumerable<T> collection, DependencyFunction<T> df, IEqualityComparer<T>? comparer) :
            this(comparer)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (df == null)
                throw new ArgumentNullException(nameof(df));

            var list = collection.AsReadOnly();
            int count = list.Count;

            for (int i = 0; i < count; ++i)
            {
                var ei = list[i];
                bool edge = false;

                for (int j = 0; j < count; ++j)
                {
                    var ej = list[j];

                    if (df(ei, ej))
                    {
                        AddEdge(ei, ej);
                        edge = true;
                    }
                }

                if (!edge)
                    Vertices.Add(ei);
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
        /// Gets a set containing the vertices of the graph.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public VertexSet Vertices => new(this);

        ISet<T> IGraph<T>.Vertices => Vertices;

        IReadOnlySet<T> IReadOnlyGraph<T>.Vertices => Vertices;

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IEnumerable<GraphEdge<T>> Edges
        {
            get
            {
                foreach (var i in AdjacencyList)
                {
                    var adjRow = i.Value;
                    if (adjRow != null)
                    {
                        var a = i.Key;
                        foreach (var b in adjRow)
                            yield return new GraphEdge<T>(a, b);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public int Size => AdjacencyList.Select(x => x.Value?.Count ?? 0).Sum();

        /// <inheritdoc/>
        public bool AddEdge(T from, T to)
        {
            var adjList = AdjacencyList;

            if (!adjList.TryGetValue(from, out var adjRow))
            {
                adjRow = NewAdjacencyRow();
                adjList.Add(from, adjRow);
            }
            else if (adjRow == null)
            {
                adjRow = NewAdjacencyRow();
                adjList[from] = adjRow;
            }

            return adjRow.Add(to);
        }

        /// <inheritdoc/>
        public bool ContainsEdge(T from, T to) =>
            AdjacencyList.TryGetValue(from, out var adjRow) &&
            adjRow != null &&
            adjRow.Contains(to);

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

                if (m_Graph.AdjacencyList.TryGetValue(source, out var adjRow) &&
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
        public bool HasPath(T from, T to) => ContainsEdge(from, to) || HasTransitivePath(from, to);

        /// <inheritdoc/>
        public void Clear() => AdjacencyList.Clear();

        /// <inheritdoc/>
        public IEnumerable<T> VerticesAdjacentTo(T vertex)
        {
            AdjacencyList.TryGetValue(vertex, out var adjRow);
            return adjRow ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Creates a new graph instance inheriting parent class settings such as comparer.
        /// </summary>
        /// <returns>The new graph instance.</returns>
        protected Graph<T> NewGraph() => new(Comparer);

        /// <summary>
        /// Creates a new adjacency row instance inheriting parent class settings such as comparer.
        /// </summary>
        /// <returns>The new adjacency row instance.</returns>
        protected AdjacencyRow NewAdjacencyRow() => new(Comparer);
    }
}

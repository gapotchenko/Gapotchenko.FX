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
    [DebuggerDisplay("Order = {Order}, Size = {Size}")]
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
                AddEdge(edge.A, edge.B);

            foreach (var vertex in graph.Vertices)
                AddVertex(vertex);
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
                    AddVertex(ei);
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

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IEnumerable<T> Vertices =>
            AdjacencyList
            .SelectMany(x => (x.Value ?? Enumerable.Empty<T>()).Append(x.Key))
            .Distinct(Comparer);

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IEnumerable<(T A, T B)> Edges
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
                            yield return (a, b);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public int Order => Vertices.Count();

        /// <inheritdoc/>
        public int Size => AdjacencyList.Select(x => x.Value?.Count ?? 0).Sum();

        /// <inheritdoc/>
        public bool AddVertex(T v)
        {
            if (ContainsVertex(v))
                return false;
            AdjacencyList.Add(v, null);
            return true;
        }

        /// <inheritdoc/>
        public bool RemoveVertex(T v)
        {
            bool hit = false;
            var adjList = AdjacencyList;

            hit |= adjList.Remove(v);

            foreach (var i in adjList)
            {
                var adjRow = i.Value;
                if (adjRow != null)
                    hit |= adjRow.Remove(v);
            }

            return hit;
        }

        /// <inheritdoc/>
        public bool ContainsVertex(T v) =>
            AdjacencyList.ContainsKey(v) ||
            AdjacencyList.Any(x => x.Value?.Contains(v) ?? false);

        /// <inheritdoc/>
        public bool AddEdge(T a, T b)
        {
            var adjList = AdjacencyList;

            if (!adjList.TryGetValue(a, out var adjRow))
            {
                adjRow = NewAdjacencyRow();
                adjList.Add(a, adjRow);
            }
            else if (adjRow == null)
            {
                adjRow = NewAdjacencyRow();
                adjList[a] = adjRow;
            }

            return adjRow.Add(b);
        }

        /// <inheritdoc/>
        public bool ContainsEdge(T a, T b) =>
            AdjacencyList.TryGetValue(a, out var adjRow) &&
            adjRow != null &&
            adjRow.Contains(b);

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
        public bool AreTransitiveVertices(T a, T b) => new ReachibilityTraverser(this, b, false).CanBeReachedFrom(a);

        /// <inheritdoc/>
        public bool AreReachableVertices(T a, T b) => ContainsEdge(a, b) || AreTransitiveVertices(a, b);

        /// <inheritdoc/>
        public void Clear() => AdjacencyList.Clear();

        /// <inheritdoc/>
        public IEnumerable<T> VerticesAdjacentTo(T v)
        {
            AdjacencyList.TryGetValue(v, out var adjRow);
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

        /// <inheritdoc/>
        public void Reduce()
        {
            foreach (var i in AdjacencyList)
            {
                var adjRow = i.Value;
                if (adjRow == null)
                    continue;

                var a = i.Key;

                var removeList = new List<T>();

                foreach (var b in adjRow)
                {
                    if (adjRow.Contains(b) && AreTransitiveVertices(a, b))
                        removeList.Add(b);
                }

                adjRow.ExceptWith(removeList);
            }
        }

        /// <summary>
        /// Gets a transitively reduced graph.
        /// </summary>
        /// <returns>The transitively reduced graph.</returns>
        public Graph<T> GetReduction()
        {
            var graph = Clone();
            graph.Reduce();
            return graph;
        }

        IGraph<T> IGraph<T>.GetReduction() => GetReduction();

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetReduction() => GetReduction();

        /// <inheritdoc/>
        public void Transpose()
        {
            var edges = Edges.ToList();
            var vertices = Vertices.ToList();

            Clear();
            TransposeCore(this, edges, vertices);
        }

        /// <summary>
        /// Gets a graph transposition by reversing its edge directions.
        /// </summary>
        /// <returns>The transposed graph.</returns>
        public Graph<T> GetTransposition()
        {
            var graph = NewGraph();
            TransposeCore(graph, Edges, Vertices);
            return graph;
        }

        static void TransposeCore(Graph<T> graph, IEnumerable<(T A, T B)> edges, IEnumerable<T> vertices)
        {
            foreach (var edge in edges)
                graph.AddEdge(edge.B, edge.A);

            foreach (var vertex in vertices)
                graph.AddVertex(vertex);
        }

        IGraph<T> IGraph<T>.GetTransposition() => GetTransposition();

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetTransposition() => GetTransposition();

        /// <summary>
        /// Clones the graph.
        /// </summary>
        /// <returns>The new graph.</returns>
        public Graph<T> Clone() => new(this, Comparer);

        IGraph<T> ICloneable<IGraph<T>>.Clone() => Clone();

        IReadOnlyGraph<T> ICloneable<IReadOnlyGraph<T>>.Clone() => Clone();

        object ICloneable.Clone() => Clone();
    }
}

using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Math.Topology
{
    [DebuggerDisplay("Order = {Order}")]
    [DebuggerTypeProxy(typeof(GraphDebugView<>))]
    public class Graph<T> : IGraph<T>
        where T : notnull
    {
        public Graph() :
            this(null)
        {
        }

        public Graph(IEqualityComparer<T>? comparer)
        {
            m_AdjacencyList = new(comparer);
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

        /// <summary>
        /// Determines whether the graph contains a specified vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns><c>true</c> when the graph contains a specified vertex; otherwise, <c>false</c>.</returns>
        public bool ContainsVertex(T v) =>
            AdjacencyList.ContainsKey(v) ||
            AdjacencyList.Any(x => x.Value?.Contains(v) ?? false);

        /// <summary>
        /// Adds the specified edge.
        /// </summary>
        /// <param name="a">The A vertex of the edge.</param>
        /// <param name="b">The B vertex of the edge.</param>
        /// <returns><c>true</c> if the edge is added to the <see cref="Graph{T}"/> object; <c>false</c> if the edge is already present.</returns>
        public bool AddEdge(T a, T b)
        {
            var adjList = AdjacencyList;

            if (!adjList.TryGetValue(a, out var adjRow))
            {
                adjRow = new AdjacencyRow(Comparer);
                adjList.Add(a, adjRow);
            }
            else if (adjRow == null)
            {
                adjRow = new AdjacencyRow(Comparer);
                adjList[a] = adjRow;
            }

            return adjRow.Add(b);
        }

        /// <inheritdoc/>
        public bool AreAdjacent(T a, T b) =>
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
        public bool AreTransitive(T a, T b) => new ReachibilityTraverser(this, b, false).CanBeReachedFrom(a);

        /// <inheritdoc/>
        public bool AreReachable(T a, T b) => AreAdjacent(a, b) || AreTransitive(a, b);

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
                    if (adjRow.Contains(b) && AreTransitive(a, b))
                        removeList.Add(b);
                }

                adjRow.ExceptWith(removeList);
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
        /// Clones a graph.
        /// </summary>
        /// <returns>The new graph.</returns>
        public Graph<T> Clone()
        {
            var graph = NewGraph();

            foreach (var edge in Edges)
                graph.AddEdge(edge.A, edge.B);

            foreach (var vertex in Vertices)
                graph.AddVertex(vertex);

            return graph;
        }

        IGraph<T> ICloneable<IGraph<T>>.Clone() => Clone();

        IReadOnlyGraph<T> ICloneable<IReadOnlyGraph<T>>.Clone() => Clone();

        object ICloneable.Clone() => Clone();
    }
}

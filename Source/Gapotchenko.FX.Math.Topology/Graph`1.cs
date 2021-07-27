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

        protected internal sealed class AdjacencyRow : HashSet<T>
        {
            public AdjacencyRow(IEqualityComparer<T> comparer) :
                base(comparer)
            {
            }

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
        Dictionary<T, AdjacencyRow?> m_AdjacencyList;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected internal IDictionary<T, AdjacencyRow?> AdjacencyList => m_AdjacencyList;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IEnumerable<T> Vertices =>
            AdjacencyList
            .SelectMany(x => (x.Value ?? Enumerable.Empty<T>()).Append(x.Key))
            .Distinct(Comparer);

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

        /// <summary>
        /// <para>
        /// Gets the order.
        /// </para>
        /// <para>
        /// The order of a graph is |V|, the number of its vertices.
        /// </para>
        /// </summary>
        public int Order => Vertices.Count();

        public int Size => AdjacencyList.Select(x => x.Value?.Count ?? 0).Sum();

        /// <summary>
        /// Adds the specified vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns><c>true</c> if the vertex is added to the <see cref="Graph{T}"/> object; <c>false</c> if the vertex is already present.</returns>
        public bool AddVertex(T v)
        {
            if (ContainsVertex(v))
                return false;
            AdjacencyList.Add(v, null);
            return true;
        }

        /// <summary>
        /// Removes the specified vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns><c>true</c> if the vertex was removed from the <see cref="Graph{T}"/>; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// <para>
        /// Gets a value indicating whether specified vertices are adjacent.
        /// </para>
        /// <para>
        /// Adjacent vertices are those connected by an edge.
        /// </para>
        /// </summary>
        /// <param name="a">The vertex A.</param>
        /// <param name="b">The vertex B.</param>
        /// <returns><c>true</c> when a specified vertex <paramref name="a">A</paramref> is adjacent to vertex <paramref name="b">B</paramref>; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// <para>
        /// Gets a value indicating whether specified vertices are transitive.
        /// </para>
        /// <para>
        /// Transitive vertices are those connected by at least one intermediate vertex.
        /// </para>
        /// </summary>
        /// <param name="a">The vertex A.</param>
        /// <param name="b">The vertex B.</param>
        /// <returns><c>true</c> when a specified vertex <paramref name="a">A</paramref> can reach vertex <paramref name="b">B</paramref> via one or more intermediate vertices; otherwise, <c>false</c>.</returns>
        public bool AreTransitive(T a, T b) => new ReachibilityTraverser(this, b, false).CanBeReachedFrom(a);

        /// <summary>
        /// Gets a value indicating whether specified vertices are reachable.
        /// </summary>
        /// <param name="a">The vertex A.</param>
        /// <param name="b">The vertex B.</param>
        /// <returns><c>true</c> when a specified vertex <paramref name="a">A</paramref> can reach vertex <paramref name="b">B</paramref>; otherwise, <c>false</c>.</returns>
        public bool AreReachable(T a, T b) => AreAdjacent(a, b) || AreTransitive(a, b);

        /// <summary>
        /// Clears the graph.
        /// </summary>
        public void Clear() => AdjacencyList.Clear();

        /// <summary>
        /// Gets the adjacent vertices of a specified vertex.
        /// </summary>
        /// <param name="v">The vertex.</param>
        /// <returns>Sequence of adjacent vertices.</returns>
        public IEnumerable<T> AdjacentVertices(T v)
        {
            AdjacencyList.TryGetValue(v, out var adjRow);
            return adjRow ?? Enumerable.Empty<T>();
        }

        Graph<T> NewGraph() => new Graph<T>(Comparer);

        /// <summary>
        /// Performs a transitive reduction of the graph.
        /// </summary>
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

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetReduction() => GetReduction();

        IGraph<T> IGraph<T>.GetReduction() => GetReduction();

        /// <summary>
        /// Transposes the graph by reversing edge directions.
        /// </summary>
        public void Transpose()
        {
            var edges = Edges.ToList();
            var vertices = Vertices.ToList();

            Clear();

            TransposeCore(this, edges, vertices);
        }

        /// <summary>
        /// Gets a graph transposition by reversing edge directions.
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

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetTransposition() => GetTransposition();

        IGraph<T> IGraph<T>.GetTransposition() => GetTransposition();

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

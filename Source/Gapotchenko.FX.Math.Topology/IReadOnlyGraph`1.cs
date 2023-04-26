namespace Gapotchenko.FX.Math.Topology;

/// <summary>
/// Provides a read-only graph abstraction.
/// </summary>
/// <remarks>
/// Graph is a set of vertices and edges.
/// Vertices represent the objects and edges represent the relations between them.
/// </remarks>
/// <typeparam name="TVertex">The type of vertices in the graph.</typeparam>
public partial interface IReadOnlyGraph<TVertex>
{
    /// <summary>
    /// Gets a set containing vertices of the graph.
    /// </summary>
    /// <remarks>
    /// Vertices represent objects contained in a graph.
    /// </remarks>
    IReadOnlySet<TVertex> Vertices { get; }

    /// <summary>
    /// Gets a set containing edges of the graph.
    /// </summary>
    /// <remarks>
    /// Edges represent relations between graph vertices.
    /// </remarks>
    IReadOnlySet<GraphEdge<TVertex>> Edges { get; }

    /// <summary>
    /// Gets incoming vertices adjacent to the specified vertex.
    /// </summary>
    /// <remarks>
    /// Adjacent incoming vertices are source end-vertices of incoming edges incident with the specified vertex.
    /// </remarks>
    /// <param name="vertex">The vertex to find the adjacent incoming vertices for.</param>
    /// <returns>Sequence of incoming vertices adjacent to the specified <paramref name="vertex"/>.</returns>
    IEnumerable<TVertex> IncomingVerticesAdjacentTo(TVertex vertex);

    /// <summary>
    /// Gets outgoing vertices adjacent to the specified vertex.
    /// </summary>
    /// <remarks>
    /// Adjacent outgoing vertices are destination end-vertices of outgoing edges incident with the specified vertex.
    /// </remarks>
    /// <param name="vertex">The vertex to find the adjacent destination vertices for.</param>
    /// <returns>Sequence of outgoing vertices adjacent to the specified <paramref name="vertex"/>.</returns>
    IEnumerable<TVertex> OutgoingVerticesAdjacentTo(TVertex vertex);

    /// <summary>
    /// Gets all vertices adjacent to the specified vertex.
    /// </summary>
    /// <remarks>
    /// Adjacent vertices are end-vertices of edges incident with the specified vertex.
    /// </remarks>
    /// <param name="vertex">The vertex to find the adjacent vertices for.</param>
    /// <returns>Sequence of all vertices adjacent to the specified <paramref name="vertex"/>.</returns>
    IEnumerable<TVertex> VerticesAdjacentTo(TVertex vertex);

    /// <summary>
    /// Gets incoming vertices that have a path to the specified vertex.
    /// </summary>
    /// <param name="vertex">The vertex to find the incoming connected vertices for.</param>
    /// <returns>Sequence of incoming vertices that have a path to the specified <paramref name="vertex"/>.</returns>
    IEnumerable<TVertex> IncomingVerticesConnectedWith(TVertex vertex);

    /// <summary>
    /// Gets outgoing vertices that have a path from the specified vertex.
    /// </summary>
    /// <param name="vertex">The vertex to find the outgoing connected vertices for.</param>
    /// <returns>Sequence of outgoing vertices that have a path from the specified <paramref name="vertex"/>.</returns>
    IEnumerable<TVertex> OutgoingVerticesConnectedWith(TVertex vertex);

    /// <summary>
    /// Gets all vertices that have a path from/to the specified vertex.
    /// </summary>
    /// <param name="vertex">The vertex to find the connected vertices for.</param>
    /// <returns>Sequence of all vertices that have a path from/to the specified <paramref name="vertex"/>.</returns>
    IEnumerable<TVertex> VerticesConnectedWith(TVertex vertex);

    /// <summary>
    /// Gets incoming edges incident with the specified vertex.
    /// </summary>
    /// <param name="vertex">The vertex to find the incoming incident edges for.</param>
    /// <returns>Sequence of incoming edges incident with the specified <paramref name="vertex"/>.</returns>
    IEnumerable<GraphEdge<TVertex>> IncomingEdgesIncidentWith(TVertex vertex);

    /// <summary>
    /// Gets outgoing edges incident with the specified vertex.
    /// </summary>
    /// <param name="vertex">The vertex to find the outgoing incident edges for.</param>
    /// <returns>Sequence of outgoing edges incident with the specified <paramref name="vertex"/>.</returns>
    IEnumerable<GraphEdge<TVertex>> OutgoingEdgesIncidentWith(TVertex vertex);

    /// <summary>
    /// Gets incoming and outgoing edges incident with the specified vertex.
    /// </summary>
    /// <param name="vertex">The vertex to find the incident edges for.</param>
    /// <returns>Sequence of edges incident with the specified <paramref name="vertex"/>.</returns>
    IEnumerable<GraphEdge<TVertex>> EdgesIncidentWith(TVertex vertex);

    /// <summary>
    /// Gets a value indicating whether the current graph is directed.
    /// </summary>
    bool IsDirected { get; }

    /// <summary>
    /// Determines whether the current graph contains a cycle.
    /// </summary>
    bool IsCyclic { get; }

    /// <summary>
    /// Determines whether the current graph is connected.
    /// </summary>
    /// <remarks>
    /// A graph is connected if every pair of vertices has a path between them.
    /// A graph containing less than two vertices is connected by the definition.
    /// </remarks>
    bool IsConnected { get; }

    /// <summary>
    /// Gets a value indicating whether there is a path from the specified source vertex to the destination.
    /// </summary>
    /// <remarks>
    /// A path consists of one or more edges with or without intermediate vertices.
    /// The vertices that have a path between them are called connected vertices.
    /// </remarks>
    /// <param name="from">The source vertex.</param>
    /// <param name="to">The destination vertex.</param>
    /// <returns>
    /// <see langword="true"/> when the specified source vertex can reach the destination;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool HasPath(TVertex from, TVertex to);

    /// <summary>
    /// <para>
    /// Determines whether the specified vertex is isolated.
    /// An isolated vertex is not connected to any other vertex.
    /// </para>
    /// <para>
    /// An isolated vertex is a vertex with degree zero; that is, a vertex that is not an end-vertex of any edge.
    /// </para>
    /// </summary>
    /// <param name="vertex">The vertex.</param>
    /// <returns>
    /// <see langword="true"/> when the specified vertex is isolated;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool IsVertexIsolated(TVertex vertex);

    /// <summary>
    /// Gets a sequence of isolated vertices of the current graph.
    /// </summary>
    /// <remarks>
    /// Isolated vertices are vertices with degree zero; that is, vertices that are not incident with any edges.
    /// </remarks>
    IEnumerable<TVertex> IsolatedVertices { get; }

    /// <summary>
    /// Gets an indegree of the specified vertex.
    /// </summary>
    /// <remarks>
    /// Indegree of a vertex is a number of incoming edges incident with the vertex.
    /// </remarks>
    /// <param name="vertex">The vertex to find an indegree of.</param>
    /// <returns>An indegree of the specified vertex.</returns>
    int GetVertexIndegree(TVertex vertex);

    /// <summary>
    /// Gets an outdegree of the specified vertex.
    /// </summary>
    /// <remarks>
    /// Outdegree of a vertex is a number of outgoing edges incident with the vertex.
    /// </remarks>
    /// <param name="vertex">The vertex to find an outdegree of.</param>
    /// <returns>An outdegree of the specified vertex.</returns>
    int GetVertexOutdegree(TVertex vertex);

    /// <summary>
    /// Gets a degree of the specified vertex.
    /// </summary>
    /// <remarks>
    /// Degree of a vertex is a number of all edges incident with the vertex.
    /// </remarks>
    /// <param name="vertex">The vertex to find a degree of.</param>
    /// <returns>A degree of the specified vertex.</returns>
    int GetVertexDegree(TVertex vertex);

    /// <summary>
    /// Gets a vertex-induced subgraph of the current graph from the specified vertices.
    /// </summary>
    /// <param name="vertices">The vertices to induce the subgraph from.</param>
    /// <returns>A vertex-induced subgraph of the current graph.</returns>
    IReadOnlyGraph<TVertex> GetSubgraph(IEnumerable<TVertex> vertices);

    /// <summary>
    /// Gets an edge-induced subgraph of the current graph from the specified edges.
    /// </summary>
    /// <param name="edges">The edges to induce the subgraph from.</param>
    /// <returns>An edge-induced subgraph of the current graph.</returns>
    IReadOnlyGraph<TVertex> GetSubgraph(IEnumerable<GraphEdge<TVertex>> edges);

    /// <summary>
    /// Gets a transposition of the current graph by reversing its edge directions.
    /// </summary>
    /// <returns>A transposed graph.</returns>
    IReadOnlyGraph<TVertex> GetTransposition();

    /// <summary>
    /// Gets a transitive reduction of the current graph.
    /// </summary>
    /// <remarks>
    /// Transitive reduction removes transitive relations with shorter alternative paths.
    /// The worst estimated algorithmic complexity of the operation is <c>O(n^2.3729)</c> where <c>n</c> is a graph order (number of vertices).
    /// The actual complexity depends on a structure of the graph.
    /// </remarks>
    /// <returns>A transitively reduced graph.</returns>
    IReadOnlyGraph<TVertex> GetTransitiveReduction();

    /// <summary>
    /// Gets a reflexive reduction of the current graph.
    /// </summary>
    /// <remarks>
    /// Reflexive reduction removes loops (also called self-loops or buckles) from a graph.
    /// A loop exists when a vertex has an incident edge outgoing to itself.
    /// </remarks>
    /// <returns>A graph without loops.</returns>
    IReadOnlyGraph<TVertex> GetReflexiveReduction();

    /// <summary>
    /// Determines whether the current graph and the specified one contain the same vertices and edges.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns><see langword="true"/> if the current graph is equal to other; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
    bool GraphEquals(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Determines whether the current graph is an edge-induced subgraph of a specified graph.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns><see langword="true" /> if the current graph is an edge-induced subgraph of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
    bool IsEdgeInducedSubgraphOf(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Determines whether the current graph is an edge-induced supergraph of a specified graph.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns><see langword="true" /> if the current graph is an edge-induced supergraph of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
    bool IsEdgeInducedSupergraphOf(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Determines whether the current graph is a proper (strict) subgraph of a specified graph.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns><see langword="true" /> if the current graph is a proper subgraph of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
    bool IsProperSubgraphOf(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Determines whether the current graph is a proper (strict) supergraph of a specified graph.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns><see langword="true" /> if the current graph is a proper supergraph of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
    bool IsProperSupergraphOf(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Determines whether the current graph is a subgraph of a specified graph.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns><see langword="true" /> if the current graph is a subgraph of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
    bool IsSubgraphOf(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Determines whether the current graph is a supergraph of a specified graph.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns><see langword="true" /> if the current graph is a supergraph of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
    bool IsSupergraphOf(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Determines whether the current graph is a vertex-induced subgraph of a specified graph.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns><see langword="true" /> if the current graph is a vertex-induced subgraph of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
    bool IsVertexInducedSubgraphOf(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Determines whether the current graph is a vertex-induced supergraph of a specified graph.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns><see langword="true" /> if the current graph is a vertex-induced supergraph of other; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">other is <see langword="null" />.</exception>
    bool IsVertexInducedSupergraphOf(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Gets a graph containing vertices and edges that are present in both the current and a specified graphs.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns>A graph containing vertices and edges that are present in both the current and a specified graphs.</returns>
    IReadOnlyGraph<TVertex> Intersect(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Gets a graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns>A graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both.</returns>
    IReadOnlyGraph<TVertex> Union(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Gets a graph containing vertices and edges that are present in the current graph but not in the specified graph.
    /// </summary>
    /// <param name="other">The graph to compare to the current one.</param>
    /// <returns>A graph containing vertices and edges that are present in the current graph but not in the specified graph.</returns>
    IReadOnlyGraph<TVertex> Except(IReadOnlyGraph<TVertex> other);

    /// <summary>
    /// Gets a sequence of graph vertices sorted in topological order.
    /// </summary>
    /// <remarks>
    /// Topological order of a directed graph is an order of its vertices such that for every directed edge u → v, u comes before v.
    /// </remarks>
    /// <returns>Sequence of graph vertices sorted in topological order.</returns>
    /// <exception cref="CircularDependencyException">Graph contains a cycle.</exception>
    IOrderedEnumerable<TVertex> OrderTopologically();

    /// <summary>
    /// Gets a sequence of graph vertices sorted in reverse topological order.
    /// </summary>
    /// <remarks>
    /// Reverse topological order of a directed graph is an order of its vertices such that for every directed edge u → v, v comes before u.
    /// </remarks>
    /// <returns>Sequence of graph vertices sorted in reverse topological order.</returns>
    /// <exception cref="CircularDependencyException">Graph contains a cycle.</exception>
    IOrderedEnumerable<TVertex> OrderTopologicallyInReverse();
}

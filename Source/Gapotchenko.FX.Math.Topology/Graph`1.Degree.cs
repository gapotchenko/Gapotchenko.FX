namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc />
    public int GetVertexIndegree(TVertex vertex) =>
        m_AdjacencyList
            .Where(kv => kv.Value?.Contains(vertex) == true)
            .Count();

    /// <inheritdoc />
    public int GetVertexOutdegree(TVertex vertex) =>
        m_AdjacencyList.TryGetValue(vertex, out var adjacencyRow) ?
            adjacencyRow?.Count ?? 0 :
            0;

    /// <inheritdoc />
    public int GetVertexDegree(TVertex vertex) =>
        GetVertexIndegree(vertex) + GetVertexOutdegree(vertex);
}

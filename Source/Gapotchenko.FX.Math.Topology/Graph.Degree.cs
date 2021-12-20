using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <inheritdoc />
        public int GetVertexIndegree(T vertex) =>
            m_AdjacencyList
                .Where(kv => kv.Value?.Contains(vertex) == true)
                .Count();

        /// <inheritdoc />
        public int GetVertexOutdegree(T vertex) =>
            m_AdjacencyList.TryGetValue(vertex, out var adjacencyRow) ?
                adjacencyRow?.Count ?? 0 :
                0;

        /// <inheritdoc />
        public int GetVertexDegree(T vertex) =>
            GetVertexIndegree(vertex) + GetVertexOutdegree(vertex);
    }
}

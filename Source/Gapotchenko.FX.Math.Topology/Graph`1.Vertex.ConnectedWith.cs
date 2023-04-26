namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public IEnumerable<TVertex> IncomingVerticesConnectedWith(TVertex vertex) =>
        EnumerateVerticesConnectedWith(vertex, IncomingVerticesAdjacentTo);

    /// <inheritdoc/>
    public IEnumerable<TVertex> OutgoingVerticesConnectedWith(TVertex vertex) =>
        EnumerateVerticesConnectedWith(vertex, OutgoingVerticesAdjacentTo);

    /// <inheritdoc/>
    public IEnumerable<TVertex> VerticesConnectedWith(TVertex vertex) =>
        IncomingVerticesConnectedWith(vertex)
        .Union(OutgoingVerticesConnectedWith(vertex), VertexComparer);

    IEnumerable<TVertex> EnumerateVerticesConnectedWith(
        TVertex vertex,
        Func<TVertex, IEnumerable<TVertex>> verticesAdjacentTo)
    {
        var visitedVertices = new HashSet<TVertex>(VertexComparer);
        bool self = false;

        IEnumerable<TVertex> Traverse(TVertex vertex)
        {
            if (self)
            {
                if (visitedVertices.Add(vertex) == false)
                    yield break;

                yield return vertex;
            }
            else
            {
                self = true;
            }

            foreach (var i in verticesAdjacentTo(vertex))
            {
                foreach (var j in Traverse(i))
                    yield return j;
            }
        }

        return Traverse(vertex);
    }
}

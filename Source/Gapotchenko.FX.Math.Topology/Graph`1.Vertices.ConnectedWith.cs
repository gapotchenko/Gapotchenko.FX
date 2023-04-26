namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public IEnumerable<TVertex> IncomingVerticesConnectedWith(TVertex vertex) =>
        IsDirected ?
            new ModificationGuard(this).Protect(
                EnumerateVerticesConnectedWith(vertex, IncomingVerticesAdjacentToCore)) :
            VerticesConnectedWith(vertex);

    IEnumerable<TVertex> IncomingVerticesConnectedWithCore(TVertex vertex) =>
        EnumerateVerticesConnectedWith(vertex, IncomingVerticesAdjacentToCore);

    /// <inheritdoc/>
    public IEnumerable<TVertex> OutgoingVerticesConnectedWith(TVertex vertex) =>
        IsDirected ?
            new ModificationGuard(this).Protect(
                EnumerateVerticesConnectedWith(vertex, OutgoingVerticesAdjacentToCore)) :
            VerticesConnectedWith(vertex);

    IEnumerable<TVertex> OutgoingVerticesConnectedWithCore(TVertex vertex) =>
        EnumerateVerticesConnectedWith(vertex, OutgoingVerticesAdjacentToCore);

    /// <inheritdoc/>
    public IEnumerable<TVertex> VerticesConnectedWith(TVertex vertex) =>
        new ModificationGuard(this).Protect(VerticesConnectedWithCore(vertex));

    IEnumerable<TVertex> VerticesConnectedWithCore(TVertex vertex) =>
        IsDirected ?
            IncomingVerticesConnectedWithCore(vertex).Union(
                OutgoingVerticesConnectedWithCore(vertex),
                VertexComparer) :
            VerticesConnectedWithUndirectedCore(vertex);

    IEnumerable<TVertex> VerticesConnectedWithUndirectedCore(TVertex vertex) =>
        EnumerateVerticesConnectedWith(vertex, VerticesAdjacentToCore);

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

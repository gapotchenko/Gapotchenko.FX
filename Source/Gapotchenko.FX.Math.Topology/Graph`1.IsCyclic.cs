using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        /// <inheritdoc/>
        public bool IsCyclic
        {
            get
            {
                var comparer = VertexComparer;
                var visited = new HashSet<TVertex>(comparer);
                var recStack = new HashSet<TVertex>(comparer);

                bool IsCyclicHelper(TVertex v)
                {
                    if (recStack.Contains(v))
                        return true;

                    if (!visited.Add(v))
                        return false;

                    recStack.Add(v);

                    foreach (var i in DestinationVerticesAdjacentTo(v))
                        if (IsCyclicHelper(i))
                            return true;

                    recStack.Remove(v);

                    return false;
                }

                foreach (var v in Vertices)
                {
                    if (IsCyclicHelper(v))
                        return true;
                }

                return false;
            }
        }
    }
}

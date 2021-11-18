using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <inheritdoc/>
        public bool IsCyclic
        {
            get
            {
                var comparer = Comparer;
                var visited = new HashSet<T>(comparer);
                var recStack = new HashSet<T>(comparer);

                bool IsCyclicHelper(T v)
                {
                    if (recStack.Contains(v))
                        return true;

                    if (!visited.Add(v))
                        return false;

                    recStack.Add(v);

                    foreach (var i in VerticesAdjacentTo(v))
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

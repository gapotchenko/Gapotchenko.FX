using System.Collections.Generic;
using System.Text;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        /// <summary>
        /// Graph adjacency row represents a set of vertices that relate to another vertex.
        /// </summary>
        protected internal sealed class AdjacencyRow : HashSet<TVertex>
        {
            internal AdjacencyRow(IEqualityComparer<TVertex>? comparer) :
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

        /// <summary>
        /// Creates a new adjacency row instance.
        /// </summary>
        /// <returns>The new adjacency row instance.</returns>
        protected AdjacencyRow NewAdjacencyRow() => new(VertexComparer);
    }
}

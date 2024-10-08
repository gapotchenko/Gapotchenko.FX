// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Text;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <summary>
    /// Graph adjacency row represents a set of vertices that relate to another vertex.
    /// </summary>
    protected sealed class AdjacencyRow : HashSet<TVertex>
    {
        internal AdjacencyRow(IEqualityComparer<TVertex>? comparer) :
            base(comparer)
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            // TODO: refactor to $"{{ {string.Join(", ", this)} }}".

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

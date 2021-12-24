using System;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Graph incidence options.
    /// </summary>
    [Flags]
    public enum GraphIncidenceOptions
    {
        /// <summary>
        /// No options.
        /// </summary>
        None,

        /// <summary>
        /// Perform reflexive reduction by not calling a graph incidence function for all v → v edges where v ∈ V and V is a set of vertices of the graph.
        /// </summary>
        ReflexiveReduction = 1 << 1,

        /// <summary>
        /// <para>
        /// Exclude isolated vertices.
        /// </para>
        /// <para>
        /// Isolated vertices are vertices with degree zero; that is, vertices that are not incident with any edge.
        /// </para>
        /// </summary>
        ExcludeIsolatedVertices = 1 << 2
    }
}

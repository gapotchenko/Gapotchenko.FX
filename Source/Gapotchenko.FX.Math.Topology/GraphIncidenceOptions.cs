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
        /// <para>
        /// Perform reflexive reduction by not calling a graph incidence function for all v → v edges where v ∈ V and V is a set of vertices of the graph.
        /// </para>
        /// <para>
        /// This option eliminates loops (also called self-loops or buckles) from appearing in the resulting graph.
        /// </para>
        /// </summary>
        ReflexiveReduction = 1 << 1,

        /// <summary>
        /// Produce a connected graph by ignoring the vertices not connected by edges.
        /// </summary>
        Connected = 1 << 2
    }
}

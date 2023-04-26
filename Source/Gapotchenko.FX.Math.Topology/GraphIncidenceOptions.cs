﻿namespace Gapotchenko.FX.Math.Topology;

/// <summary>
/// Specifies graph incidence options.
/// </summary>
[Flags]
public enum GraphIncidenceOptions
{
    /// <summary>
    /// No options.
    /// </summary>
    None,

    /// <summary>
    /// Specifies to perform a reflexive reduction
    /// by not calling a graph incidence function for all <c>v → v</c> edges
    /// where <c>v ∈ V</c> and <c>V</c> is a set of graph vertices.
    /// </summary>
    /// <remarks>
    /// This option eliminates loops (also called self-loops or buckles) from appearing in a resulting graph.
    /// A loop exists when a vertex has an incident edge outgoing to itself.
    /// </remarks>
    ReflexiveReduction = 1 << 1,

    /// <summary>
    /// Specifies to produce a mostly connected graph
    /// by excluding vertices that are not incident with any edge.
    /// </summary>
    Connected = 1 << 2
}

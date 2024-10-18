// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Math.Metrics;

/// <summary>
/// Defines traits of a string metric.
/// </summary>
[Flags]
public enum StringMetricTraits
{
    /// <summary>
    /// No traits.
    /// </summary>
    None,

    /// <summary>
    /// The metric considers element insertions.
    /// </summary>
    Insertion = 1 << 0,

    /// <summary>
    /// The metric considers element deletions.
    /// </summary>
    Deletion = 1 << 1,

    /// <summary>
    /// The metric considers element substitutions.
    /// </summary>
    Substitution = 1 << 2,

    /// <summary>
    /// The metric considers element transpositions.
    /// </summary>
    Transposition = 1 << 3
}

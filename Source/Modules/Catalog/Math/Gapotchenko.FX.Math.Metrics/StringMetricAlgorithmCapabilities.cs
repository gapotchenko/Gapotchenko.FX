namespace Gapotchenko.FX.Math.Metrics;

/// <summary>
/// Defines capabilities of a string metric algorithm.
/// </summary>
[Flags]
public enum StringMetricAlgorithmCapabilities
{
    /// <summary>
    /// No capabilities.
    /// </summary>
    None,

    /// <summary>
    /// Measures character insertions.
    /// </summary>
    Insertion = 1 << 0,

    /// <summary>
    /// Measures character deletions.
    /// </summary>
    Deletion = 1 << 1,

    /// <summary>
    /// Measures character substitutions.
    /// </summary>
    Substitution = 1 << 2,

    /// <summary>
    /// Measures character transpositions.
    /// </summary>
    Transposition = 1 << 3
}

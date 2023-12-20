namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Defines the types of interval boundaries.
/// </summary>
public enum IntervalBoundaryKind : byte
{
    /// <summary>
    /// Empty boundary, ∅.
    /// </summary>
    Empty,

    /// <summary>
    /// Negative infinity, -∞.
    /// </summary>
    NegativeInfinity,

    /// <summary>
    /// <para>
    /// Indicates that the bound limit point is included in the interval.
    /// </para>
    /// <para>
    /// An inclusive boundary is also called a closed boundary.
    /// </para>
    /// </summary>
    Inclusive,

    /// <summary>
    /// <para>
    /// Indicates that the bound limit point is not included in the interval.
    /// </para>
    /// <para>
    /// An exclusive boundary is also called an open boundary.
    /// </para>
    /// </summary>
    Exclusive,

    /// <summary>
    /// Positive infinity, +∞.
    /// </summary>
    PositiveInfinity
}

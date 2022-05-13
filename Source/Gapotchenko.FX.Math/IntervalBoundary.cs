namespace Gapotchenko.FX.Math
{
    /// <summary>
    /// Defines the types of interval boundaries.
    /// </summary>
    public enum IntervalBoundary
    {
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
        /// Indicates that the boundary is unbounded to the negative infinity.
        /// </summary>
        NegativeInfinity,

        /// <summary>
        /// Indicates that the boundary is unbounded to the positive infinity.
        /// </summary>
        PositiveInfinity
    }
}

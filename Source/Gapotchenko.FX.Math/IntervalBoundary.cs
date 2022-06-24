namespace Gapotchenko.FX.Math;

/// <summary>
/// Provides static methods for creating interval boundaries.
/// </summary>
public static class IntervalBoundary
{
    /// <summary>
    /// <para>
    /// Creates an inclusive interval boundary with the specified value.
    /// </para>
    /// <para>
    /// An inclusive boundary is also called a closed boundary.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of a boundary value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The inclusive interval boundary.</returns>
    public static IntervalBoundary<T> Inclusive<T>(T value) => new(IntervalBoundaryKind.Inclusive, value);

    /// <summary>
    /// <para>
    /// Creates an exclusive interval boundary with the specified value.
    /// </para>
    /// <para>
    /// An exclusive boundary is also called an open boundary.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of a boundary value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The inclusive interval boundary.</returns>
    public static IntervalBoundary<T> Exclusive<T>(T value) => new(IntervalBoundaryKind.Exclusive, value);
}

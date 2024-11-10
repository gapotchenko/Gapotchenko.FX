using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Math;

/// <summary>
/// Provides extended static methods for mathematical functions.
/// </summary>
public static partial class MathEx
{
    /// <summary>
    /// Swaps two values.
    /// </summary>
    /// <typeparam name="T">The type of values to swap.</typeparam>
    /// <param name="val1">The first of two values to swap.</param>
    /// <param name="val2">The second of two values to swap.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap<T>(ref T val1, ref T val2)
    {
        var temp = val1;
        val1 = val2;
        val2 = temp;
    }

    /// <summary>
    /// Returns a linear interpolation between specified <paramref name="a"/> and <paramref name="b"/> values by a given <paramref name="t" /> coefficient belonging to the interval <c>[0,1]</c>.
    /// </summary>
    /// <remarks>
    /// When <paramref name="t"/> is <c>0</c>, the value of <paramref name="a"/> is returned.
    /// When <paramref name="t"/> is <c>1</c>, the value of <paramref name="b"/> is returned.
    /// When <paramref name="t"/> is <c>0.5</c>, the midpoint of <paramref name="a"/> and <paramref name="b"/> is returned.
    /// </remarks>
    /// <param name="a">The start value.</param>
    /// <param name="b">The end value.</param>
    /// <param name="t">The interpolation coefficient between the start and end values.</param>
    /// <returns>
    /// The linear interpolation between <paramref name="a"/> and <paramref name="b"/> by <paramref name="t"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Lerp(double a, double b, double t) => a + (b - a) * t;

    /// <inheritdoc cref="Lerp(double, double, double)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Lerp(float a, float b, float t) => a + (b - a) * t;
}

namespace Gapotchenko.FX.Math;

using Math = System.Math;

partial class MathEx
{
    /// <summary>
    /// Returns the smaller of three 32-bit signed integers.
    /// </summary>
    /// <param name="val1">The first of three 32-bit signed integers to compare.</param>
    /// <param name="val2">The second of three 32-bit signed integers to compare.</param>
    /// <param name="val3">The third of three 32-bit signed integers to compare.</param>
    /// <returns>
    /// Parameter <paramref name="val1"/>, <paramref name="val2"/> or <paramref name="val3"/>, whichever is smaller.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int Min(int val1, int val2, int val3) => Math.Min(Math.Min(val1, val2), val3);

    /// <summary>
    /// Returns the larger of three 32-bit signed integers.
    /// </summary>
    /// <param name="val1">The first of three 32-bit signed integers to compare.</param>
    /// <param name="val2">The second of three 32-bit signed integers to compare.</param>
    /// <param name="val3">The third of three 32-bit signed integers to compare.</param>
    /// <returns>
    /// Parameter <paramref name="val1"/>, <paramref name="val2"/> or <paramref name="val3"/>, whichever is larger.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int Max(int val1, int val2, int val3) => Math.Max(Math.Max(val1, val2), val3);

    /// <summary>
    /// Returns the smaller of two <see cref="DateTime"/> structures.
    /// </summary>
    /// <param name="val1">The first of two <see cref="DateTime"/> structures to compare.</param>
    /// <param name="val2">The second of two <see cref="DateTime"/> structures to compare.</param>
    /// <returns>
    /// Parameter <paramref name="val1"/> or <paramref name="val2"/>, whichever is smaller.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DateTime Min(DateTime val1, DateTime val2) => val1 <= val2 ? val1 : val2;

    /// <summary>
    /// Returns the larger of two <see cref="DateTime"/> structures.
    /// </summary>
    /// <param name="val1">The first of two <see cref="DateTime"/> structures to compare.</param>
    /// <param name="val2">The second of two <see cref="DateTime"/> structures to compare.</param>
    /// <returns>
    /// Parameter <paramref name="val1"/> or <paramref name="val2"/>, whichever is larger.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DateTime Max(DateTime val1, DateTime val2) => val1 >= val2 ? val1 : val2;

    /// <summary>
    /// Returns the smaller of three <see cref="DateTime"/> structures.
    /// </summary>
    /// <param name="val1">The first of three <see cref="DateTime"/> structures to compare.</param>
    /// <param name="val2">The second of three <see cref="DateTime"/> structures to compare.</param>
    /// <param name="val3">The third of three <see cref="DateTime"/> structures to compare.</param>
    /// <returns>
    /// Parameter <paramref name="val1"/>, <paramref name="val2"/> or <paramref name="val3"/>, whichever is smaller.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DateTime Min(DateTime val1, DateTime val2, DateTime val3) => Min(Min(val1, val2), val3);

    /// <summary>
    /// Returns the larger of three <see cref="DateTime"/> structures.
    /// </summary>
    /// <param name="val1">The first of three <see cref="DateTime"/> structures to compare.</param>
    /// <param name="val2">The second of three <see cref="DateTime"/> structures to compare.</param>
    /// <param name="val3">The third of three <see cref="DateTime"/> structures to compare.</param>
    /// <returns>
    /// Parameter <paramref name="val1"/>, <paramref name="val2"/> or <paramref name="val3"/>, whichever is larger.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DateTime Max(DateTime val1, DateTime val2, DateTime val3) => Max(Max(val1, val2), val3);
}

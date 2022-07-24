using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Math;

partial class MathEx
{
    /// <summary>
    /// Returns the smaller of two values.
    /// </summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>
    /// Parameter <paramref name="val1"/> or <paramref name="val2"/>, whichever is smaller.
    /// </returns>
    [return: NotNullIfNotNull("val1")]
    [return: NotNullIfNotNull("val2")]
    public static T? Min<T>(T? val1, T? val2) where T : IComparable<T>
    {
        if (val1 == null)
            return val2;
        if (val2 == null)
            return val1;

        // Give a preference to the first value when both values are equal.
        // This is important for reference types and composite value types.
        if (val1.CompareTo(val2) <= 0)
            return val1;
        else
            return val2;
    }

    /// <summary>
    /// Returns the smaller of two values.
    /// </summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <param name="comparer">
    /// The optional comparer.
    /// When this parameter is <c>null</c>, a default comparer for type <typeparamref name="T"/> is used.
    /// </param>
    /// <returns>
    /// Parameter <paramref name="val1"/> or <paramref name="val2"/>, whichever is smaller.
    /// </returns>
    [return: NotNullIfNotNull("val1")]
    [return: NotNullIfNotNull("val2")]
    public static T? Min<T>(T? val1, T? val2, IComparer<T>? comparer = null)
    {
        if (val1 == null)
            return val2;
        if (val2 == null)
            return val1;

        comparer ??= Comparer<T>.Default;

        // Give a preference to the first value when both values are equal.
        // This is important for reference types and composite value types.
        if (comparer.Compare(val1, val2) <= 0)
            return val1;
        else
            return val2;
    }

    /// <summary>
    /// Returns the larger of two values.
    /// </summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <returns>
    /// Parameter <paramref name="val1"/> or <paramref name="val2"/>, whichever is larger.
    /// </returns>
    [return: NotNullIfNotNull("val1")]
    [return: NotNullIfNotNull("val2")]
    public static T? Max<T>(T? val1, T? val2) where T : IComparable<T>
    {
        if (val1 == null)
            return val2;
        if (val2 == null)
            return val1;

        // Give a preference to the first value when both values are equal.
        // This is important for reference types and composite value types.
        if (val1.CompareTo(val2) >= 0)
            return val1;
        else
            return val2;
    }

    /// <summary>
    /// Returns the larger of two values.
    /// </summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="val1">The first of two values to compare.</param>
    /// <param name="val2">The second of two values to compare.</param>
    /// <param name="comparer">
    /// The optional comparer.
    /// When this parameter is <c>null</c>, a default comparer for type <typeparamref name="T"/> is used.
    /// </param>
    /// <returns>
    /// Parameter <paramref name="val1"/> or <paramref name="val2"/>, whichever is larger.
    /// </returns>
    [return: NotNullIfNotNull("val1")]
    [return: NotNullIfNotNull("val2")]
    public static T? Max<T>(T? val1, T? val2, IComparer<T>? comparer = null)
    {
        if (val1 == null)
            return val2;
        if (val2 == null)
            return val1;

        comparer ??= Comparer<T>.Default;

        // Give a preference to the first value when both values are equal.
        // This is important for reference types and composite value types.
        if (comparer.Compare(val1, val2) >= 0)
            return val1;
        else
            return val2;
    }

    /// <summary>
    /// Returns the smaller of three values.
    /// </summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="val1">The first of three values to compare.</param>
    /// <param name="val2">The second of three values to compare.</param>
    /// <param name="val3">The third of three values to compare.</param>
    /// <returns>
    /// Parameter <paramref name="val1"/>, <paramref name="val2"/> or <paramref name="val3"/>, whichever is smaller.
    /// </returns>
    [return: NotNullIfNotNull("val1")]
    [return: NotNullIfNotNull("val2")]
    [return: NotNullIfNotNull("val3")]
    public static T? Min<T>(T? val1, T? val2, T? val3) where T : IComparable<T> =>
        Min(Min(val1, val2), val3);

    /// <summary>
    /// Returns the smaller of three values.
    /// </summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="val1">The first of three values to compare.</param>
    /// <param name="val2">The second of three values to compare.</param>
    /// <param name="val3">The third of three values to compare.</param>
    /// <param name="comparer">
    /// The optional comparer.
    /// When this parameter is <c>null</c>, a default comparer for type <typeparamref name="T"/> is used.
    /// </param>
    /// <returns>
    /// Parameter <paramref name="val1"/>, <paramref name="val2"/> or <paramref name="val3"/>, whichever is smaller.
    /// </returns>
    [return: NotNullIfNotNull("val1")]
    [return: NotNullIfNotNull("val2")]
    [return: NotNullIfNotNull("val3")]
    public static T? Min<T>(T? val1, T? val2, T? val3, IComparer<T>? comparer = null) =>
        Min(Min(val1, val2, comparer), val3, comparer);

    /// <summary>
    /// Returns the larger of three values.
    /// </summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="val1">The first of three values to compare.</param>
    /// <param name="val2">The second of three values to compare.</param>
    /// <param name="val3">The third of three values to compare.</param>
    /// <returns>
    /// Parameter <paramref name="val1"/>, <paramref name="val2"/> or <paramref name="val3"/>, whichever is larger.
    /// </returns>
    [return: NotNullIfNotNull("val1")]
    [return: NotNullIfNotNull("val2")]
    [return: NotNullIfNotNull("val3")]
    public static T? Max<T>(T? val1, T? val2, T? val3) where T : IComparable<T> =>
        Max(Max(val1, val2), val3);

    /// <summary>
    /// Returns the larger of three values.
    /// </summary>
    /// <typeparam name="T">The type of values to compare.</typeparam>
    /// <param name="val1">The first of three values to compare.</param>
    /// <param name="val2">The second of three values to compare.</param>
    /// <param name="val3">The third of three values to compare.</param>
    /// <param name="comparer">
    /// The optional comparer.
    /// When this parameter is <c>null</c>, a default comparer for type <typeparamref name="T"/> is used.
    /// </param>
    /// <returns>
    /// Parameter <paramref name="val1"/>, <paramref name="val2"/> or <paramref name="val3"/>, whichever is larger.
    /// </returns>
    [return: NotNullIfNotNull("val1")]
    [return: NotNullIfNotNull("val2")]
    [return: NotNullIfNotNull("val3")]
    public static T? Max<T>(T? val1, T? val2, T? val3, IComparer<T>? comparer = null) =>
        Max(Max(val1, val2, comparer), val3, comparer);
}

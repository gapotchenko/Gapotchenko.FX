using Gapotchenko.FX.Runtime.CompilerServices;
using System.Diagnostics;

namespace Gapotchenko.FX;

partial class Fn
{
    /// <summary>
    /// Applies a specified accumulator function over a specified sequence of values.
    /// </summary>
    /// <typeparam name="T">The type of values to aggregate.</typeparam>
    /// <param name="f">The accumulator function.</param>
    /// <param name="val1">The first of three values to aggregate.</param>
    /// <param name="val2">The second of three values to aggregate.</param>
    /// <param name="val3">The third of three values to aggregate.</param>
    /// <returns>The final aggregated value calculated as <c>f(f(val1, val2), val3)</c>.</returns>
    public static T Aggregate<T>(
        Func<T, T, T> f,
        T val1, T val2, T val3)
    {
        if (f == null)
            _ThrowArgumentNullException(nameof(f));

        return f(f(val1, val2), val3);
    }

    [DoesNotReturn, StackTraceHidden]
    static void _ThrowArgumentNullException(string paramName)
    {
        // This is a separate method to allow the inlining of a caller method.
        throw new ArgumentNullException(paramName);
    }

    /// <summary>
    /// Applies a specified accumulator function over a specified sequence of values.
    /// </summary>
    /// <typeparam name="T">The type of values to aggregate.</typeparam>
    /// <param name="f">The accumulator function.</param>
    /// <param name="val1">The first of four values to aggregate.</param>
    /// <param name="val2">The second of four values to aggregate.</param>
    /// <param name="val3">The third of four values to aggregate.</param>
    /// <param name="val4">The fourth of four values to aggregate.</param>
    /// <returns>The final aggregated value calculated as <c>f(f(f(val1, val2), val3), val4)</c>.</returns>
    public static T Aggregate<T>(
        Func<T, T, T> f,
        T val1, T val2, T val3, T val4) =>
        f(Aggregate(f, val1, val2, val3), val4);

    /// <summary>
    /// Applies a specified accumulator function over a specified sequence of values.
    /// </summary>
    /// <typeparam name="T">The type of values to aggregate.</typeparam>
    /// <param name="f">The accumulator function.</param>
    /// <param name="val1">The first of five values to aggregate.</param>
    /// <param name="val2">The second of five values to aggregate.</param>
    /// <param name="val3">The third of five values to aggregate.</param>
    /// <param name="val4">The fourth of five values to aggregate.</param>
    /// <param name="val5">The fifth of five values to aggregate.</param>
    /// <returns>The final aggregated value calculated as <c>f(f(f(f(val1, val2), val3), val4), val5)</c>.</returns>
    public static T Aggregate<T>(
        Func<T, T, T> f,
        T val1, T val2, T val3, T val4, T val5) =>
        f(Aggregate(f, val1, val2, val3, val4), val5);

    /// <summary>
    /// Applies a specified accumulator function over a specified sequence of values.
    /// </summary>
    /// <typeparam name="T">The type of values to aggregate.</typeparam>
    /// <param name="f">The accumulator function.</param>
    /// <param name="val1">The first of six values to aggregate.</param>
    /// <param name="val2">The second of six values to aggregate.</param>
    /// <param name="val3">The third of six values to aggregate.</param>
    /// <param name="val4">The fourth of six values to aggregate.</param>
    /// <param name="val5">The fifth of six values to aggregate.</param>
    /// <param name="val6">The sixth of six values to aggregate.</param>
    /// <returns>The final aggregated value calculated as <c>f(f(f(f(f(val1, val2), val3), val4), val5), val6)</c>.</returns>
    public static T Aggregate<T>(
        Func<T, T, T> f,
        T val1, T val2, T val3, T val4, T val5, T val6) =>
        f(Aggregate(f, val1, val2, val3, val4, val5), val6);

    /// <summary>
    /// Applies a specified accumulator function over a specified sequence of values.
    /// </summary>
    /// <typeparam name="T">The type of values to aggregate.</typeparam>
    /// <param name="f">The accumulator function.</param>
    /// <param name="val1">The first of seven values to aggregate.</param>
    /// <param name="val2">The second of seven values to aggregate.</param>
    /// <param name="val3">The third of seven values to aggregate.</param>
    /// <param name="val4">The fourth of seven values to aggregate.</param>
    /// <param name="val5">The fifth of seven values to aggregate.</param>
    /// <param name="val6">The sixth of seven values to aggregate.</param>
    /// <param name="val7">The seventh of seven values to aggregate.</param>
    /// <returns>The final aggregated value calculated as <c>f(f(f(f(f(f(val1, val2), val3), val4), val5), val6), val7)</c>.</returns>
    public static T Aggregate<T>(
        Func<T, T, T> f,
        T val1, T val2, T val3, T val4, T val5, T val6, T val7) =>
        f(Aggregate(f, val1, val2, val3, val4, val5, val6), val7);

    /// <summary>
    /// Applies a specified accumulator function over a specified sequence of values.
    /// </summary>
    /// <typeparam name="T">The type of values to aggregate.</typeparam>
    /// <param name="f">The accumulator function.</param>
    /// <param name="val1">The first of eight values to aggregate.</param>
    /// <param name="val2">The second of eight values to aggregate.</param>
    /// <param name="val3">The third of eight values to aggregate.</param>
    /// <param name="val4">The fourth of eight values to aggregate.</param>
    /// <param name="val5">The fifth of eight values to aggregate.</param>
    /// <param name="val6">The sixth of eight values to aggregate.</param>
    /// <param name="val7">The seventh of eight values to aggregate.</param>
    /// <param name="val8">The eighth of eight values to aggregate.</param>
    /// <returns>The final aggregated value calculated as <c>f(f(f(f(f(f(f(val1, val2), val3), val4), val5), val6), val7), val8)</c>.</returns>
    public static T Aggregate<T>(
        Func<T, T, T> f,
        T val1, T val2, T val3, T val4, T val5, T val6, T val7, T val8) =>
        f(Aggregate(f, val1, val2, val3, val4, val5, val6, val7), val8);

    /// <summary>
    /// Applies a specified accumulator function over a specified sequence of values.
    /// </summary>
    /// <remarks>
    /// To aggregate the values of any enumerable sequence,
    /// use <see cref="Enumerable.Aggregate{TSource}(System.Collections.Generic.IEnumerable{TSource}, Func{TSource, TSource, TSource})"/> extension method.
    /// </remarks>
    /// <typeparam name="T">The type of values to aggregate.</typeparam>
    /// <param name="f">The accumulator function.</param>
    /// <param name="val1">The first value to aggregate.</param>
    /// <param name="val2">The second value to aggregate.</param>
    /// <param name="val3">The third value to aggregate.</param>
    /// <param name="val4">The fourth value to aggregate.</param>
    /// <param name="val5">The fifth value to aggregate.</param>
    /// <param name="val6">The sixth value to aggregate.</param>
    /// <param name="val7">The seventh value to aggregate.</param>
    /// <param name="val8">The eighth value to aggregate.</param>
    /// <param name="rest">The rest of values to aggregate.</param>
    /// <returns>The final aggregated value calculated as <c>...f(f(f(f(f(f(f(f(val1, val2), val3), val4), val5), val6), val7), val8), ...)</c>.</returns>
    public static T Aggregate<T>(
        Func<T, T, T> f,
        T val1, T val2, T val3, T val4, T val5, T val6, T val7, T val8,
        params T[]? rest)
    {
        if (rest == null && TypeTraits<T>.IsValueType)
            throw new ArgumentNullException(nameof(rest), "The argument cannot be null when T is a value type.");

        var a = Aggregate(f, val1, val2, val3, val4, val5, val6, val7, val8);

        if (rest == null)
        {
            // The automatic disambiguation for a 'rest = null' scenario when T is a reference type and imaginary val9 is meant to be null.
            a = f(a, default!);
        }
        else
        {
            foreach (var val in rest)
                a = f(a, val);
        }

        return a;
    }
}

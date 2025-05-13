// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Linq.Operators;

/// <summary>
/// Provides extension polyfill methods for LINQ operators.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class LinqOperatorPolyfills
{
    /// <summary>
    /// Passes the current value to the specified function as an argument.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The method represents a LINQ-style pipe operator for programming languages that do not have a built-in pipe operator.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
    /// <typeparam name="TInput">The type of pipe input.</typeparam>
    /// <typeparam name="TOutput">The type of pipe output.</typeparam>
    /// <param name="input">The pipe input.</param>
    /// <param name="func">The function which calculates the pipe result from the specified pipe <paramref name="input"/>.</param>
    /// <returns>A pipe output which is calculated by <paramref name="func"/> from the specified pipe <paramref name="input"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="func"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TOutput PipeTo<TInput, TOutput>(this TInput input, Func<TInput, TOutput> func) =>
        (func ?? throw new ArgumentNullException(nameof(func)))
        (input);

    /// <summary>
    /// Passes the current value to the specified function as an argument.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The method represents a LINQ-style pipe operator for programming languages that do not have a built-in pipe operator.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
    /// <typeparam name="TInput">The type of pipe input.</typeparam>
    /// <param name="input">The pipe input.</param>
    /// <param name="action">The method that receives the specified pipe <paramref name="input"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void PipeTo<TInput>(this TInput input, Action<TInput> action) =>
        (action ?? throw new ArgumentNullException(nameof(action)))
        (input);

#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY

    /// <inheritdoc cref="PipeTo{TInput, TOutput}(TInput, Func{TInput, TOutput})"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use PipeTo method instead.")]
    public static TOutput PipeOperator<TInput, TOutput>(
#if SOURCE_COMPATIBILITY
        this
#endif
        TInput input,
        Func<TInput, TOutput> func) =>
        PipeTo(input, func);

    /// <inheritdoc cref="PipeTo{TInput}(TInput, Action{TInput})"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use PipeTo method instead.")]
    public static void PipeOperator<TInput>(
#if SOURCE_COMPATIBILITY
        this
#endif
        TInput input,
        Action<TInput> action) =>
        PipeTo(input, action);

#endif
}

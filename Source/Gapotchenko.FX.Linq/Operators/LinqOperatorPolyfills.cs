namespace Gapotchenko.FX.Linq.Operators;

/// <summary>
/// Provides extension methods for LINQ operator polyfills.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class LinqOperatorPolyfills
{
    /// <summary>
    /// <para>
    /// A LINQ-style pipe operator for programming languages that do not have a built-in pipe operator.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="TInput">The type of pipe input.</typeparam>
    /// <typeparam name="TOutput">The type of pipe output.</typeparam>
    /// <param name="input">The pipe input.</param>
    /// <param name="func">The function which calculates the pipe result from the specified pipe <paramref name="input"/>.</param>
    /// <returns>A pipe output which is calculated by <paramref name="func"/> from the specified pipe <paramref name="input"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="func"/> is <see langword="null"/>.</exception>
    public static TOutput PipeOperator<TInput, TOutput>(this TInput input, Func<TInput, TOutput> func) =>
        (func ?? throw new ArgumentNullException(nameof(func)))
        (input);

    /// <summary>
    /// <para>
    /// A LINQ-style pipe operator for programming languages that do not have a built-in pipe operator.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="TInput">The type of pipe input.</typeparam>
    /// <param name="input">The pipe input.</param>
    /// <param name="action">The method that receives the specified pipe <paramref name="input"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <see langword="null"/>.</exception>
    public static void PipeOperator<TInput>(this TInput input, Action<TInput> action) =>
        (action ?? throw new ArgumentNullException(nameof(action)))
        (input);
}

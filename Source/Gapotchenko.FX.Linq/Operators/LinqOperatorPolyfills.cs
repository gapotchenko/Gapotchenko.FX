namespace Gapotchenko.FX.Linq.Operators;

/// <summary>
/// Provides extension methods for LINQ operator polyfills.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class LinqOperatorPolyfills
{
    /// <summary>
    /// A LINQ-style pipe operator.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </remarks>
    /// <typeparam name="T">The type of pipe input.</typeparam>
    /// <typeparam name="TResult">The type of pipe result.</typeparam>
    /// <param name="input">The pipe input.</param>
    /// <param name="func">The function which calculates the pipe result from the specified pipe <paramref name="input"/>.</param>
    /// <returns>The pipe result which is calculated by <paramref name="func"/> from the specified pipe <paramref name="input"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="func"/> is <see langword="null"/>.</exception>
    public static TResult Pipe<T, TResult>(this T input, Func<T, TResult> func) =>
        (func ?? throw new ArgumentNullException(nameof(func)))
        (input);

    /// <summary>
    /// A LINQ-style pipe operator.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </remarks>
    /// <typeparam name="T">The type of pipe input.</typeparam>
    /// <param name="input">The pipe input.</param>
    /// <param name="action">The method that receives the specified pipe <paramref name="input"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <see langword="null"/>.</exception>
    public static void Pipe<T>(this T input, Action<T> action) =>
        (action ?? throw new ArgumentNullException(nameof(action)))
        (input);
}

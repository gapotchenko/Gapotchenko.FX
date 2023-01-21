namespace Gapotchenko.FX.Linq.Operators;

/// <summary>
/// Provides extension methods for compiler operator polyfills.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class LinqOperatorExtensions
{
    /// <summary>
    /// A LINQ-style pipe operator.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </remarks>
    /// <typeparam name="TInput">The type of pipe input.</typeparam>
    /// <typeparam name="TResult">The type of pipe result.</typeparam>
    /// <param name="input">The pipe input.</param>
    /// <param name="func">The function which calculates the pipe result from the specified pipe <paramref name="input"/>.</param>
    /// <returns>The pipe result which is calculated by <paramref name="func"/> from the specified pipe <paramref name="input"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="func"/> is <see langword="null"/>.</exception>
    public static TResult Pipe<TInput, TResult>(this TInput input, Func<TInput, TResult> func) =>
        (func ?? throw new ArgumentNullException(nameof(func)))(input);
}

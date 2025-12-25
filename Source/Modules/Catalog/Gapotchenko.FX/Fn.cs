namespace Gapotchenko.FX;

/// <summary>
/// Provides primitives for lambda calculus and functional composition.
/// </summary>
public static partial class Fn
{
    /// <summary>
    /// A pure function that returns a value of its single parameter <paramref name="x"/>:
    /// <code>f(x) = x</code>
    /// </summary>
    /// <typeparam name="T">The type a function works with.</typeparam>
    /// <param name="x">The parameter.</param>
    /// <returns>The value of parameter <paramref name="x"/>.</returns>
    public static T Identity<T>(T x) => x;

    /// <summary>
    /// A pure function that returns the <see langword="default"/> value of type <typeparamref name="T"/>:
    /// <code>
    /// f() = default(T)
    /// </code>
    /// </summary>
    /// <typeparam name="T">The type of the <see langword="default"/> value to return.</typeparam>
    /// <returns>The <see langword="default"/> value of type <typeparamref name="T"/>.</returns>
    public static T? Default<T>() => default;

    /// <summary>
    /// Gets a delegate to a pure parameterless function that does nothing.
    /// </summary>
    public static Action Empty { get; } = () => { };

    /// <summary>
    /// Ignores a specified value.
    /// Useful in programming languages that do not have a built-in ignore function.
    /// </summary>
    /// <remarks>
    /// A typical usage of ignore function is to fire and forget a parallel <see cref="System.Threading.Tasks.Task"/>
    /// without producing a compiler warning.
    /// </remarks>
    /// <typeparam name="T">The type of a value to ignore.</typeparam>
    /// <param name="value">The value to ignore.</param>
    public static void Ignore<T>(T value)
    {
        _ = value;
    }
}

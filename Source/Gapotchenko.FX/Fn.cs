using System;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Provides primitives for lambda calculus and functional composition.
    /// </summary>
    public static partial class Fn
    {
        /// <summary>
        /// A pure function that returns a value of its single parameter, e.g. f(x) = x.
        /// </summary>
        /// <typeparam name="T">The type a function works with.</typeparam>
        /// <param name="x">The parameter.</param>
        /// <returns>The value of parameter <paramref name="x"/>.</returns>
        public static T Identity<T>(T x) => x;

        /// <summary>
        /// A pure function that returns a default value of <typeparamref name="T"/>, e.g. f() = default(T).
        /// </summary>
        /// <typeparam name="T">The type a function works with.</typeparam>
        /// <returns>The default value of <typeparamref name="T"/>.</returns>
        public static T Default<T>() => default;

        /// <summary>
        /// Gets a delegate to a pure parameterless function that does nothing.
        /// </summary>
        public static Action Empty { get; } = () => { };
    }
}

using System;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Provides lambda calculus and functional composition primitives for type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to provide lambda calculus and functional composition primitives for.</typeparam>
    public static class Fn<T>
    {
        static class Factory
        {
            public static readonly Func<T> Default = Fn.Default<T>;
            public static readonly Func<T, T> Identity = Fn.Identity;
        }

        /// <summary>
        /// Gets a delegate to a pure function that returns a default value of type <typeparamref name="T"/>, e.g. f() = default(T).
        /// </summary>
        public static Func<T> Default { get; } = Factory.Default;

        /// <summary>
        /// Gets a delegate to a pure identity function that returns a value of its single argument, e.g. f(x) = x.
        /// </summary>
        public static Func<T, T> Identity { get; } = Factory.Identity;
    }
}

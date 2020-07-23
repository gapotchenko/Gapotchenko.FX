using System;

#nullable enable

namespace Gapotchenko.FX
{
    /// <summary>
    /// Provides static methods for creating <see cref="LazyEvaluation{T}"/> values.
    /// </summary>
    public static class LazyEvaluation
    {
        /// <summary>
        /// Creates a new instance of the <see cref="LazyEvaluation{T}"/> struct.
        /// When lazy evaluation occurs, the default constructor of the target type <typeparamref name="T"/> is used.
        /// </summary>
        /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
        /// <returns>New <see cref="LazyEvaluation{T}"/> instance.</returns>
        public static LazyEvaluation<T> Create<T>() where T : new() => new LazyEvaluation<T>(() => new T());

        /// <summary>
        /// Creates a new instance of the <see cref="LazyEvaluation{T}"/> struct.
        /// </summary>
        /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
        /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
        /// <returns>New <see cref="LazyEvaluation{T}"/> instance.</returns>
        public static LazyEvaluation<T> Create<T>(Func<T> valueFactory) => new LazyEvaluation<T>(valueFactory);
    }
}

using System;

#nullable enable

namespace Gapotchenko.FX.Threading
{
    /// <summary>
    /// Provides static methods for creating <see cref="EvaluateOnce{T}"/> objects.
    /// </summary>
    public static class EvaluateOnce
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EvaluateOnce{T}"/> struct.
        /// When lazy evaluation occurs, the default constructor of the target type <typeparamref name="T"/> is used.
        /// </summary>
        /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
        /// <returns>New <see cref="EvaluateOnce{T}"/> instance.</returns>
        public static EvaluateOnce<T> Create<T>() where T : new() =>
            new EvaluateOnce<T>(() => new T());

        /// <summary>
        /// Creates a new instance of the <see cref="EvaluateOnce{T}"/> struct.
        /// </summary>
        /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
        /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
        /// <returns>New <see cref="EvaluateOnce{T}"/> instance.</returns>
        public static EvaluateOnce<T> Create<T>(Func<T> valueFactory) => new EvaluateOnce<T>(valueFactory);

        /// <summary>
        /// Creates a new instance of the <see cref="EvaluateOnce{T}"/> struct.
        /// </summary>
        /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
        /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
        /// <param name="syncLock">
        /// An object used as the mutually exclusive lock for value evaluation.
        /// When the given value is null, an unique synchronization lock object is used.
        /// </param>
        /// <returns>New <see cref="EvaluateOnce{T}"/> instance.</returns>
        public static EvaluateOnce<T> Create<T>(Func<T> valueFactory, object syncLock) => new EvaluateOnce<T>(valueFactory, syncLock);
    }
}

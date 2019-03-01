using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Provides typed constructions related to a functional notion of emptiness.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public static class Empty<T>
    {
#if !TF_ARRAY_EMPTY
        static class ArrayFactory
        {
            public static readonly T[] Array = new T[0];
        }
#endif

        /// <summary>
        /// Gets an empty array instance.
        /// </summary>
        public static T[] Array =>
#if !TF_ARRAY_EMPTY
            ArrayFactory.Array;
#else
            System.Array.Empty<T>();
#endif

        static class FuncFactory
        {
            public static readonly Func<T> DefaultFunc = Empty.DefaultFunc<T>;
            public static readonly Func<T, T> IdentityFunc = Empty.IdentityFunc;
        }

        /// <summary>
        /// Gets a <see cref="Func{TResult}"/> instance that represents a pure function that returns a default value of <typeparamref name="T"/>, e.g. f() = default(T).
        /// </summary>
        public static Func<T> DefaultFunc { get; } = FuncFactory.DefaultFunc;

        /// <summary>
        /// Gets a <see cref="Func{T, TResult}"/> instance that represents a pure identity function that returns a value of its single argument, e.g. f(x) = x.
        /// </summary>
        public static Func<T, T> IdentityFunc { get; } = FuncFactory.IdentityFunc;
    }
}

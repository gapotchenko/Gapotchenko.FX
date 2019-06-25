using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Returns an empty array.
        /// </summary>
        public static T[] Array =>
#if !TF_ARRAY_EMPTY
            ArrayFactory.Array;
#else
            System.Array.Empty<T>();
#endif

        static class TaskFactory
        {
            public static readonly Task<T> Task = FromResult(default);

            static Task<T> FromResult(T value)
            {
#if NET40
                var tcs = new TaskCompletionSource<T>();
                tcs.SetResult(value);
                return tcs.Task;
#else
                return System.Threading.Tasks.Task.FromResult(value);
#endif
            }
        }

        /// <summary>
        /// Returns an empty <see cref="Task{TResult}"/> that has already completed successfully with the default result.
        /// </summary>
        public static Task<T> Task => TaskFactory.Task;
    }
}

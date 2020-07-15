using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

#nullable enable

namespace Gapotchenko.FX.Collections.Concurrent
{
    /// <summary>
    /// <see cref="ConcurrentBag{T}"/> extensions.
    /// </summary>
    public static class ConcurrentBagExtensions
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of the target <see cref="ConcurrentBag{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of collection.</typeparam>
        /// <param name="target">The <see cref="ConcurrentBag{T}"/> where the elements should be added to.</param>
        /// <param name="collection">The collection whose elements should be added.</param>
        public static void AddRange<T>(this ConcurrentBag<T> target, IEnumerable<T> collection)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var i in collection)
                target.Add(i);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the target <see cref="ConcurrentBag{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of collection.</typeparam>
        /// <param name="target">The <see cref="ConcurrentBag{T}"/> where the elements should be added to.</param>
        /// <param name="collection">The collection whose elements should be added.</param>
        public static void AddRange<T>(this ConcurrentBag<T> target, params T[] collection) => AddRange(target, (IEnumerable<T>)collection);
    }
}

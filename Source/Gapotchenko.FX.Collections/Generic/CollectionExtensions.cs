using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Collections.Generic
{
    /// <summary>
    /// Generic collection extensions.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of the target.
        /// </summary>
        /// <typeparam name="T">The type of the elements of collection.</typeparam>
        /// <param name="target">The collection where the elements should be added to.</param>
        /// <param name="collection">The collection whose elements should be added.</param>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> collection)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (target is List<T> list)
            {
                // Optimized implementation for List<T>.
                list.AddRange(collection);
            }
            else
            {
                // Generic implementation.
                foreach (var i in collection)
                    target.Add(i);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the target.
        /// </summary>
        /// <typeparam name="T">The type of the elements of collection.</typeparam>
        /// <param name="target">The collection where the elements should be added to.</param>
        /// <param name="collection">The collection whose elements should be added.</param>
        public static void AddRange<T>(this ICollection<T> target, params T[] collection) => AddRange(target, (IEnumerable<T>)collection);
    }
}

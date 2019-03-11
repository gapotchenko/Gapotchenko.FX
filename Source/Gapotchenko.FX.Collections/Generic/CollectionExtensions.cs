using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            foreach (var i in collection)
                target.Add(i);
        }

        /// <summary>
        /// Indicates whether the specified collection is null or empty.
        /// </summary>
        /// <param name="value">The collection to test.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty collection; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> value) => value == null || value.Count == 0;

        /// <summary>
        /// Indicates whether the specified set is null or empty.
        /// </summary>
        /// <param name="value">The set to test.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty set; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty<T>(this ISet<T> value) => value == null || value.Count == 0;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Collections.Generic
{
    /// <summary>
    /// Generic list extensions.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Performs in-place stable sort of the elements in entire <see cref="List{T}"/> using the specified comparer.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing elements,
        /// or <c>null</c> to use the default comparer.
        /// </param>
        public static void StableSort<T>(this List<T> list, IComparer<T>? comparer)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count < 2)
                return;

            var sortedList = list.OrderBy(Fn.Identity, comparer).ToList();

            list.Clear();
            list.AddRange(sortedList);
        }

        /// <summary>
        /// Performs in-place stable sort of the elements in entire <see cref="List{T}"/> using the default comparer.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list.</param>
        public static void StableSort<T>(this List<T> list) => StableSort(list, null);

        /// <summary>
        /// Clones the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>A cloned list.</returns>
        public static List<T> Clone<T>(this List<T> list) => new(list ?? throw new ArgumentNullException(nameof(list)));

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="IList{T}"/> at the specified index.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list where the elements should be inserted to.</param>
        /// <param name="index">The index where the elements should be inserted at.</param>
        /// <param name="collection">The collection whose elements should be inserted.</param>
        public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> collection)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (index > list.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (list is List<T> l)
            {
                // Optimized implementation for List<T>.
                l.InsertRange(index, collection);
            }
            else
            {
                // Generic implementation.
                foreach (var i in collection)
                    list.Insert(index++, i);
            }
        }
    }
}

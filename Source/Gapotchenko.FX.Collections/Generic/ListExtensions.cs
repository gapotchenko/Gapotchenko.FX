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

            if (list is List<T> l)
            {
                // Optimized implementation for List<T>.
                l.InsertRange(index, collection);
            }
            else
            {
                if (collection == null)
                    throw new ArgumentNullException(nameof(collection));
                if (index > list.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                foreach (var i in collection)
                    list.Insert(index++, i);
            }
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="list">The list where the elements should be removed from.</param>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public static void RemoveRange<T>(this IList<T> list, int index, int count)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list is List<T> l)
            {
                // Optimized implementation for List<T>.
                l.RemoveRange(index, count);
            }
            else
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "The argument value cannot be negative.");
                if (count < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "The argument value cannot be negative.");
                if (list.Count - index < count)
                    throw new ArgumentException("Invalid offset and length.");

                for (int i = 0; i < count; ++i)
                    list.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list where the elements should be removed from.</param>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the <see cref="IList{T}"/>.</returns>
        public static int RemoveAll<T>(this IList<T> list, Predicate<T> match)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list is List<T> l)
            {
                // Optimized implementation for List<T>.
                l.RemoveAll(match);
            }
            else
            {
                if (match == null)
                    throw new ArgumentNullException(nameof(match));

                int count = list.Count;
                int index = 0;
                while (index < count && !match(list[index]))
                    ++index;
                if (index >= count)
                    return 0;

                for (int i = index + 1; ;)
                {
                    while (i < count && match(list[i]))
                        ++i;
                    if (i >= count)
                        break;
                    list[index++] = list[i++];
                }

                int removedElementsCount = count - index;
                list.RemoveRange(index, removedElementsCount);
                return removedElementsCount;
            }

            return 0;
        }
    }
}

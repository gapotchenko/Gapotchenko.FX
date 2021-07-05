using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gapotchenko.FX.Linq
{
    partial class EnumerableEx
    {
        /// <summary>
        /// <para>
        /// Returns a new enumerable collection that contains the last count elements from source.
        /// </para>
        /// <para>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the enumerable collection.</typeparam>
        /// <param name="source">An enumerable collection instance.</param>
        /// <param name="count">The number of elements to take from the end of the collection.</param>
        /// <returns>A new enumerable collection that contains the last count elements from source.</returns>
#if TFF_ENUMERABLE_TAKELAST
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<TSource> TakeLast<TSource>(IEnumerable<TSource> source, int count) => Enumerable.TakeLast(source, count);
#else
        public static IEnumerable<TSource> TakeLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
                throw new ArgumentException(nameof(source));

            static IEnumerable<TSource> Iterator(IEnumerable<TSource> source, int count)
            {
                Queue<TSource> queue;

                using (var e = source.GetEnumerator())
                {
                    if (!e.MoveNext())
                        yield break;

                    queue = new Queue<TSource>();
                    queue.Enqueue(e.Current);

                    while (e.MoveNext())
                    {
                        if (queue.Count < count)
                        {
                            // Fill the queue.
                            queue.Enqueue(e.Current);
                        }
                        else
                        {
                            // Swap old queue elements with the new ones.
                            do
                            {
                                queue.Dequeue();
                                queue.Enqueue(e.Current);
                            }
                            while (e.MoveNext());

                            break;
                        }
                    }
                }

                // Flush the queue.
                do
                {
                    yield return queue.Dequeue();
                }
                while (queue.Count > 0);
            }

            return count <= 0 ?
                Enumerable.Empty<TSource>() :
                Iterator(source, count);
        }
#endif

        /// <summary>
        /// <para>
        /// Returns a new enumerable collection that contains the elements from source
        /// with the last count elements of the source collection omitted.
        /// </para>
        /// <para>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the enumerable collection.</typeparam>
        /// <param name="source">An enumerable collection instance.</param>
        /// <param name="count">The number of elements to omit from the end of the collection.</param>
        /// <returns>A new enumerable collection that contains the elements from source minus count elements from the end of the collection.</returns>
#if TFF_ENUMERABLE_SKIPLAST
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<TSource> SkipLast<TSource>(IEnumerable<TSource> source, int count) => Enumerable.SkipLast(source, count);
#else
        public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
                throw new ArgumentException(nameof(source));

            static IEnumerable<TSource> Iterator(IEnumerable<TSource> source, int count)
            {
                var queue = new Queue<TSource>();
                using var e = source.GetEnumerator();

                while (e.MoveNext())
                {
                    if (queue.Count == count)
                    {
                        do
                        {
                            yield return queue.Dequeue();
                            queue.Enqueue(e.Current);
                        }
                        while (e.MoveNext());

                        break;
                    }
                    else
                    {
                        queue.Enqueue(e.Current);
                    }
                }
            }

            return count <= 0 ?
                source.Skip(0) :
                Iterator(source, count);
        }
#endif
    }
}

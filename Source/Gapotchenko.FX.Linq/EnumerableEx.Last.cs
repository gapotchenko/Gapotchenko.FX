namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
    /// <summary>
    /// Returns the last element of a sequence, or the specified default value if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return the last element of.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// <paramref name="defaultValue"/> if the source sequence is empty;
    /// otherwise, the last element in the <see cref="IEnumerable{T}"/>.
    /// </returns>
#if TFF_ENUMERABLE_LASTORDEFAULT_VALUE
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource LastOrDefault<TSource>(IEnumerable<TSource> source, TSource defaultValue) => Enumerable.LastOrDefault(source, defaultValue);
#else
    public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue) =>
        LastOrDefaultCore(source, null, defaultValue);
#endif

    /// <summary>
    /// Returns the last element of a sequence that satisfies a condition or the specified default value if no such element is found.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return an element from.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// <paramref name="defaultValue"/> if the sequence is empty or if no elements pass the test in the predicate function;
    /// otherwise, the last element that passes the test in the predicate function.
    /// </returns>
#if TFF_ENUMERABLE_LASTORDEFAULT_VALUE
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue) =>
        Enumerable.LastOrDefault(source, predicate, defaultValue);
#else
    public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue) =>
        LastOrDefaultCore(
            source,
            predicate ?? throw new ArgumentNullException(nameof(predicate)),
            defaultValue);
#endif

#if !TFF_ENUMERABLE_LASTORDEFAULT_VALUE
    static TSource LastOrDefaultCore<TSource>(this IEnumerable<TSource> source, Func<TSource, bool>? predicate, TSource defaultValue)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (source is IList<TSource> list)
        {
            int count = list.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                var result = list[i];
                if (predicate == null || predicate(result))
                    return result;
            }
            return defaultValue;
        }
        else
        {
            TSource result = defaultValue;

            foreach (var i in source)
                if (predicate == null || predicate(i))
                    result = i;

            return result;
        }
    }
#endif

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

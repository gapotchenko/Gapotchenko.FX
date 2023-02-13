namespace Gapotchenko.FX.Linq;

partial class EnumerableExtensions
{
    /// <summary>
    /// <para>
    /// Produces the set difference of two sequences according to a specified key selector function.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">An <see cref="IEnumerable{TSource}" /> whose keys that are not also in <paramref name="second"/> will be returned.</param>
    /// <param name="second">An <see cref="IEnumerable{TKey}" /> whose keys that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
#if TFF_ENUMERABLE_EXCEPTBY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(
#if !TFF_ENUMERABLE_EXCEPTBY
        this
#endif
        IEnumerable<TSource> first,
        IEnumerable<TKey> second,
        Func<TSource, TKey> keySelector) =>
#if TFF_ENUMERABLE_EXCEPTBY
        // Redirect to BCL implementation.
        first.ExceptBy(second, keySelector);
#else
        // FX implementation.
        ExceptBy(first, second, keySelector, null);
#endif

    /// <summary>
    /// Produces the set difference of two sequences according to a specified key selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">An <see cref="IEnumerable{TSource}" /> whose keys that are not also in <paramref name="second"/> will be returned.</param>
    /// <param name="second">An <see cref="IEnumerable{TKey}" /> whose keys that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{TKey}" /> to compare values.</param>
    /// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
#if TFF_ENUMERABLE_EXCEPTBY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(
#if !TFF_ENUMERABLE_EXCEPTBY
        this
#endif
        IEnumerable<TSource> first,
        IEnumerable<TKey> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer)
    {
#if TFF_ENUMERABLE_EXCEPTBY
        // Redirect to BCL implementation.
        return first.ExceptBy(second, keySelector, comparer);
#else
        // FX implementation.

        if (first is null)
            throw new ArgumentNullException(nameof(first));
        if (second is null)
            throw new ArgumentNullException(nameof(second));
        if (keySelector is null)
            throw new ArgumentNullException(nameof(keySelector));

        var set = new HashSet<TKey>(second, comparer);

        foreach (var element in first)
            if (set.Add(keySelector(element)))
                yield return element;
#endif
    }

    /// <summary>Produces the set union of two sequences according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">An <see cref="IEnumerable{T}" /> whose distinct elements form the first set for the union.</param>
    /// <param name="second">An <see cref="IEnumerable{T}" /> whose distinct elements form the second set for the union.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> that contains the elements from both input sequences, excluding duplicates.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
    /// <remarks>
    /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its `GetEnumerator` method directly or by using `foreach` in Visual C# or `For Each` in Visual Basic.</para>
    /// <para>The default equality comparer, <see cref="EqualityComparer{T}.Default" />, is used to compare values.</para>
    /// <para>When the object returned by this method is enumerated, <see cref="O:Enumerable.UnionBy" /> enumerates <paramref name="first" /> and <paramref name="second" /> in that order and yields each element that has not already been yielded.</para>
    /// </remarks>
#if TFF_ENUMERABLE_UNIONBY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IEnumerable<TSource> UnionBy<TSource, TKey>(
#if !TFF_ENUMERABLE_UNIONBY
        this
#endif
        IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keySelector) =>
#if TFF_ENUMERABLE_UNIONBY
        // Redirect to BCL implementation.
        first.UnionBy(second, keySelector);
#else
        // FX implementation.
        UnionBy(first, second, keySelector, null);
#endif

    /// <summary>Produces the set union of two sequences according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">An <see cref="IEnumerable{T}" /> whose distinct elements form the first set for the union.</param>
    /// <param name="second">An <see cref="IEnumerable{T}" /> whose distinct elements form the second set for the union.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}" /> to compare values.</param>
    /// <returns>An <see cref="IEnumerable{T}" /> that contains the elements from both input sequences, excluding duplicates.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
    /// <remarks>
    /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its `GetEnumerator` method directly or by using `foreach` in Visual C# or `For Each` in Visual Basic.</para>
    /// <para>If <paramref name="comparer" /> is <see langword="null" />, the default equality comparer, <see cref="EqualityComparer{T}.Default" />, is used to compare values.</para>
    /// <para>When the object returned by this method is enumerated, <see cref="O:Enumerable.UnionBy" /> enumerates <paramref name="first" /> and <paramref name="second" /> in that order and yields each element that has not already been yielded.</para>
    /// </remarks>
#if TFF_ENUMERABLE_UNIONBY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IEnumerable<TSource> UnionBy<TSource, TKey>(
#if !TFF_ENUMERABLE_UNIONBY
        this
#endif
        IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer)
    {
#if TFF_ENUMERABLE_UNIONBY
        // Redirect to BCL implementation.
        return first.UnionBy(second, keySelector, comparer);
#else
        // FX implementation.

        if (first is null)
            throw new ArgumentNullException(nameof(first));
        if (second is null)
            throw new ArgumentNullException(nameof(second));
        if (keySelector is null)
            throw new ArgumentNullException(nameof(keySelector));

        var set = new HashSet<TKey>(comparer);

        foreach (var element in first)
            if (set.Add(keySelector(element)))
                yield return element;

        foreach (var element in second)
            if (set.Add(keySelector(element)))
                yield return element;
#endif
    }

    /// <summary>Produces the set intersection of two sequences according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">An <see cref="IEnumerable{T}" /> whose distinct elements that also appear in <paramref name="second" /> will be returned.</param>
    /// <param name="second">An <see cref="IEnumerable{T}" /> whose distinct elements that also appear in the first sequence will be returned.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
    /// <remarks>
    /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its `GetEnumerator` method directly or by using `foreach` in Visual C# or `For Each` in Visual Basic.</para>
    /// <para>The intersection of two sets A and B is defined as the set that contains all the elements of A that also appear in B, but no other elements.</para>
    /// <para>When the object returned by this method is enumerated, `Intersect` yields distinct elements occurring in both sequences in the order in which they appear in <paramref name="first" />.</para>
    /// <para>The default equality comparer, <see cref="EqualityComparer{T}.Default" />, is used to compare values.</para>
    /// </remarks>
#if TFF_ENUMERABLE_INTERSECTBY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IEnumerable<TSource> IntersectBy<TSource, TKey>(
#if !TFF_ENUMERABLE_INTERSECTBY
        this
#endif
        IEnumerable<TSource> first,
        IEnumerable<TKey> second,
        Func<TSource, TKey> keySelector) =>
#if TFF_ENUMERABLE_INTERSECTBY
        // Redirect to BCL implementation.
        first.IntersectBy(second, keySelector);
#else
        // FX implementation.
        IntersectBy(first, second, keySelector, null);
#endif

    /// <summary>Produces the set intersection of two sequences according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="first">An <see cref="IEnumerable{T}" /> whose distinct elements that also appear in <paramref name="second" /> will be returned.</param>
    /// <param name="second">An <see cref="IEnumerable{T}" /> whose distinct elements that also appear in the first sequence will be returned.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{TKey}" /> to compare keys.</param>
    /// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="first" /> or <paramref name="second" /> is <see langword="null" />.</exception>
    /// <remarks>
    /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling its `GetEnumerator` method directly or by using `foreach` in Visual C# or `For Each` in Visual Basic.</para>
    /// <para>The intersection of two sets A and B is defined as the set that contains all the elements of A that also appear in B, but no other elements.</para>
    /// <para>When the object returned by this method is enumerated, `Intersect` yields distinct elements occurring in both sequences in the order in which they appear in <paramref name="first" />.</para>
    /// <para>If <paramref name="comparer" /> is <see langword="null" />, the default equality comparer, <see cref="EqualityComparer{T}.Default" />, is used to compare values.</para>
    /// </remarks>
#if TFF_ENUMERABLE_INTERSECTBY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IEnumerable<TSource> IntersectBy<TSource, TKey>(
#if !TFF_ENUMERABLE_INTERSECTBY
        this
#endif
        IEnumerable<TSource> first,
        IEnumerable<TKey> second,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer)
    {
#if TFF_ENUMERABLE_INTERSECTBY
        // Redirect to BCL implementation.
        return first.IntersectBy(second, keySelector, comparer);
#else
        // FX implementation.

        if (first is null)
            throw new ArgumentNullException(nameof(first));

        if (second is null)
            throw new ArgumentNullException(nameof(second));

        if (keySelector is null)
            throw new ArgumentNullException(nameof(keySelector));

        var set = new HashSet<TKey>(second, comparer);

        foreach (var element in first)
            if (set.Remove(keySelector(element)))
                yield return element;
#endif
    }
}

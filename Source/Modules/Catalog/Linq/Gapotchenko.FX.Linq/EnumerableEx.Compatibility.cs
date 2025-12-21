// --------------------------------------------------------------------------
// API Compatibility Layer
// --------------------------------------------------------------------------

namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY

    /// <inheritdoc cref="EnumerableExtensions.ReifyList{TSource}(IEnumerable{TSource})"/>
    [Obsolete("Use IEnumerable<T>.ReifyList() extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(source))]
    public static IReadOnlyList<TSource>? AsReadOnlyList<TSource>(
#if SOURCE_COMPATIBILITY
        this
#endif
        IEnumerable<TSource>? source) => source?.ReifyList();

    /// <inheritdoc cref="EnumerableExtensions.ReifyList{TSource}(IEnumerable{TSource})"/>
    [Obsolete("Use IEnumerable<T>.ReifyList() extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(source))]
    public static IReadOnlyList<TSource>? AsReadOnly<TSource>(
#if SOURCE_COMPATIBILITY && !NET7_0_OR_GREATER
        this
#endif
        IEnumerable<TSource>? source) => source?.ReifyList();

    /// <inheritdoc cref="EnumerableExtensions.PartitionBy{TElement, TKey}(IEnumerable{TElement}, Func{TElement, TKey})"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<IGrouping<TKey, TElement>> PartitionBy<TElement, TKey>(IEnumerable<TElement> source, Func<TElement, TKey> keySelector) =>
        EnumerableExtensions.PartitionBy(source, keySelector);

#endif // SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY

#if BINARY_COMPATIBILITY

    /// <inheritdoc cref="EnumerableExtensions.CountIsAtLeast{TSource}(IEnumerable{TSource}, int)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool CountIsAtLeast<TSource>(IEnumerable<TSource> source, int value) =>
        EnumerableExtensions.CountIsAtLeast(source, value);

    /// <inheritdoc cref="EnumerablePolyfills.DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
        EnumerablePolyfills.DistinctBy(source, keySelector);

    /// <inheritdoc cref="EnumerablePolyfills.DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IEqualityComparer{TKey}?)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer) =>
        EnumerablePolyfills.DistinctBy(source, keySelector, comparer);

    /// <inheritdoc cref="EnumerablePolyfills.FirstOrDefault{TSource}(IEnumerable{TSource}, TSource)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source, TSource defaultValue) => EnumerablePolyfills.FirstOrDefault(source, defaultValue);

    /// <inheritdoc cref="EnumerablePolyfills.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool}, TSource)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue) =>
        EnumerablePolyfills.FirstOrDefault(source, predicate, defaultValue);

    /// <inheritdoc cref="EnumerableExtensions.Memoize{T}(IEnumerable{T})"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<T> Memoize<T>(IEnumerable<T> source) => Memoize(source, false);

    /// <inheritdoc cref="EnumerableExtensions.Memoize{T}(IEnumerable{T}, bool)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<T> Memoize<T>(IEnumerable<T> source, bool isThreadSafe)
    {
        if (source == null)
            return null!; // behavioral compatibility with the oldest versions
        else
            return EnumerableExtensions.Memoize(source, isThreadSafe);
    }

    /// <inheritdoc cref="EnumerableExtensions.ScalarOrDefault{TSource}(IEnumerable{TSource})"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource? ScalarOrDefault<TSource>(IEnumerable<TSource> source) =>
        EnumerableExtensions.ScalarOrDefault(source);

    /// <inheritdoc cref="EnumerableExtensions.ScalarOrDefault{TSource}(IEnumerable{TSource}, TSource)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource ScalarOrDefault<TSource>(IEnumerable<TSource> source, TSource defaultValue) =>
        EnumerableExtensions.ScalarOrDefault(source, defaultValue);

    /// <inheritdoc cref="EnumerableExtensions.ScalarOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource? ScalarOrDefault<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate) =>
        EnumerableExtensions.ScalarOrDefault(source, predicate);

    /// <inheritdoc cref="EnumerableExtensions.ScalarOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool}, TSource)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource ScalarOrDefault<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue) =>
        EnumerableExtensions.ScalarOrDefault(source, predicate, defaultValue);

    /// <inheritdoc cref="EnumerablePolyfills.SingleOrDefault{TSource}(IEnumerable{TSource}, TSource)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource SingleOrDefault<TSource>(IEnumerable<TSource> source, TSource defaultValue) =>
        EnumerablePolyfills.SingleOrDefault(source, defaultValue);

    /// <inheritdoc cref="EnumerablePolyfills.SingleOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool}, TSource)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource SingleOrDefault<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue) =>
        EnumerablePolyfills.SingleOrDefault(source, predicate, defaultValue);

    /// <inheritdoc cref="EnumerablePolyfills.ToHashSet{TSource}(IEnumerable{TSource})"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashSet<TSource> ToHashSet<TSource>(IEnumerable<TSource> source) =>
        EnumerablePolyfills.ToHashSet(source);

    /// <inheritdoc cref="EnumerablePolyfills.ToHashSet{TSource}(IEnumerable{TSource}, IEqualityComparer{TSource}?)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashSet<TSource> ToHashSet<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer) =>
        EnumerablePolyfills.ToHashSet(source, comparer);

    /// <inheritdoc cref="EnumerablePolyfills.TryGetNonEnumeratedCount{TSource}(IEnumerable{TSource}, out int)"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool TryGetNonEnumeratedCount<TSource>(IEnumerable<TSource> source, out int count) =>
        EnumerablePolyfills.TryGetNonEnumeratedCount(source, out count);

    /// <inheritdoc cref="EnumerablePolyfills.TryGetNonEnumeratedCount{TSource}(IEnumerable{TSource})"/>
    [Obsolete("Use extension method of IEnumerable<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int? TryGetNonEnumeratedCount<TSource>(IEnumerable<TSource> source) =>
        EnumerablePolyfills.TryGetNonEnumeratedCount(source);

#endif // BINARY_COMPATIBILITY
}

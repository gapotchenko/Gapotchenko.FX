// API compatibility layer.

namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
#if BINARY_COMPATIBILITY
    /// <inheritdoc cref="EnumerablePolyfills.FirstOrDefault{TSource}(IEnumerable{TSource}, TSource)"/>
    [Obsolete("Use IEnumerable<T>.FirstOrDefault(source, defaultValue) extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source, TSource defaultValue) => EnumerablePolyfills.FirstOrDefault(source, defaultValue);
#endif

#if BINARY_COMPATIBILITY
    /// <inheritdoc cref="EnumerablePolyfills.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool}, TSource)"/>
    [Obsolete("Use IEnumerable<T>.FirstOrDefault(source, predicate, defaultValue) extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue) =>
        EnumerablePolyfills.FirstOrDefault(source, predicate, defaultValue);
#endif

#if BINARY_COMPATIBILITY
    /// <inheritdoc cref="EnumerablePolyfills.ToHashSet{TSource}(IEnumerable{TSource})"/>
    [Obsolete("Use IEnumerable<T>.ToHashSet() extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashSet<TSource> ToHashSet<TSource>(IEnumerable<TSource> source) =>
        EnumerablePolyfills.ToHashSet(source);
#endif

#if BINARY_COMPATIBILITY
    /// <inheritdoc cref="EnumerablePolyfills.ToHashSet{TSource}(IEnumerable{TSource}, IEqualityComparer{TSource}?)"/>
    [Obsolete("Use IEnumerable<T>.ToHashSet(comparer) extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashSet<TSource> ToHashSet<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer) =>
        EnumerablePolyfills.ToHashSet(source, comparer);
#endif

#if BINARY_COMPATIBILITY
    /// <inheritdoc cref="EnumerablePolyfills.DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
    [Obsolete("Use IEnumerable<T>.DistinctBy(keySelector) extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
        EnumerablePolyfills.DistinctBy(source, keySelector);
#endif

#if BINARY_COMPATIBILITY
    /// <inheritdoc cref="EnumerablePolyfills.DistinctBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IEqualityComparer{TKey}?)"/>
    [Obsolete("Use IEnumerable<T>.DistinctBy(keySelector, comparer) extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer) =>
        EnumerablePolyfills.DistinctBy(source, keySelector, comparer);
#endif

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
#endif

#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY
    /// <inheritdoc cref="EnumerableExtensions.ReifyList{TSource}(IEnumerable{TSource})"/>
    [Obsolete("Use IEnumerable<T>.ReifyList() extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(source))]
    public static IReadOnlyList<TSource>? AsReadOnly<TSource>(
#if SOURCE_COMPATIBILITY && !NET7_0_OR_GREATER
        this
#endif
        IEnumerable<TSource>? source) => source?.ReifyList();
#endif
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Kirill Rode
// Year of introduction: 2021

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Math.Graphs;

#if BINARY_COMPATIBILITY

/// <summary>
/// Provides topological sort extension methods.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class TopologicalSortExtensions
{
    /// <inheritdoc cref="EnumerableExtensions.OrderTopologically{TSource}(IEnumerable{TSource}, Func{TSource, TSource, bool}, IEqualityComparer{TSource}?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologically<TSource>(
        IEnumerable<TSource> source,
        Func<TSource, TSource, bool> dependencyFunction,
        IEqualityComparer<TSource>? comparer = null) =>
        EnumerableExtensions.OrderTopologically(source, dependencyFunction, comparer);

    /// <inheritdoc cref="EnumerableExtensions.OrderTopologicallyBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TKey, TKey, bool}, IEqualityComparer{TKey}?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologicallyBy<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, TKey, bool> dependencyFunction,
        IEqualityComparer<TKey>? comparer = null) =>
        EnumerableExtensions.OrderTopologicallyBy(source, keySelector, dependencyFunction, comparer);

    /// <inheritdoc cref="EnumerableExtensions.OrderTopologically{TSource}(IEnumerable{TSource}, Func{TSource, IEnumerable{TSource}?}, IEqualityComparer{TSource}?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologically<TSource>(
        IEnumerable<TSource> source,
        Func<TSource, IEnumerable<TSource>?> dependencyFunction,
        IEqualityComparer<TSource>? comparer = null) =>
        EnumerableExtensions.OrderTopologically(source, dependencyFunction, comparer);

    /// <inheritdoc cref="EnumerableExtensions.OrderTopologicallyBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TKey, IEnumerable{TKey}?}, IEqualityComparer{TKey}?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologicallyBy<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, IEnumerable<TKey>?> dependencyFunction,
        IEqualityComparer<TKey>? comparer = null) =>
        EnumerableExtensions.OrderTopologicallyBy(source, keySelector, dependencyFunction, comparer);

    /// <inheritdoc cref="EnumerableExtensions.OrderTopologically{TSource}(IEnumerable{TSource}, IReadOnlyGraph{TSource})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologically<TSource>(
        IEnumerable<TSource> source,
        IReadOnlyGraph<TSource> dependencyGraph) =>
        EnumerableExtensions.OrderTopologically(source, dependencyGraph);

    /// <inheritdoc cref="EnumerableExtensions.OrderTopologicallyBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IReadOnlyGraph{TKey})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologicallyBy<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IReadOnlyGraph<TKey> dependencyGraph) =>
        EnumerableExtensions.OrderTopologicallyBy(source, keySelector, dependencyGraph);
}

#endif

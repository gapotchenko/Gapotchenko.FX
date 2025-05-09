// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Kirill Rode
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

/// <summary>
/// Provides topological sort extension methods.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class TopologicalSortExtensions
{
    /// <summary>
    /// <para>
    /// Sorts the elements of a sequence in topological order according to the specified dependency function.
    /// </para>
    /// <para>
    /// The sort is stable.
    /// Circular dependencies are ignored and resolved according to the original order of elements in the sequence.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The sequence of values to order.</param>
    /// <param name="dependencyFunction">
    /// <para>
    /// The dependency function that defines dependencies between the elements of a sequence.
    /// </para>
    /// <para>
    /// Given a pair of elements <c>arg1</c> and <c>arg2</c>, returns a Boolean value indicating whether <c>arg2</c> should appear before <c>arg1</c> in the resulting order.
    /// </para>
    /// </param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare elements.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted in topological order according to the specified dependency function.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="dependencyFunction"/> is <see langword="null"/>.</exception>
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologically<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, TSource, bool> dependencyFunction,
        IEqualityComparer<TSource>? comparer = null) =>
        OrderTopologicallyBy(source, Fn.Identity, dependencyFunction, comparer);

    /// <summary>
    /// <para>
    /// Sorts the elements of a sequence in topological order according to a key and specified dependency function.
    /// </para>
    /// <para>
    /// The sort is stable.
    /// Circular dependencies are ignored and resolved according to the original order of elements in the sequence.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The sequence of values to order.</param>
    /// <param name="keySelector">The function to extract a key from an element.</param>
    /// <param name="dependencyFunction">
    /// <para>
    /// The dependency function that defines dependencies between the elements of a sequence.
    /// </para>
    /// <para>
    /// Given a pair of element keys as <c>arg1</c> and <c>arg2</c>, returns a Boolean value indicating whether <c>arg2</c> should appear before <c>arg1</c> in the resulting order.
    /// </para>
    /// </param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted in topological order according to a key and specified dependency function.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="dependencyFunction"/> is <see langword="null"/>.</exception>
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologicallyBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, TKey, bool> dependencyFunction,
        IEqualityComparer<TKey>? comparer = null)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));
        if (dependencyFunction == null)
            throw new ArgumentNullException(nameof(dependencyFunction));

        return new TopologicallyOrderedEnumerable.PrimaryEnumerable<TSource, TKey>(source, keySelector, comparer, GraphFactory);

        IReadOnlyGraph<TKey> GraphFactory(in TopologicallyOrderedEnumerable.GraphFactoryContext<TSource, TKey> context) =>
            new Graph<TKey>(
                context.Vertices,
                new GraphIncidenceFunction<TKey>(dependencyFunction),
                comparer,
                GraphIncidenceOptions.ReflexiveReduction | GraphIncidenceOptions.Connected);
    }

    /// <summary>
    /// <para>
    /// Sorts the elements of a sequence in topological order according to the specified dependency function.
    /// </para>
    /// <para>
    /// The sort is stable.
    /// Circular dependencies are ignored and resolved according to the original order of elements in the sequence.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The sequence of values to order.</param>
    /// <param name="dependencyFunction">
    /// The dependency function that defines dependencies between the elements of a sequence.
    /// Given an element, returns a set of elements that must appear before it in the resulting order.
    /// </param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare elements.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted in topological order according to the specified dependency function.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="dependencyFunction"/> is <see langword="null"/>.</exception>
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologically<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, IEnumerable<TSource>?> dependencyFunction,
        IEqualityComparer<TSource>? comparer = null) =>
        OrderTopologicallyBy(source, Fn.Identity, dependencyFunction, comparer);

    /// <summary>
    /// <para>
    /// Sorts the elements of a sequence in topological order according to a key and specified dependency function.
    /// </para>
    /// <para>
    /// The sort is stable.
    /// Circular dependencies are ignored and resolved according to the original order of elements in the sequence.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The sequence of values to order.</param>
    /// <param name="keySelector">The function to extract a key from an element.</param>
    /// <param name="dependencyFunction">
    /// The dependency function that defines dependencies between the elements of a sequence.
    /// Given an element key, returns a set of element keys that must appear before it in the resulting order.
    /// </param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted in topological order according to a key and specified dependency function.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="dependencyFunction"/> is <see langword="null"/>.</exception>
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologicallyBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, IEnumerable<TKey>?> dependencyFunction,
        IEqualityComparer<TKey>? comparer = null)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));
        if (dependencyFunction == null)
            throw new ArgumentNullException(nameof(dependencyFunction));

        return new TopologicallyOrderedEnumerable.PrimaryEnumerable<TSource, TKey>(source, keySelector, comparer, GraphFactory);

        IReadOnlyGraph<TKey> GraphFactory(in TopologicallyOrderedEnumerable.GraphFactoryContext<TSource, TKey> context)
        {
            var graph = new Graph<TKey>(comparer);
            var edges = graph.Edges;

            foreach (var vertex in context.Vertices)
            {
                var adjacentVertices = dependencyFunction(vertex);
                if (adjacentVertices != null)
                {
                    foreach (var adjacentVertex in adjacentVertices)
                        edges.Add(vertex, adjacentVertex);
                }
            }

            return graph;
        }
    }

    /// <summary>
    /// <para>
    /// Sorts the elements of a sequence in topological order according to the specified dependency graph.
    /// </para>
    /// <para>
    /// The sort is stable.
    /// Circular dependencies are ignored and resolved according to the original order of elements in the sequence.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The sequence of values to order.</param>
    /// <param name="dependencyGraph">
    /// The dependency graph that defines dependencies between the elements of a sequence.
    /// </param>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted in topological order according to the specified dependency graph.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="dependencyGraph"/> is <see langword="null"/>.</exception>
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologically<TSource>(
        this IEnumerable<TSource> source,
        IReadOnlyGraph<TSource> dependencyGraph) =>
        OrderTopologicallyBy(source, Fn.Identity, dependencyGraph);

    /// <summary>
    /// <para>
    /// Sorts the elements of a sequence in topological order according to a key and specified dependency graph.
    /// </para>
    /// <para>
    /// The sort is stable.
    /// Circular dependencies are ignored and resolved according to the original order of elements in the sequence.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The sequence of values to order.</param>
    /// <param name="keySelector">The function to extract a key from an element.</param>
    /// <param name="dependencyGraph">
    /// The dependency graph that defines dependencies between the elements of a sequence.
    /// </param>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted in topological order according to a key and specified dependency graph.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="dependencyGraph"/> is <see langword="null"/>.</exception>
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologicallyBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IReadOnlyGraph<TKey> dependencyGraph)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));
        if (dependencyGraph == null)
            throw new ArgumentNullException(nameof(dependencyGraph));

        return new TopologicallyOrderedEnumerable.PrimaryEnumerable<TSource, TKey>(
            source,
            keySelector,
            null, // comparer is not used because the graph is already constructed 
            GraphFactory,
            true);

        IReadOnlyGraph<TKey> GraphFactory(in TopologicallyOrderedEnumerable.GraphFactoryContext<TSource, TKey> context) => dependencyGraph;
    }
}

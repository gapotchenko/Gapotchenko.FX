namespace Gapotchenko.FX.ValueTuple;

/// <summary>
/// Provides equality comparison functionality for value tuples.
/// </summary>
public static class ValueTupleEqualityComparer
{
    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1}"/> with a specified element equality comparer.
    /// </summary>
    /// <typeparam name="T1">The type of the value tuple's first element.</typeparam>
    /// <param name="comparer1">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing the value tuple's first element,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <returns>A new instance of value tuple equality comparer.</returns>
    public static IEqualityComparer<ValueTuple<T1>> Create<T1>(IEqualityComparer<T1>? comparer1) =>
        new ValueTupleEqualityComparer<T1>(comparer1);

    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1, T2}"/> with specified element equality comparers.
    /// </summary>
    /// <typeparam name="T1">The type of the value tuple's first element.</typeparam>
    /// <typeparam name="T2">The type of the value tuple's second element.</typeparam>
    /// <param name="comparer1">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing the value tuple's first element,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <param name="comparer2">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing the value tuple's second element,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <returns>A new instance of value tuple equality comparer.</returns>
    public static IEqualityComparer<ValueTuple<T1, T2>> Create<T1, T2>(IEqualityComparer<T1>? comparer1, IEqualityComparer<T2>? comparer2) =>
        new ValueTupleEqualityComparer<T1, T2>(comparer1, comparer2);

    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1, T2, T3}"/> with specified element equality comparers.
    /// </summary>
    /// <typeparam name="T1">The type of the value tuple's first element.</typeparam>
    /// <typeparam name="T2">The type of the value tuple's second element.</typeparam>
    /// <typeparam name="T3">The type of the value tuple's third element.</typeparam>
    /// <param name="comparer1">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing the value tuple's first element,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <param name="comparer2">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing the value tuple's second element,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <param name="comparer3">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing the value tuple's third element,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <returns>A new instance of value tuple equality comparer.</returns>
    public static IEqualityComparer<ValueTuple<T1, T2, T3>> Create<T1, T2, T3>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3) =>
        new ValueTupleEqualityComparer<T1, T2, T3>(comparer1, comparer2, comparer3);

    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1, T2, T3, T4}"/> with specified element equality comparers.
    /// </summary>
    /// <typeparam name="T1">The type of the value tuple's first element.</typeparam>
    /// <typeparam name="T2">The type of the value tuple's second element.</typeparam>
    /// <typeparam name="T3">The type of the value tuple's third element.</typeparam>
    /// <typeparam name="T4">The type of the value tuple's fourth element.</typeparam>
    /// <param name="comparer1">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing the value tuple's first element,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <param name="comparer2">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing the value tuple's second element,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <param name="comparer3">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing the value tuple's third element,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <param name="comparer4">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing the value tuple's fourth element,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
    /// </param>
    /// <returns>A new instance of value tuple equality comparer.</returns>
    public static IEqualityComparer<ValueTuple<T1, T2, T3, T4>> Create<T1, T2, T3, T4>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4) =>
        new ValueTupleEqualityComparer<T1, T2, T3, T4>(comparer1, comparer2, comparer3, comparer4);
}

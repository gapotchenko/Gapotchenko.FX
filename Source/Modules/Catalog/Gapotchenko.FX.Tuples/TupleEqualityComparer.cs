// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Tuples;

/// <summary>
/// Provides equality comparison functionality for tuples.
/// </summary>
public static class TupleEqualityComparer
{
    /// <summary>
    /// Creates an equality comparer for 1-tuple with the specified equality comparer for its component.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <param name="comparer1">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the tuple's first component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T1"/> type.
    /// </param>
    /// <returns>An instance of tuple equality comparer.</returns>
    public static IEqualityComparer<Tuple<T1>> Create<T1>(IEqualityComparer<T1>? comparer1) =>
        new TupleEqualityComparer<T1>(comparer1);

    /// <summary>
    /// Creates an equality comparer for 2-tuple with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1}(IEqualityComparer{T1}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the tuple's second component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T2"/> type.
    /// </param>
    public static IEqualityComparer<Tuple<T1, T2>> Create<T1, T2>(IEqualityComparer<T1>? comparer1, IEqualityComparer<T2>? comparer2) =>
        new TupleEqualityComparer<T1, T2>(comparer1, comparer2);

    /// <summary>
    /// Creates an equality comparer for 3-tuple with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2}(IEqualityComparer{T1}?, IEqualityComparer{T2}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the tuple's third component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T3"/> type.
    /// </param>
    public static IEqualityComparer<Tuple<T1, T2, T3>> Create<T1, T2, T3>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3) =>
        new TupleEqualityComparer<T1, T2, T3>(comparer1, comparer2, comparer3);

    /// <summary>
    /// Creates an equality comparer for 4-tuple with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2, T3}(IEqualityComparer{T1}?, IEqualityComparer{T2}?, IEqualityComparer{T3}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3"><inheritdoc/></typeparam>
    /// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3"><inheritdoc/></param>
    /// <param name="comparer4">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the tuple's fourth component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T4"/> type.
    /// </param>
    public static IEqualityComparer<Tuple<T1, T2, T3, T4>> Create<T1, T2, T3, T4>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4) =>
        new TupleEqualityComparer<T1, T2, T3, T4>(comparer1, comparer2, comparer3, comparer4);

    /// <summary>
    /// Creates an equality comparer for 5-tuple with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2, T3, T4}(IEqualityComparer{T1}?, IEqualityComparer{T2}?, IEqualityComparer{T3}?, IEqualityComparer{T4}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3"><inheritdoc/></typeparam>
    /// <typeparam name="T4"><inheritdoc/></typeparam>
    /// <typeparam name="T5">The type of the tuple's fifth component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3"><inheritdoc/></param>
    /// <param name="comparer4"><inheritdoc/></param>
    /// <param name="comparer5">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the tuple's fifth component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T5"/> type.
    /// </param>
    public static IEqualityComparer<Tuple<T1, T2, T3, T4, T5>> Create<T1, T2, T3, T4, T5>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4,
        IEqualityComparer<T5>? comparer5) =>
        new TupleEqualityComparer<T1, T2, T3, T4, T5>(comparer1, comparer2, comparer3, comparer4, comparer5);

    /// <summary>
    /// Creates an equality comparer for 6-tuple with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2, T3, T4, T5}(IEqualityComparer{T1}?, IEqualityComparer{T2}?, IEqualityComparer{T3}?, IEqualityComparer{T4}?, IEqualityComparer{T5}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3"><inheritdoc/></typeparam>
    /// <typeparam name="T4"><inheritdoc/></typeparam>
    /// <typeparam name="T5"><inheritdoc/></typeparam>
    /// <typeparam name="T6">The type of the tuple's sixth component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3"><inheritdoc/></param>
    /// <param name="comparer4"><inheritdoc/></param>
    /// <param name="comparer5"><inheritdoc/></param>
    /// <param name="comparer6">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the tuple's sixth component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T6"/> type.
    /// </param>
    public static IEqualityComparer<Tuple<T1, T2, T3, T4, T5, T6>> Create<T1, T2, T3, T4, T5, T6>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4,
        IEqualityComparer<T5>? comparer5,
        IEqualityComparer<T6>? comparer6) =>
        new TupleEqualityComparer<T1, T2, T3, T4, T5, T6>(comparer1, comparer2, comparer3, comparer4, comparer5, comparer6);

    /// <summary>
    /// Creates an equality comparer for 7-tuple with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2, T3, T4, T5}(IEqualityComparer{T1}?, IEqualityComparer{T2}?, IEqualityComparer{T3}?, IEqualityComparer{T4}?, IEqualityComparer{T5}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3"><inheritdoc/></typeparam>
    /// <typeparam name="T4"><inheritdoc/></typeparam>
    /// <typeparam name="T5"><inheritdoc/></typeparam>
    /// <typeparam name="T6"><inheritdoc/></typeparam>
    /// <typeparam name="T7">The type of the tuple's seventh component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3"><inheritdoc/></param>
    /// <param name="comparer4"><inheritdoc/></param>
    /// <param name="comparer5"><inheritdoc/></param>
    /// <param name="comparer6"><inheritdoc/></param>
    /// <param name="comparer7">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the tuple's seventh component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T7"/> type.
    /// </param>
    public static IEqualityComparer<Tuple<T1, T2, T3, T4, T5, T6, T7>> Create<T1, T2, T3, T4, T5, T6, T7>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4,
        IEqualityComparer<T5>? comparer5,
        IEqualityComparer<T6>? comparer6,
        IEqualityComparer<T7>? comparer7) =>
        new TupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7>(comparer1, comparer2, comparer3, comparer4, comparer5, comparer6, comparer7);

    /// <summary>
    /// Creates an equality comparer for 8-tuple with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2, T3, T4, T5}(IEqualityComparer{T1}?, IEqualityComparer{T2}?, IEqualityComparer{T3}?, IEqualityComparer{T4}?, IEqualityComparer{T5}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3"><inheritdoc/></typeparam>
    /// <typeparam name="T4"><inheritdoc/></typeparam>
    /// <typeparam name="T5"><inheritdoc/></typeparam>
    /// <typeparam name="T6"><inheritdoc/></typeparam>
    /// <typeparam name="T7"><inheritdoc/></typeparam>
    /// <typeparam name="T8">The type of the tuple's eighth component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3"><inheritdoc/></param>
    /// <param name="comparer4"><inheritdoc/></param>
    /// <param name="comparer5"><inheritdoc/></param>
    /// <param name="comparer6"><inheritdoc/></param>
    /// <param name="comparer7"><inheritdoc/></param>
    /// <param name="comparer8">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the tuple's eighth component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T8"/> type.
    /// </param>
    public static IEqualityComparer<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4,
        IEqualityComparer<T5>? comparer5,
        IEqualityComparer<T6>? comparer6,
        IEqualityComparer<T7>? comparer7,
        IEqualityComparer<T8>? comparer8) =>
        new TupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8>(comparer1, comparer2, comparer3, comparer4, comparer5, comparer6, comparer7, comparer8);
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Tuples;

/// <summary>
/// Provides equality comparison functionality for value tuples.
/// </summary>
public static class ValueTupleEqualityComparer
{
    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1}"/> with the specified equality comparer for its component.
    /// </summary>
    /// <typeparam name="T1">The type of the value tuple's first component.</typeparam>
    /// <param name="comparer1">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the value tuple's first component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T1"/> type.
    /// </param>
    /// <returns>An instance of value tuple equality comparer.</returns>
    public static IEqualityComparer<ValueTuple<T1>> Create<T1>(IEqualityComparer<T1>? comparer1) =>
        comparer1 is null
            ? EqualityComparer<ValueTuple<T1>>.Default
            : new ValueTupleEqualityComparer<T1>(comparer1);

    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1, T2}"/> with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1}(IEqualityComparer{T1}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2">The type of the value tuple's second component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the value tuple's second component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T2"/> type.
    /// </param>
    public static IEqualityComparer<(T1, T2)> Create<T1, T2>(IEqualityComparer<T1>? comparer1, IEqualityComparer<T2>? comparer2) =>
        comparer1 is null && comparer2 is null
            ? EqualityComparer<(T1, T2)>.Default
            : new ValueTupleEqualityComparer<T1, T2>(comparer1, comparer2);

    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1, T2, T3}"/> with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2}(IEqualityComparer{T1}?, IEqualityComparer{T2}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3">The type of the value tuple's third component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the value tuple's third component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T3"/> type.
    /// </param>
    public static IEqualityComparer<(T1, T2, T3)> Create<T1, T2, T3>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3) =>
        comparer1 is null && comparer2 is null && comparer3 is null
            ? EqualityComparer<(T1, T2, T3)>.Default
            : new ValueTupleEqualityComparer<T1, T2, T3>(comparer1, comparer2, comparer3);

    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1, T2, T3, T4}"/> with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2, T3}(IEqualityComparer{T1}?, IEqualityComparer{T2}?, IEqualityComparer{T3}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3"><inheritdoc/></typeparam>
    /// <typeparam name="T4">The type of the value tuple's fourth component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3"><inheritdoc/></param>
    /// <param name="comparer4">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the value tuple's fourth component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T4"/> type.
    /// </param>
    public static IEqualityComparer<(T1, T2, T3, T4)> Create<T1, T2, T3, T4>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4) =>
        comparer1 is null && comparer2 is null && comparer3 is null && comparer4 is null
            ? EqualityComparer<(T1, T2, T3, T4)>.Default
            : new ValueTupleEqualityComparer<T1, T2, T3, T4>(comparer1, comparer2, comparer3, comparer4);

    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2, T3, T4}(IEqualityComparer{T1}?, IEqualityComparer{T2}?, IEqualityComparer{T3}?, IEqualityComparer{T4}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3"><inheritdoc/></typeparam>
    /// <typeparam name="T4"><inheritdoc/></typeparam>
    /// <typeparam name="T5">The type of the value tuple's fifth component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3"><inheritdoc/></param>
    /// <param name="comparer4"><inheritdoc/></param>
    /// <param name="comparer5">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the value tuple's fifth component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T5"/> type.
    /// </param>
    public static IEqualityComparer<(T1, T2, T3, T4, T5)> Create<T1, T2, T3, T4, T5>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4,
        IEqualityComparer<T5>? comparer5) =>
        comparer1 is null && comparer2 is null && comparer3 is null && comparer4 is null && comparer5 is null
            ? EqualityComparer<(T1, T2, T3, T4, T5)>.Default
            : new ValueTupleEqualityComparer<T1, T2, T3, T4, T5>(comparer1, comparer2, comparer3, comparer4, comparer5);

    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2, T3, T4, T5}(IEqualityComparer{T1}?, IEqualityComparer{T2}?, IEqualityComparer{T3}?, IEqualityComparer{T4}?, IEqualityComparer{T5}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3"><inheritdoc/></typeparam>
    /// <typeparam name="T4"><inheritdoc/></typeparam>
    /// <typeparam name="T5"><inheritdoc/></typeparam>
    /// <typeparam name="T6">The type of the value tuple's sixth component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3"><inheritdoc/></param>
    /// <param name="comparer4"><inheritdoc/></param>
    /// <param name="comparer5"><inheritdoc/></param>
    /// <param name="comparer6">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the value tuple's sixth component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T6"/> type.
    /// </param>
    public static IEqualityComparer<(T1, T2, T3, T4, T5, T6)> Create<T1, T2, T3, T4, T5, T6>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4,
        IEqualityComparer<T5>? comparer5,
        IEqualityComparer<T6>? comparer6) =>
        comparer1 is null && comparer2 is null && comparer3 is null && comparer4 is null && comparer5 is null && comparer6 is null
            ? EqualityComparer<(T1, T2, T3, T4, T5, T6)>.Default
            : new ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6>(comparer1, comparer2, comparer3, comparer4, comparer5, comparer6);

    /// <summary>
    /// Creates a value tuple equality comparer for <see cref="ValueTuple{T1, T2, T3, T4, T5, T6, T7}"/> with the specified equality comparers for its components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2, T3, T4, T5, T6}(IEqualityComparer{T1}?, IEqualityComparer{T2}?, IEqualityComparer{T3}?, IEqualityComparer{T4}?, IEqualityComparer{T5}?, IEqualityComparer{T6}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3"><inheritdoc/></typeparam>
    /// <typeparam name="T4"><inheritdoc/></typeparam>
    /// <typeparam name="T5"><inheritdoc/></typeparam>
    /// <typeparam name="T6"><inheritdoc/></typeparam>
    /// <typeparam name="T7">The type of the value tuple's seventh component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3"><inheritdoc/></param>
    /// <param name="comparer4"><inheritdoc/></param>
    /// <param name="comparer5"><inheritdoc/></param>
    /// <param name="comparer6"><inheritdoc/></param>
    /// <param name="comparer7">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the value tuple's seventh component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T7"/> type.
    /// </param>
    public static IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7)> Create<T1, T2, T3, T4, T5, T6, T7>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4,
        IEqualityComparer<T5>? comparer5,
        IEqualityComparer<T6>? comparer6,
        IEqualityComparer<T7>? comparer7) =>
        comparer1 is null && comparer2 is null && comparer3 is null && comparer4 is null && comparer5 is null && comparer6 is null && comparer7 is null
            ? EqualityComparer<(T1, T2, T3, T4, T5, T6, T7)>.Default
            : new ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7>(comparer1, comparer2, comparer3, comparer4, comparer5, comparer6, comparer7);

    /// <summary>
    /// Creates an equality comparer for value tuple with eight components with specified equality comparers for the components.
    /// </summary>
    /// <inheritdoc cref="Create{T1, T2, T3, T4, T5, T6, T7}(IEqualityComparer{T1}?, IEqualityComparer{T2}?, IEqualityComparer{T3}?, IEqualityComparer{T4}?, IEqualityComparer{T5}?, IEqualityComparer{T6}?, IEqualityComparer{T7}?)"/>
    /// <typeparam name="T1"><inheritdoc/></typeparam>
    /// <typeparam name="T2"><inheritdoc/></typeparam>
    /// <typeparam name="T3"><inheritdoc/></typeparam>
    /// <typeparam name="T4"><inheritdoc/></typeparam>
    /// <typeparam name="T5"><inheritdoc/></typeparam>
    /// <typeparam name="T6"><inheritdoc/></typeparam>
    /// <typeparam name="T7"><inheritdoc/></typeparam>
    /// <typeparam name="T8">The type of the value tuple's eighth component.</typeparam>
    /// <param name="comparer1"><inheritdoc/></param>
    /// <param name="comparer2"><inheritdoc/></param>
    /// <param name="comparer3"><inheritdoc/></param>
    /// <param name="comparer4"><inheritdoc/></param>
    /// <param name="comparer5"><inheritdoc/></param>
    /// <param name="comparer6"><inheritdoc/></param>
    /// <param name="comparer7"><inheritdoc/></param>
    /// <param name="comparer8">
    /// The <see cref="IEqualityComparer{T}"/> to use when comparing the value tuple's eighth component,
    /// or <see langword="null"/> to use the default equality comparer for <typeparamref name="T8"/> type.
    /// </param>
    public static IEqualityComparer<(T1, T2, T3, T4, T5, T6, T7, T8)> Create<T1, T2, T3, T4, T5, T6, T7, T8>(
        IEqualityComparer<T1>? comparer1,
        IEqualityComparer<T2>? comparer2,
        IEqualityComparer<T3>? comparer3,
        IEqualityComparer<T4>? comparer4,
        IEqualityComparer<T5>? comparer5,
        IEqualityComparer<T6>? comparer6,
        IEqualityComparer<T7>? comparer7,
        IEqualityComparer<T8>? comparer8) =>
        comparer1 is null && comparer2 is null && comparer3 is null && comparer4 is null && comparer5 is null && comparer6 is null && comparer7 is null && comparer8 is null
            ? EqualityComparer<(T1, T2, T3, T4, T5, T6, T7, T8)>.Default
            : new ValueTupleEqualityComparer<T1, T2, T3, T4, T5, T6, T7, T8>(comparer1, comparer2, comparer3, comparer4, comparer5, comparer6, comparer7, comparer8);
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NETCOREAPP3_0_OR_GREATER
#define TFF_READONLYSPAN_CONTAINS
#endif

#if NET8_0_OR_GREATER
#define TFF_READONLYSPAN_CONTAINSANY
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Memory;

partial class ReadOnlySpanPolyfills
{
    /// <summary>
    /// Indicates whether a specified value is found in a read-only span.
    /// Values are compared using <see cref="IEquatable{T}.Equals(T)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the span.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>
    /// <see langword="true"/> if found,
    /// <see langword="false"/> otherwise.
    /// </returns>
#if TFF_READONLYSPAN_CONTAINS && NET7_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(
#if !TFF_READONLYSPAN_CONTAINS
        this
#endif
        ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
    {
#if TFF_READONLYSPAN_CONTAINS && NET7_0_OR_GREATER
        return span.Contains(value);
#else
        return IndexOf(span, value) >= 0;
#endif
    }

    #region ContainsAny

    /// <summary>
    /// <para>
    /// Searches for an occurrence of <paramref name="value0"/> or <paramref name="value1"/>.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">One of the values to search for.</param>
    /// <param name="value1">One of the values to search for.</param>
    /// <returns>
    /// <see langword="true"/> if found.
    /// If not found, returns <see langword="false"/>.
    /// </returns>
#if TFF_READONLYSPAN_CONTAINSANY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny<T>(
#if !TFF_READONLYSPAN_CONTAINSANY
        this
#endif
        ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T>
    {
#if TFF_READONLYSPAN_CONTAINSANY
        return span.ContainsAny(value0, value1);
#else
        return span.IndexOfAny(value0, value1) >= 0;
#endif
    }

    /// <summary>
    /// <para>
    /// Searches for an occurrence of <paramref name="value0"/>, <paramref name="value1"/> or <paramref name="value2"/>.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="value0">One of the values to search for.</param>
    /// <param name="value1">One of the values to search for.</param>
    /// <param name="value2">One of the values to search for.</param>
    /// <returns>
    /// <see langword="true"/> if found.
    /// If not found, returns <see langword="false"/>.
    /// </returns>
#if TFF_READONLYSPAN_CONTAINSANY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny<T>(
#if !TFF_READONLYSPAN_CONTAINSANY
        this
#endif
        ReadOnlySpan<T> span, T value0, T value1, T value2) where T : IEquatable<T>
    {
#if TFF_READONLYSPAN_CONTAINSANY
        return span.ContainsAny(value0, value1, value2);
#else
        return span.IndexOfAny(value0, value1, value2) >= 0;
#endif
    }

    /// <summary>
    /// <para>
    /// Searches for an occurrence of any of the specified <paramref name="values"/>.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The set of values to search for.</param>
    /// <returns>
    /// <see langword="true"/> if found.
    /// If not found, returns <see langword="false"/>.
    /// </returns>
#if TFF_READONLYSPAN_CONTAINSANY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny<T>(
#if !TFF_READONLYSPAN_CONTAINSANY
        this
#endif
        ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>
    {
#if TFF_READONLYSPAN_CONTAINSANY
        return span.ContainsAny(values);
#else
        return span.IndexOfAny(values) >= 0;
#endif
    }

    #endregion
}

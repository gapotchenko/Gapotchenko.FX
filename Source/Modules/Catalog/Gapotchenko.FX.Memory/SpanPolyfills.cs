// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NET8_0_OR_GREATER
#define TFF_SPAN_CONTAINSANY
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Memory;

/// <summary>
/// Provides polyfills for <see cref="Span{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class SpanPolyfills
{
    /// <inheritdoc cref="ReadOnlySpanPolyfills.StartsWith{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T}?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer) =>
        ReadOnlySpanPolyfills.StartsWith(span, value, comparer);

    /// <inheritdoc cref="ReadOnlySpanPolyfills.EndsWith{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T}?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith<T>(this Span<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer) =>
        ReadOnlySpanPolyfills.EndsWith(span, value, comparer);

    #region ContainsAny

    /// <inheritdoc cref="ReadOnlySpanPolyfills.ContainsAny{T}(ReadOnlySpan{T}, T, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny<T>(
#if !TFF_SPAN_CONTAINSANY
        this
#endif
        Span<T> span, T value0, T value1) where T : IEquatable<T>
    {
#if TFF_SPAN_CONTAINSANY
        return span.ContainsAny(value0, value1);
#else
        return ReadOnlySpanPolyfills.ContainsAny(span, value0, value1);
#endif
    }

    /// <inheritdoc cref="ReadOnlySpanPolyfills.ContainsAny{T}(ReadOnlySpan{T}, T, T, T)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny<T>(
#if !TFF_SPAN_CONTAINSANY
        this
#endif
        Span<T> span, T value0, T value1, T value2) where T : IEquatable<T>
    {
#if TFF_SPAN_CONTAINSANY
        return span.ContainsAny(value0, value1, value2);
#else
        return ReadOnlySpanPolyfills.ContainsAny(span, value0, value1, value2);
#endif
    }

    /// <inheritdoc cref="ReadOnlySpanPolyfills.ContainsAny{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsAny<T>(
#if !TFF_SPAN_CONTAINSANY
        this
#endif
        Span<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>
    {
#if TFF_SPAN_CONTAINSANY
        return span.ContainsAny(values);
#else
        return ReadOnlySpanPolyfills.ContainsAny(span, values);
#endif
    }

    #endregion
}

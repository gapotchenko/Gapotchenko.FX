// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if TODO

#if NETCOREAPP3_0_OR_GREATER
#define TFF_SPAN_CONTAINS
#endif

#if NET8_0_OR_GREATER
#define TFF_SPAN_CONTAINSANY
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Memory;

partial class SpanPolyfills
{
    /// <summary>
    /// Indicates whether a specified value is found in a span.
    /// Values are compared using <see cref="IEquatable{T}.Equals(T)"/>.
    /// </summary>
    /// <inheritdoc cref="ReadOnlySpanPolyfills.Contains{T}(ReadOnlySpan{T}, T)"/>
    [OverloadResolutionPriority(1)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(
#if !TFF_SPAN_CONTAINS
        this
#endif
        Span<T> span, T value) where T : IEquatable<T>
    {
#if TFF_SPAN_CONTAINS
        return span.Contains(value);
#else
        return ReadOnlySpanPolyfills.Contains(span, value);
#endif
    }

    #region ContainsAny

    /// <inheritdoc cref="ReadOnlySpanPolyfills.ContainsAny{T}(ReadOnlySpan{T}, T, T)"/>
    [OverloadResolutionPriority(1)]
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
    [OverloadResolutionPriority(1)]
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
    [OverloadResolutionPriority(1)]
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

#endif

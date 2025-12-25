// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if TODO

// This file is not longer needed because C# 14.0 does automatic inference of ReadOnlySpan<T> from Span<T>.

#if NET10_0_OR_GREATER
#define TFF_SPAN_STARTSWITH
#define TFF_SPAN_ENDSWITH
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Memory;

/// <summary>
/// Provides polyfill methods for <see cref="Span{T}"/> structure.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class SpanPolyfills
{
    // This type is partial.
    // For the rest of the implementation, please take a look at the neighboring source files.

    /// <inheritdoc cref="ReadOnlySpanPolyfills.StartsWith{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T}?)"/>
#if TFF_SPAN_STARTSWITH
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    [OverloadResolutionPriority(1)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>(
#if !TFF_SPAN_STARTSWITH
        this
#endif
        Span<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer) =>
#if TFF_SPAN_STARTSWITH
        span.StartsWith(value, comparer);
#else
        ReadOnlySpanPolyfills.StartsWith(span, value, comparer);
#endif

    /// <inheritdoc cref="ReadOnlySpanPolyfills.EndsWith{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T}?)"/>
#if TFF_SPAN_ENDSWITH
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    [OverloadResolutionPriority(1)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith<T>(
#if !TFF_SPAN_STARTSWITH
        this
#endif
        Span<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer) =>
#if TFF_SPAN_STARTSWITH
        span.EndsWith(value, comparer);
#else
        ReadOnlySpanPolyfills.EndsWith(span, value, comparer);
#endif
}

#endif

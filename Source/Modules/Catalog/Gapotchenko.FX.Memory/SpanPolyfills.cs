// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Memory;

/// <summary>
/// Provides polyfills for <see cref="Span{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class SpanPolyfills
{
    // This class is partial. Please take a look at the neighboring source files.

    /// <inheritdoc cref="ReadOnlySpanPolyfills.StartsWith{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T}?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer) =>
        ReadOnlySpanPolyfills.StartsWith(span, value, comparer);

    /// <inheritdoc cref="ReadOnlySpanPolyfills.EndsWith{T}(ReadOnlySpan{T}, ReadOnlySpan{T}, IEqualityComparer{T}?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWith<T>(this Span<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer) =>
        ReadOnlySpanPolyfills.EndsWith(span, value, comparer);
}

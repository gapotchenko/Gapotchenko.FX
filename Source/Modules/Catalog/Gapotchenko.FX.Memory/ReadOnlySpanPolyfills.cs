#if NET6_0_OR_GREATER
#define TFF_READONLYSPAN_SEQUENCEEQUAL
#endif

#if NET8_0_OR_GREATER
#define TFF_READONLYSPAN_CONTAINSANY
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Memory;

/// <summary>
/// Provides polyfills for <see cref="ReadOnlySpan{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class ReadOnlySpanPolyfills
{
    // This class is partial. Please take a look at the neighboring source files.

    /// <summary>
    /// <para>
    /// Determines whether two sequences are equal by comparing the elements using an <see cref="IEqualityComparer{T}"/>.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="span">The first sequence to compare.</param>
    /// <param name="other">The second sequence to compare.</param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.
    /// </param>
    /// <returns><see langword="true"/> if the two sequences are equal; otherwise, <see langword="false"/>.</returns>
    public static bool SequenceEqual<T>(
#if !TFF_READONLYSPAN_SEQUENCEEQUAL
        this
#endif
        ReadOnlySpan<T> span,
        ReadOnlySpan<T> other,
        IEqualityComparer<T>? comparer = null)
    {
#if TFF_READONLYSPAN_SEQUENCEEQUAL
        return span.SequenceEqual(other, comparer);
#else
        comparer ??= EqualityComparer<T>.Default;
        for (int i = 0; i < span.Length; i++)
        {
            if (!comparer.Equals(span[i], other[i]))
                return false;
        }
        return true;
#endif
    }

    /// <summary>
    /// <para>
    /// Determines whether the beginning of a span matches the specified value by using a specified equality comparer.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the elements in the span.</typeparam>
    /// <param name="span">The source span.</param>
    /// <param name="value">A <see cref="ReadOnlySpan{T}"/> value to match.</param>
    /// <param name="comparer">
    /// An <see cref="IEqualityComparer{T}"/> to use to compare elements,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> matches the beginning of the <paramref name="span"/>; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
    {
        int valueLength = value.Length;
        return
            valueLength <= span.Length &&
            span[..valueLength].SequenceEqual(value, comparer);
    }

    /// <summary>
    /// <para>
    /// Determines whether the ending of a span matches the specified value by using a specified equality comparer.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the elements in the span.</typeparam>
    /// <param name="span">The source span.</param>
    /// <param name="value">A <see cref="ReadOnlySpan{T}"/> value to match.</param>
    /// <param name="comparer">
    /// An <see cref="IEqualityComparer{T}"/> to use to compare elements,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> matches the ending of the <paramref name="span"/>; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool EndsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer)
    {
        int valueLength = value.Length;
        int spanLength = span.Length;
        return
            valueLength <= spanLength &&
            span[(spanLength - valueLength)..].SequenceEqual(value, comparer);
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

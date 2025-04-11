#if NET6_0_OR_GREATER
#define TFF_READONLYSPAN_SEQUENCEEQUAL
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Memory;

/// <summary>
/// Provides polyfills for <see cref="ReadOnlySpan{T}"/>.
/// </summary>
#if NET7_0_OR_GREATER
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public static partial class ReadOnlySpanPolyfills
{
    // This class is partial. Please take a look at the neighboring source files.

    /// <summary>
    /// Searches for the specified value and returns the index of its first occurrence.
    /// Values are compared using <see cref="IEquatable{T}.Equals(T)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the span and value.</typeparam>
    /// <param name="span">The span to search.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>
    /// The index of the occurrence of the value in the span.
    /// If not found, returns <c>-1</c>.
    /// </returns>
#if NET7_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static int IndexOf<T>(ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
    {
#if NET7_0_OR_GREATER
        return span.IndexOf(value);
#else
        // Older versions of .NET did not support nullable reference types
        // in the implementation of this method. The polyfill fixes that.

        if (typeof(T).IsValueType)
        {
#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
            return span.IndexOf(value);
#pragma warning restore CS8631
        }
        else
        {
            for (int i = 0, n = span.Length; i < n; ++i)
            {
                if (Equals(span[i], value))
                    return i;
            }
            return -1;
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool Equals<T>(T a, T b) where T : IEquatable<T>? =>
        a is null ? b is null : a.Equals(b);

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
#if TFF_READONLYSPAN_SEQUENCEEQUAL
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
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
}

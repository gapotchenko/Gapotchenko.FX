// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Runtime.InteropServices;

namespace Gapotchenko.FX.Memory;

/// <summary>
/// Equality comparer for contiguous regions of memory represented by <see cref="ReadOnlySpan{T}"/> type.
/// </summary>
public static partial class SpanEqualityComparer
{
    #region Equals

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("SpanEqualityComparer.Equals(object, object) method cannot be used. Use SpanEqualityComparer.Equals<T>(ReadOnlySpan<T>, ReadOnlySpan<T>) method instead.", true)]
    public static new bool Equals(object? objA, object? objB) => throw new NotSupportedException();

    /// <summary>
    /// Determines whether two spans are equal
    /// by comparing the elements using <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the spans.</typeparam>
    /// <param name="x">The first span to compare.</param>
    /// <param name="y">The second span to compare.</param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the two spans are equal; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Equals<T>(ReadOnlySpan<T> x, ReadOnlySpan<T> y, IEqualityComparer<T>? comparer = null)
    {
        return
            DistinguishableEquals(x, y) ??
            x.SequenceEqual(y, comparer);
    }

    /// <summary>
    /// Determines whether two spans are equal
    /// by comparing the elements using <see cref="IEquatable{T}.Equals(T)"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the spans.</typeparam>
    /// <param name="x">The first span to compare.</param>
    /// <param name="y">The second span to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two spans are equal; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Equals<T>(ReadOnlySpan<T> x, ReadOnlySpan<T> y) where T : IEquatable<T>?
    {
        return
            DistinguishableEquals(x, y) ??
            System.MemoryExtensions.SequenceEqual(x, y);
    }

    static bool? DistinguishableEquals<T>(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
    {
        if (x == y)
            return true;

        if (x == null || y == null)
            return false;

        return null;
    }

    #endregion

    #region GetHashCode

    /// <summary>
    /// Returns a hash code for the specified span by combining the hash codes
    /// returned by <see cref="IEqualityComparer{T}.GetHashCode(T)"/>
    /// for its elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span to compute a hash code for.</param>
    /// <param name="comparer">
    /// The <see cref="IEqualityComparer{T}"/> implementation to use when computing hash codes for elements,
    /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> for the type of an element.
    /// </param>
    /// <returns>A hash code representing the content of the specified span.</returns>
    public static int GetHashCode<T>(ReadOnlySpan<T> span, IEqualityComparer<T>? comparer = null)
    {
        return
            GetDistinguishableHashCode(span) ??
            HashOperations.GetHashCode(span, comparer);
    }

    /// <summary>
    /// Returns a hash code for the specified span of bytes.
    /// </summary>
    /// <param name="span">The span of bytes to compute a hash code for.</param>
    /// <returns>A hash code representing the content of the specified span of bytes.</returns>
    public static int GetHashCode(ReadOnlySpan<byte> span)
    {
        return
            GetDistinguishableHashCode(span) ??
            HashOperations.GetHashCode(span);
    }

    static int? GetDistinguishableHashCode<T>(ReadOnlySpan<T> span)
    {
        if (span == null)
            return -1;
        else if (span.Length == 0)
            return 0;
        else
            return null;
    }

    #endregion
}

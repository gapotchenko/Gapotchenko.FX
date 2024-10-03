namespace Gapotchenko.FX.Memory;

#pragma warning disable CA2265

/// <summary>
/// Equality comparer for contiguous regions of memory represented by <see cref="ReadOnlySpan{T}"/> type.
/// </summary>
public static class SpanEqualityComparer
{
    /// <summary>
    /// Determines whether specified read-only spans are equal.
    /// </summary>
    /// <typeparam name="T">The span element type.</typeparam>
    /// <param name="x">The first read-only span to compare.</param>
    /// <param name="y">The second read-only span to compare.</param>
    /// <returns><see langword="true"/> if the specified read-only spans are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Equals<T>(ReadOnlySpan<T> x, ReadOnlySpan<T> y) where T : IEquatable<T> =>
        x == y ||
        x != null && y != null &&
        x.SequenceEqual(y);

    /// <summary>
    /// Determines whether specified spans are equal.
    /// </summary>
    /// <typeparam name="T">The span element type.</typeparam>
    /// <param name="x">The first read-only span to compare.</param>
    /// <param name="y">The second read-only span to compare.</param>
    /// <returns><see langword="true"/> if the specified spans are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Equals<T>(Span<T> x, Span<T> y) where T : IEquatable<T> =>
        x == y ||
        x != null && y != null &&
        x.SequenceEqual(y);

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("SpanEqualityComparer.Equals(object, object) method cannot be used. Use SpanEqualityComparer.Equals<T>(ReadOnlySpan<T>, ReadOnlySpan<T>) method instead.", true)]
    public static new bool Equals(object? objA, object? objB) => throw new NotSupportedException();

    /// <summary>
    /// Returns a hash code for the specified read-only span.
    /// </summary>
    /// <typeparam name="T">The span element type.</typeparam>
    /// <param name="span">The read-only span.</param>
    /// <returns>A hash code for the specified read-only span.</returns>
    public static int GetHashCode<T>(ReadOnlySpan<T> span) where T : IEquatable<T>
    {
        if (span == null)
            return 0;

        // FNV-1a
        uint hash = 2166136261;
        foreach (var i in span)
            hash = (hash ^ (uint)(i?.GetHashCode() ?? 0)) * 16777619;
        return (int)hash;
    }

    /// <summary>
    /// Returns a hash code for the specified span.
    /// </summary>
    /// <typeparam name="T">The span element type.</typeparam>
    /// <param name="span">The span.</param>
    /// <returns>A hash code for the specified span.</returns>
    public static int GetHashCode<T>(Span<T> span) where T : IEquatable<T> =>
        GetHashCode((ReadOnlySpan<T>)span);

    /// <summary>
    /// Returns a hash code for the specified read-only span of bytes.
    /// </summary>
    /// <param name="span">The read-only span of bytes.</param>
    /// <returns>A hash code for the specified read-only span of bytes.</returns>
    public static int GetHashCode(ReadOnlySpan<byte> span)
    {
        if (span == null)
            return 0;

        // FNV-1a
        uint hash = 2166136261;
        foreach (var i in span)
            hash = (hash ^ i) * 16777619;
        return (int)hash;
    }

    /// <summary>
    /// Returns a hash code for the specified span of bytes.
    /// </summary>
    /// <param name="span">The span of bytes.</param>
    /// <returns>A hash code for the specified span of bytes.</returns>
    public static int GetHashCode(Span<byte> span) =>
        GetHashCode((ReadOnlySpan<byte>)span);
}

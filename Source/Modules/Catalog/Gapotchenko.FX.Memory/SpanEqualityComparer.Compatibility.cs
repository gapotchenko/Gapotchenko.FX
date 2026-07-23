using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Memory;

partial class SpanEqualityComparer
{
#if BINARY_COMPATIBILITY // 2026

    /// <summary>
    /// Determines whether specified spans are equal.
    /// </summary>
    /// <typeparam name="T">The span element type.</typeparam>
    /// <param name="x">The first read-only span to compare.</param>
    /// <param name="y">The second read-only span to compare.</param>
    /// <returns><see langword="true"/> if the specified spans are equal; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(-1000)]
    public static bool Equals<T>(Span<T> x, Span<T> y)
    {
        return
            x == y ||
            x != null && y != null &&
            x.SequenceEqual(y);
    }

    /// <summary>
    /// Returns a hash code for the specified span by combining the hash codes
    /// returned by <see cref="IEqualityComparer{T}.GetHashCode(T)"/>
    /// for its elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The span to compute a hash code for.</param>
    /// <returns>
    /// A hash code representing the content of the specified span.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(-1000)]
    public static int GetHashCode<T>(ReadOnlySpan<T> span) => GetHashCode(span, null);

    /// <summary>
    /// Returns a hash code for the specified span.
    /// </summary>
    /// <typeparam name="T">The span element type.</typeparam>
    /// <param name="span">The span.</param>
    /// <returns>A hash code for the specified span.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(-1000)]
    public static int GetHashCode<T>(Span<T> span) => GetHashCode((ReadOnlySpan<T>)span);

    /// <summary>
    /// Returns a hash code for the specified span of bytes.
    /// </summary>
    /// <param name="span">The span of bytes.</param>
    /// <returns>A hash code for the specified span of bytes.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [OverloadResolutionPriority(-1000)]
    public static int GetHashCode(Span<byte> span) => GetHashCode((ReadOnlySpan<byte>)span);

#endif
}

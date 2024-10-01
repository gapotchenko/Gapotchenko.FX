#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
#define TFF_ARRAYSEGMENT_SLICE
#endif

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfill extension methods for <see cref="ArraySegment{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ArraySegmentPolyfills
{
    /// <summary>
    /// Forms a slice out of the current array segment starting at the specified index.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </remarks>
    /// <typeparam name="T">The type of the elements in the array segment.</typeparam>
    /// <param name="segment">The array segment.</param>
    /// <param name="index">The index at which to begin the slice.</param>
    /// <returns>An array segment that consists of all elements of the current array segment from <paramref name="index"/> to the end of the array segment.</returns>
    /// <exception cref="InvalidOperationException">The underlying array is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Index was out of range.</exception>
#if TFF_ARRAYSEGMENT_SLICE
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static ArraySegment<T> Slice<T>(
#if !TFF_ARRAYSEGMENT_SLICE
        this
#endif
        in ArraySegment<T> segment,
        int index)
    {
#if TFF_ARRAYSEGMENT_SLICE
        return segment.Slice(index);
#else
        var array = segment.Array;
        if (array == null)
            throw new InvalidOperationException("The underlying array is null.");

        int segmentCount = segment.Count;

        if ((uint)index > (uint)segmentCount)
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than or equal to the size of the collection.");

        return new(array, segment.Offset + index, segmentCount - index);
#endif
    }

    /// <summary>
    /// Forms a slice out of the current array segment starting at the specified index.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </remarks>
    /// <typeparam name="T">The type of the elements in the array segment.</typeparam>
    /// <param name="segment">The array segment.</param>
    /// <param name="index">The index at which to begin the slice.</param>
    /// <param name="count">The desired length of the slice.</param>
    /// <returns>An array segment that consists of all elements of the current array segment from <paramref name="index"/> to the end of the array segment.</returns>
    /// <exception cref="InvalidOperationException">The underlying array is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Index was out of range.</exception>
#if TFF_ARRAYSEGMENT_SLICE
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static ArraySegment<T> Slice<T>(
#if !TFF_ARRAYSEGMENT_SLICE
        this
#endif
        in ArraySegment<T> segment, int index,
        int count)
    {
#if TFF_ARRAYSEGMENT_SLICE
        return segment.Slice(index, count);
#else
        var array = segment.Array;
        if (array == null)
            throw new InvalidOperationException("The underlying array is null.");

        int segmentCount = segment.Count;

        if ((uint)index > (uint)segmentCount || (uint)count > (uint)(segmentCount - index))
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than or equal to the size of the collection.");

        return new(array, segment.Offset + index, count);
#endif
    }
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Buffers;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.IO;

partial class StreamPolyfills
{
    /// <summary>
    /// Reads a sequence of bytes from the current stream and
    /// advances the position within the stream by the number of bytes read.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </remarks>
    /// <param name="stream">The stream to read the bytes from.</param>
    /// <param name="buffer">
    /// A region of memory.
    /// When this method returns, the contents of this region are replaced by the bytes read from the current source.
    /// </param>
    /// <returns>
    /// The total number of bytes read into the buffer.
    /// This can be less than the size of the buffer if that many bytes are not currently available,
    /// or <c>0</c> (zero) if the buffer's length is <c>0</c> or the end of the stream has been reached.
    /// </returns>
#if TFF_STREAM_SPAN
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    // TODO: method use should be discouraged because the polyfill implementation incurs memory copying.
#endif
    public static int Read(
#if !TFF_STREAM_SPAN
        this
#endif
        Stream stream,
        Span<byte> buffer)
    {
        ArgumentNullException.ThrowIfNull(stream);

#if TFF_STREAM_SPAN
        return stream.Read(buffer);
#else
        int count = buffer.Length;

        var arrayPool = ArrayPool<byte>.Shared;
        byte[] array = arrayPool.Rent(count);
        try
        {
            int result = stream.Read(array, 0, count);
            new ReadOnlySpan<byte>(array, 0, result).CopyTo(buffer);
            return result;
        }
        finally
        {
            arrayPool.Return(array);
        }
#endif
    }

#if TFF_VALUETASK

    /// <summary>
    /// Asynchronously reads a sequence of bytes from the current stream,
    /// advances the position within the stream by the number of bytes read,
    /// and monitors cancellation requests.
    /// </summary>
    /// <param name="stream">The stream to read the bytes from.</param>
    /// <param name="buffer">The region of memory to write the data into.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous read operation.
    /// The value of its <see cref="ValueTask{TResult}.Result"/> property contains the total number of bytes read into the buffer.
    /// The result value can be less than the length of the buffer if that many bytes are not currently available,
    /// or it can be <c>0</c> (zero) if the length of the buffer is <c>0</c> or if the end of the stream has been reached.
    /// </returns>
#if TFF_STREAM_SPAN
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    // TODO: method use should be discouraged because the polyfill implementation incurs memory copying.
#endif
    public static ValueTask<int> ReadAsync(
#if !TFF_STREAM_SPAN
        this
#endif
        Stream stream,
        Memory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

#if TFF_STREAM_SPAN
        return stream.ReadAsync(buffer, cancellationToken);
#else
        if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> segment))
            return new(stream.ReadAsync(segment.Array!, segment.Offset, segment.Count, cancellationToken));
        else
            return CoreImpl(stream, buffer, cancellationToken);

        static async ValueTask<int> CoreImpl(Stream stream, Memory<byte> buffer, CancellationToken cancellationToken)
        {
            int count = buffer.Length;

            var arrayPool = ArrayPool<byte>.Shared;
            byte[] array = arrayPool.Rent(count);
            try
            {
                int result = await stream.ReadAsync(array, 0, count, cancellationToken).ConfigureAwait(false);
                new ReadOnlySpan<byte>(array, 0, result).CopyTo(buffer.Span);
                return result;
            }
            finally
            {
                arrayPool.Return(array);
            }
        }
#endif
    }

#endif
}

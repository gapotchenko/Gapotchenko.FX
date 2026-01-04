// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Buffers;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.IO;

partial class StreamPolyfills
{
    /// <summary>
    /// Writes a sequence of bytes to the current stream and
    /// advances the current position within this stream by the number of bytes written.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </remarks>
    /// <param name="stream">The stream to write the bytes to.</param>
    /// <param name="buffer">The region of memory to write data from.</param>
#if TFF_STREAM_SPAN
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    // TODO: method use should be discouraged because the polyfill implementation incurs memory copying.
#endif
    public static void Write(
#if !TFF_STREAM_SPAN
        this
#endif
        Stream stream,
        ReadOnlySpan<byte> buffer)
    {
        ArgumentNullException.ThrowIfNull(stream);

#if TFF_STREAM_SPAN
        stream.Write(buffer);
#else
        int count = buffer.Length;

        var pool = ArrayPool<byte>.Shared;
        byte[] array = pool.Rent(count);
        try
        {
            buffer.CopyTo(array);
            stream.Write(array, 0, count);
        }
        finally
        {
            pool.Return(array);
        }
#endif
    }

    /// <summary>
    /// Asynchronously writes a sequence of bytes to the current stream,
    /// advances the current position within this stream by the number of bytes written,
    /// and monitors cancellation requests.
    /// </summary>
    /// <param name="stream">The stream to write the bytes to.</param>
    /// <param name="buffer">The region of memory to write data from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
#if TFF_STREAM_SPAN
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    // TODO: method use should be discouraged because the polyfill implementation incurs memory copying.
#endif
    public static Task WriteAsync(
#if !TFF_STREAM_SPAN
        this
#endif
        Stream stream,
        ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

#if TFF_STREAM_SPAN
        return stream.WriteAsync(buffer, cancellationToken).AsTask();
#else
        if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> array))
        {
            return stream.WriteAsync(array.Array!, array.Offset, array.Count, cancellationToken);
        }
        else
        {
            return CoreImpl(stream, buffer, cancellationToken);
        }

        static async Task CoreImpl(Stream stream, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            int count = buffer.Length;
            var pool = ArrayPool<byte>.Shared;
            byte[] sharedBuffer = pool.Rent(count);
            try
            {
                buffer.Span.CopyTo(sharedBuffer);
                await stream.WriteAsync(sharedBuffer, 0, count, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                pool.Return(sharedBuffer);
            }
        }
#endif
    }
}

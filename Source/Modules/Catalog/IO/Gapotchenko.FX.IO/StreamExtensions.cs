// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Buffers;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Provides extension methods for <see cref="Stream"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class StreamExtensions
{
    /// <inheritdoc cref="CopyBlockTo(Stream, Stream, long)"/>
    public static void CopyBlockTo(this Stream source, Stream destination, long count)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        CopyBlockToCore(source, destination, count, GetCopyBufferSize(source));
    }

    /// <summary>
    /// Reads the bytes from the source stream and writes them to destination stream.
    /// </summary>
    /// <param name="source">The source stream.</param>
    /// <param name="destination">The destination stream.</param>
    /// <param name="count">The count of bytes to copy.</param>
    /// <param name="bufferSize">The buffer size.</param>
    public static void CopyBlockTo(this Stream source, Stream destination, long count, int bufferSize)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));
        if (bufferSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(bufferSize));

        CopyBlockToCore(source, destination, count, bufferSize);
    }

    static void CopyBlockToCore(Stream source, Stream destination, long count, int bufferSize)
    {
        if (count == 0)
        {
            // There is nothing to do.
            return;
        }

        var arrayPool = ArrayPool<byte>.Shared;
        byte[] buffer = arrayPool.Rent(bufferSize);
        try
        {
            bufferSize = buffer.Length;
            do
            {
                int bytesToRead = (int)Math.Min(bufferSize, count);
                int bytesRead = source.Read(buffer, 0, bytesToRead);

                if (bytesRead == 0)
                {
                    // The source stream has reached the end prematurely.
                    throw new EndOfStreamException();
                }

                destination.Write(buffer, 0, bytesRead);
                count -= bytesRead;
            }
            while (count != 0);
        }
        finally
        {
            arrayPool.Return(buffer);
        }
    }

#if TFF_ASYNC_STREAM

#if BINARY_COMPATIBILITY

    /// <inheritdoc cref="CopyBlockToAsync(Stream, Stream, long, CancellationToken)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task CopyBlockToAsync(Stream source, Stream destination, long count) =>
        source.CopyBlockToAsync(destination, count, CancellationToken.None);

    /// <inheritdoc cref="CopyBlockToAsync(Stream, Stream, long, int, CancellationToken)"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Task CopyBlockToAsync(Stream source, Stream destination, long count, int bufferSize) =>
        source.CopyBlockToAsync(destination, count, bufferSize, CancellationToken.None);

#endif

    /// <inheritdoc cref="CopyBlockToAsync(Stream, Stream, long, int, CancellationToken)"/>
    public static Task CopyBlockToAsync(
        this Stream source,
        Stream destination,
        long count,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        return CopyBlockToAsyncCore(
            source,
            destination,
            count,
            GetCopyBufferSize(source),
            cancellationToken);
    }

    /// <summary>
    /// Reads the bytes from the source stream and writes them to destination stream.
    /// </summary>
    /// <param name="source">The source stream.</param>
    /// <param name="destination">The destination stream.</param>
    /// <param name="count">The count of bytes to copy.</param>
    /// <param name="bufferSize">The buffer size.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    public static Task CopyBlockToAsync(
        this Stream source,
        Stream destination,
        long count,
        int bufferSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));
        if (bufferSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(bufferSize));

        return CopyBlockToAsyncCore(source, destination, count, bufferSize, cancellationToken);
    }

    static async Task CopyBlockToAsyncCore(
        Stream source,
        Stream destination,
        long count,
        int bufferSize,
        CancellationToken cancellationToken)
    {
        if (count == 0)
        {
            // There is nothing to do.
            return;
        }

        var arrayPool = ArrayPool<byte>.Shared;
        byte[] buffer = arrayPool.Rent(bufferSize);
        try
        {
            bufferSize = buffer.Length;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                int bytesToRead = (int)(Math.Min(bufferSize, count));
                int bytesRead = await
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    source.ReadAsync(buffer.AsMemory(0, bytesToRead), cancellationToken)
#else
                    source.ReadAsync(buffer, 0, bytesToRead, cancellationToken)
#endif
                    .ConfigureAwait(false);

                if (bytesRead == 0)
                {
                    // The source stream has reached the end prematurely.
                    throw new EndOfStreamException();
                }

                await
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken)
#else
                    destination.WriteAsync(buffer, 0, bytesRead, cancellationToken)
#endif
                    .ConfigureAwait(false);
                count -= bytesRead;
            }
            while (count != 0);
        }
        finally
        {
            arrayPool.Return(buffer);
        }
    }

#endif

    static int GetCopyBufferSize(Stream source)
    {
        // This value was originally picked to be the largest multiple of 4096 that is still smaller than the large object heap threshold (85K).
        // The CopyTo{Async} buffer is short-lived and is likely to be collected at Gen0, and it offers a significant improvement in Copy
        // performance.  Since then, the base implementations of CopyTo{Async} have been updated to use ArrayPool, which will end up rounding
        // this size up to the next power of two (131,072), which will by default be on the large object heap.  However, most of the time
        // the buffer should be pooled, the LOH threshold is now configurable and thus may be different than 85K, and there are measurable
        // benefits to using the larger buffer size.  So, for now, this value remains.
        const int DefaultCopyBufferSize = 81920;

        int bufferSize = DefaultCopyBufferSize;

        if (source.CanSeek)
        {
            long length = source.Length;
            long position = source.Position;
            if (length <= position) // Handles negative overflows
            {
                // There are no bytes left in the stream to copy.
                // However, because CopyTo{Async} is virtual, we need to
                // ensure that any override is still invoked to provide its
                // own validation, so we use the smallest legal buffer size here.
                bufferSize = 1;
            }
            else
            {
                long remaining = length - position;
                if (remaining > 0)
                {
                    // In the case of a positive overflow, stick to the default size
                    bufferSize = (int)Math.Min(bufferSize, remaining);
                }
            }
        }

        return bufferSize;
    }

    /// <summary>
    /// Enumerates the byte contents of a stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>The sequence of bytes representing the contents of a stream.</returns>
    public static IEnumerable<byte> AsEnumerable(this Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        for (; ; )
        {
            int b = stream.ReadByte();
            if (b == -1)
            {
                // End of file.
                break;
            }

            yield return (byte)b;
        }
    }
}

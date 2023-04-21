// Portions (c) .NET Foundation 

#if NET7_0_OR_GREATER
#define TFF_STREAM_READEXACTLY
#endif

using Gapotchenko.FX.IO.Properties;
using System.Diagnostics;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Provides polyfill extension methods for <see cref="Stream"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class StreamPolyfills
{
#if TFF_MEMORY && !TFF_MEMORY_OOB
    /// <summary>
    /// Reads bytes from the current stream and advances the position within the stream until the <paramref name="buffer"/> is filled.
    /// </summary>
    /// <param name="stream">The stream to read the bytes from.</param>
    /// <param name="buffer">A region of memory. When this method returns, the contents of this region are replaced by the bytes read from the current stream.</param>
    /// <exception cref="EndOfStreamException">
    /// The end of the stream is reached before filling the <paramref name="buffer"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// When <paramref name="buffer"/> is empty, this read operation will be completed without waiting for available data in the stream.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
#if TFF_STREAM_READEXACTLY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static void ReadExactly(
#if !TFF_STREAM_READEXACTLY
        this
#endif
        Stream stream,
        Span<byte> buffer)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

#if TFF_STREAM_READEXACTLY
        stream.ReadExactly(buffer);
#else
        ReadAtLeastCore(stream, buffer, buffer.Length, throwOnEndOfStream: true);
#endif
    }

    /// <summary>
    /// Asynchronously reads bytes from the current stream, advances the position within the stream until the <paramref name="buffer"/> is filled,
    /// and monitors cancellation requests.
    /// </summary>
    /// <param name="stream">The stream to read the bytes from.</param>
    /// <param name="buffer">The buffer to write the data into.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    /// <exception cref="EndOfStreamException">
    /// The end of the stream is reached before filling the <paramref name="buffer"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// When <paramref name="buffer"/> is empty, this read operation will be completed without waiting for available data in the stream.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
#if TFF_STREAM_READEXACTLY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static ValueTask ReadExactlyAsync(
#if !TFF_STREAM_READEXACTLY
        this
#endif
        Stream stream,
        Memory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

#if TFF_STREAM_READEXACTLY
        return stream.ReadExactlyAsync(buffer, cancellationToken);
#else
        return ReadExactlyAsyncImpl(stream, buffer, cancellationToken);

        static async ValueTask ReadExactlyAsyncImpl(
            Stream stream,
            Memory<byte> buffer,
            CancellationToken cancellationToken) =>
            await ReadAtLeastAsyncCore(stream, buffer, buffer.Length, throwOnEndOfStream: true, cancellationToken).ConfigureAwait(false);
#endif
    }

    /// <summary>
    /// Reads at least a minimum number of bytes from the current stream and advances the position within the stream by the number of bytes read.
    /// </summary>
    /// <param name="stream">The stream to read the bytes from.</param>
    /// <param name="buffer">A region of memory. When this method returns, the contents of this region are replaced by the bytes read from the current stream.</param>
    /// <param name="minimumBytes">The minimum number of bytes to read into the buffer.</param>
    /// <param name="throwOnEndOfStream">
    /// <see langword="true"/> to throw an exception if the end of the stream is reached before reading <paramref name="minimumBytes"/> of bytes;
    /// <see langword="false"/> to return less than <paramref name="minimumBytes"/> when the end of the stream is reached.
    /// The default is <see langword="true"/>.
    /// </param>
    /// <returns>
    /// The total number of bytes read into the buffer. This is guaranteed to be greater than or equal to <paramref name="minimumBytes"/>
    /// when <paramref name="throwOnEndOfStream"/> is <see langword="true"/>. This will be less than <paramref name="minimumBytes"/> when the
    /// end of the stream is reached and <paramref name="throwOnEndOfStream"/> is <see langword="false"/>. This can be less than the number
    /// of bytes allocated in the buffer if that many bytes are not currently available.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="minimumBytes"/> is negative, or is greater than the length of <paramref name="buffer"/>.
    /// </exception>
    /// <exception cref="EndOfStreamException">
    /// <paramref name="throwOnEndOfStream"/> is <see langword="true"/> and the end of the stream is reached before reading
    /// <paramref name="minimumBytes"/> bytes of data.
    /// </exception>
    /// <remarks>
    /// <para>
    /// When <paramref name="minimumBytes"/> is 0 (zero), this read operation will be completed without waiting for available data in the stream.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
#if TFF_STREAM_READEXACTLY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int ReadAtLeast(
#if !TFF_STREAM_READEXACTLY
        this
#endif
        Stream stream,
        Span<byte> buffer,
        int minimumBytes,
        bool throwOnEndOfStream = true)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

#if TFF_STREAM_READEXACTLY
        return stream.ReadAtLeast(buffer, minimumBytes, throwOnEndOfStream);
#else
        ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

        return ReadAtLeastCore(stream, buffer, minimumBytes, throwOnEndOfStream);
#endif
    }

    /// <summary>
    /// Asynchronously reads at least a minimum number of bytes from the current stream, advances the position within the stream by the
    /// number of bytes read, and monitors cancellation requests.
    /// </summary>
    /// <param name="stream">The stream to read the bytes from.</param>
    /// <param name="buffer">The region of memory to write the data into.</param>
    /// <param name="minimumBytes">The minimum number of bytes to read into the buffer.</param>
    /// <param name="throwOnEndOfStream">
    /// <see langword="true"/> to throw an exception if the end of the stream is reached before reading <paramref name="minimumBytes"/> of bytes;
    /// <see langword="false"/> to return less than <paramref name="minimumBytes"/> when the end of the stream is reached.
    /// The default is <see langword="true"/>.
    /// </param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous read operation. The value of its <see cref="ValueTask{TResult}.Result"/> property contains the
    /// total number of bytes read into the buffer. This is guaranteed to be greater than or equal to <paramref name="minimumBytes"/> when
    /// <paramref name="throwOnEndOfStream"/> is <see langword="true"/>. This will be less than <paramref name="minimumBytes"/> when the end
    /// of the stream is reached and <paramref name="throwOnEndOfStream"/> is <see langword="false"/>. This can be less than the number of
    /// bytes allocated in the buffer if that many bytes are not currently available.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="minimumBytes"/> is negative, or is greater than the length of <paramref name="buffer"/>.
    /// </exception>
    /// <exception cref="EndOfStreamException">
    /// <paramref name="throwOnEndOfStream"/> is <see langword="true"/> and the end of the stream is reached before reading
    /// <paramref name="minimumBytes"/> bytes of data.
    /// </exception>
    /// <remarks>
    /// <para>
    /// When <paramref name="minimumBytes"/> is 0 (zero), this read operation will be completed without waiting for available data in the stream.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
#if TFF_STREAM_READEXACTLY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static ValueTask<int> ReadAtLeastAsync(
#if !TFF_STREAM_READEXACTLY
        this
#endif
        Stream stream,
        Memory<byte> buffer,
        int minimumBytes,
        bool throwOnEndOfStream = true,
        CancellationToken cancellationToken = default)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

#if TFF_STREAM_READEXACTLY
        return stream.ReadAtLeastAsync(buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
#else
        ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

        return ReadAtLeastAsyncCore(stream, buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
#endif
    }

#if !TFF_STREAM_READEXACTLY
    static int ReadAtLeastCore(Stream stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
    {
        Debug.Assert(minimumBytes <= buffer.Length);

        int totalRead = 0;
        while (totalRead < minimumBytes)
        {
            int read = stream.Read(buffer.Slice(totalRead));
            if (read == 0)
            {
                if (throwOnEndOfStream)
                    throw new EndOfStreamException();

                break;
            }

            totalRead += read;
        }

        return totalRead;
    }

    static async ValueTask<int> ReadAtLeastAsyncCore(
        Stream stream,
        Memory<byte> buffer,
        int minimumBytes,
        bool throwOnEndOfStream,
        CancellationToken cancellationToken)
    {
        Debug.Assert(minimumBytes <= buffer.Length);

        int totalRead = 0;
        while (totalRead < minimumBytes)
        {
            int read = await stream.ReadAsync(buffer.Slice(totalRead), cancellationToken).ConfigureAwait(false);
            if (read == 0)
            {
                if (throwOnEndOfStream)
                    throw new EndOfStreamException();

                return totalRead;
            }

            totalRead += read;
        }

        return totalRead;
    }

    static void ValidateReadAtLeastArguments(int bufferLength, int minimumBytes)
    {
        if (minimumBytes < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumBytes), Resources.ArgumentOutOfRange_NonNegativeNumberRequired);
        if (bufferLength < minimumBytes)
            throw new ArgumentOutOfRangeException(nameof(minimumBytes), Resources.ArgumentOutOfRange_NotGreaterThanBufferLength);
    }
#endif

#else

#if !TFF_STREAM_READEXACTLY
    static int ReadAtLeastCore(Stream stream, ArraySegment<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
    {
        Debug.Assert(minimumBytes <= buffer.Count);

        int totalRead = 0;
        while (totalRead < minimumBytes)
        {
            var segment = buffer.Slice(totalRead);
            int read = stream.Read(segment.Array!, segment.Offset, segment.Count);
            if (read == 0)
            {
                if (throwOnEndOfStream)
                    throw new EndOfStreamException();

                break;
            }

            totalRead += read;
        }

        return totalRead;
    }
#endif

#endif

    /// <summary>
    /// Reads <paramref name="count"/> number of bytes from the current stream and advances the position within the stream.
    /// </summary>
    /// <param name="stream">The stream to read the bytes from.</param>
    /// <param name="buffer">
    /// An array of bytes. When this method returns, the buffer contains the specified byte array with the values
    /// between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced
    /// by the bytes read from the current stream.
    /// </param>
    /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
    /// <param name="count">The number of bytes to be read from the current stream.</param>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is outside the bounds of <paramref name="buffer"/>.
    /// -or-
    /// <paramref name="count"/> is negative.
    /// -or-
    /// The range specified by the combination of <paramref name="offset"/> and <paramref name="count"/> exceeds the
    /// length of <paramref name="buffer"/>.
    /// </exception>
    /// <exception cref="EndOfStreamException">
    /// The end of the stream is reached before reading <paramref name="count"/> number of bytes.
    /// </exception>
    /// <remarks>
    /// <para>
    /// When <paramref name="count"/> is 0 (zero), this read operation will be completed without waiting for available data in the stream.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
#if TFF_STREAM_READEXACTLY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static void ReadExactly(
#if !TFF_STREAM_READEXACTLY
        this
#endif
        Stream stream,
        byte[] buffer,
        int offset,
        int count)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

#if TFF_STREAM_READEXACTLY
        stream.ReadExactly(buffer, offset, count);
#else
        ValidateBufferArguments(buffer, offset, count);

#if TFF_MEMORY && !TFF_MEMORY_OOB
        stream.ReadExactly(buffer.AsSpan(offset, count));
#else
        ReadAtLeastCore(stream, new(buffer, offset, count), count, throwOnEndOfStream: true);
#endif
#endif
    }

#if TFF_VALUETASK
    /// <summary>
    /// Asynchronously reads <paramref name="count"/> number of bytes from the current stream, advances the position within the stream,
    /// and monitors cancellation requests.
    /// </summary>
    /// <param name="stream">The stream to read the bytes from.</param>
    /// <param name="buffer">The buffer to write the data into.</param>
    /// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing data from the stream.</param>
    /// <param name="count">The number of bytes to be read from the current stream.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is outside the bounds of <paramref name="buffer"/>.
    /// -or-
    /// <paramref name="count"/> is negative.
    /// -or-
    /// The range specified by the combination of <paramref name="offset"/> and <paramref name="count"/> exceeds the
    /// length of <paramref name="buffer"/>.
    /// </exception>
    /// <exception cref="EndOfStreamException">
    /// The end of the stream is reached before reading <paramref name="count"/> number of bytes.
    /// </exception>
    /// <remarks>
    /// <para>
    /// When <paramref name="count"/> is 0 (zero), this read operation will be completed without waiting for available data in the stream.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
#if TFF_STREAM_READEXACTLY
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static ValueTask ReadExactlyAsync(
#if !TFF_STREAM_READEXACTLY
        this
#endif
        Stream stream,
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken = default)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

#if TFF_STREAM_READEXACTLY
        return stream.ReadExactlyAsync(buffer, offset, count);
#else
        ValidateBufferArguments(buffer, offset, count);

        return stream.ReadExactlyAsync(buffer.AsMemory(offset, count), cancellationToken);
#endif
    }
#endif

#if !TFF_STREAM_READEXACTLY
    static void ValidateBufferArguments(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), Resources.ArgumentOutOfRange_NonNegativeNumberRequired);
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), Resources.ArgumentOutOfRange_NonNegativeNumberRequired);

        if (buffer.Length - offset < count)
        {
            throw new ArgumentException(
                "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.",
                nameof(count));
        }
    }
#endif
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if NET7_0_OR_GREATER
#define TFF_STREAM_READATLEAST
#endif

using Gapotchenko.FX.IO.Properties;
using System.Diagnostics;

namespace Gapotchenko.FX.IO;

partial class StreamPolyfills
{
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
#if TFF_STREAM_READATLEAST
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int ReadAtLeast(
#if !TFF_STREAM_READATLEAST
        this
#endif
        Stream stream,
        Span<byte> buffer,
        int minimumBytes,
        bool throwOnEndOfStream = true)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

#if TFF_STREAM_READATLEAST
        return stream.ReadAtLeast(buffer, minimumBytes, throwOnEndOfStream);
#else
        ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

        return ReadAtLeastCore(stream, buffer, minimumBytes, throwOnEndOfStream);
#endif
    }

#if TFF_VALUETASK

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
#if TFF_STREAM_READATLEAST
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static ValueTask<int> ReadAtLeastAsync(
#if !TFF_STREAM_READATLEAST
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

#if TFF_STREAM_READATLEAST
        return stream.ReadAtLeastAsync(buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
#else
        ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

        return ReadAtLeastAsyncCore(stream, buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
#endif
    }

#endif // TFF_VALUE_TASK

#if !TFF_STREAM_READATLEAST

    static int ReadAtLeastCore(Stream stream, Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
    {
        Debug.Assert(minimumBytes <= buffer.Length);

        int totalRead = 0;
        while (totalRead < minimumBytes)
        {
            int read = Read(stream, buffer.Slice(totalRead));
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

#if TFF_VALUETASK

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

#endif // TFF_VALUETASK

    static void ValidateReadAtLeastArguments(int bufferLength, int minimumBytes)
    {
        if (minimumBytes < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumBytes), Resources.ArgumentOutOfRange_NonNegativeNumberRequired);
        if (bufferLength < minimumBytes)
            throw new ArgumentOutOfRangeException(nameof(minimumBytes), Resources.ArgumentOutOfRange_NotGreaterThanBufferLength);
    }

#endif // !TFF_STREAM_READATLEAST

#if !TFF_STREAM_READATLEAST && (!TFF_MEMORY || TFF_MEMORY_OOB)

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
}

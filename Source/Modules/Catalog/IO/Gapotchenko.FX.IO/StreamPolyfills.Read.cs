// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_STREAM_READ_SPAN
#endif

using System.Buffers;

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
    /// or zero (<c>0</c>) if the buffer's length is zero or the end of the stream has been reached.
    /// </returns>
    public static int Read(
        //this // Not promoted because the polyfill method incurs memory allocations and copying.
        Stream stream,
        Span<byte> buffer)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

#if TFF_STREAM_READ_SPAN
        return stream.Read(buffer);
#else
        int count = buffer.Length;

        var arrayPool = ArrayPool<byte>.Shared;
        byte[] array = arrayPool.Rent(count);
        try
        {
            int bytesRead = stream.Read(array, 0, count);
            if (bytesRead > 0)
                array.AsSpan(0, bytesRead).CopyTo(buffer);
            return bytesRead;
        }
        finally
        {
            arrayPool.Return(array);
        }
#endif
    }
}

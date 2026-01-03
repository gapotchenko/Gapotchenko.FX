// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Buffers;

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
    /// <param name="buffer">
    /// A region of memory.
    /// This method writes the contents of this region to the current stream.
    /// </param>
#if TFF_STREAM_SPAN
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    // TODO: method use should be discouraged because the polyfill implementation incurs memory allocations and copying.
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
}

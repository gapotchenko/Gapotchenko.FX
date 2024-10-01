// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if NET6_0_OR_GREATER
#define TFF_SAFEBUFFER_READSPAN
#endif

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.InteropServices;

/// <summary>
/// Provides polyfills for <see cref="SafeBuffer"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class SafeBufferPolyfills
{
    /// <summary>
    /// Reads bytes from memory starting at the offset, and writes them into a span.
    /// The number of bytes that will be read is determined by the length of the span.
    /// </summary>
    /// <param name="safeBuffer">The safe buffer.</param>
    /// <param name="byteOffset">The location from which to start reading.</param>
    /// <param name="buffer">The output span to write to.</param>
    [CLSCompliant(false)]
#if TFF_SAFEBUFFER_READSPAN
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static unsafe void ReadSpan(
#if !TFF_SAFEBUFFER_READSPAN
        this
#endif
        SafeBuffer safeBuffer,
        ulong byteOffset,
        Span<byte> buffer)
    {
        if (safeBuffer is null)
            throw new ArgumentNullException(nameof(safeBuffer));

#if TFF_SAFEBUFFER_READSPAN
        safeBuffer.ReadSpan(byteOffset, buffer);
#else
        byte* handle = null;
#if !NET5_0_OR_GREATER
        RuntimeHelpers.PrepareConstrainedRegions();
#endif
        try
        {
            safeBuffer.AcquirePointer(ref handle);

            var ptr = handle + byteOffset;
            SpaceCheck(safeBuffer, handle, ptr, checked((nuint)buffer.Length));

            new ReadOnlySpan<byte>(ptr, buffer.Length).CopyTo(buffer);
        }
        finally
        {
            if (handle != null)
                safeBuffer.ReleasePointer();
        }
#endif
    }

#if !TFF_SAFEBUFFER_READSPAN
    static unsafe void SpaceCheck(SafeBuffer safeBuffer, void* handle, byte* ptr, nuint sizeInBytes)
    {
        var _numBytes = safeBuffer.ByteLength;
        if (_numBytes < sizeInBytes)
            throw new ArgumentException("Buffer is too small.");
        if ((ulong)(ptr - (byte*)handle) > (_numBytes - sizeInBytes))
            throw new ArgumentException("Buffer is too small.");
    }
#endif
}

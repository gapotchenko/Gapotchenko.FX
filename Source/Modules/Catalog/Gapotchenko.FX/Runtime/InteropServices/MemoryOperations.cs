using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Runtime.InteropServices;

/// <summary>
/// Provides interoperability operations for memory.
/// </summary>
public static class MemoryOperations
{
    /// <summary>
    /// Copies a block of memory.
    /// </summary>
    /// <param name="source">The source pointer.</param>
    /// <param name="destination">The destination pointer.</param>
    /// <param name="size">The amount of bytes to copy.</param>
    [CLSCompliant(false)]
    public static unsafe void BlockCopy(void* source, void* destination, int size)
    {
#if TFF_UNSAFE
        // This code uses hardware acceleration when possible.
        Unsafe.CopyBlockUnaligned(destination, source, (uint)size);
#else
        var s = (byte*)source;
        var d = (byte*)destination;
        var n = (uint)size;

        int wordSize = IntPtr.Size;

        if (n < wordSize)
        {
            for (uint i = 0; i < n; i++, d++, s++)
                *d = *s;
        }
        else
        {
            if (wordSize == 4)
            {
                // 32-bit architecture.
                uint n4 = n >> 2;
                for (uint i = 0; i < n4; i++, d += 4, s += 4)
                    *(uint*)d = *(uint*)s;
            }
            else
            {
                // 64-bit architecture.
                uint n8 = n >> 3;
                for (uint i = 0; i < n8; i++, d += 8, s += 8)
                    *(ulong*)d = *(ulong*)s;

                if ((n & 4) != 0)
                {
                    *(uint*)d = *(uint*)s;
                    d += 4;
                    s += 4;
                }
            }

            if ((n & 2) != 0)
            {
                *(ushort*)d = *(ushort*)s;
                d += 2;
                s += 2;
            }

            if ((n & 1) != 0)
                *d = *s;
        }
#endif
    }

    /// <summary>
    /// Determines whether specified blocks of memory are equal.
    /// </summary>
    /// <param name="ptr1">A pointer to the first block to compare.</param>
    /// <param name="ptr2">A pointer to the second block to compare.</param>
    /// <param name="size">The amount of bytes to compare.</param>
    /// <returns><see langword="true"/> if the specified blocks are equal; otherwise, <see langword="false"/>.</returns>
    [CLSCompliant(false)]
    public static unsafe bool BlockEquals(void* ptr1, void* ptr2, int size)
    {
#if true
        // This code uses hardware acceleration when possible.
        return
            new ReadOnlySpan<byte>(ptr1, size)
            .SequenceEqual(
                new ReadOnlySpan<byte>(ptr2, size));
#else
        var a = (byte*)ptr1;
        var b = (byte*)ptr2;
        var n = (uint)size;

        int wordSize = IntPtr.Size;

        if (n < wordSize)
        {
            for (uint i = 0; i < n; i++, a++, b++)
            {
                if (*a != *b)
                    return false;
            }
        }
        else
        {
            if (wordSize == 4)
            {
                // 32-bit architecture.
                uint n4 = n >> 2;
                for (uint i = 0; i < n4; i++, a += 4, b += 4)
                {
                    if (*(uint*)a != *(uint*)b)
                        return false;
                }
            }
            else
            {
                // 64-bit architecture.
                uint n8 = n >> 3;
                for (uint i = 0; i < n8; i++, a += 8, b += 8)
                {
                    if (*(ulong*)a != *(ulong*)b)
                        return false;
                }

                if ((n & 4) != 0)
                {
                    if (*(uint*)a != *(uint*)b)
                        return false;
                    a += 4;
                    b += 4;
                }
            }

            if ((n & 2) != 0)
            {
                if (*(ushort*)a != *(ushort*)b)
                    return false;
                a += 2;
                b += 2;
            }

            if ((n & 1) != 0)
            {
                if (*a != *b)
                    return false;
            }
        }

        return true;
#endif
    }
}

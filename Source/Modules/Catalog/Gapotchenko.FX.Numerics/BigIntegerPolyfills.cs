// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
#define TFF_BIGINTEGER_NEWAPI
#endif

using System.Numerics;

namespace Gapotchenko.FX.Numerics;

/// <summary>
/// Provides polyfill extension methods for <see cref="BigInteger"/> structure.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class BigIntegerPolyfills
{
    /// <summary>
    /// Returns the value of the <see cref="BigInteger"/> as a byte array using the fewest number of bytes possible.
    /// If the value is zero, returns an array of one byte whose element is <c>0x00</c>.
    /// </summary>
    /// <param name="value">The <see cref="BigInteger"/> value.</param>
    /// <param name="isUnsigned">
    /// <see langword="true"/> to use unsigned encoding;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="isBigEndian">
    /// <see langword="true"/> to write the bytes in a big-endian byte order;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>The value of the <see cref="BigInteger"/> object converted to an array of bytes.</returns>
    /// <exception cref="OverflowException">
    /// If <paramref name="isUnsigned"/> is <see langword="true"/> and <see cref="BigInteger.Sign"/> is negative.
    /// </exception>
    public static byte[] ToByteArray(
#if !TFF_BIGINTEGER_NEWAPI
        this
#endif
        BigInteger value,
        bool isUnsigned = false,
        bool isBigEndian = false)
    {
#if TFF_BIGINTEGER_NEWAPI
        return value.ToByteArray(isUnsigned, isBigEndian);
#else
        byte[] bytes = value.ToByteArray();
        int length = bytes.Length;

        if (isUnsigned)
        {
            if (value.Sign < 0)
                throw new OverflowException("Negative values do not have an unsigned representation.");

            if (length > 1 && bytes[^1] == 0)
                --length;
        }

        if (isBigEndian)
        {
            byte[] result = new byte[length];
            for (int i = 0; i < length; ++i)
                result[i] = bytes[length - i - 1];
            return result;
        }
        else if (length == bytes.Length)
        {
            return bytes;
        }
        else
        {
            byte[] result = new byte[length];
            Array.Copy(bytes, result, length);
            return result;
        }
#endif
    }
}

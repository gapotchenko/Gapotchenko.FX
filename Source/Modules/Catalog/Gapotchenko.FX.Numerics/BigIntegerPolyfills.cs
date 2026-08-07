// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
#define TFF_BIGINTEGER_API_2_1
#endif

#if NET5_0_OR_GREATER
#define TFF_BIGINTEGER_GETBITLENGTH
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
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </remarks>
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
#if !TFF_BIGINTEGER_API_2_1
        this
#endif
        BigInteger value,
        bool isUnsigned = false,
        bool isBigEndian = false)
    {
#if TFF_BIGINTEGER_API_2_1
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

    /// <summary>
    /// Gets the number of bytes that will be output by <see cref="ToByteArray(BigInteger, bool, bool)"/>.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </remarks>
    /// <param name="value">The <see cref="BigInteger"/> value.</param>
    /// <param name="isUnsigned">
    /// <see langword="true"/> to use unsigned encoding;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>The number of bytes.</returns>
    /// <exception cref="OverflowException">
    /// If <paramref name="isUnsigned"/> is <see langword="true"/> and <see cref="BigInteger.Sign"/> is negative.
    /// </exception>
    public static int GetByteCount(
#if !TFF_BIGINTEGER_API_2_1
        this
#endif
        BigInteger value,
        bool isUnsigned = false)
    {
#if TFF_BIGINTEGER_API_2_1
        return value.GetByteCount(isUnsigned);
#else
        byte[] bytes = value.ToByteArray();
        int count = bytes.Length;
        if (isUnsigned)
        {
            if (value.Sign < 0)
                throw new OverflowException("Negative values do not have an unsigned representation.");

            if (count > 1 && bytes[^1] == 0)
                --count;
        }
        return count;
#endif
    }

    /// <summary>
    /// Gets the number of bits required for shortest two's complement representation of the current instance without the sign bit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// <para>
    /// This method returns <c>0</c> if the value is equal to <see cref="BigInteger.Zero"/> or <see cref="BigInteger.MinusOne"/>.
    /// For positive integers the return value is equal to the ordinary binary representation string length.
    /// </para>
    /// </remarks>
    /// <param name="value">The <see cref="BigInteger"/> value.</param>
    /// <returns>The minimum non-negative number of bits in two's complement notation without the sign bit.</returns>
    public static long GetBitLength(
#if !TFF_BIGINTEGER_GETBITLENGTH
        this
#endif
        BigInteger value)
    {
#if TFF_BIGINTEGER_GETBITLENGTH
        return value.GetBitLength();
#else
        byte[] bytes = value.ToByteArray();
        var (length, zero) = GetSignificantLength(value.Sign, bytes);

        --length;

        byte mostSignificantByte = (byte)(bytes[length] ^ zero);
        if (mostSignificantByte == 0)
            return 0;

        return length * 8L + BitOperations.Log2(mostSignificantByte) + 1;

        static (int Length, byte Zero) GetSignificantLength(int sign, byte[] bytes)
        {
            int length = bytes.Length;
            byte zero = sign < 0 ? (byte)0xff : (byte)0x00;
            while (length > 1 && bytes[length - 1] == zero)
                --length;
            return (length, zero);
        }
#endif
    }
}

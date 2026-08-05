using System.Numerics;

namespace Gapotchenko.FX.Security.Cryptography.Utils;

static class BigIntegerUtil
{
    public static BigInteger FromBytes(ReadOnlySpan<byte> bytes, bool isUnsigned = false, bool isBigEndian = false)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        return new BigInteger(bytes, isUnsigned, isBigEndian);
#else
        if (isBigEndian)
        {
            int extraLength = isUnsigned ? 1 : 0;
            byte[] littleEndian = new byte[bytes.Length + extraLength];
            for (int i = 0; i < bytes.Length; ++i)
                littleEndian[i] = bytes[bytes.Length - i - 1];
            return new BigInteger(littleEndian);
        }
        else
        {
            return new BigInteger(isUnsigned ? [.. bytes, 0] : [.. bytes]);
        }
#endif
    }

    public static byte[] ToBytes(in BigInteger value, bool isUnsigned = false, bool isBigEndian = false)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
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

    public static int GetByteCount(in BigInteger value, bool isUnsigned = false)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
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

    public static long GetBitLength(in BigInteger value)
    {
#if NET5_0_OR_GREATER
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

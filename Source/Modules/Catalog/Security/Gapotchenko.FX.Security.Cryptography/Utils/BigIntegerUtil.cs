using System.Numerics;

namespace Gapotchenko.FX.Security.Cryptography.Utils;

static class BigIntegerUtil
{
    public static long GetBitLength(in BigInteger value)
    {
#if NET5_0_OR_GREATER
        return value.GetBitLength();
#else
        byte[] bytes = value.ToByteArray();
        int length = GetSignificantLength(bytes);
        if (length == 0)
            return 0;
        byte mostSignificantByte = bytes[length - 1];
        return (length - 1) * 8 + BitOperations.Log2(mostSignificantByte);
#endif
    }

    static int GetSignificantLength(byte[] bytes)
    {
        int length = bytes.Length;
        while (length > 1 && bytes[length - 1] == 0)
            --length;
        return length;
    }

    //static byte[] ToBytesLE(BigInteger value)
    //{
    //    byte[] bytes = value.ToByteArray();
    //    int length = bytes.Length;
    //    while (length > 1 && bytes[length - 1] == 0)
    //        --length;

    //    if (length == bytes.Length)
    //        return bytes;
    //    else
    //        return bytes[..length];
    //}
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

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
}

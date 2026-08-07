// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Numerics;
using System.Numerics;

namespace Gapotchenko.FX.Security.Cryptography.Utils;

static class PrimeUtil
{
    public static BigInteger GeneratePrime(int bits)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(bits, 2);

        int byteCount = (bits + 7) / 8;
        int excessBits = byteCount * 8 - bits;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        Span<byte> bytes = stackalloc byte[byteCount];
#else
        byte[] bytes = new byte[byteCount + 1];
#endif

        for (; ; )
        {
            RandomNumberGenerator.Fill(bytes);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            ref byte last = ref bytes[^1];
#else
            bytes[^1] = 0; // unsigned
            ref byte last = ref bytes[^2];
#endif
            last = (byte)((last & (0xff >>> excessBits)) | (0x80 >>> excessBits));
            bytes[0] |= 1;

            BigInteger candidate;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            candidate = new BigInteger(bytes, isUnsigned: true);
#else
            candidate = new BigInteger(bytes);
#endif

            if (IsProbablePrime(candidate))
                return candidate;
        }
    }

    static bool IsProbablePrime(BigInteger value)
    {
        if (value < 2)
            return false;

        foreach (int prime in m_SmallPrimes)
        {
            if (value == prime)
                return true;
            if (value % prime == 0)
                return false;
        }

        var d = value - 1;
        int s = 0;
        while (d.IsEven)
        {
            d >>= 1;
            ++s;
        }

        int byteCount = value.GetByteCount(true);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        Span<byte> witnessBytes = stackalloc byte[byteCount];
#else
        byte[] witnessBytes = new byte[byteCount + 1];
#endif

        for (int i = 0; i < 64; ++i)
        {
            BigInteger witness;

            RandomNumberGenerator.Fill(witnessBytes);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            witness = new BigInteger(witnessBytes, isUnsigned: true);
#else
            witnessBytes[^1] = 0;
            witness = new BigInteger(witnessBytes);
#endif

            var a = witness % (value - 3) + 2;
            var x = BigInteger.ModPow(a, d, value);
            if (x == 1 || x == value - 1)
                continue;

            bool nextWitness = false;
            for (int r = 1; r < s; ++r)
            {
                x = BigInteger.ModPow(x, 2, value);
                if (x == value - 1)
                {
                    nextWitness = true;
                    break;
                }
            }

            if (!nextWitness)
                return false;
        }

        return true;
    }

    static readonly int[] m_SmallPrimes =
    [
        3, 5, 7, 11, 13, 17, 19, 23, 29, 31,
        37, 41, 43, 47, 53, 59, 61, 67, 71, 73,
        79, 83, 89, 97, 101, 103, 107, 109, 113, 127
    ];
}

using System.Numerics;

namespace Gapotchenko.FX.Security.Cryptography.Utils;

static class PrimeUtil
{
    public static BigInteger GeneratePrime(int bits)
    {
        int byteCount = (bits + 7) / 8;
        int excessBits = byteCount * 8 - bits;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        Span<byte> bytes = stackalloc byte[byteCount];
#else
        byte[] bytes = new byte[byteCount];
#endif

        for (; ; )
        {
            RandomNumberGenerator.Fill(bytes);
            bytes[^1] &= (byte)(0xff >>> excessBits);
            bytes[^1] |= (byte)(0x80 >>> excessBits);
            bytes[0] |= 1;

            var candidate = new BigInteger(bytes);
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

        int byteCount;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        byteCount = value.GetByteCount();
#else
        byteCount = value.ToByteArray().Length;
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
        Span<byte> witnessBytes = stackalloc byte[byteCount];
#else
        byte[] witnessBytes = new byte[byteCount];
#endif

        for (int i = 0; i < 64; ++i)
        {
            RandomNumberGenerator.Fill(witnessBytes);
            var a = new BigInteger(witnessBytes) % (value - 3) + 2;
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

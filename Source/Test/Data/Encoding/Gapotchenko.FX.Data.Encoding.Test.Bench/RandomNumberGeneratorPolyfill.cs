using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Encoding.Tests.Bench
{
    static class RandomNumberGeneratorPolyfill
    {

#if NETFRAMEWORK

        static RandomNumberGenerator m_Rng = RandomNumberGenerator.Create();

        public static void Fill(Span<byte> data)
        {
            byte[] array = ArrayPool<byte>.Shared.Rent(data.Length);
            try
            {
                lock (m_Rng)
                    m_Rng.GetBytes(array);
                new ReadOnlySpan<byte>(array, 0, data.Length).CopyTo(data);
            }
            finally
            {
                Array.Clear(array, 0, data.Length);
                ArrayPool<byte>.Shared.Return(array);
            }
        }

#else

        public static void Fill(Span<byte> data) => RandomNumberGenerator.Fill(data);

#endif

        public static int GetInt32(int fromInclusive, int toExclusive)
        {
#if NETFRAMEWORK
            if (fromInclusive >= toExclusive)
                throw new ArgumentException("Invalid random range.");

            // The total possible range is [0, 4,294,967,295).
            // Subtract one to account for zero being an actual possibility.
            uint range = (uint)toExclusive - (uint)fromInclusive - 1;

            // If there is only one possible choice, nothing random will actually happen, so return
            // the only possibility.
            if (range == 0)
            {
                return fromInclusive;
            }

            // Create a mask for the bits that we care about for the range. The other bits will be
            // masked away.
            uint mask = range;
            mask |= mask >> 1;
            mask |= mask >> 2;
            mask |= mask >> 4;
            mask |= mask >> 8;
            mask |= mask >> 16;

            Span<uint> resultSpan = stackalloc uint[1];
            uint result;

            do
            {
                Fill(MemoryMarshal.AsBytes(resultSpan));
                result = mask & resultSpan[0];
            }
            while (result > range);

            return (int)result + fromInclusive;
#else
            return RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);
#endif
        }

        public static int GetInt32(int toExclusive)
        {
#if NETFRAMEWORK
            if (toExclusive <= 0)
                throw new ArgumentOutOfRangeException(nameof(toExclusive));
            return GetInt32(0, toExclusive);
#else
            return RandomNumberGenerator.GetInt32(toExclusive);
#endif
        }
    }
}

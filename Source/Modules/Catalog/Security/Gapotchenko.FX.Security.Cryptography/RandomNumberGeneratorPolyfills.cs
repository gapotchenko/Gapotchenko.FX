// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#region Target framework features

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_RANDOMNUMBERGENERATOR_FILL
#define TFF_RANDOMNUMBERGENERATOR_GETBYTES_SPAN
#endif

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_RANDOMNUMBERGENERATOR_GETINT32
#endif

#if NET6_0_OR_GREATER
#define TFF_RANDOMNUMBERGENERATOR_GETBYTES_INT32
#endif

#endregion

using System.Buffers;
using System.Buffers.Binary;

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides polyfill extensions for <see cref="RandomNumberGenerator"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class RandomNumberGeneratorPolyfills
{
    /// <summary>
    /// Provides static member polyfills for <see cref="RandomNumberGenerator"/> class.
    /// </summary>
    extension(RandomNumberGenerator)
    {
        /// <summary>
        /// Generates a random integer between 0 (inclusive) and a specified exclusive upper bound
        /// using a cryptographically strong random number generator.
        /// </summary>
        /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
        /// <returns>A random integer between 0 (inclusive) and toExclusive (exclusive).</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="toExclusive"/> parameter is less than or equal to 0.
        /// </exception>
#if TFF_RANDOMNUMBERGENERATOR_GETINT32
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static int GetInt32(int toExclusive)
        {
#if TFF_RANDOMNUMBERGENERATOR_GETINT32
            return RandomNumberGenerator.GetInt32(toExclusive);
#else
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(toExclusive);

            return GetInt32(0, toExclusive);
#endif
        }

        /// <summary>
        /// Generates a random integer between a specified inclusive lower bound and a specified exclusive upper bound
        /// using a cryptographically strong random number generator.
        /// </summary>
        /// <param name="fromInclusive">The inclusive lower bound of the random range.</param>
        /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
        /// <returns>A random integer between <paramref name="fromInclusive"/> (inclusive) and <paramref name="toExclusive"/> (exclusive).</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="toExclusive"/> parameter is less than or equal to the <paramref name="fromInclusive"/> parameter.
        /// </exception>
#if TFF_RANDOMNUMBERGENERATOR_GETINT32
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static int GetInt32(int fromInclusive, int toExclusive)
        {
#if TFF_RANDOMNUMBERGENERATOR_GETINT32
            return RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);
#else
            if (fromInclusive >= toExclusive)
                throw new ArgumentOutOfRangeException("Invalid random range.");

            // The total possible range is [0, 4,294,967,295).
            // Subtract one to account for zero being an actual possibility.
            uint range = (uint)toExclusive - (uint)fromInclusive - 1;

            // If there is only one possible choice, nothing random will actually happen, so return
            // the only possibility.
            if (range == 0)
                return fromInclusive;

            // Create a mask for the bits that we care about for the range. The other bits will be
            // masked away.
            uint mask = range;
            mask |= mask >> 1;
            mask |= mask >> 2;
            mask |= mask >> 4;
            mask |= mask >> 8;
            mask |= mask >> 16;

            uint result;

            var rng = UseRng(out bool disposable);
            var arrayPool = ArrayPool<byte>.Shared;
            byte[] oneUintBytes = arrayPool.Rent(4);
            try
            {
                do
                {
                    rng.GetBytes(oneUintBytes);
                    uint oneUint = BinaryPrimitives.ReadUInt32LittleEndian(oneUintBytes);
                    result = mask & oneUint;
                }
                while (result > range);
            }
            finally
            {
                arrayPool.Return(oneUintBytes);
                if (disposable)
                    rng.Dispose();
            }

            return (int)result + fromInclusive;
#endif
        }

        /// <summary>
        /// Creates an array of bytes with a cryptographically strong random sequence of values.
        /// </summary>
        /// <param name="count">The number of bytes of random values to create.</param>
        /// <returns>An array populated with cryptographically strong random values.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than zero.</exception>
#if TFF_RANDOMNUMBERGENERATOR_GETBYTES_INT32
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static byte[] GetBytes(int count)
        {
#if TFF_RANDOMNUMBERGENERATOR_GETBYTES_INT32
            return RandomNumberGenerator.GetBytes(count);
#else
            ArgumentOutOfRangeException.ThrowIfNegative(count);

            byte[] data = new byte[count];

            var rng = UseRng(out bool disposable);
            rng.GetBytes(data);
            if (disposable)
                rng.Dispose();

            return data;
#endif
        }

        /// <summary>
        /// Fills a span with cryptographically strong random bytes.
        /// </summary>
        /// <param name="data">The span to fill with cryptographically strong random bytes.</param>
#if TFF_RANDOMNUMBERGENERATOR_FILL
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static void Fill(Span<byte> data)
        {
#if TFF_RANDOMNUMBERGENERATOR_FILL
            RandomNumberGenerator.Fill(data);
#else
            var rng = UseRng(out bool disposable);
            rng.GetBytes(data);
            if (disposable)
                rng.Dispose();
#endif
        }
    }

#if !TFF_RANDOMNUMBERGENERATOR_FILL || !TFF_RANDOMNUMBERGENERATOR_GETBYTES_INT32

    static RandomNumberGenerator UseRng(out bool disposable)
    {
        if (m_SharedRngCsp is { } sharedRng)
        {
            // Reuse the existing shared RNG.
            disposable = false;
            return sharedRng;
        }
        else
        {
            var rng = RandomNumberGenerator.Create();
            if (rng is RNGCryptoServiceProvider rngCsp)
            {
                // RNG is thread-safe and thus can be shared.

                // Keep it around for later use.
                m_SharedRngCsp = rngCsp;

                // Do not dispose it now to be reused later.
                disposable = false;
                return rngCsp;
            }
            else
            {
                // RNG does not have thread-safety guarantees.
                // It should be used only once.
                disposable = true;
                return rng;
            }
        }
    }

    /// <summary>
    /// A shared <see cref="RNGCryptoServiceProvider"/> instance which is guaranteed to be thread-safe.
    /// </summary>
    static RNGCryptoServiceProvider? m_SharedRngCsp;

#endif

    /// <summary>
    /// Fills a span with cryptographically strong random bytes.
    /// </summary>
    /// <param name="rng">The <see cref="RandomNumberGenerator"/> instance.</param>
    /// <param name="data">The span to fill with cryptographically strong random bytes.</param>
    public static void GetBytes(
#if !TFF_RANDOMNUMBERGENERATOR_GETBYTES_SPAN
        this
#endif
        RandomNumberGenerator rng,
        Span<byte> data)
    {
        ArgumentNullException.ThrowIfNull(rng);

#if TFF_RANDOMNUMBERGENERATOR_GETBYTES_SPAN
        rng.GetBytes(data);
#else
        int n = data.Length;

        var arrayPool = ArrayPool<byte>.Shared;
        byte[] buffer = arrayPool.Rent(n);
        try
        {
            rng.GetBytes(buffer, 0, n);
            buffer.AsSpan(0, n).CopyTo(data);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(buffer);
            arrayPool.Return(buffer);
        }
#endif
    }
}

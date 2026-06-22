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

#if NET6_0_OR_GREATER
#define TFF_RANDOMNUMBERGENERATOR_GETBYTES_INT32
#endif

#endregion

using System.Buffers;

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

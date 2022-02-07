using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Provides extension methods for <see cref="IChecksumAlgorithm{T}"/>.
    /// </summary>
    public static class ChecksumAlgorithmExtensions
    {
        const int BufferSize = 4096;

        /// <summary>
        /// Computes the checksum for the specified <see cref="Stream"/> object.
        /// </summary>
        /// <param name="algorithm">The algorithm to use for checksum computation.</param>
        /// <param name="inputStream">The input stream to compute the checksum for.</param>
        /// <returns>The computed checksum.</returns>
        /// <exception cref="ArgumentNullException">The argument is <see langword="null"/>.</exception>
        public static T ComputeChecksum<T>(this IChecksumAlgorithm<T> algorithm, Stream inputStream)
            where T : struct
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            var iterator = algorithm.CreateIterator();

            var buffer = new byte[BufferSize];
            for (; ; )
            {
                int bytesRead = inputStream.Read(buffer, 0, BufferSize);
                if (bytesRead <= 0)
                    break;
                iterator.ComputeBlock(buffer.AsSpan(0, bytesRead));
            }

            return iterator.ComputeFinal();
        }

        /// <summary>
        /// Asynchronously computes the checksum for the specified <see cref="Stream"/> object.
        /// </summary>
        /// <param name="algorithm">The algorithm to use for checksum computation.</param>
        /// <param name="inputStream">The input stream to compute the checksum for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The computed checksum.</returns>
        /// <exception cref="ArgumentNullException">The argument is <see langword="null"/>.</exception>
        public static async Task<T> ComputeChecksumAsync<T>(this IChecksumAlgorithm<T> algorithm, Stream inputStream, CancellationToken cancellationToken = default)
            where T : struct
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            var iterator = algorithm.CreateIterator();

            var buffer = new byte[BufferSize];
            for (; ; )
            {
                int bytesRead = await
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    inputStream.ReadAsync(buffer, cancellationToken)
#else
                    inputStream.ReadAsync(buffer, 0, BufferSize, cancellationToken)
#endif
                    .ConfigureAwait(false);
                if (bytesRead <= 0)
                    break;
                iterator.ComputeBlock(buffer.AsSpan(0, bytesRead));
            }

            return iterator.ComputeFinal();
        }
    }
}

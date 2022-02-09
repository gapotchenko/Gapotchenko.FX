using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Defines the interface of a generic checksum algorithm.
    /// </summary>
    /// <typeparam name="T">The type of the checksum value.</typeparam>
    public interface IChecksumAlgorithm<T> : IChecksumAlgorithm
        where T : struct
    {
        /// <summary>
        /// Computes the checksum for the specified byte span.
        /// </summary>
        /// <param name="data">The input to compute the checksum for.</param>
        /// <returns>The computed checksum.</returns>
        T ComputeChecksum(ReadOnlySpan<byte> data);

        /// <summary>
        /// Computes the checksum for the specified <see cref="Stream"/> object.
        /// </summary>
        /// <param name="inputStream">The input stream to compute the checksum for.</param>
        /// <returns>The computed checksum.</returns>
        /// <exception cref="ArgumentNullException">The argument is <see langword="null"/>.</exception>
        T ComputeChecksum(Stream inputStream);

        /// <summary>
        /// Asynchronously computes the checksum for the specified <see cref="Stream"/> object.
        /// </summary>
        /// <param name="inputStream">The input stream to compute the checksum for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The computed checksum.</returns>
        /// <exception cref="ArgumentNullException">The argument is <see langword="null"/>.</exception>
        Task<T> ComputeChecksumAsync(Stream inputStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates an iterator for checksum computation.
        /// </summary>
        /// <returns>An iterator for checksum computation.</returns>
        IChecksumIterator<T> CreateIterator();
    }
}

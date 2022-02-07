using System;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Defines the interface of a checksum algorithm.
    /// </summary>
    /// <typeparam name="T">The type of the checksum value.</typeparam>
    public interface IChecksumAlgorithm<T>
        where T : struct
    {
        /// <summary>
        /// Gets the size, in bits, of the computed checksum value.
        /// </summary>
        int ChecksumSize { get; }

        /// <summary>
        /// Computes the checksum for the specified byte span.
        /// </summary>
        /// <param name="data">The input to compute the checksum for.</param>
        /// <returns>The computed checksum.</returns>
        T ComputeChecksum(ReadOnlySpan<byte> data);

        /// <summary>
        /// Creates an iterator for checksum computation.
        /// </summary>
        /// <returns>An iterator for checksum computation.</returns>
        IChecksumIterator<T> CreateIterator();
    }
}

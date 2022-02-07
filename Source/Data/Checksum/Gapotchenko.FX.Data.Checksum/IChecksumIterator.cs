using System;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Represents an iterator for checksum computation.
    /// </summary>
    /// <typeparam name="T">The type of the checksum value.</typeparam>
    public interface IChecksumIterator<T>
    {
        /// <summary>
        /// Computes the checksum for the specified byte span.
        /// </summary>
        /// <param name="data">The input to compute the checksum for.</param>
        void ComputeBlock(ReadOnlySpan<byte> data);

        /// <summary>
        /// Finalizes the checksum computation after the last data is processed.
        /// </summary>
        /// <returns>The computed checksum.</returns>
        T ComputeFinal();

        /// <summary>
        /// Resets the iterator to its initial state.
        /// </summary>
        void Reset();
    }
}

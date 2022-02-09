using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Defines the interface of a checksum algorithm.
    /// </summary>
    public interface IChecksumAlgorithm
    {
        /// <summary>
        /// Gets the size, in bits, of the computed checksum value.
        /// </summary>
        int ChecksumSize { get; }

        /// <summary>
        /// Creates a hash algorithm for checksum computation.
        /// </summary>
        /// <returns>A hash algorithm for checksum computation.</returns>
        HashAlgorithm CreateHashAlgorithm();
    }
}

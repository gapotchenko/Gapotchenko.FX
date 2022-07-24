using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Represents a unifying interface for all CRC-32 checksum algorithm implementations.
/// </summary>
[CLSCompliant(false)]
public interface ICrc32 : IChecksumAlgorithm<uint>
{
    /// <summary>
    /// Creates a hash algorithm for checksum computation with the specified bit converter.
    /// </summary>
    /// <param name="bitConverter">The bit converter to use for conversion of the computed checksum to a hash.</param>
    /// <returns>A hash algorithm for checksum computation.</returns>
    HashAlgorithm CreateHashAlgorithm(IBitConverter bitConverter);
}

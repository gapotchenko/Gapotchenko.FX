using System;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Represents a unifying interface for all CRC-16 checksum algorithm implementations.
    /// </summary>
    [CLSCompliant(false)]
    public interface ICrc16 : IChecksumAlgorithm<ushort>
    {
    }
}

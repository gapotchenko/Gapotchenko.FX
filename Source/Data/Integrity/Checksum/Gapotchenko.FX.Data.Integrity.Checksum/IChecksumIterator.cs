using System;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Represents an iterator for generic checksum computation.
/// </summary>
public interface IChecksumIterator
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
    object ComputeFinal();

    /// <summary>
    /// Resets the iterator to its initial state.
    /// </summary>
    void Reset();
}

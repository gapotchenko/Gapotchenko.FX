namespace Gapotchenko.FX.Data.Integrity.Checksum;

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
    /// <param name="data">The input data to compute the checksum for.</param>
    /// <returns>The computed checksum.</returns>
    new T ComputeChecksum(ReadOnlySpan<byte> data);

    /// <summary>
    /// Computes the checksum for the specified <see cref="Stream"/> object.
    /// </summary>
    /// <param name="stream">The input stream to compute the checksum for.</param>
    /// <returns>The computed checksum.</returns>
    /// <exception cref="ArgumentNullException">The argument is <see langword="null"/>.</exception>
    new T ComputeChecksum(Stream stream);

    /// <summary>
    /// Asynchronously computes the checksum for the specified <see cref="Stream"/> object.
    /// </summary>
    /// <param name="stream">The input stream to compute the checksum for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The computed checksum.</returns>
    /// <exception cref="ArgumentNullException">The argument is <see langword="null"/>.</exception>
    new Task<T> ComputeChecksumAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an iterator for checksum computation.
    /// </summary>
    /// <returns>An iterator for checksum computation.</returns>
    new IChecksumIterator<T> CreateIterator();
}

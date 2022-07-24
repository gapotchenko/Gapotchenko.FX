using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// The base class for <see cref="IChecksumAlgorithm{T}"/> implementations.
/// </summary>
public abstract partial class ChecksumAlgorithm<T> : IChecksumAlgorithm<T>
    where T : struct
{
    /// <inheritdoc/>
    public abstract int ChecksumSize { get; }

    /// <inheritdoc/>
    public virtual T ComputeChecksum(ReadOnlySpan<byte> data)
    {
        var iterator = CreateIterator();
        iterator.ComputeBlock(data);
        return iterator.ComputeFinal();
    }

    object IChecksumAlgorithm.ComputeChecksum(ReadOnlySpan<byte> data) => ComputeChecksum(data);

    const int BufferSize = 4096;

    /// <inheritdoc/>
    public virtual T ComputeChecksum(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var iterator = CreateIterator();

        var buffer = new byte[BufferSize];
        for (; ; )
        {
            int bytesRead = stream.Read(buffer, 0, BufferSize);
            if (bytesRead <= 0)
                break;
            iterator.ComputeBlock(buffer.AsSpan(0, bytesRead));
        }

        return iterator.ComputeFinal();
    }

    object IChecksumAlgorithm.ComputeChecksum(Stream stream) => ComputeChecksum(stream);

    /// <inheritdoc/>
    public virtual async Task<T> ComputeChecksumAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var iterator = CreateIterator();

        var buffer = new byte[BufferSize];
        for (; ; )
        {
            int bytesRead = await
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                stream.ReadAsync(buffer, cancellationToken)
#else
                stream.ReadAsync(buffer, 0, BufferSize, cancellationToken)
#endif
                .ConfigureAwait(false);
            if (bytesRead <= 0)
                break;
            iterator.ComputeBlock(buffer.AsSpan(0, bytesRead));
        }

        return iterator.ComputeFinal();
    }

    async Task<object> IChecksumAlgorithm.ComputeChecksumAsync(Stream stream, CancellationToken cancellationToken) =>
        await ComputeChecksumAsync(stream, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc/>
    public IChecksumIterator<T> CreateIterator() => CreateIteratorCore();

    IChecksumIterator IChecksumAlgorithm.CreateIterator() => CreateIterator();

    /// <summary>
    /// Creates an iterator for checksum computation.
    /// </summary>
    /// <returns>An iterator for checksum computation.</returns>
    protected abstract IChecksumIterator<T> CreateIteratorCore();

    /// <inheritdoc/>
    public HashAlgorithm CreateHashAlgorithm() => CreateHashAlgorithmCore(null);

    /// <summary>
    /// Creates a hash algorithm for checksum computation with the specified bit converter.
    /// </summary>
    /// <param name="bitConverter">
    /// The bit converter to use for conversion of the computed checksum value to a hash byte representation
    /// or <see langword="null"/> to use the default bit converter.
    /// </param>
    /// <returns>A hash algorithm for checksum computation.</returns>
    protected virtual HashAlgorithm CreateHashAlgorithmCore(IBitConverter? bitConverter) =>
        new HashAlgorithmImpl(
            this,
            bitConverter ?? LittleEndianBitConverter.Instance);

    /// <summary>
    /// Gets a hash byte representation of the specified checksum value.
    /// </summary>
    /// <param name="checksum">The checksum.</param>
    /// <param name="bitConverter">The bit converter.</param>
    /// <returns>The hash bytes.</returns>
    protected abstract byte[] GetHashBytesCore(T checksum, IBitConverter bitConverter);
}

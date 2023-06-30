// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Defines the interface of a generic checksum algorithm.
/// </summary>
/// <typeparam name="T">The type of the checksum value.</typeparam>
public interface IChecksumAlgorithm<T> : IChecksumAlgorithm
    where T : struct
{
    /// <inheritdoc cref="IChecksumAlgorithm.ComputeChecksum(ReadOnlySpan{byte})"/>
    new T ComputeChecksum(ReadOnlySpan<byte> data);

    /// <inheritdoc cref="IChecksumAlgorithm.ComputeChecksum(Stream)"/>
    new T ComputeChecksum(Stream stream);

    /// <inheritdoc cref="IChecksumAlgorithm.ComputeChecksumAsync(Stream, CancellationToken)"/>
    new Task<T> ComputeChecksumAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IChecksumAlgorithm.CreateIterator"/>
    new IChecksumIterator<T> CreateIterator();
}

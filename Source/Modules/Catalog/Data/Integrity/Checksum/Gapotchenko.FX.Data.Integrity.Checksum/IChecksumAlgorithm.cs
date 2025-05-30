﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

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
    /// Computes the checksum for the specified byte span.
    /// </summary>
    /// <param name="data">The input data to compute the checksum for.</param>
    /// <returns>The computed checksum.</returns>
    object ComputeChecksum(ReadOnlySpan<byte> data);

    /// <summary>
    /// Computes the checksum for the specified <see cref="Stream"/> object.
    /// </summary>
    /// <param name="stream">The input stream to compute the checksum for.</param>
    /// <returns>The computed checksum.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    object ComputeChecksum(Stream stream);

    /// <summary>
    /// Asynchronously computes the checksum for the specified <see cref="Stream"/> object.
    /// </summary>
    /// <param name="stream">The input stream to compute the checksum for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The computed checksum.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    Task<object> ComputeChecksumAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an iterator for a checksum computation.
    /// </summary>
    /// <returns>An iterator for a checksum computation.</returns>
    IChecksumIterator CreateIterator();

    /// <summary>
    /// Creates a hash algorithm for a checksum computation.
    /// </summary>
    /// <returns>A hash algorithm for a checksum computation.</returns>
    HashAlgorithm CreateHashAlgorithm();
}

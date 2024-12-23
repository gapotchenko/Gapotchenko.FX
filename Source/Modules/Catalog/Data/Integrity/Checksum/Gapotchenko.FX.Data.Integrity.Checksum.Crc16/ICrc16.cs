﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Represents a unifying interface for all CRC-16 checksum algorithm implementations.
/// </summary>
[CLSCompliant(false)]
public interface ICrc16 : IChecksumAlgorithm<ushort>
{
    /// <summary>
    /// Creates a hash algorithm for checksum computation with the specified bit converter.
    /// </summary>
    /// <param name="bitConverter">The bit converter to use for conversion of the computed checksum to a hash.</param>
    /// <returns>A hash algorithm for checksum computation.</returns>
    HashAlgorithm CreateHashAlgorithm(IBitConverter bitConverter);
}

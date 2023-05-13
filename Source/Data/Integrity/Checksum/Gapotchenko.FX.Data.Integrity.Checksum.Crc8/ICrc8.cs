// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Represents a unifying interface for all CRC-8 checksum algorithm implementations.
/// </summary>
public interface ICrc8 : IChecksumAlgorithm<byte>
{
}


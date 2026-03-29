// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;

/// <summary>
/// MS-CFB format constants.
/// </summary>
static class CfbConstants
{
    /// <summary>
    /// The size of the compound file header, in bytes.
    /// The header always occupies exactly 512 bytes regardless of the sector size.
    /// </summary>
    public const int HeaderSize = 512;

    /// <summary>
    /// The size of a directory entry, in bytes.
    /// </summary>
    public const int DirectoryEntrySize = 128;

    /// <summary>
    /// Maximum size of a stream stored in the mini-stream.
    /// Streams smaller than this threshold use the mini-FAT chain.
    /// </summary>
    public const int MiniStreamCutOffSize = 4096;

    /// <summary>
    /// Size of a mini-sector, in bytes.
    /// </summary>
    public const int MiniSectorSize = 64;

    /// <summary>
    /// The maximum length of a directory entry name including the null terminator, in characters (UTF-16).
    /// </summary>
    public const int MaxNameLength = 32;

    /// <summary>
    /// Number of DIFAT entries stored directly in the header.
    /// </summary>
    public const int HeaderDifatCount = 109;

    /// <summary>
    /// Compound file signature bytes.
    /// </summary>
    public static ReadOnlySpan<byte> Signature => [0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1];

    // Special sector IDs.
    public const uint FreeSectorId = 0xFFFFFFFF;
    public const uint EndOfChainSectorId = 0xFFFFFFFE;
    public const uint FatSectorId = 0xFFFFFFFD;
    public const uint DifatSectorId = 0xFFFFFFFC;

    // Special directory entry IDs.
    public const uint NoStream = 0xFFFFFFFF;

    // Root directory entry ID.
    public const uint RootEntryId = 0;
}

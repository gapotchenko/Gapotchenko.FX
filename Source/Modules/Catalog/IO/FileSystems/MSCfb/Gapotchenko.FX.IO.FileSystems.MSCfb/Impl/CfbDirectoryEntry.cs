// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;

/// <summary>
/// Specifies the type of an MS-CFB directory entry.
/// </summary>
enum CfbEntryType : byte
{
    Unallocated = 0,
    Storage = 1,
    Stream = 2,
    Root = 5,
}

/// <summary>
/// Represents a directory entry (either a file or a subdirectory) in an MS-CFB compound file.
/// </summary>
sealed class CfbDirectoryEntry
{
    /// <summary>
    /// The index of this entry within the directory entry array.
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    /// Entry name (case-sensitive stored, case-insensitive compared).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of this directory entry.
    /// </summary>
    public CfbEntryType Type { get; set; }

    /// <summary>
    /// Red-black tree color (0 = red, 1 = black).
    /// </summary>
    public byte Color { get; set; } = 1;

    /// <summary>
    /// Directory entry ID of the left sibling in the parent's sibling tree, or <see cref="CfbConstants.NoStream"/>.
    /// </summary>
    public uint LeftSiblingId { get; set; } = CfbConstants.NoStream;

    /// <summary>
    /// Directory entry ID of the right sibling in the parent's sibling tree, or <see cref="CfbConstants.NoStream"/>.
    /// </summary>
    public uint RightSiblingId { get; set; } = CfbConstants.NoStream;

    /// <summary>
    /// Directory entry ID of the first child (root of child sibling tree) for Storage/Root entries,
    /// or <see cref="CfbConstants.NoStream"/> for Stream entries or empty storages.
    /// </summary>
    public uint ChildId { get; set; } = CfbConstants.NoStream;

    /// <summary>
    /// Class identifier (CLSID) of the storage.
    /// </summary>
    public Guid Clsid { get; set; }

    /// <summary>
    /// User-defined flags.
    /// </summary>
    public uint StateBits { get; set; }

    /// <summary>
    /// Creation timestamp (UTC).
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// Modification timestamp (UTC).
    /// </summary>
    public DateTime ModificationTime { get; set; }

    /// <summary>
    /// Starting sector ID for the entry's data (regular FAT chain or mini-FAT chain depending on size).
    /// </summary>
    public uint StartSectorId { get; set; } = CfbConstants.EndOfChainSectorId;

    /// <summary>
    /// Size of the stream data in bytes.
    /// For Storage entries, this is always 0 (except the Root which holds the mini-stream).
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Pending write data. Non-null means this stream has been modified in memory and needs to be flushed.
    /// </summary>
    internal byte[]? PendingData { get; set; }

    /// <summary>
    /// Compares two entry names according to the MS-CFB specification:
    /// first by length (shorter names are "less"), then character-by-character using ToUpperInvariant.
    /// </summary>
    public static int CompareName(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        int lengthDiff = x.Length - y.Length;
        if (lengthDiff != 0)
            return lengthDiff;

        for (int i = 0; i < x.Length; i++)
        {
            int diff = char.ToUpperInvariant(x[i]) - char.ToUpperInvariant(y[i]);
            if (diff != 0)
                return diff;
        }

        return 0;
    }
}

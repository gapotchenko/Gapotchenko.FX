// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;

/// <summary>
/// The main context that parses and provides access to an MS-CFB compound file.
/// </summary>
sealed class CfbContext : IDisposable
{
    readonly Stream m_Stream;
    readonly bool m_LeaveOpen;
    bool m_Writable;
    bool m_Dirty;

    uint[] m_Fat = [];
    uint[] m_MiniFat = [];
    List<CfbEntry> m_Entries = [];

    /// <summary>
    /// Sector size in bytes (512 for V3, 4096 for V4).
    /// </summary>
    public int SectorSize { get; private set; }

    CfbContext(Stream stream, bool leaveOpen)
    {
        m_Stream = stream;
        m_LeaveOpen = leaveOpen;
    }

    // -------------------------------------------------------------------------
    // Factory
    // -------------------------------------------------------------------------

    public static CfbContext Open(Stream stream, bool leaveOpen, bool writable)
    {
        var ctx = new CfbContext(stream, leaveOpen) { m_Writable = writable };

        if (writable && stream.CanSeek && stream.Length == 0)
        {
            // Writable context on an empty stream: initialize as a new empty compound file.
            ctx.InitEmpty();
            ctx.m_Dirty = true;
        }
        else
        {
            ctx.ReadCompoundFile();
        }

        return ctx;
    }

    public static CfbContext Create(Stream stream, bool leaveOpen)
    {
        var ctx = new CfbContext(stream, leaveOpen) { m_Writable = true, m_Dirty = true };
        ctx.InitEmpty();
        return ctx;
    }

    void InitEmpty()
    {
        SectorSize = 512; // V3
        m_Entries =
        [
            new CfbEntry
            {
                Id = CfbConstants.RootEntryId,
                Name = "Root Entry",
                Type = CfbEntryType.Root,
                Color = 1,
                StartSectorId = CfbConstants.EndOfChainSectorId,
                CreationTime = DateTime.UtcNow,
                ModificationTime = DateTime.UtcNow,
            }
        ];
    }

    // -------------------------------------------------------------------------
    // Compound file reading
    // -------------------------------------------------------------------------

    void ReadCompoundFile()
    {
        var h = ReadHeader();
        m_Fat = BuildFat(h);
        m_MiniFat = BuildMiniFat(h);
        m_Entries = ReadDirectoryEntries(h);
    }

    // ---- Header ----

    struct HeaderInfo
    {
        public int SectorSize;
        public uint FirstDirectorySectorId;
        public uint FirstMiniFatSectorId;
        public uint MiniFatSectorCount;
        public uint FirstDifatSectorId;
        public uint DifatSectorCount;
        public uint FatSectorCount;
        public uint[] HeaderDifat;
    }

    HeaderInfo ReadHeader()
    {
        m_Stream.Position = 0;
        byte[] header = new byte[CfbConstants.HeaderSize];
        m_Stream.ReadExactly(header, 0, header.Length);

        // Validate signature.
        if (!header.AsSpan(0, 8).SequenceEqual(CfbConstants.Signature))
            throw new InvalidDataException("The stream does not contain a valid MS-CFB compound file signature.");

        ushort majorVersion = BinaryPrimitives.ReadUInt16LittleEndian(header.AsSpan(26));
        ushort sectorShift = BinaryPrimitives.ReadUInt16LittleEndian(header.AsSpan(30));
        int sectorSize = 1 << sectorShift;
        SectorSize = sectorSize;

        if (majorVersion is not 3 and not 4)
            throw new InvalidDataException($"Unsupported MS-CFB major version: {majorVersion}.");

        var h = new HeaderInfo
        {
            SectorSize = sectorSize,
            FatSectorCount = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(44)),
            FirstDirectorySectorId = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(48)),
            FirstMiniFatSectorId = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(60)),
            MiniFatSectorCount = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(64)),
            FirstDifatSectorId = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(68)),
            DifatSectorCount = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(72))
        };

        // DIFAT entries stored directly in the header (up to 109).
        uint[] headerDifat = new uint[CfbConstants.HeaderDifatCount];
        for (int i = 0; i < CfbConstants.HeaderDifatCount; i++)
            headerDifat[i] = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(76 + i * 4));

        h.HeaderDifat = headerDifat;
        return h;
    }

    // ---- FAT ----

    uint[] BuildFat(in HeaderInfo h)
    {
        var fatSectorIds = new List<uint>();

        // Collect FAT sector IDs from the header DIFAT array.
        foreach (uint id in h.HeaderDifat)
        {
            if (id == CfbConstants.FreeSectorId || id == CfbConstants.EndOfChainSectorId)
                break;
            fatSectorIds.Add(id);
        }

        // Collect FAT sector IDs from DIFAT extension sectors if any.
        if (h.DifatSectorCount > 0)
        {
            uint difatSectorId = h.FirstDifatSectorId;
            while (difatSectorId != CfbConstants.EndOfChainSectorId && difatSectorId != CfbConstants.FreeSectorId)
            {
                byte[] sector = ReadSectorBytes(difatSectorId);
                int entriesPerDifatSector = h.SectorSize / 4 - 1; // last int32 is the next DIFAT sector ID
                for (int i = 0; i < entriesPerDifatSector; i++)
                {
                    uint id = BinaryPrimitives.ReadUInt32LittleEndian(sector.AsSpan(i * 4));
                    if (id == CfbConstants.FreeSectorId || id == CfbConstants.EndOfChainSectorId)
                        break;
                    fatSectorIds.Add(id);
                }
                difatSectorId = BinaryPrimitives.ReadUInt32LittleEndian(sector.AsSpan(h.SectorSize - 4));
            }
        }

        // Build the FAT from the collected FAT sector IDs.
        int entriesPerFatSector = h.SectorSize / 4;
        uint[] fat = new uint[fatSectorIds.Count * entriesPerFatSector];
        for (int i = 0; i < fatSectorIds.Count; i++)
        {
            byte[] sector = ReadSectorBytes(fatSectorIds[i]);
            for (int j = 0; j < entriesPerFatSector; j++)
                fat[i * entriesPerFatSector + j] = BinaryPrimitives.ReadUInt32LittleEndian(sector.AsSpan(j * 4));
        }

        return fat;
    }

    // ---- Mini-FAT ----

    uint[] BuildMiniFat(in HeaderInfo h)
    {
        if (h.MiniFatSectorCount == 0 || h.FirstMiniFatSectorId == CfbConstants.EndOfChainSectorId)
            return [];

        byte[] bytes = ReadChain(h.FirstMiniFatSectorId);
        uint[] miniFat = new uint[bytes.Length / 4];
        for (int i = 0; i < miniFat.Length; i++)
            miniFat[i] = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(i * 4));

        return miniFat;
    }

    // ---- Directory ----

    List<CfbEntry> ReadDirectoryEntries(in HeaderInfo h)
    {
        byte[] dirBytes = ReadChain(h.FirstDirectorySectorId);
        int entryCount = dirBytes.Length / CfbConstants.DirectoryEntrySize;
        var entries = new List<CfbEntry>(entryCount);
        for (int i = 0; i < entryCount; i++)
            entries.Add(ParseDirectoryEntry(dirBytes, i * CfbConstants.DirectoryEntrySize, (uint)i));
        return entries;
    }

    static CfbEntry ParseDirectoryEntry(byte[] data, int offset, uint id)
    {
        var entry = new CfbEntry { Id = id };

        // Name: UTF-16LE, nameLength includes null terminator (in bytes).
        ushort nameLength = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(offset + 64));
        if (nameLength >= 2 && nameLength <= 64)
        {
            int charBytes = nameLength - 2; // subtract null terminator
            entry.Name = Encoding.Unicode.GetString(data, offset, charBytes);
        }

        entry.Type = (CfbEntryType)data[offset + 66];
        entry.Color = data[offset + 67];
        entry.LeftSiblingId = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset + 68));
        entry.RightSiblingId = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset + 72));
        entry.ChildId = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset + 76));

        // CLSID: 16 bytes at offset + 80.
        byte[] clsidBytes = new byte[16];
        Array.Copy(data, offset + 80, clsidBytes, 0, 16);
        entry.Clsid = new Guid(clsidBytes);

        entry.StateBits = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset + 96));
        entry.CreationTime = FileTimeToDateTime(BinaryPrimitives.ReadInt64LittleEndian(data.AsSpan(offset + 100)));
        entry.ModificationTime = FileTimeToDateTime(BinaryPrimitives.ReadInt64LittleEndian(data.AsSpan(offset + 108)));
        entry.StartSectorId = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset + 116));

        // Size: lower 32 bits are used in V3 (upper 32 bits must be zero), full 64 bits used in V4.
        entry.Size = BinaryPrimitives.ReadInt64LittleEndian(data.AsSpan(offset + 120));

        return entry;
    }

    static DateTime FileTimeToDateTime(long fileTime)
    {
        if (fileTime <= 0)
            return DateTime.MinValue;
        try
        {
            return DateTime.FromFileTimeUtc(fileTime);
        }
        catch (ArgumentOutOfRangeException)
        {
            return DateTime.MinValue;
        }
    }

    // -------------------------------------------------------------------------
    // Low-level sector I/O
    // -------------------------------------------------------------------------

    long GetSectorOffset(uint sectorId) =>
        CfbConstants.HeaderSize + (long)sectorId * SectorSize;

    byte[] ReadSectorBytes(uint sectorId)
    {
        byte[] buffer = new byte[SectorSize];
        m_Stream.Position = GetSectorOffset(sectorId);
        m_Stream.ReadExactly(buffer, 0, buffer.Length);
        return buffer;
    }

    /// <summary>
    /// Reads a sector's bytes for use in a stream (may be cached in the future).
    /// </summary>
    public byte[] ReadFatSector(uint sectorId) => ReadSectorBytes(sectorId);

    // -------------------------------------------------------------------------
    // FAT chain traversal
    // -------------------------------------------------------------------------

    public IEnumerable<uint> GetSectorChain(uint firstSectorId)
    {
        uint id = firstSectorId;
        while (id < CfbConstants.DifatSectorId)
        {
            yield return id;
            if (id >= (uint)m_Fat.Length)
                break;
            id = m_Fat[id];
        }
    }

    /// <summary>
    /// Returns the sector that follows <paramref name="sectorId"/> in the FAT chain,
    /// or <see cref="CfbConstants.EndOfChainSectorId"/> if it is the last sector.
    /// </summary>
    public uint GetNextSector(uint sectorId) =>
        sectorId < (uint)m_Fat.Length ? m_Fat[sectorId] : CfbConstants.EndOfChainSectorId;

    byte[] ReadChain(uint firstSectorId)
    {
        uint[] chain = [.. GetSectorChain(firstSectorId)];
        byte[] result = new byte[chain.Length * SectorSize];
        for (int i = 0; i < chain.Length; i++)
        {
            m_Stream.Position = GetSectorOffset(chain[i]);
            m_Stream.ReadExactly(result, i * SectorSize, SectorSize);
        }
        return result;
    }

    // -------------------------------------------------------------------------
    // Mini-stream
    // -------------------------------------------------------------------------

    public IEnumerable<uint> GetMiniSectorChain(uint firstMiniSectorId)
    {
        uint id = firstMiniSectorId;
        while (id < CfbConstants.DifatSectorId)
        {
            yield return id;
            if (id >= (uint)m_MiniFat.Length)
                break;
            id = m_MiniFat[id];
        }
    }

    /// <summary>
    /// Returns the mini-sector that follows <paramref name="miniSectorId"/> in the mini-FAT chain,
    /// or <see cref="CfbConstants.EndOfChainSectorId"/> if it is the last mini-sector.
    /// </summary>
    public uint GetNextMiniSector(uint miniSectorId) =>
        miniSectorId < (uint)m_MiniFat.Length ? m_MiniFat[miniSectorId] : CfbConstants.EndOfChainSectorId;

    uint[]? m_RootSectorChain;

    uint[] GetRootSectorChain()
    {
        if (m_RootSectorChain is null)
        {
            var root = GetRootEntry();
            m_RootSectorChain = [.. GetSectorChain(root.StartSectorId)];
        }
        return m_RootSectorChain;
    }

    /// <summary>
    /// Reads the bytes of mini-sector <paramref name="miniSectorId"/> from the root entry's stream.
    /// </summary>
    public byte[] ReadMiniSector(uint miniSectorId)
    {
        long offset = (long)miniSectorId * CfbConstants.MiniSectorSize;
        int containerSectorIndex = (int)(offset / SectorSize);
        int containerSectorOffset = (int)(offset % SectorSize);

        uint[] rootChain = GetRootSectorChain();
        if (containerSectorIndex >= rootChain.Length)
            throw new InvalidDataException("Mini-sector index is out of range of the mini-stream container.");

        byte[] containerSector = ReadSectorBytes(rootChain[containerSectorIndex]);
        byte[] miniSectorData = new byte[CfbConstants.MiniSectorSize];
        Array.Copy(containerSector, containerSectorOffset, miniSectorData, 0, CfbConstants.MiniSectorSize);
        return miniSectorData;
    }

    // -------------------------------------------------------------------------
    // Directory access
    // -------------------------------------------------------------------------

    public CfbEntry GetRootEntry()
    {
        if (m_Entries.Count == 0)
            throw new InvalidDataException("MS-CFB compound file has no directory entries.");
        return m_Entries[(int)CfbConstants.RootEntryId];
    }

    CfbEntry? GetEntry(uint id)
    {
        if (id == CfbConstants.NoStream || id >= (uint)m_Entries.Count)
            return null;
        var entry = m_Entries[(int)id];
        return entry.Type == CfbEntryType.Unallocated ? null : entry;
    }

    /// <summary>
    /// Enumerates all direct children (streams and sub-storages) of a storage entry
    /// via an in-order traversal of its sibling red-black tree.
    /// </summary>
    public IEnumerable<CfbEntry> EnumerateChildren(CfbEntry parent)
    {
        if (parent.ChildId == CfbConstants.NoStream)
            yield break;

        foreach (var entry in WalkSiblingSubtree(parent.ChildId))
            yield return entry;
    }

    IEnumerable<CfbEntry> WalkSiblingSubtree(uint id)
    {
        if (id == CfbConstants.NoStream)
            yield break;

        var entry = GetEntry(id);
        if (entry is null)
            yield break;

        // In-order traversal: left → self → right.
        foreach (var e in WalkSiblingSubtree(entry.LeftSiblingId))
            yield return e;

        yield return entry;

        foreach (var e in WalkSiblingSubtree(entry.RightSiblingId))
            yield return e;
    }

    /// <summary>
    /// Finds a direct child of <paramref name="parent"/> by name.
    /// Uses the binary search tree structure for efficient lookup.
    /// Returns <see langword="null"/> if not found.
    /// </summary>
    public CfbEntry? TryFindChild(CfbEntry parent, string name)
    {
        if (parent.ChildId == CfbConstants.NoStream)
            return null;

        return SearchSiblingSubtree(parent.ChildId, name);
    }

    CfbEntry? SearchSiblingSubtree(uint id, string name)
    {
        if (id == CfbConstants.NoStream)
            return null;

        var entry = GetEntry(id);
        if (entry is null)
            return null;

        int cmp = CfbEntry.CompareName(name.AsSpan(), entry.Name.AsSpan());

        if (cmp == 0)
            return entry;
        else if (cmp < 0)
            return SearchSiblingSubtree(entry.LeftSiblingId, name);
        else
            return SearchSiblingSubtree(entry.RightSiblingId, name);
    }

    // -------------------------------------------------------------------------
    // Stream opening
    // -------------------------------------------------------------------------

    /// <summary>
    /// Opens a read-only, seekable stream for the data of <paramref name="entry"/>.
    /// </summary>
    public Stream OpenReadStream(CfbEntry entry)
    {
        if (entry.Type != CfbEntryType.Stream)
            throw new InvalidOperationException("Entry is not a stream.");

        // Serve pending (unsaved) data from memory if present.
        if (entry.PendingData != null)
            return new MemoryStream(entry.PendingData, writable: false);

        return new CfbFileStream(this, entry);
    }

    // -------------------------------------------------------------------------
    // Write support — entry mutation
    // -------------------------------------------------------------------------

    /// <summary>
    /// Adds a new child entry under <paramref name="parent"/> with the given name and type.
    /// </summary>
    public CfbEntry AddChild(CfbEntry parent, string name, CfbEntryType type)
    {
        // Find a free slot, or extend the list.
        uint newId = (uint)m_Entries.Count;
        for (int i = 0; i < m_Entries.Count; i++)
        {
            if (m_Entries[i].Type == CfbEntryType.Unallocated)
            {
                newId = (uint)i;
                break;
            }
        }

        var entry = new CfbEntry
        {
            Id = newId,
            Name = name,
            Type = type,
            Color = 1, // black
            CreationTime = DateTime.UtcNow,
            ModificationTime = DateTime.UtcNow,
        };

        if (newId == (uint)m_Entries.Count)
            m_Entries.Add(entry);
        else
            m_Entries[(int)newId] = entry;

        // Rebuild the parent's child BST to include the new entry.
        var childIds = EnumerateChildren(parent).Select(e => e.Id).ToList();
        childIds.Add(newId);
        parent.ChildId = BuildSiblingTree(childIds);
        parent.ModificationTime = DateTime.UtcNow;

        m_Dirty = true;
        return entry;
    }

    /// <summary>
    /// Removes <paramref name="entry"/> from <paramref name="parent"/>'s child tree and marks it as unallocated.
    /// Does not recursively remove children — callers must do that beforehand.
    /// </summary>
    public void RemoveChild(CfbEntry parent, CfbEntry entry)
    {
        // Rebuild the parent's child BST without this entry.
        var childIds = EnumerateChildren(parent).Select(e => e.Id).Where(id => id != entry.Id).ToList();
        parent.ChildId = BuildSiblingTree(childIds);
        parent.ModificationTime = DateTime.UtcNow;

        // Mark entry as unallocated.
        entry.Type = CfbEntryType.Unallocated;
        entry.Name = string.Empty;
        entry.LeftSiblingId = CfbConstants.NoStream;
        entry.RightSiblingId = CfbConstants.NoStream;
        entry.ChildId = CfbConstants.NoStream;
        entry.PendingData = null;
        entry.StartSectorId = CfbConstants.EndOfChainSectorId;
        entry.Size = 0;

        m_Dirty = true;
    }

    // ---- BST helpers ----

    uint BuildSiblingTree(List<uint> ids)
    {
        if (ids.Count == 0)
            return CfbConstants.NoStream;

        ids.Sort((a, b) => CfbEntry.CompareName(
            m_Entries[(int)a].Name.AsSpan(),
            m_Entries[(int)b].Name.AsSpan()));

        return BuildBalancedBst(ids, 0, ids.Count - 1);
    }

    uint BuildBalancedBst(List<uint> ids, int lo, int hi)
    {
        if (lo > hi)
            return CfbConstants.NoStream;

        int mid = (lo + hi) / 2;
        uint nodeId = ids[mid];
        var node = m_Entries[(int)nodeId];
        node.Color = 1; // black
        node.LeftSiblingId = BuildBalancedBst(ids, lo, mid - 1);
        node.RightSiblingId = BuildBalancedBst(ids, mid + 1, hi);
        return nodeId;
    }

    // -------------------------------------------------------------------------
    // Write support — stream opening
    // -------------------------------------------------------------------------

    /// <summary>
    /// Opens a writable, seekable, readable stream for <paramref name="entry"/>.
    /// The data is buffered in memory and committed to <see cref="CfbEntry.PendingData"/> on stream disposal.
    /// </summary>
    public Stream OpenWriteStream(CfbEntry entry, FileMode mode, FileAccess access, bool create)
    {
        MemoryStream ms;

        if (create || mode is FileMode.Create or FileMode.Truncate)
        {
            ms = new MemoryStream();
        }
        else
        {
            byte[] existing = entry.PendingData ?? (entry.Size > 0 ? ReadEntryData(entry) : []);
            ms = new MemoryStream(existing.Length);
            ms.Write(existing, 0, existing.Length);

            ms.Position = mode == FileMode.Append ? ms.Length : 0;
        }

        m_Dirty = true;
        return new CfbWriteStream(entry, ms, access);
    }

    byte[] ReadEntryData(CfbEntry entry)
    {
        if (entry.Size <= 0)
            return [];

        int size = (int)entry.Size;
        byte[] data = new byte[size];
        int bytesRead = 0;

        bool isMini = entry.Size < CfbConstants.MiniStreamCutOffSize;
        if (isMini)
        {
            foreach (uint id in GetMiniSectorChain(entry.StartSectorId))
            {
                ReadOnlySpan<byte> miniSector = ReadMiniSector(id);
                int toCopy = Math.Min(CfbConstants.MiniSectorSize, size - bytesRead);
                miniSector[..toCopy].CopyTo(data.AsSpan(bytesRead));
                bytesRead += toCopy;
                if (bytesRead >= size)
                    break;
            }
        }
        else
        {
            foreach (uint id in GetSectorChain(entry.StartSectorId))
            {
                byte[] sector = ReadSectorBytes(id);
                int toCopy = Math.Min(SectorSize, size - bytesRead);
                Array.Copy(sector, 0, data, bytesRead, toCopy);
                bytesRead += toCopy;
                if (bytesRead >= size)
                    break;
            }
        }

        return data;
    }

    // -------------------------------------------------------------------------
    // Write support — flush
    // -------------------------------------------------------------------------

    public void MarkDirty() => m_Dirty = true;

    /// <summary>
    /// Writes all pending changes to the underlying stream.
    /// Does nothing if the context is not writable or has no pending changes.
    /// </summary>
    public void Flush()
    {
        if (m_Writable && m_Dirty)
            WriteCompoundFile();
    }

    void WriteCompoundFile()
    {
        const int sectorSize = 512; // Always write V3 (512-byte sectors).
        const int entriesPerFatSector = sectorSize / 4; // 128
        const int dirEntriesPerSector = sectorSize / CfbConstants.DirectoryEntrySize; // 4
        const int miniEntriesPerFatSector = sectorSize / 4; // 128

        // ---- Step 1: Resolve data for all stream entries. ----

        var streamData = new Dictionary<uint, byte[]>();
        foreach (var e in m_Entries)
        {
            if (e.Type == CfbEntryType.Stream)
            {
                byte[] data =
                    e.PendingData ??
                    (e.StartSectorId != CfbConstants.EndOfChainSectorId ? ReadEntryData(e) : []);
                streamData[e.Id] = data;
            }
        }

        // ---- Step 2: Classify stream entries. ----

        var miniStreamEntries = m_Entries
            .Where(e => e.Type == CfbEntryType.Stream &&
                        streamData.TryGetValue(e.Id, out byte[]? d) && d.Length > 0 &&
                        d.Length < CfbConstants.MiniStreamCutOffSize)
            .ToList();

        var regularStreamEntries = m_Entries
            .Where(e => e.Type == CfbEntryType.Stream &&
                        streamData.TryGetValue(e.Id, out byte[]? d) &&
                        d.Length >= CfbConstants.MiniStreamCutOffSize)
            .ToList();

        // Update empty stream entries.
        foreach (var e in m_Entries)
        {
            if (e.Type == CfbEntryType.Stream &&
                streamData.TryGetValue(e.Id, out byte[]? d) && d.Length == 0)
            {
                e.StartSectorId = CfbConstants.EndOfChainSectorId;
                e.Size = 0;
            }
        }

        // ---- Step 3: Build mini-stream. ----

        int totalMiniSectors = miniStreamEntries.Sum(e => CeilDiv(streamData[e.Id].Length, CfbConstants.MiniSectorSize));
        byte[] miniContainer = new byte[totalMiniSectors * CfbConstants.MiniSectorSize];
        var miniFatList = new List<uint>(totalMiniSectors);
        int miniSectorCursor = 0;

        foreach (var e in miniStreamEntries)
        {
            byte[] data = streamData[e.Id];
            int count = CeilDiv(data.Length, CfbConstants.MiniSectorSize);
            e.StartSectorId = (uint)miniSectorCursor;
            e.Size = data.Length;
            data.CopyTo(miniContainer.AsSpan(miniSectorCursor * CfbConstants.MiniSectorSize));
            for (int i = 0; i < count; i++)
                miniFatList.Add(i == count - 1 ? CfbConstants.EndOfChainSectorId : (uint)(miniSectorCursor + i + 1));
            miniSectorCursor += count;
        }

        // ---- Step 4: Fixed sector counts. ----

        int numDirSectors = CeilDiv(m_Entries.Count, dirEntriesPerSector);
        int miniContainerSectors = totalMiniSectors > 0 ? CeilDiv(miniContainer.Length, sectorSize) : 0;
        int numMiniFatSectors = totalMiniSectors > 0 ? CeilDiv(miniFatList.Count, miniEntriesPerFatSector) : 0;
        int totalRegularStreamSectors = regularStreamEntries.Sum(e => CeilDiv(streamData[e.Id].Length, sectorSize));
        int baseSectors = totalRegularStreamSectors + miniContainerSectors + numMiniFatSectors + numDirSectors;

        // ---- Step 5: FAT sector count (fixed-point iteration). ----

        int fatSectorCount = Math.Max(1, CeilDiv(baseSectors + 1, entriesPerFatSector));
        int difatSectorCount = 0;

        for (int iter = 0; iter < 12; iter++)
        {
            difatSectorCount = fatSectorCount > CfbConstants.HeaderDifatCount
                ? CeilDiv(fatSectorCount - CfbConstants.HeaderDifatCount, entriesPerFatSector - 1)
                : 0;
            int newFatCount = CeilDiv(baseSectors + fatSectorCount + difatSectorCount, entriesPerFatSector);
            if (newFatCount == fatSectorCount)
                break;
            fatSectorCount = newFatCount;
        }

        int totalSectors = baseSectors + fatSectorCount + difatSectorCount;

        // ---- Step 6: Assign sector IDs. ----

        int sectorCursor = 0;

        // Regular stream data sectors.
        foreach (var e in regularStreamEntries)
        {
            byte[] data = streamData[e.Id];
            e.StartSectorId = (uint)sectorCursor;
            e.Size = data.Length;
            sectorCursor += CeilDiv(data.Length, sectorSize);
        }

        // Mini-stream container (root entry).
        var rootEntry = GetRootEntry();
        uint firstMiniContainerSector;
        if (miniContainerSectors > 0)
        {
            firstMiniContainerSector = (uint)sectorCursor;
            rootEntry.StartSectorId = firstMiniContainerSector;
            rootEntry.Size = miniContainer.Length;
            sectorCursor += miniContainerSectors;
        }
        else
        {
            firstMiniContainerSector = CfbConstants.EndOfChainSectorId;
            rootEntry.StartSectorId = CfbConstants.EndOfChainSectorId;
            rootEntry.Size = 0;
        }

        // Mini-FAT sectors.
        uint firstMiniFatSector = CfbConstants.EndOfChainSectorId;
        if (numMiniFatSectors > 0)
        {
            firstMiniFatSector = (uint)sectorCursor;
            sectorCursor += numMiniFatSectors;
        }

        // Directory sectors.
        uint firstDirSector = (uint)sectorCursor;
        sectorCursor += numDirSectors;

        // FAT sectors.
        uint firstFatSector = (uint)sectorCursor;
        sectorCursor += fatSectorCount;

        // DIFAT sectors.
        uint firstDifatSector = (uint)sectorCursor;
        sectorCursor += difatSectorCount;

        Debug.Assert(sectorCursor == totalSectors);

        // ---- Step 7: Build FAT array. ----

        uint[] fat = new uint[totalSectors];
        Array.Fill(fat, CfbConstants.FreeSectorId);

        foreach (var e in regularStreamEntries)
        {
            int count = CeilDiv((int)streamData[e.Id].Length, sectorSize);
            for (int i = 0; i < count; i++)
                fat[e.StartSectorId + i] = i == count - 1 ? CfbConstants.EndOfChainSectorId : e.StartSectorId + (uint)i + 1;
        }

        for (int i = 0; i < miniContainerSectors; i++)
            fat[firstMiniContainerSector + i] = i == miniContainerSectors - 1 ? CfbConstants.EndOfChainSectorId : firstMiniContainerSector + (uint)i + 1;

        for (int i = 0; i < numMiniFatSectors; i++)
            fat[firstMiniFatSector + i] = i == numMiniFatSectors - 1 ? CfbConstants.EndOfChainSectorId : firstMiniFatSector + (uint)i + 1;

        for (int i = 0; i < numDirSectors; i++)
            fat[firstDirSector + i] = i == numDirSectors - 1 ? CfbConstants.EndOfChainSectorId : firstDirSector + (uint)i + 1;

        for (int i = 0; i < fatSectorCount; i++)
            fat[firstFatSector + i] = CfbConstants.FatSectorId;

        for (int i = 0; i < difatSectorCount; i++)
            fat[firstDifatSector + i] = CfbConstants.DifatSectorId;

        // ---- Step 8: Write to stream. ----

        m_Stream.SetLength(CfbConstants.HeaderSize + (long)totalSectors * sectorSize);
        m_Stream.Position = 0;

        WriteHeader(
            firstDirSector, firstMiniFatSector, (uint)numMiniFatSectors,
            fatSectorCount, firstDifatSector, difatSectorCount, firstFatSector, sectorSize);

        foreach (var e in regularStreamEntries)
            WriteSectorData(e.StartSectorId, streamData[e.Id], sectorSize);

        if (miniContainerSectors > 0)
            WriteSectorData(firstMiniContainerSector, miniContainer, sectorSize);

        if (numMiniFatSectors > 0)
        {
            while (miniFatList.Count % miniEntriesPerFatSector != 0)
                miniFatList.Add(CfbConstants.FreeSectorId);
            byte[] miniFatBytes = new byte[miniFatList.Count * 4];
            for (int i = 0; i < miniFatList.Count; i++)
                BinaryPrimitives.WriteUInt32LittleEndian(miniFatBytes.AsSpan(i * 4), miniFatList[i]);
            WriteSectorData(firstMiniFatSector, miniFatBytes, sectorSize);
        }

        WriteDirectorySectors(firstDirSector, numDirSectors, sectorSize);
        WriteFatSectors(firstFatSector, fat, fatSectorCount, sectorSize);

        if (difatSectorCount > 0)
            WriteDifatSectors(firstDifatSector, firstFatSector, fatSectorCount, difatSectorCount, sectorSize);

        m_Stream.Flush();

        // Refresh in-memory FAT/MiniFAT from the freshly written data so that
        // any subsequent reads use the correct sector assignments.
        m_Dirty = false;
        var h = ReadHeader();
        m_Fat = BuildFat(h);
        m_MiniFat = BuildMiniFat(h);
        m_RootSectorChain = null;
        foreach (var e in m_Entries)
            e.PendingData = null;
    }

    // ---- Write helpers ----

    void WriteHeader(
        uint firstDirSector, uint firstMiniFatSector, uint numMiniFatSectors,
        int fatSectorCount, uint firstDifatSector, int difatSectorCount,
        uint firstFatSector, int sectorSize)
    {
        byte[] h = new byte[CfbConstants.HeaderSize];

        CfbConstants.Signature.CopyTo(h);
        // CLSID: 16 zero bytes at offset 8.
        BinaryPrimitives.WriteUInt16LittleEndian(h.AsSpan(24), 0x003E); // minor version
        BinaryPrimitives.WriteUInt16LittleEndian(h.AsSpan(26), 3);      // major version = 3 (V3)
        BinaryPrimitives.WriteUInt16LittleEndian(h.AsSpan(28), 0xFFFE); // byte order mark
        BinaryPrimitives.WriteUInt16LittleEndian(h.AsSpan(30), 9);      // sector shift = 9 → 512 bytes
        BinaryPrimitives.WriteUInt16LittleEndian(h.AsSpan(32), 6);      // mini sector shift = 6 → 64 bytes
        // Bytes 34–39: reserved (zeros).
        // Bytes 40–43: directory sector count (V3 always 0).
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(44), (uint)fatSectorCount);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(48), firstDirSector);
        // Bytes 52–55: transaction signature (0).
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(56), (uint)CfbConstants.MiniStreamCutOffSize);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(60), firstMiniFatSector);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(64), numMiniFatSectors);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(68), difatSectorCount > 0 ? firstDifatSector : CfbConstants.EndOfChainSectorId);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(72), (uint)difatSectorCount);

        // Inline DIFAT: first 109 FAT sector IDs.
        int inlineDifat = Math.Min(fatSectorCount, CfbConstants.HeaderDifatCount);
        for (int i = 0; i < CfbConstants.HeaderDifatCount; i++)
        {
            uint val = i < inlineDifat ? firstFatSector + (uint)i : CfbConstants.FreeSectorId;
            BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(76 + i * 4), val);
        }

        m_Stream.Position = 0;
        m_Stream.Write(h, 0, h.Length);
    }

    void WriteSectorData(uint firstSector, byte[] data, int sectorSize)
    {
        m_Stream.Position = CfbConstants.HeaderSize + (long)firstSector * sectorSize;
        m_Stream.Write(data, 0, data.Length);
        int rem = data.Length % sectorSize;
        if (rem != 0)
        {
            byte[] pad = new byte[sectorSize - rem];
            m_Stream.Write(pad, 0, pad.Length);
        }
    }

    void WriteDirectorySectors(uint firstSector, int numSectors, int sectorSize)
    {
        int totalSlots = numSectors * (sectorSize / CfbConstants.DirectoryEntrySize);
        byte[] buf = new byte[numSectors * sectorSize];
        for (int i = 0; i < totalSlots; i++)
        {
            if (i < m_Entries.Count)
                SerializeDirectoryEntry(buf, i * CfbConstants.DirectoryEntrySize, m_Entries[i]);
            // else: slot is zero-filled = unallocated.
        }
        m_Stream.Position = CfbConstants.HeaderSize + (long)firstSector * sectorSize;
        m_Stream.Write(buf, 0, buf.Length);
    }

    static void SerializeDirectoryEntry(byte[] buf, int offset, CfbEntry entry)
    {
        // Buffer is already zeroed.
        if (entry.Type == CfbEntryType.Unallocated)
        {
            // NoStream for sibling/child IDs per spec.
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(offset + 68), CfbConstants.NoStream);
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(offset + 72), CfbConstants.NoStream);
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(offset + 76), CfbConstants.NoStream);
            return;
        }

        // Name: UTF-16LE, max 31 chars + null terminator.
        int nameLen = Math.Min(entry.Name.Length, CfbConstants.MaxNameLength - 1);
        if (nameLen > 0)
        {
            byte[] nameBytes = Encoding.Unicode.GetBytes(entry.Name.Substring(0, nameLen));
            nameBytes.CopyTo(buf, offset);
            BinaryPrimitives.WriteUInt16LittleEndian(buf.AsSpan(offset + 64), (ushort)(nameBytes.Length + 2));
        }

        buf[offset + 66] = (byte)entry.Type;
        buf[offset + 67] = entry.Color;
        BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(offset + 68), entry.LeftSiblingId);
        BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(offset + 72), entry.RightSiblingId);
        BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(offset + 76), entry.ChildId);

        byte[] clsid = entry.Clsid.ToByteArray();
        Array.Copy(clsid, 0, buf, offset + 80, 16);

        BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(offset + 96), entry.StateBits);

        if (entry.CreationTime != DateTime.MinValue)
            BinaryPrimitives.WriteInt64LittleEndian(buf.AsSpan(offset + 100), entry.CreationTime.ToFileTimeUtc());
        if (entry.ModificationTime != DateTime.MinValue)
            BinaryPrimitives.WriteInt64LittleEndian(buf.AsSpan(offset + 108), entry.ModificationTime.ToFileTimeUtc());

        BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(offset + 116), entry.StartSectorId);
        BinaryPrimitives.WriteInt64LittleEndian(buf.AsSpan(offset + 120), entry.Size); // V3: high 32 bits = 0
    }

    void WriteFatSectors(uint firstFatSector, uint[] fat, int fatSectorCount, int sectorSize)
    {
        int entriesPerSector = sectorSize / 4;
        for (int i = 0; i < fatSectorCount; i++)
        {
            byte[] sector = new byte[sectorSize];
            int baseIdx = i * entriesPerSector;
            for (int j = 0; j < entriesPerSector; j++)
            {
                uint val = baseIdx + j < fat.Length ? fat[baseIdx + j] : CfbConstants.FreeSectorId;
                BinaryPrimitives.WriteUInt32LittleEndian(sector.AsSpan(j * 4), val);
            }
            m_Stream.Position = CfbConstants.HeaderSize + (long)(firstFatSector + i) * sectorSize;
            m_Stream.Write(sector, 0, sector.Length);
        }
    }

    void WriteDifatSectors(uint firstDifatSector, uint firstFatSector, int fatSectorCount, int difatSectorCount, int sectorSize)
    {
        int entriesPerDifat = sectorSize / 4 - 1; // last slot = next DIFAT pointer
        int fatSectorIdx = CfbConstants.HeaderDifatCount;

        for (int i = 0; i < difatSectorCount; i++)
        {
            byte[] sector = new byte[sectorSize];
            for (int j = 0; j < sectorSize / 4; j++)
                BinaryPrimitives.WriteUInt32LittleEndian(sector.AsSpan(j * 4), CfbConstants.FreeSectorId);

            for (int j = 0; j < entriesPerDifat && fatSectorIdx < fatSectorCount; j++, fatSectorIdx++)
                BinaryPrimitives.WriteUInt32LittleEndian(sector.AsSpan(j * 4), firstFatSector + (uint)fatSectorIdx);

            uint nextDifat = i < difatSectorCount - 1
                ? firstDifatSector + (uint)i + 1
                : CfbConstants.EndOfChainSectorId;
            BinaryPrimitives.WriteUInt32LittleEndian(sector.AsSpan(sectorSize - 4), nextDifat);

            m_Stream.Position = CfbConstants.HeaderSize + (long)(firstDifatSector + i) * sectorSize;
            m_Stream.Write(sector, 0, sector.Length);
        }
    }

    static int CeilDiv(int a, int b) => (a + b - 1) / b;

    // -------------------------------------------------------------------------
    // IDisposable
    // -------------------------------------------------------------------------

    public void Dispose()
    {
        if (!m_LeaveOpen)
            m_Stream.Dispose();
    }
}

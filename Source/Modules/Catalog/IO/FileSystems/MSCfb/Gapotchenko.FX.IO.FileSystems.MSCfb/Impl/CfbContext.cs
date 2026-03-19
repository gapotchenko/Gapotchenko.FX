// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Buffers.Binary;
using System.Text;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;

/// <summary>
/// The main context that parses and provides access to an MS-CFB compound file.
/// All FAT, mini-FAT, and directory entry data is loaded lazily on first access
/// and cached for subsequent reads. Writes are performed in-place: only the
/// affected sectors (FAT, mini-FAT, directory) are written back to the stream.
/// </summary>
sealed class CfbContext : IDisposable
{
    readonly Stream m_Stream;
    readonly bool m_LeaveOpen;
    bool m_Writable;
    bool m_Dirty;

    // -------------------------------------------------------------------------
    // Sector geometry
    // -------------------------------------------------------------------------

    /// <summary>Sector size in bytes (512 for V3, 4096 for V4).</summary>
    public int SectorSize { get; private set; }

    ushort m_SectorShift;        // log2(SectorSize)
    ushort m_MiniSectorShift;    // log2(MiniSectorSize), always 6

    int m_FatEntriesPerSector;   // SectorSize / 4
    int m_DirEntriesPerSector;   // SectorSize / DirectoryEntrySize

    // -------------------------------------------------------------------------
    // Header metadata (preserved for write-back)
    // -------------------------------------------------------------------------

    uint m_FirstDirectorySectorId = CfbConstants.EndOfChainSectorId;
    uint m_FirstMiniFatSectorId = CfbConstants.EndOfChainSectorId;
    uint m_MiniFatSectorCount;

    // DIFAT stored in header (up to 109 entries).
    uint[] m_HeaderDifat = new uint[CfbConstants.HeaderDifatCount];

    // DIFAT extension chain.
    uint m_FirstDifatSectorId = CfbConstants.EndOfChainSectorId;
    uint m_DifatSectorCount;

    bool m_HeaderDirty;

    // -------------------------------------------------------------------------
    // Lazy FAT
    // -------------------------------------------------------------------------

    // Physical sector IDs of each FAT sector (index = FAT sector index).
    // Populated eagerly from DIFAT at mount - this is O(FAT-sector-count) but
    // only reads the DIFAT structure, not the FAT data itself.
    readonly List<uint> m_FatSectorIds = [];

    // Per-FAT-sector parsed data cache (null = not yet loaded from disk).
    uint[]?[] m_FatCache = [];

    // Per-FAT-sector dirty flags (true = needs write-back to stream).
    bool[] m_FatDirty = [];

    // -------------------------------------------------------------------------
    // Lazy mini-FAT
    // -------------------------------------------------------------------------

    // Physical sector IDs of each mini-FAT sector (null = not yet built).
    List<uint>? m_MiniFatSectorIds;

    // Per-mini-FAT-sector parsed data cache (null = not yet loaded from disk).
    uint[]?[]? m_MiniFatCache;

    // Per-mini-FAT-sector dirty flags.
    bool[]? m_MiniFatDirty;

    // -------------------------------------------------------------------------
    // Lazy directory entries
    // -------------------------------------------------------------------------

    // Physical sector IDs of the directory chain (null = not yet built).
    List<uint>? m_DirChain;

    // Directory entry cache keyed by entry ID (null = not loaded or unallocated).
    readonly Dictionary<uint, CfbEntry> m_DirCache = [];

    // -------------------------------------------------------------------------
    // Mini-stream root chain (lazy)
    // -------------------------------------------------------------------------

    uint[]? m_RootSectorChain;

    // -------------------------------------------------------------------------
    // Constructor / factory
    // -------------------------------------------------------------------------

    CfbContext(Stream stream, bool leaveOpen)
    {
        m_Stream = stream;
        m_LeaveOpen = leaveOpen;
    }

    public static CfbContext Open(Stream stream, bool leaveOpen, bool writable)
    {
        var ctx = new CfbContext(stream, leaveOpen) { m_Writable = writable };

        if (writable && stream.CanSeek && stream.Length == 0)
            ctx.WriteMinimalCompoundFile();

        ctx.MountCompoundFile();
        return ctx;
    }

    public static CfbContext Create(Stream stream, bool leaveOpen)
    {
        var ctx = new CfbContext(stream, leaveOpen) { m_Writable = true };
        ctx.WriteMinimalCompoundFile();
        ctx.MountCompoundFile();
        return ctx;
    }

    // -------------------------------------------------------------------------
    // Mount (lazy setup - reads only the header + DIFAT)
    // -------------------------------------------------------------------------

    void MountCompoundFile()
    {
        m_Stream.Position = 0;
        byte[] header = new byte[CfbConstants.HeaderSize];
        m_Stream.ReadExactly(header, 0, header.Length);

        if (!header.AsSpan(0, 8).SequenceEqual(CfbConstants.Signature))
            throw new InvalidDataException("The stream does not contain a valid MS-CFB compound file signature.");

        ushort majorVersion = BinaryPrimitives.ReadUInt16LittleEndian(header.AsSpan(26));
        if (majorVersion is not 3 and not 4)
            throw new InvalidDataException($"Unsupported MS-CFB major version: {majorVersion}.");

        m_SectorShift = BinaryPrimitives.ReadUInt16LittleEndian(header.AsSpan(30));
        m_MiniSectorShift = BinaryPrimitives.ReadUInt16LittleEndian(header.AsSpan(32));
        SectorSize = 1 << m_SectorShift;
        m_FatEntriesPerSector = SectorSize / 4;
        m_DirEntriesPerSector = SectorSize / CfbConstants.DirectoryEntrySize;

        uint fatSectorCount = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(44));
        m_FirstDirectorySectorId = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(48));
        m_FirstMiniFatSectorId = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(60));
        m_MiniFatSectorCount = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(64));
        m_FirstDifatSectorId = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(68));
        m_DifatSectorCount = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(72));

        for (int i = 0; i < CfbConstants.HeaderDifatCount; i++)
            m_HeaderDifat[i] = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(76 + i * 4));

        // Collect FAT sector IDs from DIFAT (reads only DIFAT sectors, not FAT data).
        CollectFatSectorIds();

        // Validate count.
        if (m_FatSectorIds.Count > (int)fatSectorCount)
            m_FatSectorIds.RemoveRange((int)fatSectorCount, m_FatSectorIds.Count - (int)fatSectorCount);

        // Allocate cache arrays sized to the number of FAT sectors.
        int fatCount = m_FatSectorIds.Count;
        m_FatCache = new uint[]?[fatCount];
        m_FatDirty = new bool[fatCount];
    }

    void CollectFatSectorIds()
    {
        // Collect IDs from the header DIFAT array.
        foreach (uint id in m_HeaderDifat)
        {
            if (id >= CfbConstants.DifatSectorId)
                break;
            m_FatSectorIds.Add(id);
        }

        // Follow DIFAT extension sectors.
        uint difatId = m_FirstDifatSectorId;
        while (difatId < CfbConstants.DifatSectorId)
        {
            byte[] buf = new byte[SectorSize];
            m_Stream.Position = GetSectorOffset(difatId);
            m_Stream.ReadExactly(buf);

            int entriesPerDifat = SectorSize / 4 - 1; // last slot = next DIFAT sector ID
            for (int i = 0; i < entriesPerDifat; i++)
            {
                uint id = BinaryPrimitives.ReadUInt32LittleEndian(buf.AsSpan(i * 4));
                if (id >= CfbConstants.DifatSectorId)
                    break;
                m_FatSectorIds.Add(id);
            }

            difatId = BinaryPrimitives.ReadUInt32LittleEndian(buf.AsSpan(SectorSize - 4));
        }
    }

    // -------------------------------------------------------------------------
    // Low-level sector I/O
    // -------------------------------------------------------------------------

    long GetSectorOffset(uint sectorId) =>
        CfbConstants.HeaderSize + (long)sectorId * SectorSize;

    /// <summary>
    /// Reads up to <c>Math.Min(buffer.Length, SectorSize)</c> bytes from a regular sector.
    /// </summary>
    public int ReadFatSector(uint sectorId, Span<byte> buffer)
    {
        int count = Math.Min(buffer.Length, SectorSize);
        m_Stream.Position = GetSectorOffset(sectorId);
        m_Stream.ReadExactly(buffer[..count]);
        return count;
    }

    /// <summary>
    /// Reads up to <c>Math.Min(buffer.Length, MiniSectorSize)</c> bytes from a mini-sector.
    /// </summary>
    public int ReadMiniSector(uint miniSectorId, Span<byte> buffer)
    {
        long offset = (long)miniSectorId * CfbConstants.MiniSectorSize;
        int containerSectorIndex = (int)(offset / SectorSize);
        int containerSectorOffset = (int)(offset % SectorSize);

        uint[] rootChain = GetRootSectorChain();
        if (containerSectorIndex >= rootChain.Length)
            throw new InvalidDataException("Mini-sector index is out of range of the mini-stream container.");

        int count = Math.Min(CfbConstants.MiniSectorSize, buffer.Length);
        m_Stream.Position = GetSectorOffset(rootChain[containerSectorIndex]) + containerSectorOffset;
        m_Stream.ReadExactly(buffer[..count]);
        return count;
    }

    void EnsureStreamCoversAtLeast(long requiredLength)
    {
        if (m_Stream.Length < requiredLength)
            m_Stream.SetLength(requiredLength);
    }

    // -------------------------------------------------------------------------
    // Lazy FAT access
    // -------------------------------------------------------------------------

    uint GetFatEntry(uint sectorId)
    {
        int fatIdx = (int)(sectorId / (uint)m_FatEntriesPerSector);
        int fatOff = (int)(sectorId % (uint)m_FatEntriesPerSector);
        if ((uint)fatIdx >= (uint)m_FatSectorIds.Count)
            return CfbConstants.FreeSectorId;
        return LoadFatSector(fatIdx)[fatOff];
    }

    void SetFatEntry(uint sectorId, uint value)
    {
        int fatIdx = (int)(sectorId / (uint)m_FatEntriesPerSector);
        int fatOff = (int)(sectorId % (uint)m_FatEntriesPerSector);
        if ((uint)fatIdx >= (uint)m_FatSectorIds.Count)
            throw new InvalidOperationException($"Sector {sectorId} is beyond the known FAT.");
        LoadFatSector(fatIdx)[fatOff] = value;
        m_FatDirty[fatIdx] = true;
        m_Dirty = true;
    }

    uint[] LoadFatSector(int fatIdx)
    {
        ref uint[]? cached = ref m_FatCache[fatIdx];
        if (cached is not null)
            return cached;

        byte[] buf = new byte[SectorSize];
        m_Stream.Position = GetSectorOffset(m_FatSectorIds[fatIdx]);
        m_Stream.ReadExactly(buf);

        uint[] data = new uint[m_FatEntriesPerSector];
        for (int i = 0; i < m_FatEntriesPerSector; i++)
            data[i] = BinaryPrimitives.ReadUInt32LittleEndian(buf.AsSpan(i * 4));

        return cached = data;
    }

    // -------------------------------------------------------------------------
    // FAT chain traversal
    // -------------------------------------------------------------------------

    public IEnumerable<uint> GetSectorChain(uint firstSectorId)
    {
        uint id = firstSectorId;
        while (id < CfbConstants.DifatSectorId)
        {
            yield return id;
            id = GetFatEntry(id);
        }
    }

    public uint GetNextSector(uint sectorId)
    {
        uint next = GetFatEntry(sectorId);
        return next < CfbConstants.DifatSectorId ? next : CfbConstants.EndOfChainSectorId;
    }

    // -------------------------------------------------------------------------
    // Lazy mini-FAT access
    // -------------------------------------------------------------------------

    void EnsureMiniFatReady()
    {
        if (m_MiniFatSectorIds is not null)
            return;

        m_MiniFatSectorIds = [];

        if (m_MiniFatSectorCount > 0 && m_FirstMiniFatSectorId < CfbConstants.DifatSectorId)
        {
            foreach (uint id in GetSectorChain(m_FirstMiniFatSectorId))
                m_MiniFatSectorIds.Add(id);
        }

        int count = m_MiniFatSectorIds.Count;
        m_MiniFatCache = new uint[]?[count];
        m_MiniFatDirty = new bool[count];
    }

    uint GetMiniFatEntry(uint miniSectorId)
    {
        EnsureMiniFatReady();
        int fatIdx = (int)(miniSectorId / (uint)m_FatEntriesPerSector);
        int fatOff = (int)(miniSectorId % (uint)m_FatEntriesPerSector);
        if ((uint)fatIdx >= (uint)m_MiniFatSectorIds!.Count)
            return CfbConstants.FreeSectorId;
        return LoadMiniFatSector(fatIdx)[fatOff];
    }

    void SetMiniFatEntry(uint miniSectorId, uint value)
    {
        EnsureMiniFatReady();
        int fatIdx = (int)(miniSectorId / (uint)m_FatEntriesPerSector);
        int fatOff = (int)(miniSectorId % (uint)m_FatEntriesPerSector);
        LoadMiniFatSector(fatIdx)[fatOff] = value;
        m_MiniFatDirty![fatIdx] = true;
        m_Dirty = true;
    }

    uint[] LoadMiniFatSector(int fatIdx)
    {
        ref uint[]? cached = ref m_MiniFatCache![fatIdx];
        if (cached is not null)
            return cached;

        byte[] buf = new byte[SectorSize];
        m_Stream.Position = GetSectorOffset(m_MiniFatSectorIds![fatIdx]);
        m_Stream.ReadExactly(buf);

        uint[] data = new uint[m_FatEntriesPerSector];
        for (int i = 0; i < m_FatEntriesPerSector; i++)
            data[i] = BinaryPrimitives.ReadUInt32LittleEndian(buf.AsSpan(i * 4));

        return cached = data;
    }

    // -------------------------------------------------------------------------
    // Mini-FAT chain traversal
    // -------------------------------------------------------------------------

    public IEnumerable<uint> GetMiniSectorChain(uint firstMiniSectorId)
    {
        uint id = firstMiniSectorId;
        while (id < CfbConstants.DifatSectorId)
        {
            yield return id;
            id = GetMiniFatEntry(id);
        }
    }

    public uint GetNextMiniSector(uint miniSectorId) =>
        GetMiniFatEntry(miniSectorId) is uint next && next < CfbConstants.DifatSectorId
            ? next
            : CfbConstants.EndOfChainSectorId;

    // -------------------------------------------------------------------------
    // Mini-stream root chain (lazy)
    // -------------------------------------------------------------------------

    uint[] GetRootSectorChain()
    {
        if (m_RootSectorChain is null)
        {
            var root = GetRootEntry();
            m_RootSectorChain = [.. GetSectorChain(root.StartSectorId)];
        }
        return m_RootSectorChain;
    }

    // -------------------------------------------------------------------------
    // Lazy directory access
    // -------------------------------------------------------------------------

    List<uint> GetOrLoadDirChain()
    {
        if (m_DirChain is not null)
            return m_DirChain;

        m_DirChain = [];
        if (m_FirstDirectorySectorId < CfbConstants.DifatSectorId)
        {
            foreach (uint id in GetSectorChain(m_FirstDirectorySectorId))
                m_DirChain.Add(id);
        }
        return m_DirChain;
    }

    /// <summary>
    /// Returns the directory entry for <paramref name="entryId"/>,
    /// loading it lazily from disk if not already cached.
    /// Returns <see langword="null"/> for out-of-range IDs or unallocated entries.
    /// </summary>
    CfbEntry? GetEntry(uint entryId)
    {
        if (entryId == CfbConstants.NoStream)
            return null;

        if (m_DirCache.TryGetValue(entryId, out var cached))
            return cached.Type == CfbEntryType.Unallocated ? null : cached;

        var entry = ReadDirectoryEntry(entryId);
        if (entry is null)
            return null;

        m_DirCache[entryId] = entry;
        return entry.Type == CfbEntryType.Unallocated ? null : entry;
    }

    CfbEntry? ReadDirectoryEntry(uint entryId)
    {
        var dirChain = GetOrLoadDirChain();
        int sectorIdx = (int)(entryId / (uint)m_DirEntriesPerSector);
        int slotIdx = (int)(entryId % (uint)m_DirEntriesPerSector);

        if (sectorIdx >= dirChain.Count)
            return null;

        byte[] buf = new byte[CfbConstants.DirectoryEntrySize];
        m_Stream.Position = GetSectorOffset(dirChain[sectorIdx]) + (long)slotIdx * CfbConstants.DirectoryEntrySize;
        m_Stream.ReadExactly(buf);
        return ParseDirectoryEntry(buf, 0, entryId);
    }

    public CfbEntry GetRootEntry()
    {
        const uint rootId = CfbConstants.RootEntryId;

        if (m_DirCache.TryGetValue(rootId, out var cached))
            return cached;

        var entry = ReadDirectoryEntry(rootId)
            ?? throw new InvalidDataException("MS-CFB compound file has no directory entries.");

        m_DirCache[rootId] = entry;
        return entry;
    }

    static CfbEntry ParseDirectoryEntry(byte[] data, int offset, uint id)
    {
        var entry = new CfbEntry { Id = id };

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
        entry.Clsid = new Guid(data.AsSpan(offset + 80, 16).ToArray());
        entry.StateBits = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset + 96));
        entry.CreationTime = FileTimeToDateTime(BinaryPrimitives.ReadInt64LittleEndian(data.AsSpan(offset + 100)));
        entry.ModificationTime = FileTimeToDateTime(BinaryPrimitives.ReadInt64LittleEndian(data.AsSpan(offset + 108)));
        entry.StartSectorId = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset + 116));
        entry.Size = BinaryPrimitives.ReadInt64LittleEndian(data.AsSpan(offset + 120));

        return entry;
    }

    static DateTime FileTimeToDateTime(long fileTime)
    {
        if (fileTime <= 0)
            return DateTime.MinValue;
        try { return DateTime.FromFileTimeUtc(fileTime); }
        catch (ArgumentOutOfRangeException) { return DateTime.MinValue; }
    }

    // -------------------------------------------------------------------------
    // Directory access - enumeration and search
    // -------------------------------------------------------------------------

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

        foreach (var e in WalkSiblingSubtree(entry.LeftSiblingId))
            yield return e;

        yield return entry;

        foreach (var e in WalkSiblingSubtree(entry.RightSiblingId))
            yield return e;
    }

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
        if (cmp == 0) return entry;
        if (cmp < 0) return SearchSiblingSubtree(entry.LeftSiblingId, name);
        return SearchSiblingSubtree(entry.RightSiblingId, name);
    }

    // -------------------------------------------------------------------------
    // Stream opening - read
    // -------------------------------------------------------------------------

    public Stream OpenReadStream(CfbEntry entry)
    {
        if (entry.Type != CfbEntryType.Stream)
            throw new InvalidOperationException("Entry is not a stream.");

        if (entry.PendingData is not null)
            return new MemoryStream(entry.PendingData, writable: false);

        return new CfbFileStream(this, entry);
    }

    // -------------------------------------------------------------------------
    // Stream opening - write
    // -------------------------------------------------------------------------

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
            Span<byte> buf = stackalloc byte[CfbConstants.MiniSectorSize];
            foreach (uint id in GetMiniSectorChain(entry.StartSectorId))
            {
                ReadMiniSector(id, buf);
                int toCopy = Math.Min(CfbConstants.MiniSectorSize, size - bytesRead);
                buf[..toCopy].CopyTo(data.AsSpan(bytesRead));
                bytesRead += toCopy;
                if (bytesRead >= size) break;
            }
        }
        else
        {
            Span<byte> buf = stackalloc byte[SectorSize];
            foreach (uint id in GetSectorChain(entry.StartSectorId))
            {
                ReadFatSector(id, buf);
                int toCopy = Math.Min(SectorSize, size - bytesRead);
                buf[..toCopy].CopyTo(data.AsSpan(bytesRead));
                bytesRead += toCopy;
                if (bytesRead >= size) break;
            }
        }

        return data;
    }

    // -------------------------------------------------------------------------
    // Write support - directory entry persistence
    // -------------------------------------------------------------------------

    /// <summary>
    /// Writes <paramref name="entry"/> to its position in the directory chain.
    /// Extends the chain if <paramref name="entry"/>'s ID is beyond the current capacity.
    /// Updates the cache.
    /// </summary>
    void WriteDirectoryEntry(CfbEntry entry)
    {
        var dirChain = GetOrLoadDirChain();
        int sectorIdx = (int)(entry.Id / (uint)m_DirEntriesPerSector);
        int slotIdx = (int)(entry.Id % (uint)m_DirEntriesPerSector);

        while (sectorIdx >= dirChain.Count)
            ExtendDirChain(dirChain);

        byte[] buf = new byte[CfbConstants.DirectoryEntrySize];
        SerializeDirectoryEntry(buf, 0, entry);
        m_Stream.Position = GetSectorOffset(dirChain[sectorIdx]) + (long)slotIdx * CfbConstants.DirectoryEntrySize;
        m_Stream.Write(buf, 0, buf.Length);

        m_DirCache[entry.Id] = entry;
    }

    /// <summary>
    /// Writes <paramref name="entry"/> to disk immediately
    /// (write-through for timestamp and metadata changes).
    /// </summary>
    public void WriteModifiedEntry(CfbEntry entry)
    {
        WriteDirectoryEntry(entry);
        m_Dirty = true;
    }

    void ExtendDirChain(List<uint> dirChain)
    {
        uint newSectorId = AllocateSector();
        // AllocateSector sets FAT entry to EndOfChain; link previous last sector.
        if (dirChain.Count > 0)
        {
            SetFatEntry(dirChain[^1], newSectorId);
        }
        else
        {
            m_FirstDirectorySectorId = newSectorId;
            m_HeaderDirty = true;
        }

        dirChain.Add(newSectorId);

        // Initialize the new sector with unallocated entries.
        byte[] buf = new byte[SectorSize];
        for (int i = 0; i < m_DirEntriesPerSector; i++)
        {
            int off = i * CfbConstants.DirectoryEntrySize;
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(off + 68), CfbConstants.NoStream);
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(off + 72), CfbConstants.NoStream);
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(off + 76), CfbConstants.NoStream);
        }
        m_Stream.Position = GetSectorOffset(newSectorId);
        m_Stream.Write(buf, 0, buf.Length);
    }

    uint FindFreeDirectorySlot()
    {
        // Check cache first.
        foreach (var kv in m_DirCache)
        {
            if (kv.Value.Type == CfbEntryType.Unallocated)
                return kv.Key;
        }

        // Scan directory sectors not yet in cache.
        var dirChain = GetOrLoadDirChain();
        uint capacity = (uint)(dirChain.Count * m_DirEntriesPerSector);
        for (uint id = 0; id < capacity; id++)
        {
            if (m_DirCache.ContainsKey(id))
                continue;
            var entry = ReadDirectoryEntry(id);
            if (entry is null || entry.Type == CfbEntryType.Unallocated)
            {
                if (entry is not null)
                    m_DirCache[id] = entry;
                return id;
            }
            m_DirCache[id] = entry;
        }

        // Extend directory chain to get more slots.
        ExtendDirChain(dirChain);
        return capacity; // first slot of the new sector
    }

    // -------------------------------------------------------------------------
    // Write support - entry mutation
    // -------------------------------------------------------------------------

    public CfbEntry AddChild(CfbEntry parent, string name, CfbEntryType type)
    {
        uint newId = FindFreeDirectorySlot();

        var entry = new CfbEntry
        {
            Id = newId,
            Name = name,
            Type = type,
            Color = 1,
            CreationTime = DateTime.UtcNow,
            ModificationTime = DateTime.UtcNow,
        };

        m_DirCache[newId] = entry;

        var childIds = EnumerateChildren(parent).Select(e => e.Id).ToList();
        childIds.Add(newId);
        parent.ChildId = BuildSiblingTree(childIds);
        parent.ModificationTime = DateTime.UtcNow;

        // Write new entry and all siblings whose pointers were updated by BuildSiblingTree.
        foreach (uint id in childIds)
            WriteDirectoryEntry(m_DirCache[id]);
        WriteDirectoryEntry(parent);

        m_Dirty = true;
        return entry;
    }

    public void RemoveChild(CfbEntry parent, CfbEntry entry)
    {
        // Free the entry's data sectors before severing the directory link.
        if (entry.Type == CfbEntryType.Stream && entry.StartSectorId < CfbConstants.DifatSectorId)
        {
            bool isMini = entry.Size > 0 && entry.Size < CfbConstants.MiniStreamCutOffSize;
            if (isMini)
                FreeMiniSectorChain(entry.StartSectorId);
            else
                FreeSectorChain(entry.StartSectorId);
        }

        var childIds = EnumerateChildren(parent)
            .Select(e => e.Id)
            .Where(id => id != entry.Id)
            .ToList();
        parent.ChildId = BuildSiblingTree(childIds);
        parent.ModificationTime = DateTime.UtcNow;

        foreach (uint id in childIds)
            WriteDirectoryEntry(m_DirCache[id]);
        WriteDirectoryEntry(parent);

        // Mark entry as unallocated and write it.
        entry.Type = CfbEntryType.Unallocated;
        entry.Name = string.Empty;
        entry.LeftSiblingId = CfbConstants.NoStream;
        entry.RightSiblingId = CfbConstants.NoStream;
        entry.ChildId = CfbConstants.NoStream;
        entry.PendingData = null;
        entry.StartSectorId = CfbConstants.EndOfChainSectorId;
        entry.Size = 0;
        WriteDirectoryEntry(entry);

        m_Dirty = true;
    }

    // ---- BST helpers ----

    uint BuildSiblingTree(List<uint> ids)
    {
        if (ids.Count == 0)
            return CfbConstants.NoStream;

        ids.Sort((a, b) => CfbEntry.CompareName(
            m_DirCache[a].Name.AsSpan(),
            m_DirCache[b].Name.AsSpan()));

        return BuildBalancedBst(ids, 0, ids.Count - 1);
    }

    uint BuildBalancedBst(List<uint> ids, int lo, int hi)
    {
        if (lo > hi)
            return CfbConstants.NoStream;

        int mid = (lo + hi) / 2;
        uint nodeId = ids[mid];
        var node = m_DirCache[nodeId];
        node.Color = 1; // black
        node.LeftSiblingId = BuildBalancedBst(ids, lo, mid - 1);
        node.RightSiblingId = BuildBalancedBst(ids, mid + 1, hi);
        return nodeId;
    }

    // -------------------------------------------------------------------------
    // Write support - sector allocation
    // -------------------------------------------------------------------------

    /// <summary>
    /// Allocates a free sector, sets its FAT entry to <see cref="CfbConstants.EndOfChainSectorId"/>
    /// as a placeholder, extends the underlying stream if needed, and returns the sector ID.
    /// </summary>
    uint AllocateSector()
    {
        for (int fatIdx = 0; fatIdx < m_FatSectorIds.Count; fatIdx++)
        {
            uint[] sector = LoadFatSector(fatIdx);
            for (int j = 0; j < m_FatEntriesPerSector; j++)
            {
                if (sector[j] == CfbConstants.FreeSectorId)
                {
                    uint newId = (uint)(fatIdx * m_FatEntriesPerSector + j);
                    sector[j] = CfbConstants.EndOfChainSectorId;
                    m_FatDirty[fatIdx] = true;
                    m_Dirty = true;
                    EnsureStreamCoversAtLeast(GetSectorOffset(newId) + SectorSize);
                    return newId;
                }
            }
        }

        // No free sectors: add a new FAT sector and retry.
        AddFatSector();
        return AllocateSector();
    }

    void AddFatSector()
    {
        // Place the new FAT sector at the logical end of the file.
        long fileLen = Math.Max(m_Stream.Length, CfbConstants.HeaderSize);
        long excess = (fileLen - CfbConstants.HeaderSize) % SectorSize;
        if (excess != 0)
            fileLen += SectorSize - excess;

        uint newFatSectorId = (uint)((fileLen - CfbConstants.HeaderSize) / SectorSize);
        m_Stream.SetLength(fileLen + SectorSize);

        // Initialize: all entries free.
        uint[] newData = new uint[m_FatEntriesPerSector];
        Array.Fill(newData, CfbConstants.FreeSectorId);
        byte[] buf = new byte[SectorSize];
        for (int i = 0; i < m_FatEntriesPerSector; i++)
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(i * 4), CfbConstants.FreeSectorId);
        m_Stream.Position = GetSectorOffset(newFatSectorId);
        m_Stream.Write(buf, 0, buf.Length);

        int newFatIdx = m_FatSectorIds.Count;
        m_FatSectorIds.Add(newFatSectorId);
        if (newFatIdx >= m_FatCache.Length)
        {
            int capacity = newFatIdx * 2;
            Array.Resize(ref m_FatCache, capacity);
            Array.Resize(ref m_FatDirty, capacity);
        }
        m_FatCache[newFatIdx] = newData;

        // Register the new FAT sector in the DIFAT.
        RegisterFatSectorInDifat(newFatIdx, newFatSectorId);

        // Mark this sector as a FAT sector in the FAT itself.
        SetFatEntry(newFatSectorId, CfbConstants.FatSectorId);
    }

    void RegisterFatSectorInDifat(int fatSectorIndex, uint fatSectorPhysId)
    {
        if (fatSectorIndex < CfbConstants.HeaderDifatCount)
        {
            m_HeaderDifat[fatSectorIndex] = fatSectorPhysId;
            m_HeaderDirty = true;
            return;
        }

        // Extended DIFAT sector required.
        int relIdx = fatSectorIndex - CfbConstants.HeaderDifatCount;
        int entriesPerDifat = SectorSize / 4 - 1;
        int difatSectorIndex = relIdx / entriesPerDifat;
        int difatSlotIndex = relIdx % entriesPerDifat;

        if (difatSectorIndex >= (int)m_DifatSectorCount)
        {
            // Allocate a new DIFAT extension sector.
            uint newDifatSectorId = AllocateSector();
            SetFatEntry(newDifatSectorId, CfbConstants.DifatSectorId);

            byte[] difatBuf = new byte[SectorSize];
            // Fill all slots with FreeSectorId, next-pointer = EndOfChain.
            for (int i = 0; i < SectorSize / 4; i++)
                BinaryPrimitives.WriteUInt32LittleEndian(difatBuf.AsSpan(i * 4), CfbConstants.FreeSectorId);
            BinaryPrimitives.WriteUInt32LittleEndian(difatBuf.AsSpan(SectorSize - 4), CfbConstants.EndOfChainSectorId);
            m_Stream.Position = GetSectorOffset(newDifatSectorId);
            m_Stream.Write(difatBuf, 0, difatBuf.Length);

            // Link previous last DIFAT sector to the new one.
            if (m_DifatSectorCount == 0)
            {
                m_FirstDifatSectorId = newDifatSectorId;
            }
            else
            {
                // Walk to the last DIFAT sector and update its next pointer.
                uint prevDifatId = m_FirstDifatSectorId;
                for (uint i = 1; i < m_DifatSectorCount; i++)
                {
                    byte[] tmp = new byte[4];
                    m_Stream.Position = GetSectorOffset(prevDifatId) + SectorSize - 4;
                    m_Stream.ReadExactly(tmp);
                    prevDifatId = BinaryPrimitives.ReadUInt32LittleEndian(tmp);
                }
                byte[] nextSlot = new byte[4];
                BinaryPrimitives.WriteUInt32LittleEndian(nextSlot, newDifatSectorId);
                m_Stream.Position = GetSectorOffset(prevDifatId) + SectorSize - 4;
                m_Stream.Write(nextSlot, 0, nextSlot.Length);
            }

            m_DifatSectorCount++;
            m_HeaderDirty = true;
        }

        // Write the FAT sector ID into the appropriate DIFAT sector slot.
        uint difatSectorPhysId = GetDifatSectorPhysId(difatSectorIndex);
        byte[] slot = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(slot, fatSectorPhysId);
        m_Stream.Position = GetSectorOffset(difatSectorPhysId) + difatSlotIndex * 4;
        m_Stream.Write(slot, 0, slot.Length);
    }

    uint GetDifatSectorPhysId(int difatSectorIndex)
    {
        uint id = m_FirstDifatSectorId;
        for (int i = 0; i < difatSectorIndex; i++)
        {
            byte[] tmp = new byte[4];
            m_Stream.Position = GetSectorOffset(id) + SectorSize - 4;
            m_Stream.ReadExactly(tmp);
            id = BinaryPrimitives.ReadUInt32LittleEndian(tmp);
        }
        return id;
    }

    void FreeSectorChain(uint firstSectorId)
    {
        uint id = firstSectorId;
        while (id < CfbConstants.DifatSectorId)
        {
            uint next = GetFatEntry(id);
            SetFatEntry(id, CfbConstants.FreeSectorId);
            id = next;
        }
    }

    void FreeMiniSectorChain(uint firstMiniSectorId)
    {
        uint id = firstMiniSectorId;
        while (id < CfbConstants.DifatSectorId)
        {
            uint next = GetMiniFatEntry(id);
            SetMiniFatEntry(id, CfbConstants.FreeSectorId);
            id = next;
        }
    }

    // -------------------------------------------------------------------------
    // Write support - sector chain allocation
    // -------------------------------------------------------------------------

    /// <summary>
    /// Allocates a contiguous (logically-chained) set of FAT sectors for <paramref name="data"/>,
    /// writes the data (with padding to sector boundaries), and returns the first sector ID.
    /// </summary>
    uint AllocateRegularSectorChain(byte[] data)
    {
        int count = CeilDiv(data.Length, SectorSize);
        uint[] chain = new uint[count];
        for (int i = 0; i < count; i++)
            chain[i] = AllocateSector();

        for (int i = 0; i < count - 1; i++)
            SetFatEntry(chain[i], chain[i + 1]);
        // chain[count-1] already = EndOfChain from AllocateSector.

        for (int i = 0; i < count; i++)
        {
            m_Stream.Position = GetSectorOffset(chain[i]);
            int srcOff = i * SectorSize;
            int toCopy = Math.Min(SectorSize, data.Length - srcOff);
            m_Stream.Write(data, srcOff, toCopy);
            if (toCopy < SectorSize)
            {
                // Pad remainder with zeros.
                byte[] pad = new byte[SectorSize - toCopy];
                m_Stream.Write(pad, 0, pad.Length);
            }
        }

        return chain[0];
    }

    /// <summary>
    /// Allocates mini-sectors for <paramref name="data"/>, writes to the mini-stream container,
    /// and returns the first mini-sector ID.
    /// </summary>
    uint AllocateMiniSectorChain(byte[] data)
    {
        int count = CeilDiv(data.Length, CfbConstants.MiniSectorSize);
        uint[] chain = new uint[count];
        for (int i = 0; i < count; i++)
            chain[i] = AllocateMiniSector();

        for (int i = 0; i < count - 1; i++)
            SetMiniFatEntry(chain[i], chain[i + 1]);
        SetMiniFatEntry(chain[count - 1], CfbConstants.EndOfChainSectorId);

        for (int i = 0; i < count; i++)
        {
            int srcOff = i * CfbConstants.MiniSectorSize;
            int toCopy = Math.Min(CfbConstants.MiniSectorSize, data.Length - srcOff);
            WriteMiniSector(chain[i], data, srcOff, toCopy);
        }

        return chain[0];
    }

    uint AllocateMiniSector()
    {
        EnsureMiniFatReady();

        for (int fatIdx = 0; fatIdx < m_MiniFatSectorIds!.Count; fatIdx++)
        {
            uint[] sector = LoadMiniFatSector(fatIdx);
            for (int j = 0; j < m_FatEntriesPerSector; j++)
            {
                if (sector[j] == CfbConstants.FreeSectorId)
                {
                    uint newId = (uint)(fatIdx * m_FatEntriesPerSector + j);
                    sector[j] = CfbConstants.EndOfChainSectorId;
                    m_MiniFatDirty![fatIdx] = true;
                    m_Dirty = true;
                    EnsureMiniStreamCovers(newId);
                    return newId;
                }
            }
        }

        // No free mini-sector: extend mini-FAT and mini-stream container.
        AddMiniFatSector();
        return AllocateMiniSector();
    }

    void AddMiniFatSector()
    {
        EnsureMiniFatReady();

        uint newSectorId = AllocateSector();
        // Link to end of mini-FAT chain.
        if (m_MiniFatSectorIds!.Count > 0)
            SetFatEntry(m_MiniFatSectorIds[^1], newSectorId);
        else
        {
            m_FirstMiniFatSectorId = newSectorId;
            m_HeaderDirty = true;
        }
        // newSectorId FAT entry is already EndOfChain (from AllocateSector).

        // Initialize all entries to free.
        uint[] newData = new uint[m_FatEntriesPerSector];
        Array.Fill(newData, CfbConstants.FreeSectorId);
        byte[] buf = new byte[SectorSize];
        for (int i = 0; i < m_FatEntriesPerSector; i++)
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(i * 4), CfbConstants.FreeSectorId);
        m_Stream.Position = GetSectorOffset(newSectorId);
        m_Stream.Write(buf, 0, buf.Length);

        int newIdx = m_MiniFatSectorIds.Count;
        m_MiniFatSectorIds.Add(newSectorId);
        Array.Resize(ref m_MiniFatCache, newIdx + 1);
        Array.Resize(ref m_MiniFatDirty, newIdx + 1);
        m_MiniFatCache![newIdx] = newData;
        // Not dirty: just written.

        m_MiniFatSectorCount++;
        m_HeaderDirty = true;
    }

    void EnsureMiniStreamCovers(uint miniSectorId)
    {
        long requiredBytes = ((long)miniSectorId + 1) * CfbConstants.MiniSectorSize;
        var root = GetRootEntry();

        if (root.Size >= requiredBytes)
            return;

        // Determine how many regular sectors the current container occupies.
        long currentContainerCapacity = root.StartSectorId < CfbConstants.DifatSectorId
            ? (long)GetSectorChain(root.StartSectorId).Count() * SectorSize
            : 0L;

        if (requiredBytes <= currentContainerCapacity)
        {
            // Mini-sectors fit within existing container sectors; just update root size.
            root.Size = requiredBytes;
            m_RootSectorChain = null;
            WriteDirectoryEntry(root);
            return;
        }

        // Need additional regular sectors for the mini-stream container.
        int extraSectors = CeilDiv((int)(requiredBytes - currentContainerCapacity), SectorSize);
        uint lastSector = CfbConstants.EndOfChainSectorId;

        if (root.StartSectorId >= CfbConstants.DifatSectorId)
        {
            // Root has no sectors yet.
            uint first = AllocateSector();
            root.StartSectorId = first;
            lastSector = first;
            extraSectors--;
        }
        else
        {
            foreach (uint id in GetSectorChain(root.StartSectorId))
                lastSector = id;
        }

        for (int i = 0; i < extraSectors; i++)
        {
            uint newSector = AllocateSector();
            SetFatEntry(lastSector, newSector);
            lastSector = newSector;
        }

        root.Size = requiredBytes;
        m_RootSectorChain = null;
        WriteDirectoryEntry(root);
    }

    void WriteMiniSector(uint miniSectorId, byte[] data, int srcOffset, int count)
    {
        long off = (long)miniSectorId * CfbConstants.MiniSectorSize;
        int containerSectorIndex = (int)(off / SectorSize);
        int containerSectorOffset = (int)(off % SectorSize);

        uint[] rootChain = GetRootSectorChain();
        if (containerSectorIndex >= rootChain.Length)
            throw new InvalidOperationException("Mini-sector is beyond the mini-stream container.");

        m_Stream.Position = GetSectorOffset(rootChain[containerSectorIndex]) + containerSectorOffset;
        m_Stream.Write(data, srcOffset, count);
        if (count < CfbConstants.MiniSectorSize)
        {
            byte[] pad = new byte[CfbConstants.MiniSectorSize - count];
            m_Stream.Write(pad, 0, pad.Length);
        }
    }

    // -------------------------------------------------------------------------
    // Write support - flush
    // -------------------------------------------------------------------------

    public void MarkDirty() => m_Dirty = true;

    public void Flush()
    {
        if (!m_Writable || !m_Dirty)
            return;

        // 1. Commit pending stream data (written by CfbWriteStream on close).
        foreach (var entry in m_DirCache.Values.ToList())
        {
            if (entry.PendingData is not null)
                CommitPendingData(entry);
        }

        // 2. Flush dirty mini-FAT sectors.
        FlushDirtyMiniFatSectors();

        // 3. Flush dirty FAT sectors.
        FlushDirtyFatSectors();

        // 4. Re-write header if FAT sector count or first-sector pointers changed.
        if (m_HeaderDirty)
            WriteHeader();

        m_Stream.Flush();
        m_Dirty = false;
    }

    void CommitPendingData(CfbEntry entry)
    {
        byte[] data = entry.PendingData!;

        bool isMiniNow = entry.StartSectorId < CfbConstants.DifatSectorId
                         && entry.Size > 0
                         && entry.Size < CfbConstants.MiniStreamCutOffSize
                         && entry.Type == CfbEntryType.Stream;

        // Free old sectors.
        if (entry.StartSectorId < CfbConstants.DifatSectorId)
        {
            if (isMiniNow)
                FreeMiniSectorChain(entry.StartSectorId);
            else
                FreeSectorChain(entry.StartSectorId);
        }

        // Allocate new sectors and write data.
        if (data.Length == 0)
        {
            entry.StartSectorId = CfbConstants.EndOfChainSectorId;
            entry.Size = 0;
        }
        else if (data.Length < CfbConstants.MiniStreamCutOffSize)
        {
            entry.StartSectorId = AllocateMiniSectorChain(data);
            entry.Size = data.Length;
        }
        else
        {
            entry.StartSectorId = AllocateRegularSectorChain(data);
            entry.Size = data.Length;
        }

        entry.PendingData = null;
        WriteDirectoryEntry(entry);
    }

    void FlushDirtyFatSectors()
    {
        for (int i = 0; i < m_FatDirty.Length; i++)
        {
            if (!m_FatDirty[i])
                continue;
            WriteFatSectorToStream(m_FatSectorIds[i], m_FatCache[i]!);
            m_FatDirty[i] = false;
        }
    }

    void FlushDirtyMiniFatSectors()
    {
        if (m_MiniFatDirty is null)
            return;
        for (int i = 0; i < m_MiniFatDirty.Length; i++)
        {
            if (!m_MiniFatDirty[i])
                continue;
            WriteFatSectorToStream(m_MiniFatSectorIds![i], m_MiniFatCache![i]!);
            m_MiniFatDirty[i] = false;
        }
    }

    void WriteFatSectorToStream(uint physSectorId, uint[] entries)
    {
        byte[] buf = new byte[SectorSize];
        for (int j = 0; j < m_FatEntriesPerSector; j++)
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(j * 4), entries[j]);
        m_Stream.Position = GetSectorOffset(physSectorId);
        m_Stream.Write(buf, 0, buf.Length);
    }

    void WriteHeader()
    {
        byte[] h = new byte[CfbConstants.HeaderSize];

        CfbConstants.Signature.CopyTo(h);
        BinaryPrimitives.WriteUInt16LittleEndian(h.AsSpan(24), 0x003E); // minor version
        BinaryPrimitives.WriteUInt16LittleEndian(h.AsSpan(26), 3);       // major version V3
        BinaryPrimitives.WriteUInt16LittleEndian(h.AsSpan(28), 0xFFFE);  // byte order
        BinaryPrimitives.WriteUInt16LittleEndian(h.AsSpan(30), m_SectorShift);
        BinaryPrimitives.WriteUInt16LittleEndian(h.AsSpan(32), m_MiniSectorShift);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(44), (uint)m_FatSectorIds.Count);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(48), m_FirstDirectorySectorId);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(56), CfbConstants.MiniStreamCutOffSize);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(60), m_FirstMiniFatSectorId);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(64), m_MiniFatSectorCount);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(68),
            m_DifatSectorCount > 0 ? m_FirstDifatSectorId : CfbConstants.EndOfChainSectorId);
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(72), m_DifatSectorCount);

        for (int i = 0; i < CfbConstants.HeaderDifatCount; i++)
            BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(76 + i * 4), m_HeaderDifat[i]);

        m_Stream.Position = 0;
        m_Stream.Write(h, 0, h.Length);
        m_HeaderDirty = false;
    }

    // -------------------------------------------------------------------------
    // Minimal compound file initialization (Create / empty Open)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Writes a valid minimal V3 compound file to the underlying stream.
    /// Layout: sector 0 = directory, sector 1 = FAT.
    /// </summary>
    void WriteMinimalCompoundFile()
    {
        const ushort sectorShift = 9;     // 512-byte sectors
        const ushort miniShift = 6;     // 64-byte mini-sectors
        const int ss = 1 << sectorShift; // 512

        // Sector 0: directory (root entry)
        // Sector 1: FAT sector
        const uint dirSectorId = 0;
        const uint fatSectorId = 1;

        // ---- Header ----
        byte[] header = new byte[CfbConstants.HeaderSize];
        CfbConstants.Signature.CopyTo(header);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(24), 0x003E);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(26), 3);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(28), 0xFFFE);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(30), sectorShift);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(32), miniShift);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(44), 1);         // 1 FAT sector
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(48), dirSectorId);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(56), CfbConstants.MiniStreamCutOffSize);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(60), CfbConstants.EndOfChainSectorId); // no mini-FAT
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(64), 0);         // mini-FAT sector count
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(68), CfbConstants.EndOfChainSectorId); // no DIFAT ext
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(72), 0);         // DIFAT ext sector count
        // Header DIFAT[0] = fatSectorId, rest = FreeSectorId
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(76), fatSectorId);
        for (int i = 1; i < CfbConstants.HeaderDifatCount; i++)
            BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(76 + i * 4), CfbConstants.FreeSectorId);

        // ---- Sector 0: Directory ----
        byte[] dirSector = new byte[ss];
        // Root entry.
        CfbEntry root = new()
        {
            Id = CfbConstants.RootEntryId,
            Name = "Root Entry",
            Type = CfbEntryType.Root,
            Color = 1,
            StartSectorId = CfbConstants.EndOfChainSectorId,
            CreationTime = DateTime.UtcNow,
            ModificationTime = DateTime.UtcNow,
        };
        SerializeDirectoryEntry(dirSector, 0, root);
        // Remaining 3 slots are unallocated (zeros + NoStream sibling IDs).
        int slotSize = CfbConstants.DirectoryEntrySize;
        for (int i = 1; i < ss / slotSize; i++)
        {
            int off = i * slotSize;
            BinaryPrimitives.WriteUInt32LittleEndian(dirSector.AsSpan(off + 68), CfbConstants.NoStream);
            BinaryPrimitives.WriteUInt32LittleEndian(dirSector.AsSpan(off + 72), CfbConstants.NoStream);
            BinaryPrimitives.WriteUInt32LittleEndian(dirSector.AsSpan(off + 76), CfbConstants.NoStream);
        }

        // ---- Sector 1: FAT ----
        byte[] fatSector = new byte[ss];
        // FAT[0] = EndOfChain (directory chain, single sector)
        // FAT[1] = FatSectorId  (this FAT sector)
        // FAT[2..] = FreeSectorId
        BinaryPrimitives.WriteUInt32LittleEndian(fatSector.AsSpan(0 * 4), CfbConstants.EndOfChainSectorId);
        BinaryPrimitives.WriteUInt32LittleEndian(fatSector.AsSpan(1 * 4), CfbConstants.FatSectorId);
        for (int i = 2; i < ss / 4; i++)
            BinaryPrimitives.WriteUInt32LittleEndian(fatSector.AsSpan(i * 4), CfbConstants.FreeSectorId);

        // ---- Write to stream ----
        m_Stream.SetLength(CfbConstants.HeaderSize + 2L * ss);
        m_Stream.Position = 0;
        m_Stream.Write(header, 0, header.Length);
        m_Stream.Write(dirSector, 0, dirSector.Length);
        m_Stream.Write(fatSector, 0, fatSector.Length);
        m_Stream.Flush();
    }

    // -------------------------------------------------------------------------
    // Directory entry serialization
    // -------------------------------------------------------------------------

    static void SerializeDirectoryEntry(byte[] buf, int offset, CfbEntry entry)
    {
        if (entry.Type == CfbEntryType.Unallocated)
        {
            Array.Clear(buf, offset, CfbConstants.DirectoryEntrySize);
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
        BinaryPrimitives.WriteInt64LittleEndian(buf.AsSpan(offset + 120), entry.Size);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

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

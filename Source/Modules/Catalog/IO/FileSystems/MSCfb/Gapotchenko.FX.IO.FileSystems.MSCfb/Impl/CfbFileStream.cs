// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;

/// <summary>
/// Provides a seekable stream over an MS-CFB entry's data.
/// Supports both regular FAT-chained streams and mini-streams, and both read and write access.
/// Data is read and written directly to the underlying sector storage - no in-memory buffering.
/// </summary>
sealed class CfbFileStream : Stream
{
    readonly CfbContext m_Context;
    readonly CfbEntry m_Entry;
    readonly FileAccess m_Access;

    long m_Size;
    bool m_IsMiniStream;

    // Sector navigation state.
    uint m_StartSectorId;
    uint m_SectorId;
    int m_SectorIndex;

    long m_Position;

    // True when m_Size or m_StartSectorId has changed and the directory entry needs updating.
    bool m_SizeDirty;

    // Read-only constructor (used by OpenReadStream).
    public CfbFileStream(CfbContext context, CfbEntry entry)
        : this(context, entry, FileAccess.Read, 0)
    {
    }

    // Full constructor used by OpenWriteStream.
    internal CfbFileStream(CfbContext context, CfbEntry entry, FileAccess access, long position = 0)
    {
        m_Context = context;
        m_Entry = entry;
        m_Access = access;
        m_Size = entry.Size;
        m_IsMiniStream =
            entry.Type == CfbEntryType.Stream &&
            entry.Size < CfbConstants.MiniStreamCutOffSize;

        m_StartSectorId = entry.StartSectorId;
        m_SectorId = m_StartSectorId;
        m_Position = position;
    }

    public override bool CanRead => m_Access != FileAccess.Write;
    public override bool CanSeek => true;
    public override bool CanWrite => m_Access != FileAccess.Read;
    public override long Length => m_Size;

    public override long Position
    {
        get => m_Position;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            m_Position = value;
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if (offset < 0 || count < 0 || offset + count > buffer.Length)
            throw new ArgumentException("Invalid buffer arguments.");

        if (!CanRead)
            throw new NotSupportedException("The stream does not support reading.");

        long remaining = m_Size - m_Position;
        if (remaining <= 0)
            return 0;

        int toRead = (int)Math.Min(count, remaining);
        int bytesRead = 0;
        int sectorSize = m_IsMiniStream ? CfbConstants.MiniSectorSize : m_Context.SectorSize;
        byte[] sectorBuffer = new byte[sectorSize];

        while (bytesRead < toRead)
        {
            int sectorIndex = (int)(m_Position / sectorSize);
            int sectorOffset = (int)(m_Position % sectorSize);

            NavigateToSector(sectorIndex, allocate: false);

            if (m_SectorId >= CfbConstants.DifatSectorId)
                break;

            if (m_IsMiniStream)
                m_Context.ReadMiniSector(m_SectorId, sectorBuffer);
            else
                m_Context.ReadFatSector(m_SectorId, sectorBuffer);

            int available = sectorSize - sectorOffset;
            int toCopy = Math.Min(available, toRead - bytesRead);
            sectorBuffer.AsSpan(sectorOffset, toCopy).CopyTo(buffer.AsSpan(offset + bytesRead));

            bytesRead += toCopy;
            m_Position += toCopy;
        }

        return bytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if (offset < 0 || count < 0 || offset + count > buffer.Length)
            throw new ArgumentException("Invalid buffer arguments.");

        if (!CanWrite)
            throw new NotSupportedException("The stream does not support writing.");

        if (count == 0)
            return;

        // If this write would push a mini-stream over the cutoff, promote to a regular stream first.
        if (m_IsMiniStream && m_Position + count >= CfbConstants.MiniStreamCutOffSize)
            PromoteToRegularStream();

        int sectorSize = m_IsMiniStream ? CfbConstants.MiniSectorSize : m_Context.SectorSize;
        int written = 0;

        while (written < count)
        {
            int sectorIndex = (int)(m_Position / sectorSize);
            int sectorOffset = (int)(m_Position % sectorSize);

            NavigateToSector(sectorIndex, allocate: true);

            int available = sectorSize - sectorOffset;
            int toCopy = Math.Min(available, count - written);

            if (m_IsMiniStream)
            {
                m_Context.WriteMiniSectorRange(m_SectorId, sectorOffset, buffer, offset + written, toCopy);
            }
            else
            {
                m_Context.Stream.Position = m_Context.GetSectorOffset(m_SectorId) + sectorOffset;
                m_Context.Stream.Write(buffer, offset + written, toCopy);
            }

            written += toCopy;
            m_Position += toCopy;
        }

        if (m_Position > m_Size)
        {
            m_Size = m_Position;
            m_SizeDirty = true;
        }
    }

    public override void SetLength(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        if (!CanWrite)
            throw new NotSupportedException("The stream does not support writing.");

        if (value == m_Size)
            return;

        if (value < m_Size)
            TruncateTo(value);
        // Extension: sectors are allocated lazily on write.

        m_Size = value;
        m_SizeDirty = true;

        if (m_Position > m_Size)
            m_Position = m_Size;
    }

    void TruncateTo(long newSize)
    {
        if (newSize == 0)
        {
            if (m_StartSectorId < CfbConstants.DifatSectorId)
            {
                if (m_IsMiniStream)
                    m_Context.FreeMiniSectorChain(m_StartSectorId);
                else
                    m_Context.FreeSectorChain(m_StartSectorId);

                m_StartSectorId = CfbConstants.EndOfChainSectorId;
                m_SectorId = m_StartSectorId;
                m_SectorIndex = 0;
            }

            // Reset to mini-stream mode - the next write will determine the actual mode.
            m_IsMiniStream = m_Entry.Type == CfbEntryType.Stream;
            return;
        }

        int sectorSize = m_IsMiniStream ? CfbConstants.MiniSectorSize : m_Context.SectorSize;
        int keptSectors = (int)((newSize + sectorSize - 1) / sectorSize);

        NavigateToSector(keptSectors - 1, allocate: false);

        if (m_SectorId >= CfbConstants.DifatSectorId)
            return; // Chain is already shorter than needed.

        uint tail = m_IsMiniStream
            ? m_Context.GetNextMiniSector(m_SectorId)
            : m_Context.GetNextSector(m_SectorId);

        // Cap the chain at the current sector.
        if (m_IsMiniStream)
            m_Context.SetMiniFatEntry(m_SectorId, CfbConstants.EndOfChainSectorId);
        else
            m_Context.SetFatEntry(m_SectorId, CfbConstants.EndOfChainSectorId);

        // Free the rest of the chain beyond the truncation point.
        if (tail < CfbConstants.DifatSectorId)
        {
            if (m_IsMiniStream)
                m_Context.FreeMiniSectorChain(tail);
            else
                m_Context.FreeSectorChain(tail);
        }
    }

    /// <summary>
    /// Promotes this stream from mini-stream storage to regular FAT-chained storage.
    /// Reads all existing mini-sector data, frees the mini chain, and allocates a regular chain.
    /// </summary>
    void PromoteToRegularStream()
    {
        // Read all existing mini data.
        byte[] existingData = new byte[(int)m_Size];
        if (m_Size > 0 && m_StartSectorId < CfbConstants.DifatSectorId)
        {
            int read = 0;
            byte[] sectorBuffer = new byte[CfbConstants.MiniSectorSize];
            foreach (uint miniId in m_Context.GetMiniSectorChain(m_StartSectorId))
            {
                m_Context.ReadMiniSector(miniId, sectorBuffer);
                int toCopy = Math.Min(CfbConstants.MiniSectorSize, existingData.Length - read);
                if (toCopy <= 0)
                    break;
                sectorBuffer.AsSpan(0, toCopy).CopyTo(existingData.AsSpan(read));
                read += toCopy;
            }

            m_Context.FreeMiniSectorChain(m_StartSectorId);
        }

        // Allocate a regular sector chain populated with the existing data.
        m_StartSectorId = existingData.Length > 0
            ? m_Context.AllocateRegularSectorChain(existingData)
            : CfbConstants.EndOfChainSectorId;

        // Reset sector navigation since the chain has changed.
        m_SectorId = m_StartSectorId;
        m_SectorIndex = 0;
        m_IsMiniStream = false;
        m_SizeDirty = true;
    }

    /// <summary>
    /// Advances the internal sector cursor to the sector at <paramref name="targetIndex"/>.
    /// For backward seeks, resets to the start of the chain first.
    /// For forward seeks (including the common sequential case), follows FAT links one step at a time.
    /// When <paramref name="allocate"/> is <see langword="true"/> and the chain ends before the target,
    /// new sectors are allocated and linked automatically.
    /// </summary>
    void NavigateToSector(int targetIndex, bool allocate)
    {
        // If seeking backward, restart from the beginning of the chain.
        if (targetIndex < m_SectorIndex)
        {
            m_SectorIndex = 0;
            m_SectorId = m_StartSectorId;
        }

        // Handle an empty chain: allocate the first sector if requested.
        if (m_StartSectorId >= CfbConstants.DifatSectorId && allocate)
        {
            uint first = m_IsMiniStream
                ? m_Context.AllocateMiniSector()
                : m_Context.AllocateSector();
            m_StartSectorId = first;
            m_SectorId = first;
            m_SectorIndex = 0;
            m_SizeDirty = true;
        }

        // Walk forward to the target sector, one FAT link per step.
        while (m_SectorIndex < targetIndex)
        {
            if (m_SectorId >= CfbConstants.DifatSectorId)
                break;

            uint next = m_IsMiniStream
                ? m_Context.GetNextMiniSector(m_SectorId)
                : m_Context.GetNextSector(m_SectorId);

            if (next >= CfbConstants.DifatSectorId)
            {
                if (!allocate)
                    break;

                // Chain is too short - allocate a new sector and link it.
                next = m_IsMiniStream
                    ? m_Context.AllocateMiniSector()
                    : m_Context.AllocateSector();

                if (m_IsMiniStream)
                    m_Context.SetMiniFatEntry(m_SectorId, next);
                else
                    m_Context.SetFatEntry(m_SectorId, next);

                m_SizeDirty = true;
            }

            m_SectorId = next;
            m_SectorIndex++;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        long newPosition = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => m_Position + offset,
            SeekOrigin.End => m_Size + offset,
            _ => throw new ArgumentException("Invalid seek origin.", nameof(origin))
        };

        if (newPosition < 0)
            throw new IOException("An attempt was made to move the position before the beginning of the stream.");

        m_Position = newPosition;
        return m_Position;
    }

    public override void Flush()
    {
        if (!m_SizeDirty)
            return;

        m_Entry.Size = m_Size;
        m_Entry.StartSectorId = m_StartSectorId;
        m_Context.WriteDirectoryEntry(m_Entry);
        m_SizeDirty = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Flush();
        base.Dispose(disposing);
    }
}

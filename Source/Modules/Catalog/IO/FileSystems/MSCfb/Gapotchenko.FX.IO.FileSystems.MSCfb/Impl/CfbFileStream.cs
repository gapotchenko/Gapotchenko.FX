// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;

/// <summary>
/// Provides a seekable read-only stream over an MS-CFB entry's data.
/// Supports both regular FAT-chained streams and mini-streams.
/// </summary>
sealed class CfbFileStream : Stream
{
    readonly CfbContext m_Context;
    readonly long m_Size;
    readonly bool m_IsMiniStream;

    // Sector navigation state.
    readonly uint m_StartSectorId;
    uint m_SectorId;
    int m_SectorIndex;

    long m_Position;

    public CfbFileStream(CfbContext context, CfbEntry entry)
    {
        m_Context = context;
        m_Size = entry.Size;
        m_IsMiniStream =
            entry.Type == CfbEntryType.Stream &&
            entry.Size < CfbConstants.MiniStreamCutOffSize;

        m_StartSectorId = entry.StartSectorId;
        m_SectorId = m_StartSectorId;
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
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

        long remaining = m_Size - m_Position;
        if (remaining <= 0)
            return 0;

        int toRead = (int)Math.Min(count, remaining);
        int bytesRead = 0;
        int sectorSize = m_IsMiniStream ? CfbConstants.MiniSectorSize : m_Context.SectorSize;

        Span<byte> sectorBuffer = stackalloc byte[sectorSize];

        while (bytesRead < toRead)
        {
            int sectorIndex = (int)(m_Position / sectorSize);
            int sectorOffset = (int)(m_Position % sectorSize);

            NavigateToSector(sectorIndex);

            if (m_SectorId >= CfbConstants.DifatSectorId)
                break;

            if (m_IsMiniStream)
                m_Context.ReadMiniSector(m_SectorId, sectorBuffer);
            else
                m_Context.ReadFatSector(m_SectorId, sectorBuffer);

            int available = sectorSize - sectorOffset;
            int toCopy = Math.Min(available, toRead - bytesRead);
            sectorBuffer.Slice(sectorOffset, toCopy).CopyTo(buffer.AsSpan(offset + bytesRead));

            bytesRead += toCopy;
            m_Position += toCopy;
        }

        return bytesRead;
    }

    /// <summary>
    /// Advances the internal sector cursor to the sector at <paramref name="targetIndex"/>.
    /// For backward seeks, resets to the start of the chain first.
    /// For forward seeks (including the common sequential case), follows FAT links one step at a time.
    /// </summary>
    void NavigateToSector(int targetIndex)
    {
        // If seeking backward, restart from the beginning of the chain.
        if (targetIndex < m_SectorIndex)
        {
            m_SectorIndex = 0;
            m_SectorId = m_StartSectorId;
        }

        // Walk forward to the target sector, one FAT link per step.
        while (m_SectorIndex < targetIndex && m_SectorId < CfbConstants.DifatSectorId)
        {
            m_SectorId = m_IsMiniStream
                ? m_Context.GetNextMiniSector(m_SectorId)
                : m_Context.GetNextSector(m_SectorId);
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

    public override void Flush() { }

    public override void SetLength(long value) =>
        throw new NotSupportedException("The stream does not support writing.");

    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException("The stream does not support writing.");
}

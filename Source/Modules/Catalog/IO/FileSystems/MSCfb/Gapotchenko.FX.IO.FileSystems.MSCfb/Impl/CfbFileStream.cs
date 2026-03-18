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
    readonly uint[] m_SectorChain;
    readonly bool m_IsMiniStream;

    long m_Position;

    public CfbFileStream(CfbContext context, CfbEntry entry)
    {
        m_Context = context;
        m_Size = entry.Size;
        m_IsMiniStream =
            entry.Type == CfbEntryType.Stream &&
            entry.Size < CfbConstants.MiniStreamCutOffSize;

        m_SectorChain = m_IsMiniStream
            ? context.GetMiniSectorChain(entry.StartSectorId)
            : context.GetSectorChain(entry.StartSectorId);
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

        while (bytesRead < toRead)
        {
            int sectorIndex = (int)(m_Position / sectorSize);
            int sectorOffset = (int)(m_Position % sectorSize);

            if (sectorIndex >= m_SectorChain.Length)
                break;

            ReadOnlySpan<byte> sectorData = m_IsMiniStream
                ? m_Context.ReadMiniSector(m_SectorChain[sectorIndex])
                : m_Context.ReadFatSector(m_SectorChain[sectorIndex]);

            int available = sectorSize - sectorOffset;
            int toCopy = Math.Min(available, toRead - bytesRead);
            sectorData.Slice(sectorOffset, toCopy).CopyTo(buffer.AsSpan(offset + bytesRead));

            bytesRead += toCopy;
            m_Position += toCopy;
        }

        return bytesRead;
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

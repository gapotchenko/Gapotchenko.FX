// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;

/// <summary>
/// Provides a writable, seekable, readable in-memory stream for an MS-CFB stream entry.
/// On dispose, the buffered data is committed to the entry's <see cref="CfbDirectoryEntry.PendingData"/>.
/// </summary>
sealed class CfbWriteStream : Stream
{
    readonly CfbDirectoryEntry m_Entry;
    readonly MemoryStream m_Inner;
    bool m_Disposed;

    public CfbWriteStream(CfbDirectoryEntry entry, MemoryStream inner)
    {
        m_Entry = entry;
        m_Inner = inner;
    }

    public override bool CanRead => !m_Disposed;
    public override bool CanWrite => !m_Disposed;
    public override bool CanSeek => !m_Disposed;

    public override long Length => m_Inner.Length;

    public override long Position
    {
        get => m_Inner.Position;
        set => m_Inner.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count) => m_Inner.Read(buffer, offset, count);

    public override void Write(byte[] buffer, int offset, int count) => m_Inner.Write(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => m_Inner.Seek(offset, origin);

    public override void SetLength(long value) => m_Inner.SetLength(value);

    public override void Flush() => m_Inner.Flush();

    protected override void Dispose(bool disposing)
    {
        if (disposing && !m_Disposed)
        {
            m_Disposed = true;
            byte[] data = m_Inner.ToArray();
            m_Entry.PendingData = data;
            m_Entry.Size = data.Length;
            m_Entry.ModificationTime = DateTime.UtcNow;
        }

        base.Dispose(disposing);
    }
}

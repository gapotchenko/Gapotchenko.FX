using System.Diagnostics;

namespace Gapotchenko.FX.Memory;

// This class is designed to be made public if a necessity arises.
// Some additional refinements may be required for that to happen.

/// <summary>
/// Represents a stream whose backing store is <see cref="ReadOnlyMemory{T}"/>.
/// </summary>
class ReadOnlyMemoryBufferStream : Stream, IHasReadOnlyMemory<byte>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyMemoryBufferStream"/> class
    /// based on the specified memory buffer.
    /// </summary>
    /// <param name="buffer">The memory buffer from which to create this stream.</param>
    public ReadOnlyMemoryBufferStream(ReadOnlyMemory<byte> buffer) :
        this(buffer, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyMemoryBufferStream"/> class
    /// based on the specified memory buffer
    /// the ability to call <see cref="GetBuffer"/> set as specified.
    /// </summary>
    /// <param name="buffer">The memory buffer from which to create this stream.</param>
    /// <param name="publiclyVisible">
    /// <see langword="true"/> to enable <see cref="GetBuffer"/>,
    /// which returns the <see cref="Memory{T}"/> buffer from which the stream was created;
    /// otherwise, <see langword="false"/>.
    /// </param>
    public ReadOnlyMemoryBufferStream(ReadOnlyMemory<byte> buffer, bool publiclyVisible)
    {
        m_Buffer = buffer;
        m_PubliclyVisible = publiclyVisible;
    }

    readonly ReadOnlyMemory<byte> m_Buffer;
    readonly bool m_PubliclyVisible;

    /// <summary>
    /// Gets the <see cref="ReadOnlyMemory{T}"/> buffer from which the stream was created.
    /// </summary>
    /// <returns>The <see cref="ReadOnlyMemory{T}"/> buffer from which the stream was created.</returns>
    public ReadOnlyMemory<byte> GetBuffer()
    {
        if (!m_PubliclyVisible)
            StreamHelpers.ThrowNotPubliclyVisible();

        return m_Buffer;
    }

    /// <summary>
    /// Tries to get the <see cref="ReadOnlyMemory{T}"/> buffer from which the stream was created.
    /// </summary>
    /// <param name="buffer">The <see cref="ReadOnlyMemory{T}"/> buffer from which the stream was created.</param>
    /// <returns><see langword="true"/> if the buffer is publicly visible; otherwise, <see langword="false"/>.</returns>
    public bool TryGetBuffer(out ReadOnlyMemory<byte> buffer)
    {
        if (m_PubliclyVisible)
        {
            buffer = m_Buffer;
            return true;
        }
        else
        {
            buffer = default;
            return false;
        }
    }

    ReadOnlyMemory<byte> IHasReadOnlyMemory<byte>.GetMemory() => GetBuffer();

    bool IHasReadOnlyMemory<byte>.TryGetMemory(out ReadOnlyMemory<byte> memory)
    {
        bool hasBuffer = TryGetBuffer(out var buffer);
        memory = buffer;
        return hasBuffer;
    }

    /// <inheritdoc/>
    public override bool CanRead => m_IsOpen;

    /// <inheritdoc/>
    public override bool CanSeek => m_IsOpen;

    /// <inheritdoc/>
    public override bool CanWrite => false;

    /// <inheritdoc/>
    public sealed override long Length => m_Buffer.Length;

    /// <inheritdoc/>
    public override void SetLength(long value) => StreamHelpers.ThrowNotWritable();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Position;

    /// <inheritdoc/>
    public override long Position
    {
        get
        {
            EnsureOpen();

            return m_Position;
        }
        set
        {
            StreamHelpers.ValidatePosition(value);

            EnsureOpen();

            m_Position = StreamHelpers.SetPosition(value);
        }
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        EnsureOpen();

        return m_Position = StreamHelpers.Seek(offset, origin, m_Position, m_Buffer.Length);
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        StreamHelpers.ValidateBuffer(buffer, offset, count);

        return Read(buffer.AsSpan(offset, count));
    }

#if TFF_MEMORY && !TFF_MEMORY_OOB
    /// <inheritdoc/>
    public override
#endif
    int Read(Span<byte> buffer)
    {
        EnsureOpen();

        int byteCount = Math.Min(m_Buffer.Length - m_Position, buffer.Length);
        if (byteCount <= 0)
            return 0;

        m_Buffer.Span.Slice(m_Position, byteCount).CopyTo(buffer);
        m_Position += byteCount;

        return byteCount;
    }

    /// <inheritdoc/>
    public override int ReadByte()
    {
        EnsureOpen();

        if (m_Position < m_Buffer.Length)
            return m_Buffer.Span[m_Position++];
        else
            return -1;
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count) => StreamHelpers.ThrowNotWritable();

#if TFF_MEMORY && !TFF_MEMORY_OOB
    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer) => StreamHelpers.ThrowNotWritable();
#endif

    /// <inheritdoc/>
    public override void WriteByte(byte value) => StreamHelpers.ThrowNotWritable();

    /// <inheritdoc/>
    public override void Flush()
    {
    }

    bool m_IsOpen = true;

    void EnsureOpen()
    {
        if (!m_IsOpen)
            StreamHelpers.ThrowClosedAndDisposed();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            m_IsOpen = false;

        base.Dispose(disposing);
    }
}

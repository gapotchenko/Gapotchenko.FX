using System.Diagnostics;

namespace Gapotchenko.FX.Memory;

// This class is designed to be made public if a necessity arises.
// Some additional refinements may be required for that to happen.

/// <summary>
/// Represents a stream whose backing store is <see cref="Memory{T}"/>.
/// </summary>
sealed class MemoryBufferStream : Stream, IHasMemory<byte>, IHasReadOnlyMemory<byte>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryBufferStream"/> class
    /// with an expandable capacity initialized to zero.
    /// </summary>
    public MemoryBufferStream() :
        this(0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryBufferStream"/> class
    /// with a specified expandable capacity.
    /// </summary>
    /// <param name="capacity">The initial size of the memory buffer in bytes.</param>
    public MemoryBufferStream(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");

        m_Buffer = capacity == 0 ? Memory<byte>.Empty : new byte[capacity].AsMemory();
        m_Capacity = capacity;
        m_Expandable = true;
        m_Writeable = true;
        m_PubliclyVisible = true;
    }

    /// <summary>
    /// Initializes a new non-resizable instance of the <see cref="MemoryBufferStream"/> class
    /// based on the specified memory buffer
    /// with the <see cref="CanWrite"/> property set to <see langword="true"/>.
    /// </summary>
    /// <param name="buffer">The memory buffer from which to create this stream.</param>
    public MemoryBufferStream(Memory<byte> buffer) :
        this(buffer, true)
    {
    }

    /// <summary>
    /// Initializes a new non-resizable instance of the <see cref="MemoryBufferStream"/> class
    /// based on the specified memory buffer
    /// with the <see cref="CanWrite"/> property set as specified.
    /// </summary>
    /// <param name="buffer">The memory buffer from which to create this stream.</param>
    /// <param name="writeable">
    /// The settings of the <see cref="CanWrite"/> property, 
    /// which determines whether the stream supports writing.
    /// </param>
    public MemoryBufferStream(Memory<byte> buffer, bool writeable) :
        this(buffer, writeable, false)
    {
    }

    /// <summary>
    /// Initializes a new non-resizable instance of the <see cref="MemoryBufferStream"/> class
    /// based on the specified memory buffer,
    /// with the <see cref="CanWrite"/> property set as specified,
    /// and the ability to call <see cref="GetBuffer"/> set as specified.
    /// </summary>
    /// <param name="buffer">The memory buffer from which to create this stream.</param>
    /// <param name="writeable">
    /// The settings of the <see cref="CanWrite"/> property, 
    /// which determines whether the stream supports writing.
    /// </param>
    /// <param name="publiclyVisible">
    /// <see langword="true"/> to enable <see cref="GetBuffer"/>,
    /// which returns the <see cref="Memory{T}"/> buffer from which the stream was created;
    /// otherwise, <see langword="false"/>.
    /// </param>
    public MemoryBufferStream(Memory<byte> buffer, bool writeable, bool publiclyVisible)
    {
        m_Buffer = buffer;
        m_Writeable = writeable;
        m_PubliclyVisible = publiclyVisible;

        m_Length = m_Capacity = buffer.Length;
    }

    Memory<byte> m_Buffer;
    bool m_Expandable;
    bool m_Writeable;
    readonly bool m_PubliclyVisible;

    /// <summary>
    /// Gets the <see cref="Memory{T}"/> buffer from which the stream was created.
    /// </summary>
    /// <returns>The <see cref="Memory{T}"/> buffer from which the stream was created.</returns>
    public Memory<byte> GetBuffer()
    {
        if (!m_PubliclyVisible)
            StreamHelpers.ThrowNotPubliclyVisible();

        return m_Buffer;
    }

    /// <summary>
    /// Tries to get the <see cref="Memory{T}"/> buffer from which the stream was created.
    /// </summary>
    /// <param name="buffer">The <see cref="Memory{T}"/> buffer from which the stream was created.</param>
    /// <returns><see langword="true"/> if the buffer is publicly visible; otherwise, <see langword="false"/>.</returns>
    public bool TryGetBuffer(out Memory<byte> buffer)
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

    Memory<byte> IHasMemory<byte>.GetMemory()
    {
        EnsureWriteable();
        return GetBuffer();
    }

    bool IHasMemory<byte>.TryGetMemory(out Memory<byte> memory)
    {
        if (m_Writeable && TryGetBuffer(out var buffer))
        {
            memory = buffer;
            return true;
        }
        else
        {
            memory = default;
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
    public override bool CanWrite => m_Writeable;

    void EnsureWriteable()
    {
        if (!CanWrite)
            StreamHelpers.ThrowNotWritable();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Length;

    /// <inheritdoc/>
    public sealed override long Length => m_Length;

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        if (value is < 0 or > int.MaxValue)
            StreamHelpers.ThrowLengthMustBeNonNegativeAndLessThan2GB(nameof(value));

        EnsureWriteable();

        var length = m_Length;

        int newLength = (int)value;
        if (!EnsureCapacity(newLength) && newLength > length)
        {
            // Clear up the reclaimed part of the buffer.
            m_Buffer.Span[length..newLength].Clear();
        }

        m_Length = newLength;
        m_Position = Math.Min(m_Position, newLength); // avoiding seek after end
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int m_Capacity;

    /// <summary>
    /// Gets or sets the number of bytes allocated for this stream.
    /// </summary>
    public int Capacity
    {
        get
        {
            EnsureOpen();
            return m_Capacity;
        }
        set
        {
            var length = m_Length;
            if (value < length)
                throw new ArgumentOutOfRangeException(nameof(value), "Capacity cannot be less than the current stream length.");

            EnsureOpen();

            if (value == m_Capacity)
                return;

            if (!m_Expandable)
                StreamHelpers.ThrowNotExpandable();

            if (value > 0)
            {
                var newBuffer = new byte[value];
                if (length > 0)
                    m_Buffer[..length].CopyTo(newBuffer.AsMemory());
                m_Buffer = newBuffer.AsMemory();
            }
            else
            {
                m_Buffer = null;
            }

            m_Capacity = value;
        }
    }

    bool EnsureCapacity(int desiredCapacity)
    {
        if (desiredCapacity < 0)
            StreamHelpers.ThrowTooLong();

        if (TryExpandCapacity(desiredCapacity, m_Capacity) is not -1 and var newCapacity)
        {
            // Apply the new capacity.
            Capacity = newCapacity;
            return true;
        }
        else
        {
            // Capacity is not expanded.
            return false;
        }
    }

    static int TryExpandCapacity(int desiredCapacity, int existingCapacity)
    {
        if (desiredCapacity <= existingCapacity)
            return -1; // no need to expand

        uint extent = (uint)existingCapacity * 2; // using uint to avoid overflow

        int arrayMaxLength = ArrayHelpers.ArrayMaxLength;
        if (extent > arrayMaxLength)
        {
            // The capacity should be <= Array.MaxLength, but the user should be able to override that.
            return Math.Max(desiredCapacity, arrayMaxLength);
        }
        else
        {
            const int minCapacity = 256;
            return Math.Max(Math.Max(desiredCapacity, minCapacity), (int)extent);
        }
    }

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

        return m_Position = StreamHelpers.Seek(offset, origin, m_Position, m_Length);
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

        int byteCount = Math.Min(m_Length - m_Position, buffer.Length);
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

        if (m_Position < m_Length)
            return m_Buffer.Span[m_Position++];
        else
            return -1;
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        StreamHelpers.ValidateBuffer(buffer, offset, count);

        Write(buffer.AsSpan(offset, count));
    }

#if TFF_MEMORY && !TFF_MEMORY_OOB
    /// <inheritdoc/>
    public override
#endif
    void Write(ReadOnlySpan<byte> buffer)
    {
        EnsureOpen();
        EnsureWriteable();

        int count = buffer.Length;

        int position = m_Position;
        int newPosition = position + count;

        EnsureLength(newPosition);

        buffer.CopyTo(m_Buffer.Span.Slice(position, count));
        m_Position = newPosition;
    }

    /// <inheritdoc/>
    public override void WriteByte(byte value)
    {
        EnsureOpen();
        EnsureWriteable();

        int position = m_Position;
        int newPosition = position + 1;

        EnsureLength(newPosition);

        m_Buffer.Span[position] = value;
        m_Position = newPosition;
    }

    void EnsureLength(int newLength)
    {
        if (newLength < 0)
            StreamHelpers.ThrowTooLong();

        int length = m_Length;
        if (newLength > length)
        {
            if (!EnsureCapacity(newLength))
            {
                int position = m_Position;
                if (position > length)
                    m_Buffer.Span[length..position].Clear();
            }

            m_Length = newLength;
        }
    }

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
        {
            m_IsOpen = false;
            m_Writeable = false;
            m_Expandable = false;
        }

        base.Dispose(disposing);
    }
}

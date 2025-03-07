// Based on the work "Improving on .NET Memory Management for Large Objects" by Michael Sydney Balloni
// https://www.codeproject.com/Tips/894885/Improving-on-NET-Memory-Management-for-Large-Objec

using System.Diagnostics;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Creates a stream that can store a large amount of data in memory under fragmentation conditions.
/// </summary>
/// <remarks>
/// <see cref="FragmentedMemoryStream"/> is similar to <see cref="MemoryStream"/> but it uses a dynamic list of relatively small memory blocks as its backing store.
/// This enables a more efficient usage of the memory address space, as there is no need to allocate a potentially large contiguous memory block for the whole stream.
/// </remarks>
public class FragmentedMemoryStream : Stream
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FragmentedMemoryStream"/> class.
    /// </summary>
    public FragmentedMemoryStream()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FragmentedMemoryStream"/> class.
    /// </summary>
    /// <param name="buffer">The array of bytes from which to create the current stream.</param>
    public FragmentedMemoryStream(byte[] buffer)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));

        Write(buffer, 0, buffer.Length);
        Position = 0;
    }

    /// <inheritdoc/>
    public override bool CanRead => true;

    /// <inheritdoc/>
    public override bool CanSeek => true;

    /// <inheritdoc/>
    public override bool CanWrite => true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    long m_Length;

    /// <inheritdoc/>
    public override long Length => m_Length;

    /// <inheritdoc/>
    public override long Position { get; set; }

    const int BlockSize = 65536;

    readonly List<byte[]> m_Blocks = [];

    /// <summary>
    /// The block of memory currently addressed by Position
    /// </summary>
    byte[] CurrentBlock
    {
        get
        {
            while (m_Blocks.Count <= CurrentBlockIndex)
                m_Blocks.Add(new byte[BlockSize]);
            return m_Blocks[(int)CurrentBlockIndex];
        }
    }

    /// <summary>
    /// The index of a block currently addressed by <see cref="Position"/>.
    /// </summary>
    long CurrentBlockIndex => Position / BlockSize;

    /// <summary>
    /// The block offset of a byte currently addressed by <see cref="Position"/>.
    /// </summary>
    int CurrentBlockOffset => (int)(Position % BlockSize);

    /// <inheritdoc/>
    public override void Flush()
    {
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Buffer offset cannot be negative.");
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Count cannot be negative.");

        return ReadCore(buffer.AsSpan(offset, count));
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc/>
    public override int Read(Span<byte> buffer) => ReadCore(buffer);
#endif

    int ReadCore(Span<byte> buffer)
    {
        int offset = 0;
        int count = (int)Math.Min(buffer.Length, m_Length - Position);

        int read = 0;
        while (count > 0)
        {
            int currentBlockOffset = CurrentBlockOffset;
            int copySize = Math.Min(count, BlockSize - currentBlockOffset);

            CurrentBlock
                .AsSpan(currentBlockOffset, copySize)
                .CopyTo(buffer[offset..]);

            count -= copySize;
            offset += copySize;

            read += copySize;
            Position += copySize;
        }

        return read;
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                Position = offset;
                break;
            case SeekOrigin.Current:
                Position += offset;
                break;
            case SeekOrigin.End:
                Position = Length - offset;
                break;
        }
        return Position;
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        m_Length = value;
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Buffer offset cannot be negative.");
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Count cannot be negative.");

        WriteCore(buffer.AsSpan(offset, count));
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer) => WriteCore(buffer);
#endif

    void WriteCore(ReadOnlySpan<byte> buffer)
    {
        int count = buffer.Length;
        int offset = 0;

        long savedPosition = Position;
        try
        {
            while (count > 0)
            {
                int currentBlockOffset = CurrentBlockOffset;
                int copySize = Math.Min(count, BlockSize - currentBlockOffset);

                EnsureCapacity(Position + copySize);

                buffer
                    .Slice(offset, copySize)
                    .CopyTo(CurrentBlock.AsSpan(currentBlockOffset));

                count -= copySize;
                offset += copySize;

                Position += copySize;
            }
        }
        catch
        {
            Position = savedPosition;
            throw;
        }
    }

    /// <inheritdoc/>
    public override int ReadByte()
    {
        if (Position >= m_Length)
            return -1;

        byte b = CurrentBlock[CurrentBlockOffset];
        ++Position;

        return b;
    }

    /// <inheritdoc/>
    public override void WriteByte(byte value)
    {
        EnsureCapacity(Position + 1);
        CurrentBlock[CurrentBlockOffset] = value;
        ++Position;
    }

    void EnsureCapacity(long capacity)
    {
        if (capacity > m_Length)
            m_Length = capacity;
    }

    /// <summary>
    /// Returns the entire contents of the stream as a byte array.
    /// </summary>
    /// <remarks>
    /// This operation is not optimal due to the fact that a contiguous array allocation may fail when the stream is large enough and memory address space is too fragmented.
    /// Instead, use methods that operate directly on the stream whenever possible.
    /// </remarks>
    /// <returns>A byte array containing the current data of the stream.</returns>
    public virtual byte[] ToArray()
    {
        long savedPosition = Position;
        Position = 0;

        long length = Length;
        byte[] buffer = new byte[length];
        if (length <= int.MaxValue)
        {
            int r = ReadCore(buffer);
            Debug.Assert(r == length);
        }
        else
        {
            // Stream is larger than 2 GB.
            long r = ReadLargeCore(buffer);
            Debug.Assert(r == length);
        }

        Position = savedPosition;

        return buffer;
    }

    long ReadLargeCore(byte[] buffer)
    {
        long offset = 0;
        long count = Math.Min(buffer.LongLength, m_Length - Position);

        long read = 0;
        while (count > 0)
        {
            int currentBlockOffset = CurrentBlockOffset;
            long copySize = Math.Min(count, BlockSize - currentBlockOffset);

            Array.Copy(CurrentBlock, currentBlockOffset, buffer, offset, copySize);

            count -= copySize;
            offset += copySize;

            read += copySize;
            Position += copySize;
        }

        return read;
    }

    /// <inheritdoc cref="MemoryStream.WriteTo(Stream)"/>
    public virtual void WriteTo(Stream destination)
    {
        // This method is needed to mimic the interface of MemoryStream
        // (to be a drop-in replacement).

        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        long savedPosition = Position;
        Position = 0;
        try
        {
            CopyTo(destination);
        }
        finally
        {
            Position = savedPosition;
        }
    }
}

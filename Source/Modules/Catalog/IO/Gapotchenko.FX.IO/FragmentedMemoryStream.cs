// Based on the work "Improving on .NET Memory Management for Large Objects" by Michael Sydney Balloni
// https://www.codeproject.com/Tips/894885/Improving-on-NET-Memory-Management-for-Large-Objec

using Gapotchenko.FX.IO.Properties;
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
        ArgumentNullException.ThrowIfNull(buffer);

        WriteCore(buffer);
        m_Position = 0;
    }

    /// <inheritdoc/>
    public override bool CanRead => true;

    /// <inheritdoc/>
    public override bool CanSeek => true;

    /// <inheritdoc/>
    public override bool CanWrite => true;

    /// <inheritdoc/>
    public override long Length => m_Length;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    long m_Length;

    /// <inheritdoc/>
    public override long Position
    {
        get => m_Position;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            m_Position = value;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    long m_Position;

    const int BlockSize = 65536;

    readonly List<byte[]> m_Blocks = [];

    /// <summary>
    /// Gets the block of memory currently addressed by the stream position.
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
    /// Gets the index of a block currently addressed by the stream position.
    /// </summary>
    long CurrentBlockIndex => m_Position / BlockSize;

    /// <summary>
    /// Gets the block offset of a byte currently addressed by the stream position.
    /// </summary>
    int CurrentBlockOffset => (int)(m_Position % BlockSize);

    /// <inheritdoc/>
    public override void Flush()
    {
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        return ReadCore(buffer.AsSpan(offset, count));
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc/>
    public override int Read(Span<byte> buffer) => ReadCore(buffer);
#endif

    int ReadCore(Span<byte> buffer)
    {
        int offset = 0;
        int count = (int)Math.Min(buffer.Length, m_Length - m_Position);

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
            m_Position += copySize;
        }

        return read;
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        long newPosition =
            origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => m_Position + offset,
                SeekOrigin.End => m_Length + offset,
                _ => throw new ArgumentException(Resources.InvalidStreamSeekOrigin, nameof(origin)),
            };

        if (newPosition < 0)
            throw new IOException(Resources.StreamSeekBeforeBegin);
        m_Position = newPosition;

        return newPosition;
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        if (value < m_Length)
        {
            int blocksNeeded = value == 0 ? 0 : checked((int)((value - 1) / BlockSize + 1));

            // Trim excess blocks.
            while (m_Blocks.Count > blocksNeeded)
                m_Blocks.RemoveAt(m_Blocks.Count - 1);

            // Zero out the unused tail of the last partial block so that a
            // subsequent re-expansion of the stream sees clean bytes.
            if (blocksNeeded > 0 && m_Blocks.Count == blocksNeeded)
            {
                int lastBlockUsed = (int)(value % BlockSize);
                if (lastBlockUsed > 0)
                    Array.Clear(m_Blocks[blocksNeeded - 1], lastBlockUsed, BlockSize - lastBlockUsed);
            }
        }

        m_Length = value;
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

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

        long savedPosition = m_Position;
        try
        {
            while (count > 0)
            {
                int currentBlockOffset = CurrentBlockOffset;
                int copySize = Math.Min(count, BlockSize - currentBlockOffset);

                EnsureCapacity(m_Position + copySize);

                buffer
                    .Slice(offset, copySize)
                    .CopyTo(CurrentBlock.AsSpan(currentBlockOffset));

                count -= copySize;
                offset += copySize;

                m_Position += copySize;
            }
        }
        catch
        {
            m_Position = savedPosition;
            throw;
        }
    }

    /// <inheritdoc/>
    public override int ReadByte()
    {
        if (m_Position >= m_Length)
            return -1;

        byte b = CurrentBlock[CurrentBlockOffset];
        ++m_Position;

        return b;
    }

    /// <inheritdoc/>
    public override void WriteByte(byte value)
    {
        long newPosition = m_Position + 1;
        EnsureCapacity(newPosition);
        CurrentBlock[CurrentBlockOffset] = value;
        m_Position = newPosition;
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
        long length = Length;
        byte[] buffer = new byte[length];

        long savedPosition = m_Position;
        m_Position = 0;

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

        m_Position = savedPosition;

        return buffer;
    }

    long ReadLargeCore(byte[] buffer)
    {
        long offset = 0;
        long count = Math.Min(buffer.LongLength, m_Length - m_Position);

        long read = 0;
        while (count > 0)
        {
            int currentBlockOffset = CurrentBlockOffset;
            long copySize = Math.Min(count, BlockSize - currentBlockOffset);

            Array.Copy(CurrentBlock, currentBlockOffset, buffer, offset, copySize);

            count -= copySize;
            offset += copySize;

            read += copySize;
            m_Position += copySize;
        }

        return read;
    }

    /// <inheritdoc cref="MemoryStream.WriteTo(Stream)"/>
    public virtual void WriteTo(Stream destination)
    {
        // This method is needed to mimic the interface of MemoryStream
        // (to be a drop-in replacement).

        ArgumentNullException.ThrowIfNull(destination);

        long savedPosition = m_Position;
        m_Position = 0;
        try
        {
            CopyTo(destination);
        }
        finally
        {
            m_Position = savedPosition;
        }
    }
}

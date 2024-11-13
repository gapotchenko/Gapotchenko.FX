﻿// Based on the work "Improving on .NET Memory Management for Large Objects" by Michael Sydney Balloni
// https://www.codeproject.com/Tips/894885/Improving-on-NET-Memory-Management-for-Large-Objec

using System.Diagnostics;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Creates a stream that can store large amount of data in memory under fragmentation conditions.
/// </summary>
/// <remarks>
/// <see cref="FragmentedMemoryStream"/> is similar to <see cref="MemoryStream"/> but it uses a dynamic list of relatively small memory blocks as its backing store.
/// This enables a more efficient usage of the memory address space, as there is no need to allocate a potentially gigantic contiguous memory block for the whole stream.
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

    const long BlockSize = 65536;

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
    long CurrentBlockOffset => Position % BlockSize;

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

        var lcount = (long)count;

        long remaining = m_Length - Position;
        if (lcount > remaining)
            lcount = remaining;

        int read = 0;
        do
        {
            var copySize = Math.Min(lcount, BlockSize - CurrentBlockOffset);
            Buffer.BlockCopy(CurrentBlock, (int)CurrentBlockOffset, buffer, offset, (int)copySize);
            lcount -= copySize;
            offset += (int)copySize;

            read += (int)copySize;
            Position += copySize;

        } while (lcount > 0);

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

        var savedPosition = Position;
        try
        {
            do
            {
                int copySize = Math.Min(count, (int)(BlockSize - CurrentBlockOffset));

                EnsureCapacity(Position + copySize);

                Buffer.BlockCopy(buffer, offset, CurrentBlock, (int)CurrentBlockOffset, copySize);
                count -= copySize;
                offset += copySize;

                Position += copySize;
            }
            while (count > 0);
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
        var savedPosition = Position;
        Position = 0;

        int length = checked((int)Length);
        var buffer = new byte[length];
        int r = Read(buffer, 0, length);
        Debug.Assert(r == length);

        Position = savedPosition;

        return buffer;
    }

    /// <inheritdoc cref="MemoryStream.WriteTo(Stream)"/>
    public virtual void WriteTo(Stream destination)
    {
        // This method is needed to be a drop-in replacement for MemoryStream.

        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        var savedPosition = Position;
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

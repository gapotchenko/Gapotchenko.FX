﻿// Based on the work "Improving on .NET Memory Management for Large Objects" by Michael Sydney Balloni
// https://www.codeproject.com/Tips/894885/Improving-on-NET-Memory-Management-for-Large-Objec

using System.Diagnostics;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Creates a stream that can store large amount of data in random access memory under memory fragmentation conditions.
/// </summary>
/// <remarks>
/// <see cref="FragmentedMemoryStream"/> is similar to <see cref="MemoryStream"/> but uses a dynamic list of byte arrays as a backing store.
/// This allows it to use the memory address space more efficiently, as there is no need to allocate a contiguous memory block for the whole stream.
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

    /// <summary>
    /// Gets a value indicating whether the current stream supports reading.
    /// </summary>
    public override bool CanRead => true;

    /// <summary>
    /// Gets a value indicating whether the current stream supports seeking.
    /// </summary>
    public override bool CanSeek => true;

    /// <summary>
    /// Gets a value indicating whether the current stream supports writing.
    /// </summary>
    public override bool CanWrite => true;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    long m_Length;

    /// <summary>
    /// Gets the length in bytes of the stream.
    /// </summary>
    public override long Length => m_Length;

    /// <summary>
    /// Gets or sets the position within the current stream.
    /// </summary>
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

    /// <summary>
    /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
    /// </summary>
    public override void Flush()
    {
    }

    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
    /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
    /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
    /// <returns>
    /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
    /// </returns>
    /// <exception cref="ArgumentNullException">buffer</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// offset - Buffer offset cannot be negative.
    /// or
    /// count - Count cannot be negative.
    /// </exception>
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

    /// <summary>
    /// Sets the position within the current stream.
    /// </summary>
    /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
    /// <param name="origin">A value of type <see cref="SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
    /// <returns>
    /// The new position within the current stream.
    /// </returns>
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

    /// <summary>
    /// Sets the length of the current stream.
    /// </summary>
    /// <param name="value">The desired length of the current stream in bytes.</param>
    public override void SetLength(long value)
    {
        m_Length = value;
    }

    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
    /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
    /// <param name="count">The number of bytes to be written to the current stream.</param>
    /// <exception cref="ArgumentNullException">buffer</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// offset - Buffer offset cannot be negative.
    /// or
    /// count - Count cannot be negative.
    /// </exception>
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

    /// <summary>
    /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
    /// </summary>
    /// <returns>
    /// The unsigned byte cast to an <see cref="Int32"/>, or <c>-1</c> if at the end of the stream.
    /// </returns>
    public override int ReadByte()
    {
        if (Position >= m_Length)
            return -1;

        byte b = CurrentBlock[CurrentBlockOffset];
        ++Position;

        return b;
    }

    /// <summary>
    /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
    /// </summary>
    /// <param name="value">The byte to write to the stream.</param>
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
    /// This operation is not optimal due to the fact that a contiguous array allocation may fail when the stream is large enough.
    /// Instead, use methods that operate directly on stream whenever possible.
    /// </summary>
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

    /// <summary>
    /// Writes an entire content of the stream to a specified destination.
    /// </summary>
    /// <param name="destination">The destination stream to write the content to.</param>
    public virtual void WriteTo(Stream destination)
    {
        // This method has no sense from the functional composability point of view
        // but is needed to exist in order to be a drop-in replacement for MemoryStream.

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

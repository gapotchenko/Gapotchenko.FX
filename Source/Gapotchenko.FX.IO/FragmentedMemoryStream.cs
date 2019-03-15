// Based on the work "Improving on .NET Memory Management for Large Objects" by Michael Sydney Balloni
// https://www.codeproject.com/Tips/894885/Improving-on-NET-Memory-Management-for-Large-Objec

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.IO
{
    /// <summary>
    /// Creates a stream whose backing store is fragmented memory.
    /// </summary>
    /// <remarks>
    /// <see cref="FragmentedMemoryStream"/> is similar to <see cref="MemoryStream"/> but uses a dynamic list of byte arrays as a backing store.
    /// This allows to use the memory address space more efficiently, as there is no need to allocate a contiguous memory block for the whole stream.
    /// </remarks>
    public class FragmentedMemoryStream : Stream
    {
        public FragmentedMemoryStream()
        {
        }

        public FragmentedMemoryStream(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
            Position = 0;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        long _Length;

        public override long Length => _Length;

        public override long Position { get; set; }

        const long _BlockSize = 65536;

        List<byte[]> _Blocks = new List<byte[]>();

        /// <summary>
        /// The block of memory currently addressed by Position
        /// </summary>
        byte[] _Block
        {
            get
            {
                while (_Blocks.Count <= _BlockID)
                    _Blocks.Add(new byte[_BlockSize]);
                return _Blocks[(int)_BlockID];
            }
        }
        /// <summary>
        /// The id of the block currently addressed by Position
        /// </summary>
        long _BlockID => Position / _BlockSize;

        /// <summary>
        /// The offset of the byte currently addressed by Position, into the block that contains it
        /// </summary>
        long _BlockOffset => Position % _BlockSize;

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Buffer offset cannot be negative.");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Count cannot be negative.");

            var lcount = (long)count;

            long remaining = _Length - Position;
            if (lcount > remaining)
                lcount = remaining;

            int read = 0;
            do
            {
                var copySize = Math.Min(lcount, _BlockSize - _BlockOffset);
                Buffer.BlockCopy(_Block, (int)_BlockOffset, buffer, offset, (int)copySize);
                lcount -= copySize;
                offset += (int)copySize;

                read += (int)copySize;
                Position += copySize;

            } while (lcount > 0);

            return read;

        }

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

        public override void SetLength(long value)
        {
            _Length = value;
        }

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
                    int copySize = Math.Min(count, (int)(_BlockSize - _BlockOffset));

                    _EnsureCapacity(Position + copySize);

                    Buffer.BlockCopy(buffer, offset, _Block, (int)_BlockOffset, copySize);
                    count -= copySize;
                    offset += copySize;

                    Position += copySize;
                } while (count > 0);
            }
            catch
            {
                Position = savedPosition;
                throw;
            }
        }

        public override int ReadByte()
        {
            if (Position >= _Length)
                return -1;

            byte b = _Block[_BlockOffset];
            Position++;

            return b;
        }

        public override void WriteByte(byte value)
        {
            _EnsureCapacity(Position + 1);
            _Block[_BlockOffset] = value;
            Position++;
        }

        void _EnsureCapacity(long capacity)
        {
            if (capacity > _Length)
                _Length = capacity;
        }

        /// <summary>
        /// Returns the entire content of the stream as a byte array. This is not safe because the call to new byte[] may 
        /// fail if the stream is large enough. Where possible use methods which operate on streams directly instead.
        /// </summary>
        /// <returns>A byte[] containing the current data in the stream</returns>
        public virtual byte[] ToArray()
        {
            var savedPosition = Position;
            Position = 0;

            int length = checked((int)Length);
            var buffer = new byte[length];
            Read(buffer, 0, length);

            Position = savedPosition;

            return buffer;
        }

        /// <summary>
        /// Writes the entire stream into destination, regardless of Position, which remains unchanged.
        /// </summary>
        /// <param name="destination">The stream to write the content of this stream to.</param>
        public virtual void WriteTo(Stream destination)
        {
            var savedPosition = Position;
            Position = 0;

            CopyTo(destination);

            Position = savedPosition;
        }
    }
}

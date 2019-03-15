using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.IO
{
    /// <summary>
    /// Reads primitive data types as binary values with a specific bit converter, and supports reading strings in a specific encoding.
    /// </summary>
    public class BitReader : BinaryReader
    {
        [CLSCompliant(false)]
        public BitReader(Stream input, IBitConverter bitConverter) :
            base(input)
        {
            BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }

        [CLSCompliant(false)]
        public BitReader(Stream input, IBitConverter bitConverter, Encoding encoding) :
            base(input, encoding)
        {
            BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }

#if !NET40
        [CLSCompliant(false)]
        public BitReader(Stream input, IBitConverter bitConverter, Encoding encoding, bool leaveOpen) :
            base(input, encoding, leaveOpen)
        {
            BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }
#endif

        /// <summary>
        /// A bit converter instance.
        /// </summary>
        [CLSCompliant(false)]
        protected readonly IBitConverter BitConverter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        byte[] _Buffer = new byte[16];

        void _FillBuffer(int count)
        {
            var stream = BaseStream;
            if (stream == null)
                throw new ObjectDisposedException(null);
            if (count == 1)
            {
                int b = stream.ReadByte();
                if (b == -1)
                    throw new EndOfStreamException();
                _Buffer[0] = (byte)b;
            }
            else
            {
                int offset = 0;
                do
                {
                    int readBytes = stream.Read(_Buffer, offset, count - offset);
                    if (readBytes == 0)
                        throw new EndOfStreamException();
                    offset += readBytes;
                }
                while (offset < count);
            }
        }

        public override short ReadInt16()
        {
            _FillBuffer(2);
            return BitConverter.ToInt16(_Buffer);
        }

        [CLSCompliant(false)]
        public override ushort ReadUInt16()
        {
            _FillBuffer(2);
            return BitConverter.ToUInt16(_Buffer);
        }

        public override int ReadInt32()
        {
            _FillBuffer(4);
            return BitConverter.ToInt32(_Buffer);
        }

        [CLSCompliant(false)]
        public override uint ReadUInt32()
        {
            _FillBuffer(4);
            return BitConverter.ToUInt32(_Buffer);
        }

        public override long ReadInt64()
        {
            _FillBuffer(8);
            return BitConverter.ToInt64(_Buffer);
        }

        [CLSCompliant(false)]
        public override ulong ReadUInt64()
        {
            _FillBuffer(8);
            return BitConverter.ToUInt64(_Buffer);
        }

        public override float ReadSingle()
        {
            _FillBuffer(4);
            return BitConverter.ToSingle(_Buffer);
        }

        public override double ReadDouble()
        {
            _FillBuffer(8);
            return BitConverter.ToDouble(_Buffer);
        }

        public override decimal ReadDecimal()
        {
            _FillBuffer(16);
            return BitConverter.ToDecimal(_Buffer);
        }
    }
}

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
    /// Writes primitive types with a specific bit converter to a stream, and supports writing strings in a specific encoding.
    /// </summary>
    public class BitWriter : BinaryWriter
    {
        [CLSCompliant(false)]
        protected BitWriter(IBitConverter bitConverter)
        {
            BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }

        [CLSCompliant(false)]
        public BitWriter(Stream output, IBitConverter bitConverter) :
            base(output)
        {
            BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }

        [CLSCompliant(false)]
        public BitWriter(Stream output, IBitConverter bitConverter, Encoding encoding) :
            base(output, encoding)
        {
            BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }

#if !NET40

        [CLSCompliant(false)]
        public BitWriter(Stream output, IBitConverter bitConverter, Encoding encoding, bool leaveOpen) :
            base(output, encoding, leaveOpen)
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

        void _WriteBuffer(int count) => OutStream.Write(_Buffer, 0, count);

        public override void Write(short value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(2);
        }

        [CLSCompliant(false)]
        public override void Write(ushort value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(2);
        }

        public override void Write(int value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(4);
        }

        [CLSCompliant(false)]
        public override void Write(uint value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(4);
        }

        public override void Write(long value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(8);
        }

        [CLSCompliant(false)]
        public override void Write(ulong value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(8);
        }

        public override void Write(float value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(4);
        }

        public override void Write(double value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(8);
        }

        public override void Write(decimal value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(16);
        }
    }
}

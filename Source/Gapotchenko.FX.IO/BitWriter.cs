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
        /// <summary>
        /// Initializes a new instance of the <see cref="BitWriter"/> class.
        /// </summary>
        /// <param name="bitConverter">The bit converter to use.</param>
        [CLSCompliant(false)]
        protected BitWriter(IBitConverter bitConverter)
        {
            BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitWriter"/> class.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <param name="bitConverter">The bit converter to use.</param>
        [CLSCompliant(false)]
        public BitWriter(Stream output, IBitConverter bitConverter) :
            base(output)
        {
            BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitWriter"/> class.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <param name="bitConverter">The bit converter to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        [CLSCompliant(false)]
        public BitWriter(Stream output, IBitConverter bitConverter, Encoding encoding) :
            base(output, encoding)
        {
            BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }

#if !NET40
        /// <summary>
        /// Initializes a new instance of the <see cref="BitWriter"/> class.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <param name="bitConverter">The bit converter to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after the <see cref="BitWriter"/> object is disposed; otherwise, <c>false</c>.</param>
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

        /// <summary>
        /// Writes a two-byte signed integer to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte signed integer to write.</param>
        public override void Write(short value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(2);
        }

        /// <summary>
        /// Writes a two-byte unsigned integer to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte unsigned integer to write.</param>
        [CLSCompliant(false)]
        public override void Write(ushort value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(2);
        }

        /// <summary>
        /// Writes a four-byte signed integer to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte signed integer to write.</param>
        public override void Write(int value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(4);
        }

        /// <summary>
        /// Writes a four-byte unsigned integer to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte unsigned integer to write.</param>
        [CLSCompliant(false)]
        public override void Write(uint value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(4);
        }

        /// <summary>
        /// Writes an eight-byte signed integer to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte signed integer to write.</param>
        public override void Write(long value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(8);
        }

        /// <summary>
        /// Writes an eight-byte unsigned integer to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte unsigned integer to write.</param>
        [CLSCompliant(false)]
        public override void Write(ulong value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(8);
        }

        /// <summary>
        /// Writes a four-byte floating-point value to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte floating-point value to write.</param>
        public override void Write(float value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(4);
        }

        /// <summary>
        /// Writes an eight-byte floating-point value to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte floating-point value to write.</param>
        public override void Write(double value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(8);
        }

        /// <summary>
        /// Writes a decimal value to the current stream and advances the stream position by sixteen bytes.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        public override void Write(decimal value)
        {
            BitConverter.FillBytes(value, _Buffer);
            _WriteBuffer(16);
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

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
            m_BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
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
            m_BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
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
            m_BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }

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
            m_BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IBitConverter m_BitConverter;

        /// <summary>
        /// Gets or sets the current bit converter.
        /// </summary>
        [CLSCompliant(false)]
        public IBitConverter BitConverter
        {
            get => m_BitConverter;
            set => m_BitConverter = value ?? throw new ArgumentNullException(nameof(value));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        byte[] m_Buffer = new byte[16];

        void WriteBuffer(int count) => OutStream.Write(m_Buffer, 0, count);

        /// <summary>
        /// Writes a two-byte signed integer to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte signed integer to write.</param>
        public override void Write(short value)
        {
            m_BitConverter.FillBytes(value, m_Buffer);
            WriteBuffer(2);
        }

        /// <summary>
        /// Writes a two-byte unsigned integer to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte unsigned integer to write.</param>
        [CLSCompliant(false)]
        public override void Write(ushort value)
        {
            m_BitConverter.FillBytes(value, m_Buffer);
            WriteBuffer(2);
        }

        /// <summary>
        /// Writes a four-byte signed integer to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte signed integer to write.</param>
        public override void Write(int value)
        {
            m_BitConverter.FillBytes(value, m_Buffer);
            WriteBuffer(4);
        }

        /// <summary>
        /// Writes a four-byte unsigned integer to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte unsigned integer to write.</param>
        [CLSCompliant(false)]
        public override void Write(uint value)
        {
            m_BitConverter.FillBytes(value, m_Buffer);
            WriteBuffer(4);
        }

        /// <summary>
        /// Writes an eight-byte signed integer to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte signed integer to write.</param>
        public override void Write(long value)
        {
            m_BitConverter.FillBytes(value, m_Buffer);
            WriteBuffer(8);
        }

        /// <summary>
        /// Writes an eight-byte unsigned integer to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte unsigned integer to write.</param>
        [CLSCompliant(false)]
        public override void Write(ulong value)
        {
            m_BitConverter.FillBytes(value, m_Buffer);
            WriteBuffer(8);
        }

        /// <summary>
        /// Writes a four-byte floating-point value to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte floating-point value to write.</param>
        public override void Write(float value)
        {
            m_BitConverter.FillBytes(value, m_Buffer);
            WriteBuffer(4);
        }

        /// <summary>
        /// Writes an eight-byte floating-point value to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte floating-point value to write.</param>
        public override void Write(double value)
        {
            m_BitConverter.FillBytes(value, m_Buffer);
            WriteBuffer(8);
        }

        /// <summary>
        /// Writes a decimal value to the current stream and advances the stream position by sixteen bytes.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        public override void Write(decimal value)
        {
            m_BitConverter.FillBytes(value, m_Buffer);
            WriteBuffer(16);
        }
    }
}

using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Reads primitive data types as binary values with a specific bit converter, and supports reading strings in a specific encoding.
/// </summary>
public class BitReader : BinaryReader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BitReader"/> class.
    /// </summary>
    /// <param name="input">The input stream.</param>
    /// <param name="bitConverter">The bit converter to use.</param>
    [CLSCompliant(false)]
    public BitReader(Stream input, IBitConverter bitConverter) :
        base(input)
    {
        m_BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitReader" /> class.
    /// </summary>
    /// <param name="input">The input stream.</param>
    /// <param name="bitConverter">The bit converter to use.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <exception cref="ArgumentNullException">bitConverter</exception>
    [CLSCompliant(false)]
    public BitReader(Stream input, IBitConverter bitConverter, Encoding encoding) :
        base(input, encoding)
    {
        m_BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitReader"/> class.
    /// </summary>
    /// <param name="input">The input stream.</param>
    /// <param name="bitConverter">The bit converter to use.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="leaveOpen"><c>true</c> to leave the stream open after the <see cref="BitReader"/> object is disposed; otherwise, <c>false</c>.</param>
    [CLSCompliant(false)]
    public BitReader(Stream input, IBitConverter bitConverter, Encoding encoding, bool leaveOpen) :
        base(input, encoding, leaveOpen)
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
            m_Buffer[0] = (byte)b;
        }
        else
        {
            int offset = 0;
            do
            {
                int readBytes = stream.Read(m_Buffer, offset, count - offset);
                if (readBytes == 0)
                    throw new EndOfStreamException();
                offset += readBytes;
            }
            while (offset < count);
        }
    }

    /// <summary>
    /// Reads a 2-byte signed integer from the current stream and advances the current position of the stream by two bytes.
    /// </summary>
    /// <returns>
    /// A 2-byte signed integer read from the current stream.
    /// </returns>
    public override short ReadInt16()
    {
        _FillBuffer(2);
        return m_BitConverter.ToInt16(m_Buffer);
    }

    /// <summary>
    /// Reads a 2-byte unsigned integer from the current stream using little-endian encoding and advances the position of the stream by two bytes.
    /// </summary>
    /// <returns>
    /// A 2-byte unsigned integer read from this stream.
    /// </returns>
    [CLSCompliant(false)]
    public override ushort ReadUInt16()
    {
        _FillBuffer(2);
        return m_BitConverter.ToUInt16(m_Buffer);
    }

    /// <summary>Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.</summary>
    /// <returns>A 4-byte signed integer read from the current stream.</returns>
    public override int ReadInt32()
    {
        _FillBuffer(4);
        return m_BitConverter.ToInt32(m_Buffer);
    }

    /// <summary>
    /// Reads a 4-byte unsigned integer from the current stream and advances the position of the stream by four bytes.
    /// </summary>
    /// <returns>
    /// A 4-byte unsigned integer read from this stream.
    /// </returns>
    [CLSCompliant(false)]
    public override uint ReadUInt32()
    {
        _FillBuffer(4);
        return m_BitConverter.ToUInt32(m_Buffer);
    }

    /// <summary>
    /// Reads an 8-byte signed integer from the current stream and advances the current position of the stream by eight bytes.
    /// </summary>
    /// <returns>
    /// An 8-byte signed integer read from the current stream.
    /// </returns>
    public override long ReadInt64()
    {
        _FillBuffer(8);
        return m_BitConverter.ToInt64(m_Buffer);
    }

    /// <summary>
    /// Reads an 8-byte unsigned integer from the current stream and advances the position of the stream by eight bytes.
    /// </summary>
    /// <returns>
    /// An 8-byte unsigned integer read from this stream.
    /// </returns>
    [CLSCompliant(false)]
    public override ulong ReadUInt64()
    {
        _FillBuffer(8);
        return m_BitConverter.ToUInt64(m_Buffer);
    }

    /// <summary>
    /// Reads a 4-byte floating point value from the current stream and advances the current position of the stream by four bytes.
    /// </summary>
    /// <returns>
    /// A 4-byte floating point value read from the current stream.
    /// </returns>
    public override float ReadSingle()
    {
        _FillBuffer(4);
        return m_BitConverter.ToSingle(m_Buffer);
    }

    /// <summary>
    /// Reads an 8-byte floating point value from the current stream and advances the current position of the stream by eight bytes.
    /// </summary>
    /// <returns>
    /// An 8-byte floating point value read from the current stream.
    /// </returns>
    public override double ReadDouble()
    {
        _FillBuffer(8);
        return m_BitConverter.ToDouble(m_Buffer);
    }

    /// <summary>
    /// Reads a decimal value from the current stream and advances the current position of the stream by sixteen bytes.
    /// </summary>
    /// <returns>
    /// A decimal value read from the current stream.
    /// </returns>
    public override decimal ReadDecimal()
    {
        _FillBuffer(16);
        return m_BitConverter.ToDecimal(m_Buffer);
    }
}

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
    /// <param name="leaveOpen"><see langword="true"/> to leave the stream open after the <see cref="BitReader"/> object is disposed; otherwise, <see langword="false"/>.</param>
    [CLSCompliant(false)]
    public BitReader(Stream input, IBitConverter bitConverter, Encoding encoding, bool leaveOpen) :
        base(input, encoding, leaveOpen)
    {
        m_BitConverter = bitConverter ?? throw new ArgumentNullException(nameof(bitConverter));
    }

    /// <summary>
    /// Gets or sets the current bit converter.
    /// </summary>
    [CLSCompliant(false)]
    public IBitConverter BitConverter
    {
        get => m_BitConverter;
        set => m_BitConverter = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Reads a 2-byte signed integer from the current stream and advances the current position of the stream by two bytes.
    /// </summary>
    /// <returns>
    /// A 2-byte signed integer read from the current stream.
    /// </returns>
    public override short ReadInt16()
    {
        Span<byte> buffer = stackalloc byte[2];
        BaseStream.ReadExactly(buffer);
        return m_BitConverter.ToInt16(buffer);
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
        Span<byte> buffer = stackalloc byte[2];
        BaseStream.ReadExactly(buffer);
        return m_BitConverter.ToUInt16(buffer);
    }

    /// <summary>Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.</summary>
    /// <returns>A 4-byte signed integer read from the current stream.</returns>
    public override int ReadInt32()
    {
        Span<byte> buffer = stackalloc byte[4];
        BaseStream.ReadExactly(buffer);
        return m_BitConverter.ToInt32(buffer);
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
        Span<byte> buffer = stackalloc byte[4];
        BaseStream.ReadExactly(buffer);
        return m_BitConverter.ToUInt32(buffer);
    }

    /// <summary>
    /// Reads an 8-byte signed integer from the current stream and advances the current position of the stream by eight bytes.
    /// </summary>
    /// <returns>
    /// An 8-byte signed integer read from the current stream.
    /// </returns>
    public override long ReadInt64()
    {
        Span<byte> buffer = stackalloc byte[8];
        BaseStream.ReadExactly(buffer);
        return m_BitConverter.ToInt64(buffer);
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
        Span<byte> buffer = stackalloc byte[8];
        BaseStream.ReadExactly(buffer);
        return m_BitConverter.ToUInt64(buffer);
    }

    /// <summary>
    /// Reads a 4-byte floating point value from the current stream and advances the current position of the stream by four bytes.
    /// </summary>
    /// <returns>
    /// A 4-byte floating point value read from the current stream.
    /// </returns>
    public override float ReadSingle()
    {
        Span<byte> buffer = stackalloc byte[4];
        BaseStream.ReadExactly(buffer);
        return m_BitConverter.ToSingle(buffer);
    }

    /// <summary>
    /// Reads an 8-byte floating point value from the current stream and advances the current position of the stream by eight bytes.
    /// </summary>
    /// <returns>
    /// An 8-byte floating point value read from the current stream.
    /// </returns>
    public override double ReadDouble()
    {
        Span<byte> buffer = stackalloc byte[8];
        BaseStream.ReadExactly(buffer);
        return m_BitConverter.ToDouble(buffer);
    }

    /// <summary>
    /// Reads a decimal value from the current stream and advances the current position of the stream by sixteen bytes.
    /// </summary>
    /// <returns>
    /// A decimal value read from the current stream.
    /// </returns>
    public override decimal ReadDecimal()
    {
        Span<byte> buffer = stackalloc byte[16];
        BaseStream.ReadExactly(buffer);
        return m_BitConverter.ToDecimal(buffer);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IBitConverter m_BitConverter;
}

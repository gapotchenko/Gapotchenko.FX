using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of z-base-32 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericZBase32 : GenericBase32
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericZBase32"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericZBase32(TextDataEncodingAlphabet alphabet) :
            base(alphabet)
        {
        }

        abstract class CodecContextBase
        {
            public CodecContextBase(TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
            {
                m_Alphabet = alphabet;
                m_Options = options;
            }

            protected readonly TextDataEncodingAlphabet m_Alphabet;
            protected readonly DataEncodingOptions m_Options;

            #region Parameters

            protected const string Name = "ZBase32";

            protected const int MaskSymbol = (1 << BitsPerSymbol) - 1;

            #endregion

            protected ulong m_Bits;
            protected int m_Modulus;
            protected bool m_Eof;
        }

        sealed class EncoderContext : CodecContextBase, IEncoderContext
        {
            public EncoderContext(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(alphabet, options)
            {
            }

            /// <summary>
            /// Base32 encoding treats wrapping and indentation interchangeably.
            /// </summary>
            const DataEncodingOptions FormatMask = DataEncodingOptions.Wrap | DataEncodingOptions.Indent;

            readonly char[] m_Buffer = new char[SymbolsPerEncodedBlock];

            int m_LinePosition;

            void MoveLinePosition(int delta) => m_LinePosition += delta;

            void EmitLineBreak(TextWriter output)
            {
                if (m_LinePosition >= 72)
                {
                    m_LinePosition = 0;

                    if ((m_Options & FormatMask) != 0)
                        output.WriteLine();
                }
            }

            void WriteBits(TextWriter output, int bitCount)
            {
                var alphabet = m_Alphabet;

                int i = 0;
                int s = bitCount;
                int lastCount = 1;
                int pbi = 0;

                do
                {
                    s -= BitsPerSymbol;
                    int bi = s >= 0 ? s >> 3 : pbi;

                    int symbolIndex = (int)Msr(m_Bits, s) & MaskSymbol;
                    m_Buffer[i++] = alphabet[symbolIndex];

                    if (symbolIndex != 0 || bi != pbi)
                        lastCount = i;

                    pbi = bi;
                }
                while (s > 0);

                if ((m_Options & DataEncodingOptions.Unpad) == 0)
                {
                    while (i < SymbolsPerEncodedBlock)
                        m_Buffer[i++] = PaddingChar;
                }
                else if ((m_Options & DataEncodingOptions.Compress) != 0)
                {
                    i = lastCount;
                }

                EmitLineBreak(output);
                output.Write(m_Buffer, 0, i);
            }

            public void Encode(ReadOnlySpan<byte> input, TextWriter output)
            {
                if (m_Eof)
                    return;

                if (input == null)
                {
                    m_Eof = true;

                    switch (m_Modulus)
                    {
                        case 0:
                            // Nothing to do.
                            break;
                        case var k when k < BytesPerDecodedBlock:
                            WriteBits(output, k * 8);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    return;
                }

                var alphabet = m_Alphabet;

                foreach (var b in input)
                {
                    // Accumulate data bits.
                    m_Bits = (m_Bits << 8) | b;

                    if (++m_Modulus == BytesPerDecodedBlock)
                    {
                        m_Modulus = 0;

                        m_Buffer[0] = alphabet[(int)(m_Bits >> 35) & MaskSymbol];
                        m_Buffer[1] = alphabet[(int)(m_Bits >> 30) & MaskSymbol];
                        m_Buffer[2] = alphabet[(int)(m_Bits >> 25) & MaskSymbol];
                        m_Buffer[3] = alphabet[(int)(m_Bits >> 20) & MaskSymbol];
                        m_Buffer[4] = alphabet[(int)(m_Bits >> 15) & MaskSymbol];
                        m_Buffer[5] = alphabet[(int)(m_Bits >> 10) & MaskSymbol];
                        m_Buffer[6] = alphabet[(int)(m_Bits >> 5) & MaskSymbol];
                        m_Buffer[7] = alphabet[(int)m_Bits & MaskSymbol];

                        EmitLineBreak(output);
                        output.Write(m_Buffer);

                        MoveLinePosition(SymbolsPerEncodedBlock);
                    }
                }
            }
        }

        sealed class DecoderContext : CodecContextBase, IDecoderContext
        {
            public DecoderContext(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(alphabet, options)
            {
            }

            readonly byte[] m_Buffer = new byte[BytesPerDecodedBlock];

            public void Decode(ReadOnlySpan<char> input, Stream output)
            {
                if (m_Eof)
                    return;

                if (input == null)
                {
                    m_Eof = true;
                    if ((m_Options & DataEncodingOptions.Padding) != 0)
                        ValidatePaddingEof();
                    FlushDecode(output);
                    return;
                }

                var alphabet = m_Alphabet;

                foreach (var c in input)
                {
                    if (c == PaddingChar)
                    {
                        if ((m_Options & DataEncodingOptions.Padding) != 0)
                            ValidatePaddingChar();
                        FlushDecode(output);
                        continue;
                    }

                    int b = alphabet.IndexOf(c);
                    if (b == -1)
                    {
                        if ((m_Options & DataEncodingOptions.Relax) == 0)
                        {
                            if (!char.IsWhiteSpace(c))
                                throw new InvalidDataException($"Encountered a non-{Name} character.");
                        }
                        continue;
                    }

                    ValidatePaddingState();

                    // Accumulate data bits.
                    m_Bits = (m_Bits << BitsPerSymbol) | (byte)b;

                    if (++m_Modulus == SymbolsPerEncodedBlock)
                    {
                        m_Modulus = 0;

                        m_Buffer[0] = (byte)(m_Bits >> 32);
                        m_Buffer[1] = (byte)(m_Bits >> 24);
                        m_Buffer[2] = (byte)(m_Bits >> 16);
                        m_Buffer[3] = (byte)(m_Bits >> 8);
                        m_Buffer[4] = (byte)m_Bits;

                        output.Write(m_Buffer, 0, BytesPerDecodedBlock);
                    }
                }
            }

            void ReadBits(Stream output, int bitCount)
            {
                int i = 0;
                int s = bitCount;
                var lastCount = 1;

                byte pb = 0;
                do
                {
                    s -= 8;

                    byte b = (byte)Msr(m_Bits, s); ;
                    m_Buffer[i++] = b;

                    if (b != 0 || pb == 0)
                        lastCount = i;

                    pb = b;
                }
                while (s > 0);

                output.Write(m_Buffer, 0, lastCount);
            }

            void FlushDecode(Stream output)
            {
                switch (m_Modulus)
                {
                    case 0:
                        // Nothing to do.
                        return;
                    case var k when k < SymbolsPerEncodedBlock:
                        ReadBits(output, k * BitsPerSymbol);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                m_Modulus = 0;
            }

            void ValidateIncompleteByte()
            {
                //    if ((m_Options & DataEncodingOptions.Relax) == 0)
                //      throw new InvalidDataException($"Cannot decode the last byte due to missing {Name} symbol.");
            }

            void ValidateLastSymbol(ulong zeroMask)
            {
                if ((m_Options & DataEncodingOptions.Relax) == 0 &&
                    (m_Bits & zeroMask) != 0)
                {
                    throw new InvalidDataException($"The insignificant bits of the last {Name} symbol are expected to be zero.");
                }
            }

            int m_Padding;

            void ValidatePaddingChar()
            {
                if (m_Padding == 0)
                {
                    if (m_Modulus == 0)
                        throw CreateInvalidPaddingException();
                    m_Padding = m_Modulus;
                }

                if (++m_Padding == SymbolsPerEncodedBlock)
                    m_Padding = 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void ValidatePaddingState()
            {
                if (m_Padding != 0)
                    throw CreateInvalidPaddingException();
            }

            void ValidatePaddingEof()
            {
                if (m_Modulus != 0 || m_Padding != 0)
                    throw CreateInvalidPaddingException();
            }

            static Exception CreateInvalidPaddingException() => new InvalidDataException($"Invalid {Name} padding.");
        }

        /// <inheritdoc/>
        protected override IEncoderContext CreateEncoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Padding) == 0)
            {
                // Produce unpadded strings unless padding is explicitly requested.
                options |= DataEncodingOptions.Unpad;
            }

            return new EncoderContext(alphabet, options);
        }

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) => new DecoderContext(alphabet, options);
    }
}

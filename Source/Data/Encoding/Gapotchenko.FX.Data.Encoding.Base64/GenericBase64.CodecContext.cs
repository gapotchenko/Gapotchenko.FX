using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    partial class GenericBase64
    {
        sealed class CodecContext : IEncoderContext, IDecoderContext
        {
            public CodecContext(DataTextEncodingAlphabet alphabet, DataEncodingOptions options)
            {
                m_Alphabet = alphabet;
                m_Options = options;
            }

            readonly DataTextEncodingAlphabet m_Alphabet;
            readonly DataEncodingOptions m_Options;

            int m_Bits;
            int m_Modulus;
            bool m_Eof;

            int m_LinePosition;

            void IncrementLinePosition(int delta)
            {
                m_LinePosition += delta;
            }

            void InsertLineBreak(TextWriter output)
            {
                if (m_LinePosition >= 76)
                {
                    m_LinePosition = 0;

                    if ((m_Options & DataEncodingOptions.Indent) != 0)
                        output.WriteLine();
                }
            }

            const int Mask6Bits = 0x3f;
            const int Mask4Bits = 0x0f;
            const int Mask2Bits = 0x03;

            public void Encode(ReadOnlySpan<byte> input, TextWriter output)
            {
                if (m_Eof)
                    return;

                var alphabet = m_Alphabet;

                if (input == null)
                {
                    m_Eof = true;
                    switch (m_Modulus)
                    {
                        case 0:
                            // Nothing to do.
                            break;

                        case 1:
                            InsertLineBreak(output);

                            // 8 bits = 6 + 2
                            output.Write(alphabet[(m_Bits >> 2) & Mask6Bits]); // 6 bits
                            output.Write(alphabet[(m_Bits << 4) & Mask6Bits]); // 2 bits

                            if ((m_Options & DataEncodingOptions.InhibitPadding) == 0)
                            {
                                output.Write(PaddingChar);
                                output.Write(PaddingChar);
                            }
                            break;

                        case 2:
                            InsertLineBreak(output);

                            // 16 bits = 6 + 6 + 4
                            output.Write(alphabet[(m_Bits >> 10) & Mask6Bits]); // 6 bits
                            output.Write(alphabet[(m_Bits >> 4) & Mask6Bits]); // 6 bits
                            output.Write(alphabet[(m_Bits << 2) & Mask6Bits]); // 4 bits

                            if ((m_Options & DataEncodingOptions.InhibitPadding) == 0)
                                output.Write(PaddingChar);
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                {
                    foreach (var b in input)
                    {
                        // Accumulate data bits.
                        m_Bits = (m_Bits << 8) | b;

                        if (++m_Modulus == 3)
                        {
                            m_Modulus = 0;

                            InsertLineBreak(output);

                            // 3 bytes = 24 bits = 4 * 6 bits
                            output.Write(alphabet[(m_Bits >> 18) & Mask6Bits]);
                            output.Write(alphabet[(m_Bits >> 12) & Mask6Bits]);
                            output.Write(alphabet[(m_Bits >> 6) & Mask6Bits]);
                            output.Write(alphabet[m_Bits & Mask6Bits]);

                            IncrementLinePosition(4);
                        }
                    }
                }
            }

            public void Decode(ReadOnlySpan<char> input, Stream output)
            {
                if (m_Eof)
                    return;

                if (input == null)
                    m_Eof = true;

                var alphabet = m_Alphabet;

                foreach (var c in input)
                {
                    if (c == PaddingChar)
                    {
                        FlushDecode(output);
                        continue;
                    }

                    int result = alphabet.IndexOf(c);
                    if (result != -1)
                    {
                        // Accumulate data bits.
                        m_Bits = (m_Bits << 6) | result;

                        if (++m_Modulus == 4)
                        {
                            m_Modulus = 0;

                            output.WriteByte((byte)(m_Bits >> 16));
                            output.WriteByte((byte)(m_Bits >> 8));
                            output.WriteByte((byte)m_Bits);
                        }
                    }
                }

                if (m_Eof)
                    FlushDecode(output);
            }

            void FlushDecode(Stream output)
            {
                if (m_Modulus == 0)
                    return;

                switch (m_Modulus)
                {
                    case 1:
                        // 6 bits
                        ValidateIncompleteByte();
                        break;

                    case 2:
                        // 2 * 6 bits = 12 = 8 + 4
                        ValidateLastSymbol(Mask4Bits);
                        output.WriteByte((byte)(m_Bits >> 4));
                        break;

                    case 3:
                        // 3 * 6 bits = 18 = 8 + 8 + 2
                        ValidateLastSymbol(Mask2Bits);
                        output.WriteByte((byte)(m_Bits >> 10));
                        output.WriteByte((byte)(m_Bits >> 2));
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                m_Modulus = 0;
            }

            void ValidateIncompleteByte()
            {
                throw new InvalidDataException("Cannot form the last byte due to missing Base64 symbol.");
            }

            void ValidateLastSymbol(int zeroMask)
            {
                if ((m_Bits & zeroMask) != 0)
                    throw new InvalidDataException("The discarded bits of the last Base64 symbol are expected to be zero.");
            }
        }
    }
}

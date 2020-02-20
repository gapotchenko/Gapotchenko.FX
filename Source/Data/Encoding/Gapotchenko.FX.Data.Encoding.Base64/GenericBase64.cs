﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Base64 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericBase64 : DataTextEncoding, IBase64
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericBase64"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericBase64(DataTextEncodingAlphabet alphabet)
        {
            if (alphabet == null)
                throw new ArgumentNullException(nameof(alphabet));

            if (alphabet.Size != Radix)
            {
                throw new ArgumentException(
                    string.Format("The alphabet size of {0} encoding should be {1}.", Name, Radix),
                    nameof(alphabet));
            }

            Alphabet = alphabet;
        }

        /// <summary>
        /// The encoding alphabet.
        /// </summary>
        protected readonly DataTextEncodingAlphabet Alphabet;

        /// <inheritdoc/>
        public int Radix => 64;

        /// <summary>
        /// Base64 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.75f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

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
                            // 8 bits = 6 + 2
                            output.Write(alphabet[(m_Bits >> 2) & Mask6Bits]); // 6 bits
                            output.Write(alphabet[(m_Bits << 4) & Mask6Bits]); // 2 bits
                            if ((m_Options & DataEncodingOptions.NoPadding) == 0)
                            {
                                output.Write(PaddingChar);
                                output.Write(PaddingChar);
                            }
                            break;

                        case 2:
                            // 16 bits = 6 + 6 + 4
                            output.Write(alphabet[(m_Bits >> 10) & Mask6Bits]); // 6 bits
                            output.Write(alphabet[(m_Bits >> 4) & Mask6Bits]); // 6 bits
                            output.Write(alphabet[(m_Bits << 2) & Mask6Bits]); // 4 bits
                            if ((m_Options & DataEncodingOptions.NoPadding) == 0)
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

                            // 3 bytes = 24 bits = 4 * 6 bits
                            output.Write(alphabet[(m_Bits >> 18) & Mask6Bits]);
                            output.Write(alphabet[(m_Bits >> 12) & Mask6Bits]);
                            output.Write(alphabet[(m_Bits >> 6) & Mask6Bits]);
                            output.Write(alphabet[m_Bits & Mask6Bits]);
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

        /// <inheritdoc/>
        protected override IEncoderContext CreateEncoderContext(DataEncodingOptions options) => new CodecContext(Alphabet, options);

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContext(DataEncodingOptions options) => new CodecContext(Alphabet, options);

        /// <inheritdoc/>
        public override bool IsCaseSensitive => true;

        /// <inheritdoc/>
        protected override int PaddingCore => 4;

        /// <summary>
        /// The padding character.
        /// </summary>
        protected const char PaddingChar = '=';

        /// <inheritdoc/>
        protected override string PadCore(ReadOnlySpan<char> s) => PadRight(s, PaddingChar);

        /// <inheritdoc/>
        protected override ReadOnlySpan<char> UnpadCore(ReadOnlySpan<char> s) => UnpadRight(s, PaddingChar);
    }
}

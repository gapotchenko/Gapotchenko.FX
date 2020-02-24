using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Base64 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericBase64 : TextDataEncoding, IBase64
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericBase64"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericBase64(TextDataEncodingAlphabet alphabet)
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
        protected readonly TextDataEncodingAlphabet Alphabet;

        /// <inheritdoc/>
        public int Radix => 64;

        /// <summary>
        /// Base64 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.75f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        abstract class CodecContextBase
        {
            public CodecContextBase(TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
            {
                m_Alphabet = alphabet;
                m_Options = options;
            }

            protected readonly TextDataEncodingAlphabet m_Alphabet;
            protected readonly DataEncodingOptions m_Options;

            protected int m_Bits;
            protected int m_Modulus;
            protected bool m_Eof;

            protected const int Mask6Bits = 0x3f;
            protected const int Mask4Bits = 0x0f;
            protected const int Mask2Bits = 0x03;
        }

        sealed class EncoderContext : CodecContextBase, IEncoderContext
        {
            public EncoderContext(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(alphabet, options)
            {
            }

            /// <summary>
            /// Base64 encoding treats wrapping and indentation interchangeably.
            /// </summary>
            const DataEncodingOptions FormatMask = DataEncodingOptions.Wrap | DataEncodingOptions.Indent;

            readonly char[] m_Buffer = new char[4];

            int m_LinePosition;

            void MoveLinePosition(int delta) => m_LinePosition += delta;

            void EmitLineBreak(TextWriter output)
            {
                if (m_LinePosition >= 76)
                {
                    m_LinePosition = 0;

                    if ((m_Options & FormatMask) != 0)
                        output.WriteLine();
                }
            }

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
                            {
                                // 8 bits = 6 + 2
                                m_Buffer[0] = alphabet[(m_Bits >> 2) & Mask6Bits]; // 6 bits
                                m_Buffer[1] = alphabet[(m_Bits << 4) & Mask6Bits]; // 2 bits

                                int count = 2;
                                if ((m_Options & DataEncodingOptions.Unpad) == 0)
                                {
                                    m_Buffer[2] = PaddingChar;
                                    m_Buffer[3] = PaddingChar;
                                    count = 4;
                                }

                                EmitLineBreak(output);
                                output.Write(m_Buffer, 0, count);
                            }
                            break;

                        case 2:
                            {
                                // 16 bits = 6 + 6 + 4
                                m_Buffer[0] = alphabet[(m_Bits >> 10) & Mask6Bits]; // 6 bits
                                m_Buffer[1] = alphabet[(m_Bits >> 4) & Mask6Bits]; // 6 bits
                                m_Buffer[2] = alphabet[(m_Bits << 2) & Mask6Bits]; // 4 bits

                                int count = 3;
                                if ((m_Options & DataEncodingOptions.Unpad) == 0)
                                {
                                    m_Buffer[3] = PaddingChar;
                                    count = 4;
                                }

                                EmitLineBreak(output);
                                output.Write(m_Buffer, 0, count);
                            }
                            break;

                        default:
                            throw new InvalidOperationException();
                    }

                    return;
                }

                foreach (var b in input)
                {
                    // Accumulate data bits.
                    m_Bits = (m_Bits << 8) | b;

                    if (++m_Modulus == 3)
                    {
                        m_Modulus = 0;

                        // 3 bytes = 24 bits = 4 * 6 bits
                        m_Buffer[0] = alphabet[(m_Bits >> 18) & Mask6Bits];
                        m_Buffer[1] = alphabet[(m_Bits >> 12) & Mask6Bits];
                        m_Buffer[2] = alphabet[(m_Bits >> 6) & Mask6Bits];
                        m_Buffer[3] = alphabet[m_Bits & Mask6Bits];

                        EmitLineBreak(output);
                        output.Write(m_Buffer);

                        MoveLinePosition(4);
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

            readonly byte[] m_Buffer = new byte[3];

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
                                throw new InvalidDataException("Encountered a non-Base64 character.");
                        }
                        continue;
                    }

                    ValidatePaddingState();

                    // Accumulate data bits.
                    m_Bits = (m_Bits << 6) | b;

                    if (++m_Modulus == 4)
                    {
                        m_Modulus = 0;

                        m_Buffer[0] = (byte)(m_Bits >> 16);
                        m_Buffer[1] = (byte)(m_Bits >> 8);
                        m_Buffer[2] = (byte)m_Bits;

                        output.Write(m_Buffer, 0, 3);
                    }
                }
            }

            void FlushDecode(Stream output)
            {
                switch (m_Modulus)
                {
                    case 0:
                        // Nothing to do.
                        return;

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

                        m_Buffer[0] = (byte)(m_Bits >> 10);
                        m_Buffer[1] = (byte)(m_Bits >> 2);

                        output.Write(m_Buffer, 0, 2);
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                m_Modulus = 0;
            }

            void ValidateIncompleteByte()
            {
                if ((m_Options & DataEncodingOptions.Relax) == 0)
                    throw new InvalidDataException("Cannot decode the last byte due to missing Base64 symbol.");
            }

            void ValidateLastSymbol(int zeroMask)
            {
                if ((m_Options & DataEncodingOptions.Relax) == 0 &&
                    (m_Bits & zeroMask) != 0)
                {
                    throw new InvalidDataException("The insignificant bits of the last Base64 symbol are expected to be zero.");
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

                if (++m_Padding == 4)
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

            static Exception CreateInvalidPaddingException() => new InvalidDataException("Invalid Base64 padding.");
        }

        /// <inheritdoc/>
        protected override IEncoderContext CreateEncoderContext(DataEncodingOptions options) => new EncoderContext(Alphabet, GetEncoderOptions(options));

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContext(DataEncodingOptions options) => new DecoderContext(GetDecoderAlphabet(options), options);

        /// <summary>
        /// Gets encoder options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The encoder options.</returns>
        protected virtual DataEncodingOptions GetEncoderOptions(DataEncodingOptions options) => options;

        /// <summary>
        /// Gets decoder alphabet.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The alphabet.</returns>
        protected virtual TextDataEncodingAlphabet GetDecoderAlphabet(DataEncodingOptions options) => Alphabet;

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

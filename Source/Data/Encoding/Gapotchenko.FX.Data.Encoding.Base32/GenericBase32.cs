using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Base32 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericBase32 : TextDataEncoding, IBase32
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericBase32"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericBase32(TextDataEncodingAlphabet alphabet)
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

        #region Parameters

        const int BitsPerEncodedByte = 5;
        const int SymbolsPerEncodedBlock = 8;
        const int BytesPerUnencodedBlock = 5;

        #endregion

        /// <inheritdoc/>
        public int Radix => 1 << BitsPerEncodedByte;

        /// <summary>
        /// Base32 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = (float)BytesPerUnencodedBlock / SymbolsPerEncodedBlock;

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

            #region Parameters

            protected const string Name = "Base32";

            protected const int MaskAlphabet = (1 << BitsPerEncodedByte) - 1;
            protected const int Mask1Bit = (1 << 1) - 1;
            protected const int Mask2Bits = (1 << 2) - 1;
            protected const int Mask3Bits = (1 << 3) - 1;
            protected const int Mask4Bits = (1 << 4) - 1;

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

            /// <summary>
            /// Mathematical shift to the right.
            /// </summary>
            /// <param name="x">The value.</param>
            /// <param name="n">The number of bits to shift.</param>
            /// <returns>The shifted value.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static ulong Msr(ulong x, int n) =>
                (n >= 0) ?
                    x >> n :
                    x << -n;

            void WriteBits(TextWriter output, int bitCount)
            {
                var alphabet = m_Alphabet;

                int i = 0;
                int s = bitCount;
                do
                {
                    s -= BitsPerEncodedByte;
                    m_Buffer[i++] = alphabet[(int)Msr(m_Bits, s) & MaskAlphabet];
                }
                while (s > 0);

                if ((m_Options & DataEncodingOptions.Unpad) == 0)
                {
                    while (i < SymbolsPerEncodedBlock)
                        m_Buffer[i++] = PaddingChar;
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
                        case 1:
                            WriteBits(output, 1 * 8);
                            break;
                        case 2:
                            WriteBits(output, 2 * 8);
                            break;
                        case 3:
                            WriteBits(output, 3 * 8);
                            break;
                        case 4:
                            WriteBits(output, 4 * 8);
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

                    if (++m_Modulus == BytesPerUnencodedBlock)
                    {
                        m_Modulus = 0;

                        m_Buffer[0] = alphabet[(int)(m_Bits >> 35) & MaskAlphabet];
                        m_Buffer[1] = alphabet[(int)(m_Bits >> 30) & MaskAlphabet];
                        m_Buffer[2] = alphabet[(int)(m_Bits >> 25) & MaskAlphabet];
                        m_Buffer[3] = alphabet[(int)(m_Bits >> 20) & MaskAlphabet];
                        m_Buffer[4] = alphabet[(int)(m_Bits >> 15) & MaskAlphabet];
                        m_Buffer[5] = alphabet[(int)(m_Bits >> 10) & MaskAlphabet];
                        m_Buffer[6] = alphabet[(int)(m_Bits >> 5) & MaskAlphabet];
                        m_Buffer[7] = alphabet[(int)m_Bits & MaskAlphabet];

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

            readonly byte[] m_Buffer = new byte[BytesPerUnencodedBlock];

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
                    m_Bits = (m_Bits << BitsPerEncodedByte) | (byte)b;

                    if (++m_Modulus == SymbolsPerEncodedBlock)
                    {
                        m_Modulus = 0;

                        m_Buffer[0] = (byte)(m_Bits >> 32);
                        m_Buffer[1] = (byte)(m_Bits >> 24);
                        m_Buffer[2] = (byte)(m_Bits >> 16);
                        m_Buffer[3] = (byte)(m_Bits >> 8);
                        m_Buffer[4] = (byte)m_Bits;

                        output.Write(m_Buffer, 0, BytesPerUnencodedBlock);
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
                        // 5 bits
                        ValidateIncompleteByte();
                        break;

                    case 2:
                        // 2 * 5 bits = 10 = 8 + 2
                        ValidateLastSymbol(Mask2Bits);
                        output.WriteByte((byte)(m_Bits >> 2));
                        break;

                    case 3:
                        // 3 * 5 bits = 15 = 8 + 7
                        ValidateIncompleteByte();
                        output.WriteByte((byte)(m_Bits >> 7));
                        break;

                    case 4:
                        // 4 * 5 bits = 20 = 2 * 8 + 4
                        ValidateLastSymbol(Mask4Bits);

                        m_Buffer[0] = (byte)(m_Bits >> 12);
                        m_Buffer[1] = (byte)(m_Bits >> 4);

                        output.Write(m_Buffer, 0, 2);
                        break;

                    case 5:
                        // 5 * 5 bits = 25 = 3 * 8 + 1
                        ValidateLastSymbol(Mask1Bit);

                        m_Buffer[0] = (byte)(m_Bits >> 17);
                        m_Buffer[1] = (byte)(m_Bits >> 9);
                        m_Buffer[2] = (byte)(m_Bits >> 1);

                        output.Write(m_Buffer, 0, 3);
                        break;

                    case 6:
                        // 6 * 5 bits = 30 = 3 * 8 + 6
                        ValidateIncompleteByte();

                        m_Buffer[0] = (byte)(m_Bits >> 22);
                        m_Buffer[1] = (byte)(m_Bits >> 14);
                        m_Buffer[2] = (byte)(m_Bits >> 6);

                        output.Write(m_Buffer, 0, 3);
                        break;

                    case 7:
                        // 7 * 5 bits = 35 = 4 * 8 + 3
                        ValidateLastSymbol(Mask3Bits);

                        m_Buffer[0] = (byte)(m_Bits >> 27);
                        m_Buffer[1] = (byte)(m_Bits >> 19);
                        m_Buffer[2] = (byte)(m_Bits >> 11);
                        m_Buffer[3] = (byte)(m_Bits >> 3);

                        output.Write(m_Buffer, 0, 4);
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                m_Modulus = 0;
            }

            void ValidateIncompleteByte()
            {
                if ((m_Options & DataEncodingOptions.Relax) == 0)
                    throw new InvalidDataException($"Cannot decode the last byte due to missing {Name} symbol.");
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
        public override bool IsCaseSensitive => false;

        /// <inheritdoc/>
        protected sealed override int PaddingCore => SymbolsPerEncodedBlock;

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

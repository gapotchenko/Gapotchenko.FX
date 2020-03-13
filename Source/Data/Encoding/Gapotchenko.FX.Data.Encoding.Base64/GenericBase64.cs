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
                    string.Format("The alphabet size of {0} encoding should be {1}.", this, Radix),
                    nameof(alphabet));
            }

            Alphabet = alphabet;
        }

        /// <summary>
        /// The encoding alphabet.
        /// </summary>
        protected readonly TextDataEncodingAlphabet Alphabet;

        #region Parameters

        /// <summary>
        /// Number of bits per symbol.
        /// </summary>
        protected const int BitsPerSymbol = 6;

        /// <summary>
        /// Number of symbols per encoded block.
        /// </summary>
        protected const int SymbolsPerEncodedBlock = 4;

        /// <summary>
        /// Number of bytes per decoded block.
        /// </summary>
        protected const int BytesPerDecodedBlock = 3;

        /// <summary>
        /// Bit mask of an alphabet symbol.
        /// </summary>
        protected const int SymbolMask = (1 << BitsPerSymbol) - 1;

        #endregion

        /// <inheritdoc/>
        public int Radix => 1 << BitsPerSymbol;

        /// <summary>
        /// Base64 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = (float)BytesPerDecodedBlock / SymbolsPerEncodedBlock;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        const int LineWidth = 76;

        /// <summary>
        /// Base64 encoding treats wrapping and indentation interchangeably.
        /// </summary>
        const DataEncodingOptions FormatMask = DataEncodingOptions.Wrap | DataEncodingOptions.Indent;

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

            protected const string Name = "Base64";

            protected const int Mask2Bits = (1 << 2) - 1;
            protected const int Mask4Bits = (1 << 4) - 1;

            #endregion

            protected int m_Bits;
            protected int m_Modulus;
            protected bool m_Eof;
        }

        sealed class EncoderContext : CodecContextBase, IEncoderContext
        {
            public EncoderContext(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(alphabet, options)
            {
            }

            readonly char[] m_Buffer = new char[SymbolsPerEncodedBlock];

            int m_LinePosition;

            void MoveLinePosition(int delta) => m_LinePosition += delta;

            void EmitLineBreak(TextWriter output)
            {
                if (m_LinePosition >= LineWidth)
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
                                m_Buffer[0] = alphabet[(m_Bits >> 2) & SymbolMask]; // 6 bits
                                m_Buffer[1] = alphabet[(m_Bits << 4) & SymbolMask]; // 2 bits

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
                                m_Buffer[0] = alphabet[(m_Bits >> 10) & SymbolMask]; // 6 bits
                                m_Buffer[1] = alphabet[(m_Bits >> 4) & SymbolMask]; // 6 bits
                                m_Buffer[2] = alphabet[(m_Bits << 2) & SymbolMask]; // 4 bits

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

                    if (++m_Modulus == BytesPerDecodedBlock)
                    {
                        m_Modulus = 0;

                        // 3 bytes = 24 bits = 4 * 6 bits
                        m_Buffer[0] = alphabet[(m_Bits >> 18) & SymbolMask];
                        m_Buffer[1] = alphabet[(m_Bits >> 12) & SymbolMask];
                        m_Buffer[2] = alphabet[(m_Bits >> 6) & SymbolMask];
                        m_Buffer[3] = alphabet[m_Bits & SymbolMask];

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
                    m_Bits = (m_Bits << BitsPerSymbol) | b;

                    if (++m_Modulus == SymbolsPerEncodedBlock)
                    {
                        m_Modulus = 0;

                        m_Buffer[0] = (byte)(m_Bits >> 16);
                        m_Buffer[1] = (byte)(m_Bits >> 8);
                        m_Buffer[2] = (byte)m_Bits;

                        output.Write(m_Buffer, 0, BytesPerDecodedBlock);
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
                        // 3 * 6 bits = 18 = 2 * 8 + 2
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
                    throw new InvalidDataException($"Cannot decode the last byte due to missing {Name} symbol.");
            }

            void ValidateLastSymbol(int zeroMask)
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
        protected sealed override IEncoderContext CreateEncoderContext(DataEncodingOptions options) => CreateEncoderContextCore(Alphabet, options);

        /// <inheritdoc/>
        protected sealed override IDecoderContext CreateDecoderContext(DataEncodingOptions options) => CreateDecoderContextCore(Alphabet, options);

        /// <summary>
        /// Creates encoder context with specified alphabet and options.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        /// <param name="options">The options.</param>
        /// <returns>The encoder context.</returns>
        protected virtual IEncoderContext CreateEncoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) => new EncoderContext(alphabet, options);

        /// <summary>
        /// Creates decoder context with specified alphabet and options.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        /// <param name="options">The options.</param>
        /// <returns>The decoder context.</returns>
        protected virtual IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) => new DecoderContext(alphabet, options);

        /// <inheritdoc/>
        public sealed override bool IsCaseSensitive => Alphabet.IsCaseSensitive;

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

        /// <inheritdoc/>
        public override bool CanCanonicalize => Alphabet.IsCanonicalizable;

        /// <inheritdoc/>
        protected override void CanonicalizeCore(ReadOnlySpan<char> source, Span<char> destination) => Alphabet.Canonicalize(source, destination);

        /// <inheritdoc/>
        protected override int GetMaxCharCountCore(int byteCount, DataEncodingOptions options)
        {
            int charCount = (byteCount * SymbolsPerEncodedBlock + BytesPerDecodedBlock - 1) / BytesPerDecodedBlock;

            if ((options & DataEncodingOptions.Unpad) == 0)
                charCount = Pad(charCount);

            int newLineCount =
                (options & FormatMask) != 0 ?
                    Math.Max(charCount - 1, 0) / LineWidth :
                    0;

            return charCount + newLineCount * MaxNewLineCharCount;
        }

        /// <inheritdoc/>
        protected override int GetMaxByteCountCore(int charCount, DataEncodingOptions options) =>
            (charCount * BytesPerDecodedBlock + SymbolsPerEncodedBlock - 1) / SymbolsPerEncodedBlock;
    }
}

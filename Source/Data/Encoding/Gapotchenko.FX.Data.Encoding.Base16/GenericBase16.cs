using System;
using System.ComponentModel;
using System.IO;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Base64 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericBase16 : TextDataEncoding, IBase16
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericBase16"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericBase16(TextDataEncodingAlphabet alphabet)
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

        /// <summary>
        /// Number of bits per symbol.
        /// </summary>
        protected const int BitsPerSymbol = 4;

        /// <summary>
        /// Number of symbols per encoded block.
        /// </summary>
        protected const int SymbolsPerEncodedBlock = 2;

        /// <summary>
        /// Number of bytes per decoded block.
        /// </summary>
        protected const int BytesPerDecodedBlock = 1;

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

            protected const string Name = "Base16";

            protected const int MaskSymbol = (1 << BitsPerSymbol) - 1;

            #endregion

            protected bool m_Eof;
        }

        sealed class EncoderContext : CodecContextBase, IEncoderContext
        {
            public EncoderContext(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(alphabet, options)
            {
                m_Width =
                    (m_Options & DataEncodingOptions.Indent) != 0 ?
                        16 * SymbolsPerEncodedBlock :
                        32 * SymbolsPerEncodedBlock;
            }

            readonly int m_Width;
            readonly char[] m_Buffer = new char[SymbolsPerEncodedBlock];

            int m_LinePosition;
            bool m_Indent;

            void MoveLinePosition(int delta) => m_LinePosition += delta;

            void EmitBreak(TextWriter output)
            {
                if (m_LinePosition >= m_Width)
                {
                    m_LinePosition = 0;
                    if ((m_Options & DataEncodingOptions.Wrap) != 0)
                    {
                        output.WriteLine();
                        m_Indent = false;
                    }
                }

                if ((m_Options & DataEncodingOptions.Indent) != 0)
                {
                    if (m_Indent)
                        output.Write(' ');
                    else
                        m_Indent = true;
                }
            }

            public void Encode(ReadOnlySpan<byte> input, TextWriter output)
            {
                if (m_Eof)
                    return;

                if (input == null)
                {
                    m_Eof = true;
                    return;
                }

                var alphabet = m_Alphabet;

                foreach (var b in input)
                {
                    m_Buffer[0] = alphabet[(b >> BitsPerSymbol) & MaskSymbol];
                    m_Buffer[1] = alphabet[b & MaskSymbol];

                    EmitBreak(output);
                    output.Write(m_Buffer);

                    MoveLinePosition(SymbolsPerEncodedBlock);
                }
            }
        }

        sealed class DecoderContext : CodecContextBase, IDecoderContext
        {
            public DecoderContext(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(alphabet, options)
            {
            }

            byte m_Bits;
            int m_Modulus;

            public void Decode(ReadOnlySpan<char> input, Stream output)
            {
                if (m_Eof)
                    return;

                if (input == null)
                {
                    m_Eof = true;
                    FlushDecode();
                    return;
                }

                var alphabet = m_Alphabet;

                foreach (var c in input)
                {
                    int b = alphabet.IndexOf(c);
                    if (b == -1)
                    {
                        if ((m_Options & DataEncodingOptions.Relax) == 0)
                        {
                            bool ok =
                                c == '-' ||
                                char.IsWhiteSpace(c);

                            if (!ok)
                                throw new InvalidDataException($"Encountered a non-{Name} character.");
                        }
                        continue;
                    }

                    // Accumulate data bits.
                    m_Bits = (byte)((m_Bits << BitsPerSymbol) | (b & MaskSymbol));

                    if (++m_Modulus == SymbolsPerEncodedBlock)
                    {
                        m_Modulus = 0;
                        output.WriteByte(m_Bits);
                    }
                }
            }

            void FlushDecode()
            {
                switch (m_Modulus)
                {
                    case 0:
                        // Nothing to do.
                        return;

                    case 1:
                        // 4 bits
                        ValidateIncompleteByte();
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                m_Modulus = 0;
            }

            void ValidateIncompleteByte()
            {
                if ((m_Options & DataEncodingOptions.Padding) != 0)
                    throw new InvalidDataException($"Invalid {Name} padding.");

                if ((m_Options & DataEncodingOptions.Relax) == 0)
                    throw new InvalidDataException($"Cannot decode the last byte due to missing {Name} symbol.");
            }
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

        /// <inheritdoc/>
        protected override void CanonicalizeCore(ReadOnlySpan<char> source, Span<char> destination) => Alphabet.Canonicalize(source, destination);
    }
}

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Crockford Base 32 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericCrockfordBase32 : TextDataEncoding, ICrockfordBase32
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericCrockfordBase32"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericCrockfordBase32(TextDataEncodingAlphabet alphabet)
        {
            if (alphabet == null)
                throw new ArgumentNullException(nameof(alphabet));

            if (alphabet.Size != Radix &&
                alphabet.Size != ChecksumAlphabetSize)
            {
                throw new ArgumentException(
                    string.Format(
                        "The alphabet size of {0} encoding should be {1} or {2}.",
                        Name,
                        ChecksumAlphabetSize,
                        Radix),
                    nameof(alphabet));
            }

            Alphabet = alphabet;
        }

        const int RestrictedAlphabetSize = 32;

        // The 5 last characters of the alphabet allow to encode a checksum ∈ [0; 37).
        const int ChecksumAlphabetSize = RestrictedAlphabetSize + 5;

        /// <summary>
        /// The encoding alphabet.
        /// </summary>
        protected readonly TextDataEncodingAlphabet Alphabet;

        #region Parameters

        /// <summary>
        /// Number of bits per symbol.
        /// </summary>
        protected const int BitsPerSymbol = 5;

        /// <summary>
        /// Number of symbols per encoded block.
        /// </summary>
        protected const int SymbolsPerEncodedBlock = 8;

        /// <summary>
        /// Number of bytes per decoded block.
        /// </summary>
        protected const int BytesPerDecodedBlock = 5;

        #endregion

        /// <inheritdoc/>
        public int Radix => 1 << BitsPerSymbol;

        /// <summary>
        /// Crockford Base 32 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = (float)BytesPerDecodedBlock / SymbolsPerEncodedBlock;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        /// <inheritdoc/>
        protected override void ValidateOptions(DataEncodingOptions options)
        {
            base.ValidateOptions(options);

            if ((options & DataEncodingOptions.Checksum) != 0 &&
                Alphabet.Size != ChecksumAlphabetSize)
            {
                throw new ArgumentException(
                    string.Format(
                        "{0} option cannot be used for {1} encoding because it has a restricted alphabet of {2} symbols, not {3}.",
                        nameof(DataEncodingOptions.Checksum),
                        Name,
                        Alphabet.Size,
                        ChecksumAlphabetSize),
                    nameof(options));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ShiftRight(ulong x, int n) => n >= 0 ? x >> n : x << -n;

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

            protected const string Name = "Crockford Base 32";

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

                bool compress = (m_Options & DataEncodingOptions.Compress) != 0;

                int i = 0; // output symbol index
                int s = bitCount; // shift accumulator
                int pbi = 0; // previous input byte index
                int li = 1; // last output symbol index

                do
                {
                    s -= BitsPerSymbol;

                    int si = (int)ShiftRight(m_Bits, s) & MaskSymbol; // symbol index
                    m_Buffer[i++] = alphabet[si]; // map symbol

                    if (compress)
                    {
                        // bi holds the index of an input byte an output symbol was mapped for.
                        int bi = Math.Max(s, 0) >> 3;
                        if (si != 0 ||  // if non-zero symbol or
                            bi != pbi)  // the symbol encodes a number of input bytes
                        {
                            // make it go to the output.
                            li = i;
                        }
                        pbi = bi;
                    }
                }
                while (s > 0);

                if (compress)
                    i = li;

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
                    FlushDecode(output);
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
                            if (!char.IsWhiteSpace(c))
                                throw new InvalidDataException($"Encountered a non-{Name} character.");
                        }
                        continue;
                    }

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
                int i = 0; // output byte index
                int s = bitCount; // shift accumulator
                var li = 1; // last output byte index

                do
                {
                    s -= 8;

                    byte b = (byte)ShiftRight(m_Bits, s);
                    m_Buffer[i++] = b;

                    if (b != 0 || s >= 0 ||
                        (m_Options & DataEncodingOptions.Compress) != 0 && i >= 2 && m_Buffer[i - 2] == 0)
                    {
                        li = i;
                    }
                }
                while (s > 0);

                output.Write(m_Buffer, 0, li);
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
    }
}

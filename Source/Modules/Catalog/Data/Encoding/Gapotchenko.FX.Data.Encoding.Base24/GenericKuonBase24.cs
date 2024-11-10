// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Provides a generic implementation of Kuon Base24 encoding.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericKuonBase24 : TextDataEncoding, IBase24
{
    private protected GenericKuonBase24(TextDataEncodingAlphabet alphabet, char paddingChar)
    {
        if (alphabet == null)
            throw new ArgumentNullException(nameof(alphabet));

        ValidateAlphabet(alphabet);

        Alphabet = alphabet;
        PaddingChar = paddingChar;
    }

    /// <summary>
    /// The encoding alphabet.
    /// </summary>
    protected TextDataEncodingAlphabet Alphabet { get; }

    /// <summary>
    /// Validates alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    protected virtual void ValidateAlphabet(TextDataEncodingAlphabet alphabet)
    {
        if (alphabet is null)
            throw new ArgumentNullException(nameof(alphabet));

        if (alphabet.Size != Base)
        {
            throw new ArgumentException(
                string.Format("The alphabet size for {0} encoding should be equal to {1}.", this, Base),
                nameof(alphabet));
        }
    }

    /// <inheritdoc/>
    protected override DataEncodingOptions GetEffectiveOptions(DataEncodingOptions options)
    {
        if ((options & DataEncodingOptions.Padding) == 0)
        {
            // Produce unpadded strings unless padding is explicitly requested.
            options |= DataEncodingOptions.NoPadding;
        }

        return base.GetEffectiveOptions(options);
    }

    #region Parameters

    /// <summary>
    /// The base of the encoding.
    /// </summary>
    protected const int Base = 24;

    /// <summary>
    /// Number of bits per encoded symbol.
    /// </summary>
    protected const float BitsPerSymbol = 4.584962500721156181453738943947816f; // = log2(Base) = log2(24)

    /// <summary>
    /// Number of symbols per encoded block.
    /// </summary>
    protected const int SymbolsPerEncodedBlock = 7;

    /// <summary>
    /// Number of bytes per decoded block.
    /// </summary>
    protected const int BytesPerDecodedBlock = 4;

    /// <summary>
    /// Kuon Base24 encoding efficiency.
    /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
    /// </summary>
    public new const float Efficiency = (float)BytesPerDecodedBlock / SymbolsPerEncodedBlock;

    #endregion

    /// <inheritdoc/>
    public int Radix => Base;

    /// <inheritdoc/>
    protected override float EfficiencyCore => Efficiency;

    const int LineWidth = SymbolsPerEncodedBlock * 11;

    /// <summary>
    /// Kuon Base24 encoding treats wrapping and indentation interchangeably.
    /// </summary>
    const DataEncodingOptions FormatMask = DataEncodingOptions.Wrap | DataEncodingOptions.Indent;

    abstract class CodecContextBase(TextDataEncodingAlphabet alphabet, char paddingChar, DataEncodingOptions options)
    {
        protected readonly TextDataEncodingAlphabet m_Alphabet = alphabet;
        protected readonly char m_PaddingChar = paddingChar;
        protected readonly DataEncodingOptions m_Options = options;

        #region Parameters

        protected const string Name = "Kuon Base24";

        #endregion

        protected uint m_Bits;
        protected int m_Modulus;
        protected bool m_Eof;
    }

    sealed class EncoderContext(TextDataEncodingAlphabet alphabet, char paddingChar, DataEncodingOptions options) :
        CodecContextBase(alphabet, paddingChar, options),
        IEncoderContext
    {
        void WriteBits(TextWriter output, int bitCount)
        {
            var alphabet = m_Alphabet;
            int i = 0;

            float writtenBitCount = 0;

            uint a = m_Bits & (uint)((1 << bitCount) - 1);
            while (i < SymbolsPerEncodedBlock)
            {
                var si = (int)(a % Base);
                a /= Base;

                m_Buffer[i++] = Capitalize(alphabet[si]);
                writtenBitCount += BitsPerSymbol;

                if (a == 0 && writtenBitCount >= bitCount)
                {
                    // All bits of information are written.
                    // It is necessary to stop now in order to preserve the original message length upon decoding.
                    break;
                }
            }

            Array.Reverse(m_Buffer, 0, i);

            if ((m_Options & DataEncodingOptions.NoPadding) == 0)
            {
                var paddingChar = Capitalize(m_PaddingChar);

                while (i < SymbolsPerEncodedBlock)
                    m_Buffer[i++] = paddingChar;
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

                    uint a = m_Bits;
                    for (int i = SymbolsPerEncodedBlock - 1; i >= 0; --i)
                    {
                        var si = (int)(a % Base);
                        a /= Base;
                        m_Buffer[i] = Capitalize(alphabet[si]);
                    }

                    EmitLineBreak(output);
                    output.Write(m_Buffer);

                    MoveLinePosition(SymbolsPerEncodedBlock);
                }
            }
        }

        char Capitalize(char c) => TextDataEncoding.Capitalize(c, m_Options);

        readonly char[] m_Buffer = new char[SymbolsPerEncodedBlock];

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

        int m_LinePosition;
    }

    sealed class DecoderContext(TextDataEncodingAlphabet alphabet, char paddingChar, DataEncodingOptions options) :
        CodecContextBase(alphabet, paddingChar, options),
        IDecoderContext
    {
        public bool Decode(ReadOnlySpan<char> input, Stream output, bool throwOnError)
        {
            if (m_Eof)
                return true;

            var options = m_Options;
            bool padding = (options & DataEncodingOptions.Padding) != 0;

            if (input == null)
            {
                m_Eof = true;

                if (padding)
                {
                    if (!ValidatePaddingEof(throwOnError))
                        return false;
                }

                FlushDecode(output);
                return true;
            }

            var alphabet = m_Alphabet;
            bool isCaseSensitive = alphabet.IsCaseSensitive;
            var paddingChar = m_PaddingChar;
            bool relax = (options & DataEncodingOptions.Relax) != 0;

            foreach (var c in input)
            {
                if (CharEqual(c, paddingChar, isCaseSensitive))
                {
                    if (padding)
                    {
                        if (!ValidatePaddingChar(throwOnError))
                            return false;
                    }

                    FlushDecode(output);
                    continue;
                }

                int b = alphabet.IndexOf(c);
                if (b == -1)
                {
                    if (!relax)
                    {
                        if ((m_Options & DataEncodingOptions.Pure) != 0 ||
                            !char.IsWhiteSpace(c))
                        {
                            if (throwOnError)
                                throw new InvalidDataException($"Encountered an invalid {Name} character.");
                            return false;
                        }
                    }
                    continue;
                }

                if (!ValidatePaddingState(throwOnError))
                    return false;

                // Accumulate data bits.
                m_Bits = m_Bits * Base + (byte)b;

                if (++m_Modulus == SymbolsPerEncodedBlock)
                {
                    m_Modulus = 0;

                    m_Buffer[0] = (byte)(m_Bits >> 24);
                    m_Buffer[1] = (byte)(m_Bits >> 16);
                    m_Buffer[2] = (byte)(m_Bits >> 8);
                    m_Buffer[3] = (byte)m_Bits;

                    output.Write(m_Buffer, 0, BytesPerDecodedBlock);
                    m_Bits = 0;
                }
            }

            return true;
        }

        void ReadBits(Stream output, float bitCount)
        {
            int i = 0; // output byte index
            float s = bitCount; // shift accumulator

            var a = m_Bits;
            do
            {
                m_Buffer[i++] = (byte)a;
                a >>= 8;
                s -= 8;
            }
            while (s >= BitsPerSymbol);

            Array.Reverse(m_Buffer, 0, i);
            output.Write(m_Buffer, 0, i);
        }

        readonly byte[] m_Buffer = new byte[BytesPerDecodedBlock];

        void FlushDecode(Stream output)
        {
            switch (m_Modulus)
            {
                case 0:
                    // Nothing to do.
                    return;
                case var k when k > 0 && k < SymbolsPerEncodedBlock:
                    ReadBits(output, k * BitsPerSymbol);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            m_Modulus = 0;
        }

        bool ValidatePaddingChar(bool throwOnError)
        {
            if (m_Padding == 0)
            {
                if (m_Modulus == 0)
                {
                    if (throwOnError)
                        throw CreateInvalidPaddingException();
                    return false;
                }

                m_Padding = m_Modulus;
            }

            if (++m_Padding == SymbolsPerEncodedBlock)
                m_Padding = 0;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ValidatePaddingState(bool throwOnError)
        {
            if (m_Padding != 0)
            {
                if (throwOnError)
                    throw CreateInvalidPaddingException();
                return false;
            }

            return true;
        }

        bool ValidatePaddingEof(bool throwOnError)
        {
            if (m_Modulus != 0 || m_Padding != 0)
            {
                if (throwOnError)
                    throw CreateInvalidPaddingException();
                return false;
            }

            return true;
        }

        int m_Padding;

        static InvalidDataException CreateInvalidPaddingException() => new($"Invalid {Name} padding.");
    }

    /// <inheritdoc/>
    protected sealed override IEncoderContext CreateEncoderContext(DataEncodingOptions options) =>
        CreateEncoderContextCore(Alphabet, options);

    /// <inheritdoc/>
    protected sealed override IDecoderContext CreateDecoderContext(DataEncodingOptions options) =>
        CreateDecoderContextCore(Alphabet, options);

    /// <summary>
    /// Creates encoder context with specified alphabet and options.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    /// <param name="options">The options.</param>
    /// <returns>The encoder context.</returns>
    protected virtual IEncoderContext CreateEncoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) =>
        new EncoderContext(alphabet, PaddingChar, options);

    /// <summary>
    /// Creates decoder context with specified alphabet and options.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    /// <param name="options">The options.</param>
    /// <returns>A decoder context instance.</returns>
    protected virtual IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) =>
        new DecoderContext(alphabet, PaddingChar, options);

    /// <inheritdoc/>
    public sealed override bool IsCaseSensitive => Alphabet.IsCaseSensitive;

    /// <inheritdoc/>
    protected sealed override int PaddingCore => SymbolsPerEncodedBlock;

    /// <summary>
    /// The padding character.
    /// </summary>
    protected char PaddingChar { get; }

    /// <inheritdoc/>
    protected override string PadCore(ReadOnlySpan<char> s) => PadRight(s, PaddingChar);

    /// <inheritdoc/>
    protected override ReadOnlySpan<char> UnpadCore(ReadOnlySpan<char> s) => s.TrimEnd(PaddingChar);

    /// <inheritdoc/>
    public override bool CanCanonicalize => Alphabet.IsCanonicalizable;

    /// <inheritdoc/>
    protected override void CanonicalizeCore(ReadOnlySpan<char> source, Span<char> destination) => Alphabet.Canonicalize(source, destination);

    /// <inheritdoc/>
    protected override int GetMaxCharCountCore(int byteCount, DataEncodingOptions options)
    {
        int charCount = (byteCount * SymbolsPerEncodedBlock + BytesPerDecodedBlock - 1) / BytesPerDecodedBlock;

        if ((options & DataEncodingOptions.NoPadding) == 0)
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

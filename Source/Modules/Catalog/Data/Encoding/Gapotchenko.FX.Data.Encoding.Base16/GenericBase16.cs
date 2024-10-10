// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Provides a generic implementation of Base16 encoding.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericBase16 : TextDataEncoding, IBase16
{
    private protected GenericBase16(TextDataEncodingAlphabet alphabet)
    {
        if (alphabet == null)
            throw new ArgumentNullException(nameof(alphabet));

        if (alphabet.Size != Base)
        {
            throw new ArgumentException(
                string.Format("The alphabet size for {0} encoding should be equal to {1}.", this, Base),
                nameof(alphabet));
        }

        Alphabet = alphabet;
    }

    /// <summary>
    /// The encoding alphabet.
    /// </summary>
    protected TextDataEncodingAlphabet Alphabet { get; }

    #region Parameters

    private protected const string Name = "Base16";

    /// <summary>
    /// The base of the encoding.
    /// </summary>
    protected const int Base = 1 << BitsPerSymbol;

    /// <summary>
    /// Number of bits per encoded symbol.
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

    /// <summary>
    /// Base16 encoding efficiency.
    /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
    /// </summary>
    public new const float Efficiency = (float)BytesPerDecodedBlock / SymbolsPerEncodedBlock;

    /// <summary>
    /// Gets the number of Base16 symbols per encoded byte of data.
    /// </summary>
    public const int SymbolsPerByte = SymbolsPerEncodedBlock / BytesPerDecodedBlock;

    #endregion

    /// <inheritdoc/>
    public int Radix => Base;

    /// <inheritdoc/>
    protected override float EfficiencyCore => Efficiency;

    static int GetLineWidth(DataEncodingOptions options) =>
        (options & DataEncodingOptions.Indent) != 0 ?
            16 * SymbolsPerEncodedBlock :
            32 * SymbolsPerEncodedBlock;

    abstract class CodecContextBase(TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
    {
        protected readonly TextDataEncodingAlphabet m_Alphabet = alphabet;
        protected readonly DataEncodingOptions m_Options = options;

        #region Parameters

        protected const int MaskSymbol = (1 << BitsPerSymbol) - 1;

        #endregion

        protected bool m_Eof;
    }

    sealed class EncoderContext(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
        CodecContextBase(alphabet, options),
        IEncoderContext
    {
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
                m_Buffer[0] = Capitalize(alphabet[(b >> BitsPerSymbol) & MaskSymbol]);
                m_Buffer[1] = Capitalize(alphabet[b & MaskSymbol]);

                EmitBreak(output);
                output.Write(m_Buffer);

                MoveLinePosition(SymbolsPerEncodedBlock);
            }
        }

        char Capitalize(char c) => TextDataEncoding.Capitalize(c, m_Options);

        readonly char[] m_Buffer = new char[SymbolsPerEncodedBlock];

        void MoveLinePosition(int delta) => m_LinePosition += delta;

        void EmitBreak(TextWriter output)
        {
            if (m_LinePosition >= m_LineWidth)
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

        readonly int m_LineWidth = GetLineWidth(options);
        int m_LinePosition;
        bool m_Indent;
    }

    sealed class DecoderContext(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
        CodecContextBase(alphabet, options),
        IDecoderContext
    {
        public bool Decode(ReadOnlySpan<char> input, Stream output, bool throwOnError)
        {
            if (m_Eof)
                return true;

            if (input == null)
            {
                m_Eof = true;
                return FlushDecode(throwOnError);
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
                            (m_Options & DataEncodingOptions.Pure) == 0 &&
                            (c is '-' || char.IsWhiteSpace(c));

                        if (!ok)
                        {
                            if (throwOnError)
                                throw new InvalidDataException($"Encountered a non-{Name} character.");
                            return false;
                        }
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

            return true;
        }

        bool FlushDecode(bool throwOnError)
        {
            switch (m_Modulus)
            {
                case 0:
                    // Nothing to do.
                    return true;
                case 1:
                    // 4 bits
                    if (!ValidateIncompleteByte(throwOnError))
                        return false;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            m_Modulus = 0;
            return true;
        }

        byte m_Bits;
        int m_Modulus;

        bool ValidateIncompleteByte(bool throwOnError)
        {
            if ((m_Options & DataEncodingOptions.Padding) != 0)
            {
                if (throwOnError)
                    throw new InvalidDataException($"Invalid {Name} padding.");
                return false;
            }

            if ((m_Options & DataEncodingOptions.Relax) == 0)
            {
                if (throwOnError)
                    throw new InvalidDataException($"Cannot decode the last byte due to a missing {Name} input symbol.");
                return false;
            }

            return true;
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
    protected virtual IEncoderContext CreateEncoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) =>
        new EncoderContext(alphabet, options);

    /// <summary>
    /// Creates decoder context with specified alphabet and options.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    /// <param name="options">The options.</param>
    protected virtual IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) =>
        new DecoderContext(alphabet, options);

    /// <inheritdoc/>
    public sealed override bool IsCaseSensitive => Alphabet.IsCaseSensitive;

    /// <inheritdoc/>
    protected sealed override int PaddingCore => SymbolsPerEncodedBlock;

    /// <inheritdoc/>
    public override bool CanCanonicalize => Alphabet.IsCanonicalizable;

    /// <inheritdoc/>
    protected override void CanonicalizeCore(ReadOnlySpan<char> source, Span<char> destination) => Alphabet.Canonicalize(source, destination);

    /// <inheritdoc/>
    protected override int GetMaxCharCountCore(int byteCount, DataEncodingOptions options)
    {
        int charCount = (byteCount * SymbolsPerEncodedBlock + BytesPerDecodedBlock - 1) / BytesPerDecodedBlock;

        int newLineCount =
            (options & DataEncodingOptions.Wrap) != 0 ?
                Math.Max(charCount - 1, 0) / GetLineWidth(options) :
                0;

        int indentCount =
            (options & DataEncodingOptions.Indent) != 0 ?
                Math.Max(byteCount - 1, 0) - newLineCount :
                0;

        return charCount + indentCount + newLineCount * MaxNewLineCharCount;
    }

    /// <inheritdoc/>
    protected override int GetMaxByteCountCore(int charCount, DataEncodingOptions options) =>
        (charCount * BytesPerDecodedBlock + SymbolsPerEncodedBlock - 1) / SymbolsPerEncodedBlock;
}

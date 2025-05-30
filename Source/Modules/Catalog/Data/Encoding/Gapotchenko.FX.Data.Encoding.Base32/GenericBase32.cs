﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Provides a generic implementation of Base32 encoding.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericBase32 : TextDataEncoding, IBase32
{
    private protected GenericBase32(TextDataEncodingAlphabet alphabet, char paddingChar)
    {
        if (alphabet == null)
            throw new ArgumentNullException(nameof(alphabet));

        ValidateAlphabet(alphabet);

        Alphabet = alphabet;
        PaddingChar = paddingChar;
    }

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

    /// <summary>
    /// The encoding alphabet.
    /// </summary>
    protected TextDataEncodingAlphabet Alphabet { get; }

    #region Parameters

    /// <summary>
    /// The base of the encoding.
    /// </summary>
    protected const int Base = 1 << BitsPerSymbol;

    /// <summary>
    /// Number of bits per encoded symbol.
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

    /// <summary>
    /// Bit mask of an alphabet symbol.
    /// </summary>
    protected const int SymbolMask = (1 << BitsPerSymbol) - 1;

    /// <summary>
    /// Base32 encoding efficiency.
    /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
    /// </summary>
    public new const float Efficiency = (float)BytesPerDecodedBlock / SymbolsPerEncodedBlock;

    #endregion

    /// <inheritdoc/>
    public int Radix => Base;

    /// <inheritdoc/>
    protected override float EfficiencyCore => Efficiency;

    const int LineWidth = 72;

    /// <summary>
    /// Base32 encoding treats wrapping and indentation interchangeably.
    /// </summary>
    const DataEncodingOptions FormatMask = DataEncodingOptions.Wrap | DataEncodingOptions.Indent;

    /// <summary>
    /// Shifts <see cref="UInt64"/> value by specified number of bits to the right when <paramref name="n"/> is positive or to the left when <paramref name="n"/> is negative.
    /// </summary>
    /// <param name="x">The value.</param>
    /// <param name="n">The number of bits to shift by.</param>
    /// <returns>The shifted value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private protected static ulong Shift(ulong x, int n) => n >= 0 ? x >> n : x << -n;

    private protected abstract class CodecContextBase(GenericBase32 encoding, TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
    {
        protected readonly GenericBase32 m_Encoding = encoding;
        protected readonly TextDataEncodingAlphabet m_Alphabet = alphabet;
        protected readonly DataEncodingOptions m_Options = options;

        #region Parameters

        protected const int Mask1Bit = (1 << 1) - 1;
        protected const int Mask2Bits = (1 << 2) - 1;
        protected const int Mask3Bits = (1 << 3) - 1;
        protected const int Mask4Bits = (1 << 4) - 1;

        #endregion

        protected ulong m_Bits;
        protected int m_Modulus;
        protected bool m_Eof;
    }

    private protected class EncoderContext(GenericBase32 encoding, TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
        CodecContextBase(encoding, alphabet, options),
        IEncoderContext
    {
        protected readonly char[] m_Buffer = new char[SymbolsPerEncodedBlock];

        int m_LinePosition;

        void MoveLinePosition(int delta) => m_LinePosition += delta;

        protected void EmitLineBreak(TextWriter output)
        {
            if (m_LinePosition >= LineWidth)
            {
                m_LinePosition = 0;

                if ((m_Options & FormatMask) != 0)
                    output.WriteLine();
            }
        }

        protected virtual void WriteBits(TextWriter output, int bitCount)
        {
            var alphabet = m_Alphabet;

            int i = 0;
            int s = bitCount;
            do
            {
                s -= BitsPerSymbol;
                m_Buffer[i++] = Capitalize(alphabet[(int)Shift(m_Bits, s) & SymbolMask]);
            }
            while (s > 0);

            if ((m_Options & DataEncodingOptions.NoPadding) == 0)
            {
                var paddingChar = Capitalize(m_Encoding.PaddingChar);

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

                if (++m_Modulus == BytesPerDecodedBlock)
                {
                    m_Modulus = 0;

                    for (int i = 0; i < SymbolsPerEncodedBlock; ++i)
                        m_Buffer[i] = Capitalize(alphabet[(int)(m_Bits >> ((SymbolsPerEncodedBlock - 1 - i) * BitsPerSymbol)) & SymbolMask]);

                    EmitLineBreak(output);
                    output.Write(m_Buffer);

                    MoveLinePosition(SymbolsPerEncodedBlock);
                }
            }
        }

        char Capitalize(char c) => TextDataEncoding.Capitalize(c, m_Options);
    }

    private protected class DecoderContext(GenericBase32 encoding, TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
        CodecContextBase(encoding, alphabet, options),
        IDecoderContext
    {
        protected readonly byte[] m_Buffer = new byte[BytesPerDecodedBlock];

        public char? Separator { get; init; }

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

                return FlushDecode(output, throwOnError);
            }

            var alphabet = m_Alphabet;
            bool isCaseSensitive = alphabet.IsCaseSensitive;
            var paddingChar = m_Encoding.PaddingChar;
            bool relax = (options & DataEncodingOptions.Relax) != 0;
            bool pure = (m_Options & DataEncodingOptions.Pure) != 0;

            foreach (var c in input)
            {
                if (CharEqual(c, paddingChar, isCaseSensitive))
                {
                    if (padding)
                    {
                        if (!ValidatePaddingChar(throwOnError))
                            return false;
                    }

                    if (!FlushDecode(output, throwOnError))
                        return false;

                    continue;
                }

                int b = alphabet.IndexOf(c);
                if (b == -1)
                {
                    if (!relax)
                    {
                        bool ok =
                            !pure &&
                            (char.IsWhiteSpace(c) ||
                            c == Separator);

                        if (!ok)
                        {
                            if (throwOnError)
                                throw new InvalidDataException($"Encountered an invalid input character for {m_Encoding} encoding.");
                            return false;
                        }
                    }
                    continue;
                }

                if (!ValidatePaddingState(throwOnError))
                    return false;

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

            return true;
        }

        bool FlushDecode(Stream output, bool throwOnError)
        {
            if (m_Modulus != 0)
            {
                if (!FlushDecodeCore(output, throwOnError))
                    return false;
                m_Modulus = 0;
            }
            return true;
        }

        protected virtual bool FlushDecodeCore(Stream output, bool throwOnError)
        {
            switch (m_Modulus)
            {
                case 1:  // 5 bits
                    if (!ValidateIncompleteByte(throwOnError))
                        return false;
                    break;

                case 2:  // 2 * 5 bits = 10 = 8 + 2
                    if (!ValidateLastSymbol(Mask2Bits, throwOnError))
                        return false;
                    output.WriteByte((byte)(m_Bits >> 2));
                    break;

                case 3:  // 3 * 5 bits = 15 = 8 + 7
                    if (!ValidateIncompleteByte(throwOnError))
                        return false;
                    output.WriteByte((byte)(m_Bits >> 7));
                    break;

                case 4:  // 4 * 5 bits = 20 = 2 * 8 + 4
                    if (!ValidateLastSymbol(Mask4Bits, throwOnError))
                        return false;

                    m_Buffer[0] = (byte)(m_Bits >> 12);
                    m_Buffer[1] = (byte)(m_Bits >> 4);

                    output.Write(m_Buffer, 0, 2);
                    break;

                case 5:  // 5 * 5 bits = 25 = 3 * 8 + 1
                    if (!ValidateLastSymbol(Mask1Bit, throwOnError))
                        return false;

                    m_Buffer[0] = (byte)(m_Bits >> 17);
                    m_Buffer[1] = (byte)(m_Bits >> 9);
                    m_Buffer[2] = (byte)(m_Bits >> 1);

                    output.Write(m_Buffer, 0, 3);
                    break;

                case 6:  // 6 * 5 bits = 30 = 3 * 8 + 6
                    if (!ValidateIncompleteByte(throwOnError))
                        return false;

                    m_Buffer[0] = (byte)(m_Bits >> 22);
                    m_Buffer[1] = (byte)(m_Bits >> 14);
                    m_Buffer[2] = (byte)(m_Bits >> 6);

                    output.Write(m_Buffer, 0, 3);
                    break;

                case 7:  // 7 * 5 bits = 35 = 4 * 8 + 3
                    if (!ValidateLastSymbol(Mask3Bits, throwOnError))
                        return false;

                    m_Buffer[0] = (byte)(m_Bits >> 27);
                    m_Buffer[1] = (byte)(m_Bits >> 19);
                    m_Buffer[2] = (byte)(m_Bits >> 11);
                    m_Buffer[3] = (byte)(m_Bits >> 3);

                    output.Write(m_Buffer, 0, 4);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return true;
        }

        protected bool ValidateIncompleteByte(bool throwOnError)
        {
            if ((m_Options & DataEncodingOptions.Relax) == 0)
            {
                if (throwOnError)
                    throw new InvalidDataException($"Cannot decode the last byte of {m_Encoding} encoding due to a missing symbol.");
                return false;
            }

            return true;
        }

        protected bool ValidateLastSymbol(ulong zeroMask, bool throwOnError)
        {
            if ((m_Options & DataEncodingOptions.Relax) == 0 &&
                (m_Bits & zeroMask) != 0)
            {
                if (throwOnError)
                    throw new InvalidDataException($"The insignificant bits of the last symbol in {m_Encoding} encoding are expected to be zero.");
                return false;
            }

            return true;
        }

        int m_Padding;

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

        InvalidDataException CreateInvalidPaddingException() => new($"Invalid padding for {m_Encoding} encoding.");
    }

    /// <inheritdoc/>
    protected sealed override IEncoderContext CreateEncoderContext(DataEncodingOptions options) => CreateEncoderContextCore(Alphabet, options);

    /// <inheritdoc/>
    protected sealed override IDecoderContext CreateDecoderContext(DataEncodingOptions options) => CreateDecoderContextCore(Alphabet, options);

    /// <summary>
    /// Creates an encoder context with specified alphabet and options.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    /// <param name="options">The options.</param>
    /// <returns>An encoder context instance.</returns>
    protected virtual IEncoderContext CreateEncoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) =>
        new EncoderContext(this, alphabet, options);

    /// <summary>
    /// Creates a decoder context with specified alphabet and options.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    /// <param name="options">The options.</param>
    /// <returns>A decoder context instance.</returns>
    protected virtual IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) =>
        new DecoderContext(this, alphabet, options);

    /// <inheritdoc/>
    public sealed override bool IsCaseSensitive => Alphabet.IsCaseSensitive;

    /// <inheritdoc/>
    protected override int PaddingCore => SymbolsPerEncodedBlock;

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

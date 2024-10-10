// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Provides a generic implementation of z-base-32 encoding.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericZBase32 : GenericBase32
{
    /// <summary>
    /// Initializes a new instance of <see cref="GenericZBase32"/> class with the specified alphabet and padding character.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    /// <param name="paddingChar">The padding character.</param>
    protected GenericZBase32(TextDataEncodingAlphabet alphabet, char paddingChar) :
        base(alphabet, paddingChar)
    {
    }

    sealed class ZBase32EncoderContext(GenericZBase32 encoding, TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
        EncoderContext(encoding, alphabet, options)
    {
        protected override void WriteBits(TextWriter output, int bitCount)
        {
            var alphabet = m_Alphabet;

            var bits = m_Bits;

            bool compress =
                (m_Options & DataEncodingOptions.Compress) != 0 && // compression is only active when it is on and
                ((byte)bits != 0 || (bits & ((1ul << bitCount) - 1)) == 0); // the last byte is non-zero or the whole block is filled with zero bits

            int i = 0; // output symbol index
            int s = bitCount; // shift accumulator
            int pbi = 0; // previous input byte index
            int li = 1; // last output symbol index

            do
            {
                s -= BitsPerSymbol;

                int si = (int)Shift(bits, s) & SymbolMask; // symbol index
                m_Buffer[i++] = Capitalize(alphabet[si]); // map symbol

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

            if ((m_Options & DataEncodingOptions.NoPadding) == 0)
            {
                var paddingChar = Capitalize(((GenericZBase32)m_Encoding).PaddingChar);

                while (i < SymbolsPerEncodedBlock)
                    m_Buffer[i++] = paddingChar;
            }

            EmitLineBreak(output);
            output.Write(m_Buffer, 0, i);
        }

        char Capitalize(char c) => TextDataEncoding.Capitalize(c, m_Options);
    }

    sealed class ZBase32DecoderContext(GenericZBase32 encoding, TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
        DecoderContext(encoding, alphabet, options)
    {
        void ReadBits(Stream output, int bitCount)
        {
            int i = 0; // output byte index
            int s = bitCount; // shift accumulator
            var li = 1; // last output byte index

            do
            {
                s -= 8;

                byte b = (byte)Shift(m_Bits, s);
                m_Buffer[i++] = b;

                if (b != 0 ||
                    s >= 0 ||
                   (m_Options & DataEncodingOptions.Compress) != 0 && (m_Bits & ((1UL << bitCount) - 1)) == 0)
                {
                    li = i;
                }
            }
            while (s > 0);

            output.Write(m_Buffer, 0, li);
        }

        protected override bool FlushDecodeCore(Stream output, bool throwOnError)
        {
            switch (m_Modulus)
            {
                case var k when k > 0 && k < SymbolsPerEncodedBlock:
                    ReadBits(output, k * BitsPerSymbol);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return true;
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

    /// <inheritdoc/>
    protected override IEncoderContext CreateEncoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) =>
        new ZBase32EncoderContext(this, alphabet, options);

    /// <inheritdoc/>
    protected override IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) =>
        new ZBase32DecoderContext(this, alphabet, options);
}

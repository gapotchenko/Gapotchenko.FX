using System;
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

        /// <inheritdoc/>
        public int Radix => 16;

        /// <summary>
        /// Base64 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.5f;

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

            protected bool m_Eof;

            protected const int Mask4Bits = 0x0f;
        }

        sealed class EncoderContext : CodecContextBase, IEncoderContext
        {
            public EncoderContext(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(alphabet, options)
            {
                m_Width =
                    (m_Options & DataEncodingOptions.Indent) != 0 ?
                        16 * 2 :
                        32 * 2;
            }

            readonly int m_Width;
            readonly char[] m_Buffer = new char[2];

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
                    m_Buffer[0] = alphabet[(b >> 4) & Mask4Bits];
                    m_Buffer[1] = alphabet[b & Mask4Bits];

                    EmitBreak(output);
                    output.Write(m_Buffer);

                    MoveLinePosition(2);
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
                            bool ok =
                                c == '-' ||
                                char.IsWhiteSpace(c);

                            if (!ok)
                                throw new InvalidDataException("Encountered a non-Base16 character.");
                        }
                        continue;
                    }

                    // Accumulate data bits.
                    m_Bits = (byte)((m_Bits << 4) | (b & Mask4Bits));

                    if (++m_Modulus == 2)
                    {
                        m_Modulus = 0;
                        output.WriteByte(m_Bits);
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
                if ((m_Options & DataEncodingOptions.Relax) == 0)
                    throw new InvalidDataException("Cannot decode the last byte due to missing Base16 symbol.");
            }
        }

        /// <inheritdoc/>
        protected override IEncoderContext CreateEncoderContext(DataEncodingOptions options) => new EncoderContext(Alphabet, options);

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContext(DataEncodingOptions options) => new DecoderContext(GetDecoderAlphabet(options), options);

        /// <summary>
        /// Gets decoder alphabet.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The alphabet.</returns>
        protected virtual TextDataEncodingAlphabet GetDecoderAlphabet(DataEncodingOptions options) => Alphabet;

        /// <inheritdoc/>
        public override bool IsCaseSensitive => false;

        /// <inheritdoc/>
        protected override int PaddingCore => 1;
    }
}

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
    public abstract class GenericBase64 : DataTextEncoding, IBase64
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericBase64"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericBase64(in DataTextEncodingAlphabet alphabet)
        {
            if (alphabet.Size != 64)
                throw new ArgumentException("The alphabet size of a Base64 encoding should be 64.", nameof(alphabet));

            Alphabet = alphabet;
        }

        /// <summary>
        /// The encoding alphabet.
        /// </summary>
        protected readonly DataTextEncodingAlphabet Alphabet;

        /// <summary>
        /// Base64 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.75f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        struct Context
        {
            public Context(GenericBase64 base64, DataTextEncodingOptions options) :
                this()
            {
                m_Base64 = base64;
                m_Options = options;
            }

            readonly GenericBase64 m_Base64;
            readonly DataTextEncodingOptions m_Options;

            int m_Bits;
            int m_Modulus;
            bool m_Eof;

            const int Mask6Bits = 0x3f;

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
                            // 8 bits = 6 + 2
                            output.Write(m_Base64.Alphabet[(m_Bits >> 2) & Mask6Bits]); // 6 bits
                            output.Write(m_Base64.Alphabet[(m_Bits << 4) & Mask6Bits]); // 2 bits
                            if ((m_Options & DataTextEncodingOptions.NoPadding) == 0)
                            {
                                output.Write(PaddingChar);
                                output.Write(PaddingChar);
                            }
                            break;

                        case 2:
                            // 16 bits = 6 + 6 + 4
                            output.Write(m_Base64.Alphabet[(m_Bits >> 10) & Mask6Bits]); // 6 bits
                            output.Write(m_Base64.Alphabet[(m_Bits >> 4) & Mask6Bits]); // 6 bits
                            output.Write(m_Base64.Alphabet[(m_Bits << 2) & Mask6Bits]); // 4 bits
                            if ((m_Options & DataTextEncodingOptions.NoPadding) == 0)
                                output.Write(PaddingChar);
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                {
                    foreach (var b in input)
                    {
                        m_Bits = (m_Bits << 8) | b;

                        if (++m_Modulus == 3)
                        {
                            m_Modulus = 0;

                            // 3 bytes = 24 bits = 4 * 6 bits
                            output.Write(m_Base64.Alphabet[(m_Bits >> 18) & Mask6Bits]);
                            output.Write(m_Base64.Alphabet[(m_Bits >> 12) & Mask6Bits]);
                            output.Write(m_Base64.Alphabet[(m_Bits >> 6) & Mask6Bits]);
                            output.Write(m_Base64.Alphabet[m_Bits & Mask6Bits]);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override string GetStringCore(ReadOnlySpan<byte> data, DataTextEncodingOptions options)
        {
            var tw = new StringWriter();

            var context = new Context(this, options);
            context.Encode(data, tw);
            context.Encode(null, tw);

            return tw.ToString();
        }

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

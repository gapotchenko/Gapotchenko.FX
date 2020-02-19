using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Base32 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericBase32 : DataTextEncoding, IBase32
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericBase32"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericBase32(in DataTextEncodingAlphabet alphabet)
        {
            if (alphabet.Size != 32)
                throw new ArgumentException("The alphabet size for Base32 encoding should be 32.", nameof(alphabet));

            Alphabet = alphabet;
        }

        /// <summary>
        /// Base32 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.625f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        /// <summary>
        /// The encoding alphabet.
        /// </summary>
        protected readonly DataTextEncodingAlphabet Alphabet;

        /// <inheritdoc/>
        protected override string GetStringCore(ReadOnlySpan<byte> data, DataTextEncodingOptions options)
        {
            int n = data.Length;

            var sb = new StringBuilder((n + 7) << 3 / 5 + 8);

            int i = 0, index = 0;

            while (i < n)
            {
                int digit;
                byte b = data[i];
                if (index > 3)
                {
                    int nbi = i + 1;
                    byte nb = nbi < n ? data[nbi] : (byte)0;

                    digit = b & (0xff >> index);
                    index = (index + 5) & 7;
                    digit <<= index;
                    digit |= nb >> (8 - index);
                    i++;
                }
                else
                {
                    digit = (b >> (8 - (index + 5))) & 0x1f;
                    index = (index + 5) & 7;
                    if (index == 0)
                        i++;
                }
                sb.Append(Alphabet[digit]);
            }

            if ((options & DataTextEncodingOptions.NoPadding) == 0)
            {
                int padding = Padding;
                while (sb.Length % padding != 0)
                    sb.Append(PaddingChar);
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override byte[] GetBytesCore(ReadOnlySpan<char> s, DataTextEncodingOptions options)
        {
            if ((options & DataTextEncodingOptions.RequirePadding) != 0)
            {
                if (!IsPadded(s))
                    throw new InvalidDataException("Encountered unpadded input of a Base32 encoding.");
            }

            s = Unpad(s);

            int textLength = s.Length;
            int byteCount = textLength * 5 >> 3;
            var bytes = new byte[byteCount];

            for (int i = 0, index = 0, offset = 0; i < textLength; i++)
            {
                int digit = Alphabet.IndexOf(s[i]);
                if (digit == -1)
                    continue;

                if (index <= 3)
                {
                    index = (index + 5) & 7;
                    if (index == 0)
                    {
                        bytes[offset++] |= (byte)digit;
                        if (offset >= byteCount)
                            break;
                    }
                    else
                    {
                        bytes[offset] |= (byte)(digit << (8 - index));
                    }
                }
                else
                {
                    index = (index + 5) & 7;
                    bytes[offset++] |= (byte)(digit >> index);

                    if (offset >= byteCount)
                        break;

                    bytes[offset] |= (byte)(digit << (8 - index));
                }
            }

            return bytes;
        }

        /// <inheritdoc/>
        public override bool IsCaseSensitive => false;

        /// <inheritdoc/>
        protected override int PaddingCore => 8;

        /// <summary>
        /// Padding character.
        /// </summary>
        protected const char PaddingChar = '=';

        /// <inheritdoc/>
        protected override string PadCore(ReadOnlySpan<char> s) => s.ToString().PadRight((s.Length + 7) >> 3 << 3, PaddingChar);

        /// <inheritdoc/>
        protected override ReadOnlySpan<char> UnpadCore(ReadOnlySpan<char> s) => s.TrimEnd(PaddingChar);
    }
}

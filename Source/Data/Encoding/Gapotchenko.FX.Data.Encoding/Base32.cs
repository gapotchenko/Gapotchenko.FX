using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding
{
    using Encoding = System.Text.Encoding;

    /// <summary>
    /// Base32 encoding.
    /// </summary>
    public class Base32 : DataTextEncoding, IDataTextEncoding
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Base32"/> class.
        /// </summary>
        protected Base32()
        {
        }

        /// <inheritdoc/>
        public override string Name => "Base32";

        /// <summary>
        /// Base32 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.625f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static readonly string m_Symbols = "QAZ2WSX3" + "EDC4RFV5" + "TGB6YHN7" + "UJM8K9LP";

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Base32 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in Base32, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(byte[] data)
        {
            if (data == null)
                return null;

            var sb = new StringBuilder();

            int n = data.Length;
            int i = 0;
            int hi = 5;

            while (i < n)
            {
                int charIndex;

                if (hi > 8)
                {
                    charIndex = data[i++] >> (hi - 5);
                    if (i != n)
                        charIndex |= (byte)(data[i] << (16 - hi)) >> 3;
                    hi -= 3;
                }
                else if (hi == 8)
                {
                    charIndex = data[i++] >> 3;
                    hi -= 3;
                }
                else
                {
                    charIndex = (byte)(data[i] << (8 - hi)) >> 3;
                    hi += 5;
                }

                sb.Append(m_Symbols[charIndex]);
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string GetStringCore(byte[] data) => GetString(data);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base32 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(string s)
        {
            if (s == null)
                return null;

            int textLength = s.Length;
            int bytesCount = textLength * 5 / 8;
            var bytes = new byte[bytesCount];

            s = s.ToUpperInvariant();

            if (textLength < 3)
            {
                bytes[0] = (byte)(m_Symbols.IndexOf(s[0]) | m_Symbols.IndexOf(s[1]) << 5);
            }
            else
            {
                int bits = m_Symbols.IndexOf(s[0]) | m_Symbols.IndexOf(s[1]) << 5;
                int bitsCount = 10;
                int charIndex = 2;
                for (int i = 0; i < bytesCount; i++)
                {
                    bytes[i] = (byte)bits;
                    bits >>= 8;
                    bitsCount -= 8;
                    while (bitsCount < 8 && charIndex < textLength)
                    {
                        bits |= m_Symbols.IndexOf(s[charIndex++]) << bitsCount;
                        bitsCount += 5;
                    }
                }
            }

            return bytes;
        }

        /// <inheritdoc/>
        protected override byte[] GetBytesCore(string s) => GetBytes(s);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IDataTextEncoding m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="Base32"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IDataTextEncoding Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new Base32();
                return m_Instance;
            }
        }
    }
}

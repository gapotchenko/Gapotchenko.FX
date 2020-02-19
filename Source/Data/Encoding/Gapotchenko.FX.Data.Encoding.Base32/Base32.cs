﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Implements Base32 encoding described in RFC 4648.
    /// </summary>
    public class Base32 : DataTextEncoding, IDataTextEncoding
    {
        internal Base32() :
            this(new DataTextEncodingAlphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567", false))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Base32"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        internal Base32(in DataTextEncodingAlphabet alphabet)
        {
            m_Alphabet = alphabet;
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
        readonly DataTextEncodingAlphabet m_Alphabet;

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Base32 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in Base32, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Base32 symbols with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation, in Base32, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data, DataTextEncodingOptions options) => Instance.GetString(data, options);

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
                sb.Append(m_Alphabet[digit]);
            }

            if ((options & DataTextEncodingOptions.NoPadding) == 0)
            {
                int padding = Padding;
                while (sb.Length % padding != 0)
                    sb.Append(PaddingChar);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base32 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base32 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s, DataTextEncodingOptions options) => Instance.GetBytes(s, options);

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
                int digit = m_Alphabet.IndexOf(s[i]);
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

        /// <summary>
        /// The number of characters for padding of an encoded string representation.
        /// </summary>
        public new const int Padding = 8;

        /// <inheritdoc/>
        protected override int PaddingCore => Padding;

        const char PaddingChar = '=';

        /// <summary>
        /// Pads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to pad.</param>
        /// <returns>The padded encoded string.</returns>
        public new static string Pad(ReadOnlySpan<char> s) =>
            s == null ?
                null :
                s.ToString().PadRight((s.Length + 7) >> 3 << 3, PaddingChar);

        /// <inheritdoc/>
        protected override string PadCore(ReadOnlySpan<char> s) => Pad(s);

        /// <summary>
        /// Unpads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to unpad.</param>
        /// <returns>The unpadded encoded string.</returns>
        public new static ReadOnlySpan<char> Unpad(ReadOnlySpan<char> s) => s.TrimEnd(PaddingChar);

        /// <inheritdoc/>
        protected override ReadOnlySpan<char> UnpadCore(ReadOnlySpan<char> s) => Unpad(s);

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

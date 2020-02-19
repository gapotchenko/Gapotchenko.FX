﻿using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Implements Base64 encoding described in RFC 4648.
    /// </summary>
    public sealed class Base64 : DataTextEncoding, IBase64
    {
        private Base64()
        {
        }

        /// <inheritdoc/>
        public override string Name => "Base64";

        /// <summary>
        /// Base64 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.75f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Base64 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in Base64, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data) => GetString(data, DataTextEncodingOptions.Default);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Base64 symbols with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation, in Base64, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data, DataTextEncodingOptions options) =>
            data == null ?
                null :
                ApplyEncodingOptions(
                    Convert.ToBase64String(
#if TFF_MEMORY && !TFF_MEMORY_OOB
                        data
#else
                        data.ToArray()
#endif
                        ),
                    options);

        static string ApplyEncodingOptions(string s, DataTextEncodingOptions options)
        {
            if ((options & DataTextEncodingOptions.NoPadding) != 0)
                s = Unpad(s.AsSpan()).ToString();
            return s;
        }

        /// <inheritdoc/>
        protected override string GetStringCore(ReadOnlySpan<byte> data, DataTextEncodingOptions options) => GetString(data, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base64 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s) => GetBytes(s, DataTextEncodingOptions.Default);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base64 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s, DataTextEncodingOptions options) =>
            s == null ?
                null :
                Convert.FromBase64String(ApplyDecodingOptions(s, options));

        static string ApplyDecodingOptions(ReadOnlySpan<char> s, DataTextEncodingOptions options)
        {
            if ((options & DataTextEncodingOptions.RequirePadding) == 0)
                return Pad(s);
            else
                return s.ToString();
        }

        /// <inheritdoc/>
        protected override byte[] GetBytesCore(ReadOnlySpan<char> s, DataTextEncodingOptions options) => GetBytes(s, options);

        /// <inheritdoc/>
        public override bool IsCaseSensitive => true;

        /// <summary>
        /// The number of characters for padding of an encoded string representation.
        /// </summary>
        public new const int Padding = 4;

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
                s.ToString().PadRight((s.Length + 3) >> 2 << 2, PaddingChar);

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
        /// Returns a default instance of <see cref="Base64"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IDataTextEncoding Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new Base64();
                return m_Instance;
            }
        }
    }
}

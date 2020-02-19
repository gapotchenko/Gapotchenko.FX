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
    /// The base class for <see cref="IDataTextEncoding"/> implementations.
    /// </summary>
    public abstract class DataTextEncoding : DataEncoding, IDataTextEncoding
    {
        /// <inheritdoc/>
        public string GetString(ReadOnlySpan<byte> data) => GetString(data, DataTextEncodingOptions.Default);

        /// <inheritdoc/>
        public string GetString(ReadOnlySpan<byte> data, DataTextEncodingOptions options) => data == null ? null : GetStringCore(data, options);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="data"/>.</returns>
        protected abstract string GetStringCore(ReadOnlySpan<byte> data, DataTextEncodingOptions options);

        /// <inheritdoc/>
        public byte[] GetBytes(ReadOnlySpan<char> s) => GetBytes(s, DataTextEncodingOptions.Default);

        /// <inheritdoc/>
        public byte[] GetBytes(ReadOnlySpan<char> s, DataTextEncodingOptions options) => s == null ? null : GetBytesCore(s, options);

        /// <summary>
        /// Decodes the specified string to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        protected abstract byte[] GetBytesCore(ReadOnlySpan<char> s, DataTextEncodingOptions options);

        /// <inheritdoc/>
        public abstract bool IsCaseSensitive { get; }

        /// <inheritdoc/>
        public int Padding => PaddingCore;

        /// <summary>
        /// Gets the number of characters used for padding of encoded string representation.
        /// </summary>
        protected virtual int PaddingCore => 1;

        /// <inheritdoc/>
        public string Pad(ReadOnlySpan<char> s) => s == null ? null : PadCore(s);

        /// <summary>
        /// Pads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to pad.</param>
        /// <returns>The padded encoded string.</returns>
        protected virtual string PadCore(ReadOnlySpan<char> s) => s.ToString();

        /// <inheritdoc/>
        public ReadOnlySpan<char> Unpad(ReadOnlySpan<char> s) => s == null ? null : UnpadCore(s);

        /// <summary>
        /// Unpads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to unpad.</param>
        /// <returns>The unpadded encoded string.</returns>
        protected virtual ReadOnlySpan<char> UnpadCore(ReadOnlySpan<char> s) => s;

        /// <inheritdoc/>
        public bool IsPadded(ReadOnlySpan<char> s) => s.Length % Padding == 0;

        /// <inheritdoc/>
        protected override byte[] EncodeDataCore(ReadOnlySpan<byte> data) => Encoding.ASCII.GetBytes(GetString(data));

        /// <inheritdoc/>
        protected override byte[] DecodeDataCore(ReadOnlySpan<byte> data) =>
            GetBytes(
                Encoding.ASCII.GetString(
#if TFF_MEMORY && !TFF_MEMORY_OOB
                    data
#else
                    data.ToArray()
#endif
                )
                .AsSpan());
    }
}

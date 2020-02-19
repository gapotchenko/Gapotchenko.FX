using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        public byte[] GetBytes(ReadOnlySpan<char> s, DataTextEncodingOptions options)
        {
            if (s == null)
                return null;

            if ((options & DataTextEncodingOptions.RequirePadding) != 0)
            {
                if (!IsPadded(s))
                    throw new InvalidDataException(string.Format("Encountered unpadded input of {0} encoding.", Name));
            }

            return GetBytesCore(s, options);
        }

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

        #region Implementation Helpers

        string Pad(ReadOnlySpan<char> s, char paddingChar, bool right)
        {
            if (s == null)
                return null;

            int padding = Padding;

            int width =
                padding < 2 ?
                    0 :
                    (s.Length + padding - 1) / padding * padding;

            string output = s.ToString();

            if (width == 0)
                return output;
            else if (right)
                return output.PadRight(width, paddingChar);
            else
                return output.PadLeft(width, paddingChar);
        }

        /// <summary>
        /// Pads the encoded string to the right.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The padded encoded string.</returns>
        protected string PadRight(ReadOnlySpan<char> s, char paddingChar) => Pad(s, paddingChar, true);

        /// <summary>
        /// Pads the encoded string to the left.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The padded encoded string.</returns>
        protected string PadLeft(ReadOnlySpan<char> s, char paddingChar) => Pad(s, paddingChar, false);

        /// <summary>
        /// Unpads the encoded string from the right side.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The unpadded encoded string.</returns>
        protected ReadOnlySpan<char> UnpadRight(ReadOnlySpan<char> s, char paddingChar) => s.TrimEnd(paddingChar);

        /// <summary>
        /// Unpads the encoded string from the left side.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The unpadded encoded string.</returns>
        protected ReadOnlySpan<char> UnpadLeft(ReadOnlySpan<char> s, char paddingChar) => s.TrimStart(paddingChar);

        #endregion
    }
}

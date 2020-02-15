﻿using System;
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
        public string GetString(ReadOnlySpan<byte> data) => data == null ? null : GetStringCore(data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation of the contents of <paramref name="data"/>.</returns>
        protected abstract string GetStringCore(ReadOnlySpan<byte> data);

        /// <inheritdoc/>
        public byte[] GetBytes(string s) => s == null ? null : GetBytesCore(s);

        /// <summary>
        /// Decodes the specified string to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        protected abstract byte[] GetBytesCore(string s);

        /// <inheritdoc/>
        public int Padding => PaddingCore;

        /// <summary>
        /// Gets the number of characters used for padding of encoded string representation.
        /// </summary>
        protected virtual int PaddingCore => 1;

        /// <inheritdoc/>
        public string Pad(string s) => s == null ? null : PadCore(s);

        /// <summary>
        /// Pads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to pad.</param>
        /// <returns>The padded encoded string.</returns>
        protected virtual string PadCore(string s) => s;

        /// <inheritdoc/>
        public string Unpad(string s) => s == null ? null : UnpadCore(s);

        /// <summary>
        /// Unpads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to unpad.</param>
        /// <returns>The unpadded encoded string.</returns>
        protected virtual string UnpadCore(string s) => s;

        /// <inheritdoc/>
        protected override byte[] EncodeDataCore(ReadOnlySpan<byte> data) => Encoding.ASCII.GetBytes(GetString(data));

        /// <inheritdoc/>
        protected override byte[] DecodeDataCore(ReadOnlySpan<byte> data) =>
            GetBytes(Encoding.ASCII.GetString(
#if TFF_MEMORY && !TFF_MEMORY_OOB
                data
#else
                data.ToArray()
#endif
                ));
    }
}

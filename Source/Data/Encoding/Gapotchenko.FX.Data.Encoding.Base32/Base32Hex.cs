﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Implements base32-hex encoding described in RFC 4648.
    /// </summary>
    public sealed class Base32Hex : GenericBase32
    {
        private Base32Hex() :
            base(new TextDataEncodingAlphabet("0123456789ABCDEFGHIJKLMNOPQRSTUV", false))
        {
        }

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with base32-hex symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in base32-hex, of the contents of <paramref name="data"/>.</returns>
        [return: NotNullIfNotNull("data")]
        public new static string? GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with base32-hex symbols with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation, in base32-hex, of the contents of <paramref name="data"/>.</returns>
        [return: NotNullIfNotNull("data")]
        public new static string? GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as base32-hex symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public new static byte[]? GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as base32-hex symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public static byte[]? GetBytes(string? s) => GetBytes(s.AsSpan());

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as base32-hex symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public new static byte[]? GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as base32-hex symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public static byte[]? GetBytes(string? s, DataEncodingOptions options) => GetBytes(s.AsSpan(), options);

        /// <summary>
        /// The number of characters for padding of an encoded string representation.
        /// </summary>
        public new const int Padding = 8;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase32? m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="Base32Hex"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IBase32 Instance => m_Instance ??= new Base32Hex();
    }
}
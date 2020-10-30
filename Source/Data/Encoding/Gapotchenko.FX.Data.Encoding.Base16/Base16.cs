using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Base16 encoding conforming to RFC 4648.
    /// </summary>
    public sealed class Base16 : GenericBase16
    {
        private Base16() :
            base(new TextDataEncodingAlphabet("0123456789ABCDEF", false))
        {
        }

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Base16 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in Base16, of the contents of <paramref name="data"/>.</returns>
        [return: NotNullIfNotNull("data")]
        public new static string? GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Base16 symbols with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation, in Base16, of the contents of <paramref name="data"/>.</returns>
        [return: NotNullIfNotNull("data")]
        public new static string? GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base16 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public new static byte[]? GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base16 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public static byte[]? GetBytes(string? s) => GetBytes(s.AsSpan());

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base16 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public new static byte[]? GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base16 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public static byte[]? GetBytes(string? s, DataEncodingOptions options) => GetBytes(s.AsSpan(), options);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase16? m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="Base16"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IBase16 Instance => m_Instance ??= new Base16();
    }
}

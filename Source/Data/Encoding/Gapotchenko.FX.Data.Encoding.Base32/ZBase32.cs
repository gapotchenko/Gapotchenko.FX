using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides z-base-32 encoding implementation.
    /// </summary>
    public sealed class ZBase32 : GenericZBase32
    {
        private ZBase32() :
            base(new TextDataEncodingAlphabet("ybndrfg8ejkmcpqxot1uwisza345h769", false))
        {
        }

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with z-base-32 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in z-base-32, of the contents of <paramref name="data"/>.</returns>
        [return: NotNullIfNotNull("data")]
        public new static string? GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with z-base-32 symbols with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation, in z-base-32, of the contents of <paramref name="data"/>.</returns>
        [return: NotNullIfNotNull("data")]
        public new static string? GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as z-base-32 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public new static byte[]? GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as z-base-32 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public static byte[]? GetBytes(string? s) => GetBytes(s.AsSpan());

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as z-base-32 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public new static byte[]? GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as z-base-32 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        [return: NotNullIfNotNull("s")]
        public static byte[]? GetBytes(string? s, DataEncodingOptions options) => GetBytes(s.AsSpan(), options);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase32? m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="ZBase32"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IBase32 Instance => m_Instance ??= new ZBase32();
    }
}

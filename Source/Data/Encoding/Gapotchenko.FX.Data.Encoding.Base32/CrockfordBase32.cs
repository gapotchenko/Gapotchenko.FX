using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides Crockford Base 32 encoding implementation.
    /// </summary>
    public sealed class CrockfordBase32 : GenericCrockfordBase32
    {
        private CrockfordBase32() : base(
            new TextDataEncodingAlphabet(
                "0123456789ABCDEFGHJKMNPQRSTVWXYZ*~$=U",
                false,
                new Dictionary<char, string>
                {
                    ['0'] = "O",
                    ['1'] = "IL",
                }))
        {
        }

        /// <inheritdoc/>
        public override string Name => "Crockford Base 32";

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Crockford Base 32 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in Crockford Base 32, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Crockford Base 32 symbols with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation, in Crockford Base 32, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Crockford Base 32 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Crockford Base 32 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public static byte[] GetBytes(string s) => GetBytes(s.AsSpan());

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Crockford Base 32 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Crockford Base 32 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes(s.AsSpan(), options);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile ICrockfordBase32 m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="CrockfordBase32"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static ICrockfordBase32 Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new CrockfordBase32();
                return m_Instance;
            }
        }
    }
}

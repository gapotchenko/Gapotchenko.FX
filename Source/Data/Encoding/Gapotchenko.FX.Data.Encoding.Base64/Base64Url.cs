using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Implements Base64 URL (base64url) encoding described in RFC 4648.
    /// </summary>
    public sealed class Base64Url : GenericBase64
    {
        private Base64Url() :
            base(new TextDataEncodingAlphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_"))
        {
        }

        /// <inheritdoc/>
        public override string Name => "base64url";

        /// <inheritdoc/>
        protected override DataEncodingOptions GetEffectiveOptions(DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Padding) == 0)
            {
                // Produce unpadded strings unless padding is explicitly requested as suggested by RFC 4648.
                // This is necessary in order to avoid '%' escape symbols in URI for '=' padding chars.
                options |= DataEncodingOptions.Unpad;
            }

            return options;
        }

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Relax) != 0)
                alphabet = Base64LinguaFranca.Alphabet;

            return base.CreateDecoderContextCore(alphabet, options);
        }

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with base64url symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in base64url, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with base64url symbols with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation, in base64url, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as base64url symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as base64url symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public static byte[] GetBytes(string s) => GetBytes(s.AsSpan());

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as base64url symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as base64url symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes(s.AsSpan(), options);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase64 m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="Base64Url"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IBase64 Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new Base64Url();
                return m_Instance;
            }
        }
    }
}

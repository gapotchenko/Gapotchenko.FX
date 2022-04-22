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
        Base64Url() :
            base(new TextDataEncodingAlphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_"), '=')
        {
        }

        /// <inheritdoc/>
        protected override DataEncodingOptions GetEffectiveOptions(DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Padding) == 0)
            {
                // Produce unpadded strings unless padding is explicitly requested as suggested by RFC 4648.
                // This is necessary in order to avoid '%' escape symbols in URI for '=' padding chars.
                options |= DataEncodingOptions.Unpad;
            }

            return base.GetEffectiveOptions(options);
        }

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Relax) != 0)
                alphabet = Base64LinguaFranca.Alphabet;

            return base.CreateDecoderContextCore(alphabet, options);
        }

        /// <summary>
        /// Encodes all the bytes in the specified span into a string of Base64 URL characters.
        /// </summary>
        /// <param name="data">The byte span to encode.</param>
        /// <returns>The string with encoded data.</returns>
        public new static string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes all the bytes in the specified span into a string of Base64 URL characters with specified options.
        /// </summary>
        /// <param name="data">The byte span to encode.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string with encoded data.</returns>
        public new static string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

        /// <summary>
        /// Decodes all Base64 URL characters in the specified read-only span into a byte array.
        /// </summary>
        /// <param name="s">The read-only character span to decode.</param>
        /// <returns>A byte array with decoded data.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes all Base64 URL characters in the specified read-only span into a byte array with specified options.
        /// </summary>
        /// <param name="s">The read-only character span to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>A byte array with decoded data.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

        /// <summary>
        /// Decodes all Base64 URL characters in the specified string into a byte array.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>A byte array with decoded data.</returns>
        public static byte[] GetBytes(string s) => GetBytes(s.AsSpan());

        /// <summary>
        /// Decodes all Base64 URL characters in the specified string into a byte array with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>A byte array with decoded data.</returns>
        public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes(s.AsSpan(), options);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase64? m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="Base64Url"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IBase64 Instance => m_Instance ??= new Base64Url();
    }
}

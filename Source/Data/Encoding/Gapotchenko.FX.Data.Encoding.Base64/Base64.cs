using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Implements Base64 encoding described in RFC 4648.
    /// </summary>
    public sealed class Base64 : GenericBase64
    {
        Base64() :
            base(new TextDataEncodingAlphabet(Base64LinguaFranca.Symbols), '=')
        {
        }

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Relax) != 0)
                alphabet = Base64LinguaFranca.Alphabet;

            return base.CreateDecoderContextCore(alphabet, options);
        }

        /// <summary>
        /// Encodes all the bytes in the specified span into a string of Base64 characters.
        /// </summary>
        /// <param name="data">The byte span to encode.</param>
        /// <returns>The string with encoded data.</returns>
        public new static string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes all the bytes in the specified span into a string of Base64 characters with specified options.
        /// </summary>
        /// <param name="data">The byte span to encode.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string with encoded data.</returns>
        public new static string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

        /// <summary>
        /// Decodes all Base64 characters in the specified read-only span into a byte array.
        /// </summary>
        /// <param name="s">The read-only character span to decode.</param>
        /// <returns>A byte array with decoded data.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes all Base64 characters in the specified read-only span into a byte array with specified options.
        /// </summary>
        /// <param name="s">The read-only character span to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>A byte array with decoded data.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

        /// <summary>
        /// Decodes all Base64 characters in the specified string into a byte array.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>A byte array with decoded data.</returns>
        public static byte[] GetBytes(string s) => GetBytes(s.AsSpan());

        /// <summary>
        /// Decodes all Base64 characters in the specified string into a byte array with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>A byte array with decoded data.</returns>
        public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes(s.AsSpan(), options);

        /// <summary>
        /// The number of symbols for padding of the encoded data representation.
        /// </summary>
        public new const int Padding = 4;

        /// <summary>
        /// Pads the encoded read-only character span.
        /// </summary>
        /// <param name="s">The read-only character span to pad.</param>
        /// <returns>The padded encoded string.</returns>
        public new static string Pad(ReadOnlySpan<char> s) => Instance.Pad(s);

        /// <summary>
        /// Unpads the encoded read-only character span.
        /// </summary>
        /// <param name="s">The read-only character span to unpad.</param>
        /// <returns>The unpadded read-only character span.</returns>
        public new static ReadOnlySpan<char> Unpad(ReadOnlySpan<char> s) => Instance.Unpad(s);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase64? m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="Base64"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IBase64 Instance => m_Instance ??= new Base64();
    }
}

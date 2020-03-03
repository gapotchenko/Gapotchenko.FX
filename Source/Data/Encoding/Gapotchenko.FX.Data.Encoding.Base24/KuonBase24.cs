using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides Kuon Base24 encoding implementation.
    /// </summary>
    public sealed class KuonBase24 : GenericKuonBase24
    {
        private KuonBase24() :
            base(new TextDataEncodingAlphabet("ZAC2B3EF4GH5TK67P8RS9WXY", false))
        {
        }

        /// <inheritdoc/>
        public override string Name => "Kuon Base24";

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Kuon Base24 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in Kuon Base24, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Kuon Base24 symbols with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation, in Kuon Base24, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Kuon Base24 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Kuon Base24 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public static byte[] GetBytes(string s) => GetBytes(s.AsSpan());

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Kuon Base24 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Kuon Base24 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes(s.AsSpan(), options);

        /// <summary>
        /// The number of characters for padding of an encoded string representation.
        /// </summary>
        public new const int Padding = SymbolsPerEncodedBlock;

        /// <summary>
        /// Pads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to pad.</param>
        /// <returns>The padded encoded string.</returns>
        public new static string Pad(ReadOnlySpan<char> s) => Instance.Pad(s);

        /// <summary>
        /// Unpads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to unpad.</param>
        /// <returns>The unpadded encoded string.</returns>
        public new static ReadOnlySpan<char> Unpad(ReadOnlySpan<char> s) => Instance.Unpad(s);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase24 m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="KuonBase24"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IBase24 Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new KuonBase24();
                return m_Instance;
            }
        }
    }
}

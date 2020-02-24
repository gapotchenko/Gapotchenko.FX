using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <inheritdoc/>
        public override string Name => "z-base-32";

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with z-base-32 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in z-base-32, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with z-base-32 symbols with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation, in z-base-32, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as z-base-32 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as z-base-32 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase32 m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="ZBase32"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IBase32 Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new ZBase32();
                return m_Instance;
            }
        }
    }
}

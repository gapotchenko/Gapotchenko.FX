using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    using Encoding = System.Text.Encoding;

    /// <summary>
    /// Base64 encoding.
    /// </summary>
    public class Base64 : DataTextEncoding
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Base64"/> class.
        /// </summary>
        protected Base64()
        {
        }

        /// <inheritdoc/>
        public override string Name => "Base64";

        /// <summary>
        /// Base64 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.75f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Base64 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in Base64, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(byte[] data) =>
            data == null ?
                null :
                Convert.ToBase64String(data);

        /// <inheritdoc/>
        protected override string GetStringCore(byte[] data) => GetString(data);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base64 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(string s) =>
            s == null ?
                null :
                Convert.FromBase64String(s);

        /// <inheritdoc/>
        protected override byte[] GetBytesCore(string s) => GetBytes(s);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IDataTextEncoding m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="Base64"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IDataTextEncoding Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new Base64();
                return m_Instance;
            }
        }
    }
}

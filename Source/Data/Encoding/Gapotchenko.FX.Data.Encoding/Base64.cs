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
        public new static string GetString(ReadOnlySpan<byte> data) =>
            data == null ?
                null :
                Convert.ToBase64String(
#if TFF_MEMORY && !TFF_MEMORY_OOB
                    data
#else
                    data.ToArray()
#endif
                    );

        /// <inheritdoc/>
        protected override string GetStringCore(ReadOnlySpan<byte> data) => GetString(data);

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

        /// <summary>
        /// The number of characters for encoded string representation padding.
        /// </summary>
        public new const int Padding = 4;

        /// <inheritdoc/>
        protected override int PaddingCore => Padding;

        const char PaddingChar = '=';

        /// <summary>
        /// Pads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to pad.</param>
        /// <returns>The padded encoded string.</returns>
        public new static string Pad(ReadOnlySpan<char> s) =>
            s == null ?
                null :
                s.ToString().PadRight((s.Length + 3) >> 2 << 2, PaddingChar);

        /// <inheritdoc/>
        protected override string PadCore(ReadOnlySpan<char> s) => Pad(s);

        /// <summary>
        /// Unpads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to unpad.</param>
        /// <returns>The unpadded encoded string.</returns>
        public new static ReadOnlySpan<char> Unpad(ReadOnlySpan<char> s) => s.TrimEnd(PaddingChar);

        /// <inheritdoc/>
        protected override ReadOnlySpan<char> UnpadCore(ReadOnlySpan<char> s) => Unpad(s);

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

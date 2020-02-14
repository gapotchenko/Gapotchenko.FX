using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    using Encoding = System.Text.Encoding;

    /// <summary>
    /// Base64 encoding.
    /// </summary>
    public sealed class Base64 : IDataTextEncoding
    {
        private Base64()
        {
        }

        /// <summary>
        /// Base64 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public const float Efficiency = 0.75f;

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Base64 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in Base64, of the contents of <paramref name="data"/>.</returns>
        public static string GetString(byte[] data) =>
            data == null ?
                null :
                Convert.ToBase64String(data);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Base64 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public static byte[] GetBytes(string s) =>
            s == null ?
                null :
                Convert.FromBase64String(s);

        string IDataEncoding.Name => "Base64";

        float IDataEncoding.Efficiency => Efficiency;

        byte[] IDataEncoding.EncodeData(byte[] data) =>
            data == null ?
                null :
                Encoding.ASCII.GetBytes(GetString(data));

        byte[] IDataEncoding.DecodeData(byte[] data) =>
            data == null ?
                null :
                GetBytes(Encoding.ASCII.GetString(data));

        byte[] IDataTextEncoding.GetBytes(string s) => GetBytes(s);

        string IDataTextEncoding.GetString(byte[] data) => GetString(data);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IDataTextEncoding m_Instance;

        /// <summary>
        /// Returns a default instance of Base64 encoding.
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

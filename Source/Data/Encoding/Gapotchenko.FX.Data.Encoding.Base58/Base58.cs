using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides Base58 and Base58Check encoding implementations according to https://en.bitcoin.it/wiki/Base58Check_encoding.
    /// </summary>
    public sealed class Base58 : GenericBase58
    {
        Base58() :
            base(new TextDataEncodingAlphabet("123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"))
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase58? m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="Base58"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [CLSCompliant(false)]
        public static IBase58 Instance => m_Instance ??= new Base58();
    }
}

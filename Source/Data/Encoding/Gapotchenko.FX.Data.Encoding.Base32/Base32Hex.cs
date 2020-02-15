using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// base32-hex encoding conforming to RFC 4648.
    /// </summary>
    public class Base32Hex : Base32
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Base32Hex"/> class.
        /// </summary>
        protected Base32Hex() :
            base("0123456789ABCDEFGHIJKLMNOPQRSTUV")
        {
        }

        /// <inheritdoc/>
        public override string Name => "base32-hex";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IDataTextEncoding m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="Base32Hex"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new static IDataTextEncoding Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new Base32Hex();
                return m_Instance;
            }
        }
    }
}

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

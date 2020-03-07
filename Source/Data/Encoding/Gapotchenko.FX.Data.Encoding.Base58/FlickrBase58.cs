using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides Flickr Base58 encoding implementation.
    /// </summary>
    public sealed class FlickrBase58 : GenericBase58
    {
        private FlickrBase58() :
            base(new TextDataEncodingAlphabet("123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ"))
        {
        }

        /// <inheritdoc/>
        public override string Name => "Flickr Base58";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase58 m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="FlickrBase58"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IBase58 Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new FlickrBase58();
                return m_Instance;
            }
        }
    }
}

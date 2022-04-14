using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides implementation of Jochaim Henke's Base91 (basE91) encoding.
    /// </summary>
    public sealed class HenkeBase91 : GenericHenkeBase91
    {
        HenkeBase91() :
            base(new TextDataEncodingAlphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%&()*+,./:;<=>?@[]^_`{|}~\"", true))
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase91? m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="HenkeBase91"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [CLSCompliant(false)]
        public static IBase91 Instance => m_Instance ??= new HenkeBase91();
    }
}

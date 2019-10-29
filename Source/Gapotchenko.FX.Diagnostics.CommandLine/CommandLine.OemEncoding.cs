using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Gapotchenko.FX.Diagnostics
{
    partial class CommandLine
    {
        /// <summary>
        /// <para>
        /// Gets command line OEM encoding.
        /// </para>
        /// <para>
        /// OEM encoding is used by Windows console applications, and can be considered as a holdover from DOS and the original IBM PC architecture.
        /// Operating systems other than Windows have no concept of OEM encoding.
        /// </para>
        /// <para>
        /// The value of this property can be universally used under any host OS.
        /// If a host OS has no notion of OEM encoding then <see cref="Encoding.Default"/> value is returned instead.
        /// </para>
        /// </summary>
        public static Encoding OemEncoding
        {
            get
            {
                if (m_OemEncoding == null)
                    m_OemEncoding = GetOemEncodingCore();
                return m_OemEncoding;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Encoding m_OemEncoding;

        static Encoding GetOemEncodingCore()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    int lcid = NativeMethods.GetSystemDefaultLCID();
                    var ci = CultureInfo.GetCultureInfo(lcid);
                    var page = ci.TextInfo.OEMCodePage;
                    return Encoding.GetEncoding(page);
                }
                catch (Exception e) when (!e.IsControlFlowException())
                {
                    return Encoding.Default;
                }
            }
            else
            {
                return Encoding.Default;
            }
        }

        static partial class NativeMethods
        {
            [DllImport("kernel32.dll", ExactSpelling = true)]
            public static extern int GetSystemDefaultLCID();
        }
    }
}

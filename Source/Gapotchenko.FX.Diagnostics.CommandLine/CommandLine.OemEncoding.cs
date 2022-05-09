using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

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
        public static Encoding OemEncoding => m_OemEncoding ??= GetOemEncodingCore();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Encoding? m_OemEncoding;

        static Encoding GetOemEncodingCore()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    int lcid = NativeMethods.GetSystemDefaultLCID();
                    var ci = CultureInfo.GetCultureInfo(lcid);
                    var page = ci.TextInfo.OEMCodePage;

                    Encoding encoding;
                    if (page == 65001)
                    {
                        // cmd.exe cannot interpret UTF-8-with-BOM encoding, so use UTF-8 encoding without BOM.
                        encoding = new UTF8Encoding(false);
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(page);
                    }

                    return encoding;
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

#if NET
        [SupportedOSPlatform("windows")]
#endif
        static partial class NativeMethods
        {
            [DllImport("kernel32.dll", ExactSpelling = true)]
            public static extern int GetSystemDefaultLCID();
        }
    }
}

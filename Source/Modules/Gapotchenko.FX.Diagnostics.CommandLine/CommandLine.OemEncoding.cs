using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX.Diagnostics;

partial class CommandLine
{
    /// <summary>
    /// Gets the current command-line OEM encoding.
    /// </summary>
    /// <remarks>
    /// OEM encoding is used by Windows console applications, and can be considered as a holdover from DOS and the original IBM PC era.
    /// Operating systems other than Windows have no concept of OEM encoding.
    /// However, the value of this property can be universally used under any host OS.
    /// If a host OS has no notion of OEM encoding, the property returns <see cref="Encoding.Default"/> instead.
    /// </remarks>
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

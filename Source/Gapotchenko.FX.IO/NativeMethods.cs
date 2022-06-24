using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Gapotchenko.FX.IO;

#if NET
[SupportedOSPlatform("windows")]
#endif
static class NativeMethods
{
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetShortPathName(
       [MarshalAs(UnmanagedType.LPTStr)] string lpszLongPath,
       [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszShortPath,
       int cchBuffer);
}

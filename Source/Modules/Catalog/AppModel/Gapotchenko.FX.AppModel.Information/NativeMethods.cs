using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX.AppModel;

#if NET
[SupportedOSPlatform("windows")]
#endif
static class NativeMethods
{
    public static string GetModuleFileName(HandleRef hModule)
    {
        var buffer = new StringBuilder(MAX_PATH);

        int length;
        for (; ; )
        {
            length = GetModuleFileName(hModule, buffer, buffer.Capacity);
            if (length < 0x7fff && length == buffer.Capacity && Marshal.GetLastWin32Error() == 0x7a)
                buffer.EnsureCapacity(buffer.Capacity * 2);
            else
                break;
        }
        buffer.Length = length;

        return buffer.ToString();
    }

    const int MAX_PATH = 260;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);
}

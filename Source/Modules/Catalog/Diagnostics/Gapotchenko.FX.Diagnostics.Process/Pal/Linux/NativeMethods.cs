#if !HAS_TARGET_PLATFORM || LINUX

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gapotchenko.FX.Diagnostics.Pal.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
static class NativeMethods
{
    public const int SIGINT = 2;

    [DllImport("libc")]
    public static extern int kill(int pid, int signal);
}

#endif

#if !HAS_TARGET_PLATFORM || MACOS

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gapotchenko.FX.Diagnostics.Pal.MacOS;

#if NET
[SupportedOSPlatform("macos")]
#endif
static class NativeMethods
{
    /// <summary>
    /// "high kernel": proc, limits
    /// </summary>
    public const int CTL_KERN = 1;

    // int: max arguments to exec
    public const int KERN_ARGMAX = 8;

    /// <summary>
    /// struct: process entries
    /// </summary>
    public const int KERN_PROC = 14;

    public const int KERN_PROCARGS2 = 49;

    /// <summary>
    /// by process id
    /// </summary>
    public const int KERN_PROC_PID = 1;

    /// <summary>
    /// Gets or sets information about the system and environment.
    /// </summary>
    /// <param name="mib"></param>
    /// <param name="lenmib"></param>
    /// <param name="oldp">
    /// A pointer to a buffer that receives the requested data.
    /// Specify <see langword="null"/> if you don’t want to get the attribute’s value.
    /// </param>
    /// <param name="oldlenp">
    /// A variable that contains the number of bytes in the buffer in the <paramref name="oldp"/> parameter.
    /// Specify <see langword="null"/> if you don’t want to get the attribute’s value.
    /// </param>
    /// <param name="newp">
    /// A pointer to a buffer that contains new data to assign to the attribute.
    /// Specify <see langword="null"/> if you don’t want to set the attribute’s value.
    /// </param>
    /// <param name="newlenp">
    /// A variable that contains the size of the buffer in the <paramref name="newp"/> parameter, in bytes.
    /// Specify 0 if you don’t want to set the attribute’s value.
    /// </param>
    /// <returns>
    /// 0 on success, or an error code that indicates a problem occurred.
    /// Possible error codes include EFAULT, EINVAL, ENOMEM, ENOTDIR, EISDIR, ENOENT, and EPERM.
    /// </returns>
    [DllImport("libc")]
    public static extern unsafe int sysctl(int[] mib, int lenmib, void* oldp, nint* oldlenp, void* newp, nint newlenp);

    /// <summary>
    /// Interrupt.
    /// </summary>
    public const int SIGINT = 2;

    [DllImport("libc")]
    public static extern int kill(int pid, int signal);
}

#endif

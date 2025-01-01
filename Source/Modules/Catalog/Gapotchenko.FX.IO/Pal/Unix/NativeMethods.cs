// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !HAS_TARGET_PLATFORM || MACOS || LINUX || FREEBSD

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.IO.Pal.Unix;

#if NET
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("freebsd")]
#endif
static class NativeMethods
{
    [DllImport("libc", ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr realpath(string path, IntPtr buffer);

    [DllImport("libc", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern void free(IntPtr ptr);

    /// <summary>
    /// Operation not permitted.
    /// </summary>
    public const int EPERM = 1;

    /// <summary>
    /// No such file or directory.
    /// </summary>
    public const int ENOENT = 2;

    /// <summary>
    /// Permission denied.
    /// </summary>
    public const int EACCES = 13;
}

#endif

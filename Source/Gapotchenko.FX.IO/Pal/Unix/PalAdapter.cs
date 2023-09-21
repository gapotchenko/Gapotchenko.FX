// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !HAS_TARGET_PLATFORM || MACOS || LINUX || FREEBSD

using Gapotchenko.FX.IO.Properties;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.IO.Pal.Unix;

#if NET && !MACOS
[SupportedOSPlatform("macos")]
#endif
#if NET && !LINUX
[SupportedOSPlatform("linux")]
#endif
#if NET && !FREEBSD
[SupportedOSPlatform("freebsd")]
#endif
abstract class PalAdapter : IPalAdapter
{
    public virtual bool IsCaseSensitive => true; // most Unix file systems are case-sensitive

    public string GetShortPath(string path) => path; // Unix has no notion of a short path

    public string GetRealPath(string path)
    {
        var ptr = NativeMethods.realpath(path, IntPtr.Zero);

        var result = Marshal.PtrToStringAuto(ptr);
        if (result is null)
        {
            var error = Marshal.GetLastWin32Error();
            throw
                error switch
                {
                    NativeMethods.EPERM or
                    NativeMethods.EACCES =>
                        new UnauthorizedAccessException(
                            string.Format(Resources.AccessToPathXDenied, path),
                            new Win32Exception(error)),

                    NativeMethods.ENOENT =>
                        new IOException(
                            string.Format(Resources.FileSystemEntryXDoesNotExsit, path),
                            new Win32Exception(error)),

                    _ =>
                        new Win32Exception(error)
                };
        }

        NativeMethods.free(ptr);
        return result;
    }
}

#endif

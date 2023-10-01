// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !HAS_TARGET_PLATFORM || WINDOWS

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX.IO.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
static class NativeMethods
{
    public const int MAX_PATH = 260;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetShortPathName(
       [MarshalAs(UnmanagedType.LPTStr)] string lpszLongPath,
       [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszShortPath,
       int cchBuffer);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern SafeFileHandle CreateFile(
        string lpFileName,
        int dwDesiredAccess,
        int dwShareMode,
        IntPtr securityAttributes,
        int dwCreationDisposition,
        int dwFlagsAndAttributes,
        IntPtr hTemplateFile);

    public const int FILE_SHARE_READ = 0x00000001;
    public const int FILE_SHARE_WRITE = 0x00000002;

    public const int CREATION_DISPOSITION_OPEN_EXISTING = 3;

    public const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetFinalPathNameByHandle(
        [In] SafeFileHandle hFile,
        [Out] StringBuilder lpszFilePath,
        [In] int cchFilePath,
        [In] int dwFlags);

    public const int FILE_NAME_NORMALIZED = 0x0;

    public const int ERROR_SUCCESS = 0x0;
    public const int ERROR_FILE_NOT_FOUND = 0x2;
    public const int ERROR_PATH_NOT_FOUND = 0x3;
    public const int ERROR_ACCESS_DENIED = 0x5;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetFileAttributes(string lpFileName);

    public const int INVALID_FILE_ATTRIBUTES = -1;
    public const int FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
}

#endif

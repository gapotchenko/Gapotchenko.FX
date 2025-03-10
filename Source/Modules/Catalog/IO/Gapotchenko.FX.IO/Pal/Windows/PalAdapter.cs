// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !HAS_TARGET_PLATFORM || WINDOWS

using Gapotchenko.FX.IO.Properties;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX.IO.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
sealed class PalAdapter : IPalAdapter
{
    public static PalAdapter Instance { get; } = new();

    public bool IsCaseSensitive => false;

    public string GetShortPath(string path)
    {
        // Note that this method works even when 8.3 file names are globally disabled in Windows:
        // https://learn.microsoft.com/en-us/troubleshoot/windows-server/performance/stop-error-code-0x00000019#method-1

        int bufferSize = path.Length;

        var sb = new StringBuilder(bufferSize);
        int r = NativeMethods.GetShortPathName(path, sb, bufferSize);
        if (r == 0 || r > bufferSize)
            return path;

        return sb.ToString();
    }

    public string GetRealPath(string path)
    {
        using var handle = NativeMethods.CreateFile(
            path,
            0,
            NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE,
            IntPtr.Zero,
            NativeMethods.CREATION_DISPOSITION_OPEN_EXISTING,
            NativeMethods.FILE_FLAG_BACKUP_SEMANTICS,
            IntPtr.Zero);

        if (handle.IsInvalid)
            throw TranslateWin32ErrorToException(Marshal.GetLastWin32Error(), path);

        var buffer = new StringBuilder(NativeMethods.MAX_PATH);
        for (; ; )
        {
            int result = NativeMethods.GetFinalPathNameByHandle(handle, buffer, buffer.Capacity, NativeMethods.FILE_NAME_NORMALIZED);
            if (result == 0)
            {
                return
                    Marshal.GetLastWin32Error() switch
                    {
                        // GetFinalPathNameByHandle may fail for files on a RAM disk (IMDISK)
                        // with ERROR_INVALID_FUNCTION error.
                        Errors.ERROR_INVALID_FUNCTION => path,
                        var errorCode => throw TranslateWin32ErrorToException(errorCode, path)
                    };
            }
            else if (result > buffer.Capacity)
            {
                buffer.EnsureCapacity(result);
            }
            else
            {
                break;
            }
        }

        // Remove a device prefix if it is not present in the original path.
        if (buffer.Length >= PalHelpers.DevicePrefixLength && !PalHelpers.IsDevice(path.AsSpan()))
        {
            Span<char> prefix = stackalloc char[PalHelpers.DevicePrefixLength];
            for (int i = 0; i < PalHelpers.DevicePrefixLength; ++i)
                prefix[i] = buffer[i];

            if (PalHelpers.IsDevice(prefix))
                return buffer.ToString(PalHelpers.DevicePrefixLength, buffer.Length - PalHelpers.DevicePrefixLength);
        }

        return buffer.ToString();
    }

    static Exception TranslateWin32ErrorToException(int error, string path) =>
        error switch
        {
            Errors.ERROR_ACCESS_DENIED =>
                new UnauthorizedAccessException(
                    string.Format(Resources.AccessToPathXDenied, path),
                    new Win32Exception(error)),

            Errors.ERROR_FILE_NOT_FOUND or
            Errors.ERROR_PATH_NOT_FOUND =>
                new IOException(
                    string.Format(Resources.FileSystemEntryXDoesNotExsit, path),
                    new Win32Exception(error)),

            _ =>
                new Win32Exception(error)
        };

    /// <summary>
    /// Gets the length of the root of the path (drive, share, etc.).
    /// </summary>
    public int GetRootPathLength(ReadOnlySpan<char> path)
    {
        int pathLength = path.Length;
        int i = 0;

        bool deviceSyntax = PalHelpers.IsDevice(path);
        bool deviceUnc = deviceSyntax && PalHelpers.IsDeviceUnc(path);

        if ((!deviceSyntax || deviceUnc) && pathLength > 0 && PalHelpers.IsDirectorySeparator(path[0]))
        {
            // UNC or simple rooted path (e.g. "\foo", NOT "\\?\C:\foo")
            if (deviceUnc || (pathLength > 1 && PalHelpers.IsDirectorySeparator(path[1])))
            {
                // UNC (\\?\UNC\ or \\), scan past server\share

                // Start past the prefix ("\\" or "\\?\UNC\")
                i = deviceUnc ? PalHelpers.DeviceUncPrefixLength : PalHelpers.UncPrefixLength;

                // Skip two separators at most
                int n = 2;
                while (i < pathLength && (!PalHelpers.IsDirectorySeparator(path[i]) || --n > 0))
                    i++;
            }
            else
            {
                // Current drive rooted (e.g. "\foo")
                i = 1;
            }
        }
        else if (deviceSyntax)
        {
            // Device path (e.g. "\\?\.", "\\.\")
            // Skip any characters following the prefix that aren't a separator
            i = PalHelpers.DevicePrefixLength;
            while (i < pathLength && !PalHelpers.IsDirectorySeparator(path[i]))
                i++;

            // If there is another separator take it, as long as we have had at least one
            // non-separator after the prefix (e.g. don't take "\\?\\", but take "\\?\a\")
            if (i < pathLength && i > PalHelpers.DevicePrefixLength && PalHelpers.IsDirectorySeparator(path[i]))
                i++;
        }
        else if (pathLength >= 2
            && path[1] == PalHelpers.VolumeSeparatorChar
            && PalHelpers.IsValidDriveChar(path[0]))
        {
            // Valid drive specified path ("C:", "D:", etc.)
            i = 2;

            // If the colon is followed by a directory separator, move past it (e.g "C:\")
            if (pathLength > 2 && PalHelpers.IsDirectorySeparator(path[2]))
                i++;
        }

        return i;
    }
}

#endif

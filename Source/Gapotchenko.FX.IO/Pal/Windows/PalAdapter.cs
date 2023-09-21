// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !HAS_TARGET_PLATFORM || WINDOWS

using Gapotchenko.FX.IO.Properties;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Gapotchenko.FX.IO.Pal.Windows;

#if NET && !WINDOWS
[SupportedOSPlatform("windows")]
#endif
sealed class PalAdapter : IPalAdapter
{
    PalAdapter()
    {
        Debug.Assert(VolumeSeparatorChar == Path.VolumeSeparatorChar);
        Debug.Assert(DirectorySeparatorChar == Path.DirectorySeparatorChar);
        Debug.Assert(AltDirectorySeparatorChar == Path.AltDirectorySeparatorChar);
    }

    public static PalAdapter Instance { get; } = new PalAdapter();

    public bool IsCaseSensitive => false;

    public string GetShortPath(string path)
    {
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
        {
            var error = Marshal.GetLastWin32Error();
            throw
                error switch
                {
                    NativeMethods.ERROR_ACCESS_DENIED =>
                        new UnauthorizedAccessException(
                            string.Format(Resources.AccessToPathXDenied, path),
                            new Win32Exception(error)),

                    NativeMethods.ERROR_FILE_NOT_FOUND or
                    NativeMethods.ERROR_PATH_NOT_FOUND =>
                        new IOException(
                            string.Format(Resources.FileSystemEntryXDoesNotExsit, path),
                            new Win32Exception(error)),

                    _ =>
                        new Win32Exception(error)
                };
        }

        var buffer = new StringBuilder(NativeMethods.MAX_PATH);
        for (; ; )
        {
            int result = NativeMethods.GetFinalPathNameByHandle(handle, buffer, buffer.Capacity, NativeMethods.FILE_NAME_NORMALIZED);
            if (result == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            else if (result > buffer.Capacity)
                buffer.EnsureCapacity(result);
            else
                break;
        }

        // Remove a device prefix if it is not present in the original path.
        if (buffer.Length >= DevicePrefixLength && !IsDevice(path.AsSpan()))
        {
            Span<char> prefix = stackalloc char[DevicePrefixLength];
            for (int i = 0; i < DevicePrefixLength; ++i)
                prefix[i] = buffer[i];

            if (IsDevice(prefix))
                return buffer.ToString(DevicePrefixLength, buffer.Length - DevicePrefixLength);
        }

        return buffer.ToString();
    }

    /// <summary>
    /// Gets the length of the root of the path (drive, share, etc.).
    /// </summary>
    public int GetRootPathLength(ReadOnlySpan<char> path)
    {
        int pathLength = path.Length;
        int i = 0;

        bool deviceSyntax = IsDevice(path);
        bool deviceUnc = deviceSyntax && IsDeviceUnc(path);

        if ((!deviceSyntax || deviceUnc) && pathLength > 0 && IsDirectorySeparator(path[0]))
        {
            // UNC or simple rooted path (e.g. "\foo", NOT "\\?\C:\foo")
            if (deviceUnc || (pathLength > 1 && IsDirectorySeparator(path[1])))
            {
                // UNC (\\?\UNC\ or \\), scan past server\share

                // Start past the prefix ("\\" or "\\?\UNC\")
                i = deviceUnc ? DeviceUncPrefixLength : UncPrefixLength;

                // Skip two separators at most
                int n = 2;
                while (i < pathLength && (!IsDirectorySeparator(path[i]) || --n > 0))
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
            i = DevicePrefixLength;
            while (i < pathLength && !IsDirectorySeparator(path[i]))
                i++;

            // If there is another separator take it, as long as we have had at least one
            // non-separator after the prefix (e.g. don't take "\\?\\", but take "\\?\a\")
            if (i < pathLength && i > DevicePrefixLength && IsDirectorySeparator(path[i]))
                i++;
        }
        else if (pathLength >= 2
            && path[1] == VolumeSeparatorChar
            && IsValidDriveChar(path[0]))
        {
            // Valid drive specified path ("C:", "D:", etc.)
            i = 2;

            // If the colon is followed by a directory separator, move past it (e.g "C:\")
            if (pathLength > 2 && IsDirectorySeparator(path[2]))
                i++;
        }

        return i;
    }

    #region Helpers

    #region Volumes

    // C:, D:
    const char VolumeSeparatorChar = ':';

    /// <summary>
    /// Returns true if the given character is a valid drive letter
    /// </summary>
    static bool IsValidDriveChar(char value) => (uint)((value | 0x20) - 'a') <= 'z' - 'a';

    #endregion

    #region UNC

    // \\
    const int UncPrefixLength = 2;

    // \\?\UNC\, \\.\UNC\
    const int DeviceUncPrefixLength = 8;

    /// <summary>
    /// Returns true if the path is a device UNC (\\?\UNC\, \\.\UNC\)
    /// </summary>
    static bool IsDeviceUnc(ReadOnlySpan<char> path) =>
        path.Length >= DeviceUncPrefixLength &&
        IsDevice(path) &&
        path[4] == 'U' &&
        path[5] == 'N' &&
        path[6] == 'C' &&
        IsDirectorySeparator(path[7]);

    #endregion

    #region Devices

    const int DevicePrefixLength = 4;

    /// <summary>
    /// Returns true if the path uses any of the DOS device path syntaxes. ("\\.\", "\\?\", or "\??\")
    /// </summary>
    static bool IsDevice(ReadOnlySpan<char> path)
    {
        // If the path begins with any two separators is will be recognized and normalized and prepped with
        // "\??\" for internal usage correctly. "\??\" is recognized and handled, "/??/" is not.
        return
            path.Length >= DevicePrefixLength &&
            (IsExtended(path) || IsTypical(path));

        static bool IsTypical(ReadOnlySpan<char> path) =>
            IsDirectorySeparator(path[0]) &&
            IsDirectorySeparator(path[1]) &&
            path[2] is '.' or '?' &&
            IsDirectorySeparator(path[3]);

        // Returns true if the path uses the canonical form of extended syntax ("\\?\" or "\??\"). If the
        // path matches exactly (cannot use alternate directory separators) Windows will skip normalization
        // and path length checks.
        static bool IsExtended(ReadOnlySpan<char> path)
        {
            // While paths like "//?/C:/" will work, they're treated the same as "\\.\" paths.
            // Skipping of normalization will *only* occur if back slashes ('\') are used.
            return path[..4] is ['\\', '\\' or '?', '?', '\\'];
        }
    }

    #endregion

    #region Directories

    const char DirectorySeparatorChar = '\\';
    const char AltDirectorySeparatorChar = '/';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsDirectorySeparator(char c) => c is DirectorySeparatorChar or AltDirectorySeparatorChar;

    #endregion

    #endregion
}

#endif

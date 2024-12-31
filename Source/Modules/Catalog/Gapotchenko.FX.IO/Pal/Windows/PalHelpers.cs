// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !HAS_TARGET_PLATFORM || WINDOWS

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.IO.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
static class PalHelpers
{
    static PalHelpers()
    {
        Debug.Assert(VolumeSeparatorChar == Path.VolumeSeparatorChar);
        Debug.Assert(DirectorySeparatorChar == Path.DirectorySeparatorChar);
        Debug.Assert(AltDirectorySeparatorChar == Path.AltDirectorySeparatorChar);
    }

    #region Volumes

    // C:, D:
    public const char VolumeSeparatorChar = ':';

    /// <summary>
    /// Returns true if the given character is a valid drive letter
    /// </summary>
    public static bool IsValidDriveChar(char value) => (uint)((value | 0x20) - 'a') <= 'z' - 'a';

    #endregion

    #region UNC

    // \\
    public const int UncPrefixLength = 2;

    // \\?\UNC\, \\.\UNC\
    public const int DeviceUncPrefixLength = 8;

    /// <summary>
    /// Returns true if the path is a device UNC (\\?\UNC\, \\.\UNC\)
    /// </summary>
    public static bool IsDeviceUnc(ReadOnlySpan<char> path) =>
        path.Length >= DeviceUncPrefixLength &&
        IsDevice(path) &&
        path[4] == 'U' &&
        path[5] == 'N' &&
        path[6] == 'C' &&
        IsDirectorySeparator(path[7]);

    #endregion

    #region Devices

    public const int DevicePrefixLength = 4;

    /// <summary>
    /// Returns true if the path uses any of the DOS device path syntaxes. ("\\.\", "\\?\", or "\??\")
    /// </summary>
    public static bool IsDevice(ReadOnlySpan<char> path)
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
    public static bool IsDirectorySeparator(char c) => c is DirectorySeparatorChar or AltDirectorySeparatorChar;

    #endregion
}

#endif

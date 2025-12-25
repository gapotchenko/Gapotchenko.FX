// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.IO.Pal.Windows;

/// <summary>
/// Defines the error codes used by the Windows API.
/// For compatibility, the same error codes are used in .NET on platforms other than Windows.
/// </summary>
static class Errors
{
    public const int ERROR_SUCCESS = 0x0;
    public const int ERROR_INVALID_FUNCTION = 0x1;
    public const int ERROR_FILE_NOT_FOUND = 0x2;
    public const int ERROR_PATH_NOT_FOUND = 0x3;
    public const int ERROR_ACCESS_DENIED = 0x5;
    public const int ERROR_SHARING_VIOLATION = 0x00000020;
    public const int ERROR_LOCK_VIOLATION = 0x00000021;
    public const int ERROR_USER_MAPPED_FILE = 0x000004c8;
}

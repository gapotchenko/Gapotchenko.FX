// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Provides a set of static methods for querying <see cref="IOException"/> objects.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IOExceptionExtensions
{
    /// <summary>
    /// Determines whether an IO exception signifies a file access violation error.
    /// </summary>
    /// <remarks>
    /// A file access violation error usually occurs when the file is locked or opened with incompatible <see cref="FileShare"/> flags.
    /// </remarks>
    /// <param name="exception">The exception.</param>
    /// <returns><see langword="true"/> if exception represents a file access violation error; otherwise, <see langword="false"/>.</returns>
    public static bool IsFileAccessViolationError(this IOException exception)
    {
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);

        const int ERROR_USER_MAPPED_FILE = 0x000004C8;
        const int ERROR_SHARING_VIOLATION = 0x00000020;
        const int ERROR_LOCK_VIOLATION = 0x00000021;

        return errorCode is
            ERROR_USER_MAPPED_FILE or
            ERROR_SHARING_VIOLATION or
            ERROR_LOCK_VIOLATION;
    }

#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY
    /// <inheritdoc cref="IsFileAccessViolationError(IOException)"/>
    [Obsolete("Use IsFileSharingViolationException() method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsFileAccessViolationException(
#if SOURCE_COMPATIBILITY
        this
#endif
        IOException exception) =>
        IsFileAccessViolationError(exception);
#endif
}

using System;
using System.ComponentModel;
using System.IO;
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
    /// <param name="exception">The exception.</param>
    /// <returns><c>true</c> if exception represents a file access violation error; otherwise, <c>false</c>.</returns>
    public static bool IsFileAccessViolationException(this IOException exception)
    {
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
        switch (errorCode)
        {
            case 0x000004C8: // ERROR_USER_MAPPED_FILE
            case 0x00000020: // ERROR_SHARING_VIOLATION
            case 0x00000021: // ERROR_LOCK_VIOLATION
                return true;

            default:
                return false;
        }
    }
}

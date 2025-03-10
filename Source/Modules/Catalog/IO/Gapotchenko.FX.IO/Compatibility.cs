// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

namespace Gapotchenko.FX.IO;

#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY

/// <summary>
/// Provides a set of static methods for querying <see cref="IOException"/> objects.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IOExceptionExtensions
{
    /// <inheritdoc cref="FileSystem.IsFileAccessViolationError(IOException)"/>
    [Obsolete("Use FileSystem.IsFileSharingViolationError(Exception) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsFileAccessViolationException(
#if SOURCE_COMPATIBILITY
        this
#endif
        IOException exception) =>
        FileSystem.IsFileAccessViolationError(exception);
}

#endif

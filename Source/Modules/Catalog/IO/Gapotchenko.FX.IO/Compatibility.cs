// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.IO;

#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY

/// <summary>
/// Provides a set of static methods for querying <see cref="IOException"/> objects.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IOExceptionExtensions
{
    /// <inheritdoc cref="FileSystem.IsAccessViolationException(IOException)"/>
    [Obsolete("Use FileSystem.IsAccessViolationException(Exception) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsFileAccessViolationException(
#if SOURCE_COMPATIBILITY
        this
#endif
        IOException exception) =>
        FileSystem.IsAccessViolationException(exception);
}

/// <summary>
/// Provides polyfill methods for <see cref="Path"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class PathEx
{
    /// <inheritdoc cref="PathPolyfills.GetRelativePath(string, string)"/>
    [Obsolete("Use Path.GetRelativePath(string, string) method instead.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetRelativePath(string relativeTo, string path) =>
        Path.GetRelativePath(relativeTo, path);

    /// <inheritdoc cref="PathPolyfills.Join(IEnumerable{string?})"/>
    [Obsolete("Use Path.Join(params ReadOnlySpan<string>) method instead.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join(params string?[] paths) => Path.Join(paths);

    /// <inheritdoc cref="PathPolyfills.Join(IEnumerable{string?})"/>
    [Obsolete("Use Path.Join(params IEnumerable<string>) method instead.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join(IEnumerable<string?> paths) => Path.Join(paths);

    /// <inheritdoc cref="PathPolyfills.TrimEndingDirectorySeparator(string)"/>
    [Obsolete("Use Path.TrimEndingDirectorySeparator(string) method instead.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TrimEndingDirectorySeparator(string path) =>
        Path.TrimEndingDirectorySeparator(path);

    /// <inheritdoc cref="PathPolyfills.TrimEndingDirectorySeparator(ReadOnlySpan{char})"/>
    [Obsolete("Use Path.TrimEndingDirectorySeparator(ReadOnlySpan<char>) method instead.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) =>
        Path.TrimEndingDirectorySeparator(path);
}

#endif

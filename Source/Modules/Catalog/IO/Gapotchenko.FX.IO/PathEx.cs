// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.IO;

#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY

/// <summary>
/// Provides polyfill methods for <see cref="Path"/> class.
/// </summary>
#if TFF_STATIC_EXTENSIONS
[Obsolete("Use System.IO.Path type instead.")]
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public static class PathEx
{
    /// <inheritdoc cref="PathPolyfills.GetRelativePath(string, string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetRelativePath(string relativeTo, string path) =>
        Path.GetRelativePath(relativeTo, path);

    /// <inheritdoc cref="PathPolyfills.Join(IEnumerable{string?})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join(params string?[] paths) => Path.Join(paths);

    /// <inheritdoc cref="PathPolyfills.Join(IEnumerable{string?})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join(IEnumerable<string?> paths) => Path.Join(paths);

    /// <inheritdoc cref="PathPolyfills.TrimEndingDirectorySeparator(string)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TrimEndingDirectorySeparator(string path) =>
        Path.TrimEndingDirectorySeparator(path);

    /// <inheritdoc cref="PathPolyfills.TrimEndingDirectorySeparator(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) =>
        Path.TrimEndingDirectorySeparator(path);
}

#endif

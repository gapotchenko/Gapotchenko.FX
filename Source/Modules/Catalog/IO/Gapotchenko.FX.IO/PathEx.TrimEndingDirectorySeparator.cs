// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

#if NETCOREAPP3_0_OR_GREATER
#define TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR
#endif

using Gapotchenko.FX.IO.Pal;

namespace Gapotchenko.FX.IO;

partial class PathEx
{
#if TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR
    /// <summary>
    /// Trims one trailing directory separator beyond the root of the specified path.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <see cref="Path.TrimEndingDirectorySeparator(string)"/>
    /// method.
    /// </remarks>
    /// <param name="path">The path to trim.</param>
    /// <returns>The path without any trailing directory separators.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    /// <summary>
    /// Trims one trailing directory separator beyond the root of the specified path.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX for
    /// <c>System.IO.Path.TrimEndingDirectorySeparator(string)</c>
    /// method.
    /// </remarks>
    /// <param name="path">The path to trim.</param>
    /// <returns>The path without any trailing directory separators.</returns>
#endif
    [return: NotNullIfNotNull(nameof(path))]
    public static string? TrimEndingDirectorySeparator(string? path) =>
#if TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR
        path is null
            ? null
            : Path.TrimEndingDirectorySeparator(path);
#else
        TrimEndingDirectorySeparatorCore(path);
#endif

#if TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR
    /// <summary>
    /// Trims one trailing directory separator beyond the root of the specified path.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <see cref="Path.TrimEndingDirectorySeparator(string)"/>
    /// method.
    /// </remarks>
    /// <param name="path">The path to trim.</param>
    /// <returns>The path without any trailing directory separators.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    /// <summary>
    /// Trims one trailing directory separator beyond the root of the specified path.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <c>System.IO.Path.TrimEndingDirectorySeparator(ReadOnlySpan&lt;char&gt;)</c>
    /// method.
    /// </remarks>
    /// <param name="path">The path to trim.</param>
    /// <returns>The path without any trailing directory separators.</returns>
#endif
    public static ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) =>
#if TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR
        Path.TrimEndingDirectorySeparator(path);
#else
        TrimEndingDirectorySeparatorCore(path);
#endif

#if !TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR

    [return: NotNullIfNotNull(nameof(path))]
    static string? TrimEndingDirectorySeparatorCore(string? path) =>
        path != null && EndsInDirectorySeparator(path) && !IsRootPath(path.AsSpan()) ?
            path[..^1] :
            path;

    static ReadOnlySpan<char> TrimEndingDirectorySeparatorCore(ReadOnlySpan<char> path) =>
        EndsInDirectorySeparator(path) && !IsRootPath(path) ?
            path[..^1] :
            path;

    static bool IsRootPath(ReadOnlySpan<char> path) => path.Length == GetRootPathLength(path);

    static int GetRootPathLength(ReadOnlySpan<char> path)
    {
        if (PalServices.Adapter is not null and var pal)
        {
            return pal.GetRootPathLength(path);
        }
        else
        {
            // A graceful fallback.
            return path.Length > 0 && IsDirectorySeparator(path[0]) ? 1 : 0;
        }
    }

#endif
}

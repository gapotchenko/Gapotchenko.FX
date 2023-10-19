// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

#if NETCOREAPP3_0_OR_GREATER
#define TFF_PATH_TRIM_ENDING_DIRECTORY_SEPARATOR
#define TFF_PATH_JOIN
#endif

using Gapotchenko.FX.IO.Pal;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Provides polyfills for <see cref="Path"/> class.
/// </summary>
public static class PathEx
{
#if TFF_PATH_TRIM_ENDING_DIRECTORY_SEPARATOR
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
    public static string TrimEndingDirectorySeparator(string path) =>
#if TFF_PATH_TRIM_ENDING_DIRECTORY_SEPARATOR
        Path.TrimEndingDirectorySeparator(path);
#else
        TrimEndingDirectorySeparatorCore(path);
#endif

#if TFF_PATH_TRIM_ENDING_DIRECTORY_SEPARATOR
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
#if TFF_PATH_TRIM_ENDING_DIRECTORY_SEPARATOR
        Path.TrimEndingDirectorySeparator(path);
#else
        TrimEndingDirectorySeparatorCore(path);
#endif

#if !TFF_PATH_TRIM_ENDING_DIRECTORY_SEPARATOR

    [return: NotNullIfNotNull(nameof(path))]
    static string? TrimEndingDirectorySeparatorCore(string? path) =>
        path != null && EndsInDirectorySeparator(path.AsSpan()) && !IsRootPath(path.AsSpan()) ?
            path[..^1] :
            path;

    static ReadOnlySpan<char> TrimEndingDirectorySeparatorCore(ReadOnlySpan<char> path) =>
        EndsInDirectorySeparator(path) && !IsRootPath(path) ?
            path[..^1] :
            path;

    static bool EndsInDirectorySeparator(ReadOnlySpan<char> path) =>
        path.Length > 0 &&
        IsDirectorySeparator(path[^1]);

    static bool IsRootPath(ReadOnlySpan<char> path) => path.Length == GetRootPathLength(path);

    static int GetRootPathLength(ReadOnlySpan<char> path)
    {
        if (PalServices.AdapterOrDefault is not null and var pal)
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

    static bool IsDirectorySeparator(char c) =>
        c == Path.DirectorySeparatorChar ||
        c == Path.AltDirectorySeparatorChar;

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Returns a relative path from one path to another.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill provided for
    /// <see cref="Path.GetRelativePath(string, string)"/>
    /// method.
    /// </remarks>
    /// <param name="relativeTo">
    /// The source path the result should be relative to.
    /// This path is always considered to be a directory.
    /// </param>
    /// <param name="path">
    /// The destination path.
    /// </param>
    /// <returns>
    /// The relative path, or <paramref name="path"/> if the paths do not share the same root.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    /// <summary>
    /// Returns a relative path from one path to another.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill provided for
    /// <c>System.IO.Path.GetRelativePath(string, string)</c>
    /// method.
    /// </remarks>
    /// <param name="relativeTo">
    /// The source path the result should be relative to.
    /// This path is always considered to be a directory.
    /// </param>
    /// <param name="path">
    /// The destination path.
    /// </param>
    /// <returns>
    /// The relative path, or <paramref name="path"/> if the paths do not share the same root.
    /// </returns>
#endif
    public static string GetRelativePath(string relativeTo, string path) =>
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        Path.GetRelativePath(relativeTo, path);
#else
        GetRelativePathCore(relativeTo, path);
#endif

#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    static string GetRelativePathCore(string relativeTo, string path)
    {
        if (relativeTo == null)
            throw new ArgumentNullException(nameof(relativeTo));
        if (path == null)
            throw new ArgumentNullException(nameof(path));

        path = Path.GetFullPath(path);
        relativeTo = Path.GetFullPath(relativeTo);

        var separators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        IReadOnlyList<string> p1 = path.Split(separators);
        IReadOnlyList<string> p2 = relativeTo.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        var sc = FileSystem.PathComparison;

        int i;
        int n = Math.Min(p1.Count, p2.Count);
        for (i = 0; i < n; i++)
            if (!string.Equals(p1[i], p2[i], sc))
                break;

        if (i == 0)
        {
            // Cannot make a relative path, for example if the path resides on another drive.
            return path;
        }

        p1 = p1.Skip(i).Take(p1.Count - i).ToList();

        if (p1.Count == 1 && p1[0].Length == 0)
            p1 = Array.Empty<string>();

        string relativePath = string.Join(
            new string(Path.DirectorySeparatorChar, 1),
            Enumerable.Repeat("..", p2.Count - i).Concat(p1));

        if (relativePath.Length == 0)
            relativePath = ".";

        return relativePath;
    }
#endif

#if TFF_PATH_JOIN
    /// <summary>
    /// Concatenates path components into a single path.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <see cref="Path.Join(string?[])"/>
    /// method.
    /// </remarks>
    /// <param name="paths">The path components.</param>
    /// <returns>The combined paths.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    /// <summary>
    /// Concatenates path components into a single path.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <c>Path.Join(string?[])</c>
    /// method.
    /// </remarks>
    /// <param name="paths">The path components.</param>
    /// <returns>The combined paths.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
#endif
    public static string Join(params string?[] paths) =>
#if TFF_PATH_JOIN
        Path.Join(paths);
#else
        Join((IEnumerable<string?>)paths);
#endif

    /// <summary>
    /// Concatenates path components into a single path.
    /// </summary>
    /// <param name="paths">The path components.</param>
    /// <returns>The combined paths.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
    public static string Join(IEnumerable<string?> paths)
    {
        if (paths == null)
            throw new ArgumentNullException(nameof(paths));

        var builder = new StringBuilder();

        foreach (var path in paths)
        {
            if (string.IsNullOrEmpty(path))
                continue;

            if (builder.Length == 0)
            {
                builder.Append(path);
            }
            else
            {
                if (!IsDirectorySeparator(builder[^1]) && !IsDirectorySeparator(path[0]))
                    builder.Append(Path.DirectorySeparatorChar);

                builder.Append(path);
            }
        }

        return builder.ToString();
    }
}

namespace Gapotchenko.FX.IO;

/// <summary>
/// Provides polyfills for <see cref="Path"/> class.
/// </summary>
public static class PathEx
{
#if NETCOREAPP3_0_OR_GREATER
    /// <summary>
    /// Trims one trailing directory separator beyond the root of the specified path.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX for
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
#if NETCOREAPP3_0_OR_GREATER
        Path.TrimEndingDirectorySeparator(path);
#else
        (path ?? throw new ArgumentNullException(nameof(path)))
        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
#endif

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Returns a relative path from one path to another.
    /// </summary>
    /// <remarks>
    /// This is a polyfill provided by Gapotchenko.FX for
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
    /// This is a polyfill provided by Gapotchenko.FX for
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
}

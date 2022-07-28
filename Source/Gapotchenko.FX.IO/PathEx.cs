namespace Gapotchenko.FX.IO;

/// <summary>
/// Provides polyfills for <see cref="Path"/> class.
/// </summary>
public static class PathEx
{
    /// <summary>
    /// Trims one trailing directory separator beyond the root of the specified path.
    /// </summary>
    /// <param name="path">The path to trim.</param>
    /// <returns>The path without any trailing directory separators.</returns>
    public static string TrimEndingDirectorySeparator(string path) =>
#if NETCOREAPP3_0_OR_GREATER
        Path.TrimEndingDirectorySeparator(path);
#else
        (path ?? throw new ArgumentNullException(nameof(path)))
        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
#endif
}

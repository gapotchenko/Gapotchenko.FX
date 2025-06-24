// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX;
using Gapotchenko.FX.IO;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Diagnostics;

/// <summary>
/// Provides command shell operations.
/// </summary>
public static class CommandShell
{
    /// <summary>
    /// Locates the paths of a file with the specified name using <c>PATH</c> environment variable.
    /// </summary>
    /// <remarks>
    /// <c>PATH</c> environment variable plays a special role in command-line tools discovery.
    /// For more information, see <see href="https://en.wikipedia.org/wiki/PATH_(variable)"/>.
    /// </remarks>
    /// <param name="fileName">The file name.</param>
    /// <returns>A sequence of the located file paths.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
    public static IEnumerable<string> Which(string fileName) => Which(fileName, null);

    /// <summary>
    /// Locates the paths of a file with the specified name using the specified probing paths first,
    /// then by using <c>PATH</c> environment variable.
    /// </summary>
    /// <remarks>
    /// <c>PATH</c> environment variable plays a special role in command-line tools discovery.
    /// For more information, see <see href="https://en.wikipedia.org/wiki/PATH_(variable)"/>.
    /// </remarks>
    /// <param name="fileName">The file name.</param>
    /// <param name="probingPaths">The probing paths to check before the <c>PATH</c> environment variable.</param>
    /// <returns>A sequence of the located file paths.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
    public static IEnumerable<string> Which(string fileName, params IEnumerable<string>? probingPaths)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        if (fileName.Length == 0)
        {
            // Empty file name can never exist.
            return [];
        }

        if (Empty.Nullify(Path.GetDirectoryName(fileName)) is not null and var directoryName)
        {
            // The file name specifies a file path containing directories.

            fileName = Path.GetFileName(fileName);
            probingPaths = [directoryName];
        }
        else
        {
            // The file name specifies no path.

            probingPaths ??= [];

            string? path = Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrEmpty(path))
            {
                probingPaths = probingPaths.Concat(
                    path.Split(
#if NET
                        Path.PathSeparator,
#else
                        [Path.PathSeparator],
#endif
                        StringSplitOptions.RemoveEmptyEntries));
            }
        }

        return LocateFile(fileName, probingPaths);
    }

    static IEnumerable<string> LocateFile(string fileName, IEnumerable<string?> probingPaths)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string[] pathExtensions;

            string? pathExt = Environment.GetEnvironmentVariable("PATHEXT");
            if (!string.IsNullOrEmpty(pathExt))
            {
                pathExtensions = pathExt.Split(
#if NET
                    Path.PathSeparator,
#else
                    [Path.PathSeparator],
#endif
                    StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                pathExtensions = [];
            }

            return LocateFileCore(fileName, probingPaths, GetSuggestedFilePaths);

            IEnumerable<string> GetSuggestedFilePaths(string filePath)
            {
                yield return filePath;
                foreach (string extension in pathExtensions)
                    yield return filePath + extension;
            }
        }
        else
        {
            return LocateFileCore(fileName, probingPaths, null);
        }
    }

    static IEnumerable<string> LocateFileCore(
        string fileName,
        IEnumerable<string?> probingPaths,
        Func<string, IEnumerable<string>>? getSuggestedFilePaths)
    {
        foreach (string? path in probingPaths.Distinct(FileSystem.PathComparer))
        {
            if (path is null)
                continue;

            string filePath;
            try
            {
                filePath = Path.Combine(path, fileName);
            }
            catch (ArgumentException)
            {
                // Illegal characters in path.
                continue;
            }

            if (getSuggestedFilePaths is not null)
            {
                foreach (string suggestedFilePath in getSuggestedFilePaths(filePath))
                {
                    if (File.Exists(suggestedFilePath))
                        yield return suggestedFilePath;
                }
            }
            else
            {
                if (File.Exists(filePath))
                    yield return filePath;
            }
        }
    }
}

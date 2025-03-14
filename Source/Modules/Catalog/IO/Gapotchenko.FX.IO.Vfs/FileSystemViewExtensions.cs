// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Text;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides extension methods for <see cref="IFileSystemView"/>.
/// </summary>
public static class FileSystemViewExtensions
{
    #region Files

    /// <inheritdoc cref="File.OpenText(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static StreamReader OpenTextFile(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        else if (view is LocalFileSystemView)
            return File.OpenText(path);
        else
            return new StreamReader(view.OpenFileForReading(path), Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllText(string)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static string ReadAllTextFromFile(this IReadOnlyFileSystemView view, string path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        else if (view is LocalFileSystemView)
            return File.ReadAllText(path);
        else
            return ReadAllTextFromFileCore(view, path, Encoding.UTF8);
    }

    /// <inheritdoc cref="File.ReadAllText(string, Encoding)"/>
    /// <param name="view">The file system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="encoding"><inheritdoc/></param>
    public static string ReadAllTextFromFile(this IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        else
            return ReadAllTextFromFileCore(view, path, encoding);
    }

    static string ReadAllTextFromFileCore(IReadOnlyFileSystemView view, string path, Encoding encoding)
    {
        if (view is LocalFileSystemView)
        {
            return File.ReadAllText(path, encoding);
        }
        else
        {
            using var sr = new StreamReader(view.OpenFileForReading(path), encoding);
            return sr.ReadToEnd();
        }
    }

    #endregion

    #region Paths

    /// <summary>
    /// Concatenates a sequence of paths into a single path.
    /// </summary>
    /// <remarks>
    /// This method simply concatenates the specified <paramref name="paths"/>
    /// and adds a directory separator character between them if one is not already present.
    /// If the length of a specified path component is zero, the method concatenates the remaining parts.
    /// If the length of the resulting concatenated string is zero, the method returns <see cref="string.Empty"/>.
    /// </remarks>
    /// <param name="view">The file system view.</param>
    /// <param name="paths">A sequence of paths.</param>
    /// <returns>The concatenated path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
    public static string JoinPaths(
        this IReadOnlyFileSystemView view,
        params IEnumerable<string?> paths)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));
        if (paths is null)
            throw new ArgumentNullException(nameof(paths));

        char directorySeparatorChar = view.DirectorySeparatorChar;

        var builder = new StringBuilder();

        foreach (string? path in paths)
        {
            if (string.IsNullOrEmpty(path))
                continue;

            if (builder.Length != 0)
            {
                if (!IsDirectorySeparator(builder[^1], directorySeparatorChar) &&
                    !IsDirectorySeparator(path[0], directorySeparatorChar))
                {
                    builder.Append(directorySeparatorChar);
                }
            }

            builder.Append(path);
        }

        return builder.ToString();
    }

    static bool IsDirectorySeparator(char c, char directorySeparatorChar) =>
        c == directorySeparatorChar ||
        c is '/' or '\\';

    #endregion
}

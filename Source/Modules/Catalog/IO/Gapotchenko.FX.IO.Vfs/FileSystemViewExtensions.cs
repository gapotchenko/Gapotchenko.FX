// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides extension methods for <see cref="IFileSystemView"/>.
/// </summary>
public static partial class FileSystemViewExtensions
{
    // This class is partial. Please take a look at the neighboring source files.

    #region Directories

    /// <inheritdoc cref="Directory.Delete(string)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    public static void DeleteDirectory(this IFileSystemView view, string path) =>
        (view ?? throw new ArgumentNullException(nameof(view)))
        .DeleteDirectory(path, false);

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
    /// <param name="view">The file-system view.</param>
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
                if (!VfsPathKit.IsDirectorySeparator(builder[^1], directorySeparatorChar) &&
                    !VfsPathKit.IsDirectorySeparator(path[0], directorySeparatorChar))
                {
                    builder.Append(directorySeparatorChar);
                }
            }

            builder.Append(path);
        }

        return builder.ToString();
    }

    #endregion
}

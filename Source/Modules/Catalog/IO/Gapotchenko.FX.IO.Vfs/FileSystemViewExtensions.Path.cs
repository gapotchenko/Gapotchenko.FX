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

partial class FileSystemViewExtensions
{
    /// <summary>
    /// Gets the root directory information from the path contained in the specified string.
    /// </summary>
    /// <param name="view">The file-system view.</param>
    /// <param name="path">A string containing the path from which to obtain root directory information.</param>
    /// <returns>
    /// A string containing the root directory of <paramref name="path"/>,
    /// or an empty span if <paramref name="path"/> does not contain root directory information.
    /// Returns <see langword="null"/> if <paramref name="path"/> is <see langword="null"/> or is effectively empty.
    /// </returns>
    /// <inheritdoc cref="IReadOnlyFileSystemView.GetPathRoot(ReadOnlySpan{char})"/>
    public static string? GetPathRoot(this IReadOnlyFileSystemView view, string? path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (string.IsNullOrEmpty(path))
            return null;
        else if (view.GetPathRoot(path.AsSpan()) is var result && result != null)
            return result.ToString();
        else
            return null;
    }

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

    /// <summary>
    /// Returns the directory information for the specified path.
    /// </summary>
    /// <param name="view">The file-system view.</param>
    /// <param name="path">The path to retrieve the directory information from.</param>
    /// <returns>
    /// Directory information for <paramref name="path"/>,
    /// or <see langword="null"/> if <paramref name="path"/> denotes a root directory or is <see langword="null"/>.
    /// Returns <see cref="string.Empty"/> if <paramref name="path"/> does not contain directory information.
    /// </returns>
    /// <inheritdoc cref="IReadOnlyFileSystemView.GetDirectoryName(ReadOnlySpan{char})"/>
    public static string? GetDirectoryName(this IReadOnlyFileSystemView view, string? path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (string.IsNullOrEmpty(path))
            return null;
        else if (view.GetDirectoryName(path.AsSpan()) is var result && result != null)
            return result.ToString();
        else
            return null;
    }
}

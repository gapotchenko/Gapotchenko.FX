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
    /// <inheritdoc cref="Path.IsPathRooted(string?)"/>
    /// <param name="view">The file-system view.</param>
    /// <param name="path"><inheritdoc/></param>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    public static bool IsPathRooted(this IReadOnlyFileSystemView view, [NotNullWhen(true)] string? path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (path is null)
            return false;
        else
            return view.IsPathRooted(path.AsSpan());
    }

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
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
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
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
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

    /// <summary>
    /// Returns the file name and extension of the specified path string.
    /// </summary>
    /// <param name="view">The file-system view.</param>
    /// <param name="path">The path string from which to obtain the file name and extension.</param>
    /// <returns>
    /// The characters after the last directory separator character in <paramref name="path"/>.
    /// If the last character of <paramref name="path"/> is a directory separator character, this method returns <see cref="string.Empty"/>.
    /// If <paramref name="path"/> is <see langword="null"/>, this method returns <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    [return: NotNullIfNotNull(nameof(path))]
    public static string? GetFileName(this IReadOnlyFileSystemView view, string? path)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        if (string.IsNullOrEmpty(path))
            return path;

        var result = view.GetFileName(path.AsSpan());
        if (path.Length == result.Length)
            return path;

        return result.ToString();
    }
}

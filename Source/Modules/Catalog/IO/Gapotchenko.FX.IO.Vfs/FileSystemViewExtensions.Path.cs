// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Memory;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs;

partial class FileSystemViewExtensions
{
    /// <inheritdoc cref="JoinPaths(IReadOnlyFileSystemView, ReadOnlySpan{string?})"/>
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
        ReadOnlySpan<char> directorySeparatorChars = stackalloc char[] { directorySeparatorChar, view.AltDirectorySeparatorChar };

        var builder = new StringBuilder();

        foreach (string? path in paths)
        {
            if (string.IsNullOrEmpty(path))
                continue;

            if (builder.Length != 0)
            {
                if (!directorySeparatorChars.Contains(builder[^1]) &&
                    !directorySeparatorChars.Contains(path[0]))
                {
                    builder.Append(directorySeparatorChar);
                }
            }

            builder.Append(path);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Concatenates a sequence of paths into a single path.
    /// </summary>
    /// <remarks>
    /// This method simply concatenates the specified <paramref name="paths"/>
    /// and adds a directory separator character between them if one is not already present.
    /// If a specified path component is <see langword="null"/> or its length is zero, the method concatenates the remaining parts.
    /// If the length of the resulting concatenated string is zero, the method returns <see cref="string.Empty"/>.
    /// </remarks>
    /// <param name="view">The file-system view.</param>
    /// <param name="paths">A sequence of paths.</param>
    /// <returns>The concatenated path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="view"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
    [OverloadResolutionPriority(1)]
    public static string JoinPaths(
        this IReadOnlyFileSystemView view,
        params scoped ReadOnlySpan<string?> paths)
    {
        if (view is null)
            throw new ArgumentNullException(nameof(view));

        char directorySeparatorChar = view.DirectorySeparatorChar;
        ReadOnlySpan<char> directorySeparatorChars = stackalloc char[] { directorySeparatorChar, view.AltDirectorySeparatorChar };

        var builder = new StringBuilder();

        foreach (string? path in paths)
        {
            if (string.IsNullOrEmpty(path))
                continue;

            if (builder.Length != 0)
            {
                if (!directorySeparatorChars.Contains(builder[^1]) &&
                    !directorySeparatorChars.Contains(path[0]))
                {
                    builder.Append(directorySeparatorChar);
                }
            }

            builder.Append(path);
        }

        return builder.ToString();
    }
}

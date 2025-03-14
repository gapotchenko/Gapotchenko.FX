// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides path manipulation primitives for virtual file system hierarchies.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class VfsPathKit
{
    /// <summary>
    /// Splits the specified path into parts.
    /// </summary>
    /// <remarks>
    /// The path parts are normalized to eliminate references to <c>"."</c> (current) and <c>".."</c> (previous) directories.
    /// </remarks>
    /// <param name="path">The path to split.</param>
    /// <returns>
    /// The array of parts of the path, or <see langword="null"/> if the <paramref name="path"/> is <see langword="null"/>, empty or points outside the root hierarchy.
    /// The array is empty when the <paramref name="path"/> represents the root path <c>"/"</c>.
    /// </returns>
    public static string[]? Split(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        else
            return Normalize(FileSystem.SplitPath(path))?.ToArray();
    }

    static IEnumerable<string>? Normalize(IEnumerable<string> parts)
    {
        var list = new List<string>();

        foreach (string part in parts)
        {
            // Get the effective name of the part
            // by trimming off the directory separators.
            var name = part.AsSpan().Trim(['/', '\\']);

            switch (name)
            {
                case "" or ".":
                    // Stay at the current directory.
                    continue;

                case "..":
                    if (list.Count is > 0 and var count)
                    {
                        // Return to a previous directory by exiting the current.
                        list.RemoveAt(count - 1);
                    }
                    else
                    {
                        // The path points to a directory outside the root hierarchy.
                        return null;
                    }
                    break;

                default:
                    // Enter a subdirectory.
                    list.Add(part.Length == name.Length ? part : name.ToString());
                    break;
            }
        }

        return list;
    }

    /// <summary>
    /// Concatenates a sequence of parts into a single path.
    /// using the specified directory separator character.
    /// </summary>
    /// <param name="parts">The parts of the path.</param>
    /// <param name="directorySeparatorChar">The directory separator character.</param>
    /// <returns>The concatenated path, or <see langword="null"/> if the <paramref name="parts"/> value is <see langword="null"/>.</returns>
    [return: NotNullIfNotNull(nameof(parts))]
    public static string? Join(IEnumerable<string?>? parts, char directorySeparatorChar = '/') =>
        parts is null
            ? null
            : string.Join(
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                directorySeparatorChar,
#else
                $"{directorySeparatorChar}",
#endif
                parts.Where(x => !string.IsNullOrEmpty(x)));

    /// <summary>
    /// Concatenates a span of parts into a single path.
    /// using the specified directory separator character.
    /// </summary>
    /// <inheritdoc cref="Join(IEnumerable{string?}?, char)"/>
    [OverloadResolutionPriority(1)]
    [return: NotNullIfNotNull(nameof(parts))]
    public static string? Join(ReadOnlySpan<string> parts, char directorySeparatorChar = '/')
    {
        if (parts == null)
            return null!;

        var sb = new StringBuilder();
        foreach (string? part in parts)
        {
            if (string.IsNullOrEmpty(part))
                continue;
            if (sb.Length != 0)
                sb.Append(directorySeparatorChar);
            sb.Append(part);
        }
        return sb.ToString();
    }
}

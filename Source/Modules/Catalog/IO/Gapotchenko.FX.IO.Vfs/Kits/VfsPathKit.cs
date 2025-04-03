// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides path manipulation primitives for hierarchies of virtual file-system entries.
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
    /// <param name="directorySeparatorChar">The directory separator character.</param>
    /// <returns>
    /// The array of parts of the path, or <see langword="null"/> if the <paramref name="path"/> is <see langword="null"/>, empty or points outside the root hierarchy.
    /// The array is empty when the <paramref name="path"/> represents the root path <c>"/"</c>.
    /// </returns>
    public static string[]? Split(string? path, char directorySeparatorChar = DirectorySeparatorChar)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        else
        {
            return
                Normalize(
                    FileSystem.SplitPath(path),
                    directorySeparatorChar)
                ?.ToArray();
        }
    }

    static IEnumerable<string>? Normalize(IEnumerable<string> parts, char directorySeparatorChar)
    {
        var list = new List<string>();

        foreach (string part in parts)
        {
            // Get the effective name of the part
            // by trimming off the directory separators.
            var name = part.AsSpan().Trim([directorySeparatorChar, DirectorySeparatorChar, AltDirectorySeparatorChar]);

            switch (name)
            {
                case "" or ".":
                    // Stay at the current directory.
                    continue;

                case "..":
                    if (list.Count is > 0 and var count)
                    {
                        // Return to a previous directory by exiting the current directory.
                        list.RemoveAt(count - 1);
                    }
                    else
                    {
                        // The path points to a directory outside of the root hierarchy.
                        return null;
                    }
                    break;

                default:
                    // Enter to a subdirectory.
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
    public static string? Join(IEnumerable<string?>? parts, char directorySeparatorChar = DirectorySeparatorChar) =>
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
    public static string? Join(scoped ReadOnlySpan<string> parts, char directorySeparatorChar = DirectorySeparatorChar)
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

    /// <summary>
    /// Gets the root directory information from the path contained in the specified character span.
    /// </summary>
    /// <param name="path">A read-only span of characters containing the path from which to obtain root directory information.</param>
    /// <param name="directorySeparatorChar">The directory separator character.</param>
    /// <returns>
    /// A read-only span of characters containing the root directory of <paramref name="path"/>,
    /// or an empty span if <paramref name="path"/> does not contain root directory information.
    /// Returns an empty span representing <see langword="null"/> if <paramref name="path"/> is effectively empty.
    /// </returns>
    public static ReadOnlySpan<char> GetPathRoot(ReadOnlySpan<char> path, char directorySeparatorChar = DirectorySeparatorChar)
    {
        if (path.IsEmpty)
            return null;
        else if (IsDirectorySeparator(path[0], directorySeparatorChar))
            return path[..1];
        else
            return [];
    }

    /// <summary>
    /// Get a value indicating whether the specified character is a directory separator character.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <param name="directorySeparatorChar">The canonical directory separator character.</param>
    /// <returns>
    /// <see langword="true"/> if the specified character is a directory separator character;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsDirectorySeparator(char c, char directorySeparatorChar) =>
         c == directorySeparatorChar ||
         IsDirectorySeparator(c);

    /// <inheritdoc cref="IsDirectorySeparator(char, char)"/>
    public static bool IsDirectorySeparator(char c) =>
        c is DirectorySeparatorChar or AltDirectorySeparatorChar;

    /// <summary>
    /// Gets the default directory separator character.
    /// </summary>
    public const char DirectorySeparatorChar = '/';

    /// <summary>
    /// Gets the alternative directory separator character.
    /// </summary>
    public const char AltDirectorySeparatorChar = '\\';
}

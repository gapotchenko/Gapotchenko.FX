// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Memory;
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
    /// Splits the specified path into its effective parts.
    /// </summary>
    /// <remarks>
    /// The path parts are normalized to eliminate references to <c>"."</c> (current) and <c>".."</c> (previous) directories.
    /// </remarks>
    /// <param name="path">The path to split.</param>
    /// <param name="directorySeparatorChar">The directory separator character.</param>
    /// <returns>
    /// The array of effective parts of the path, or <see langword="null"/> if the <paramref name="path"/> is <see langword="null"/>, empty or points outside the root hierarchy.
    /// The array is empty when the <paramref name="path"/> represents the root path <c>"/"</c>.
    /// </returns>
    public static string[]? Split(string? path, char directorySeparatorChar = DirectorySeparatorChar)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        else
            return Minimize(Decompose(path, directorySeparatorChar), directorySeparatorChar)?.ToArray();
    }

    static IEnumerable<string>? Minimize(IEnumerable<string> parts, char directorySeparatorChar)
    {
        ReadOnlySpan<char> directorySeparatorChars = stackalloc char[] { directorySeparatorChar, AltDirectorySeparatorChar };

        var list = new List<string>();

        foreach (string part in parts)
        {
            // Get the effective name of the part
            // by trimming off the directory separators.
            var name = part.AsSpan().Trim(directorySeparatorChars);

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
    /// Decomposes the specified path into a sequence of file-system entry names.
    /// </summary>
    /// <remarks>
    /// For example, the entry names of the <c>"C:\Users\Tester\Documents"</c> path are:
    /// <list type="bullet">
    /// <item><c>C:\</c></item>
    /// <item><c>Users</c></item>
    /// <item><c>Tester</c></item>
    /// <item><c>Documents</c></item>
    /// </list>
    /// </remarks>
    /// <param name="path">The path to split.</param>
    /// <param name="directorySeparatorChar">The directory separator character.</param>
    /// <returns>
    /// A sequence of file-system entry names.
    /// The sequence is empty if <paramref name="path"/> is <see langword="null"/> or empty.
    /// </returns>
    public static IEnumerable<string> Decompose(string? path, char directorySeparatorChar) =>
        EnumerateSubpaths(path, directorySeparatorChar)
        .Reverse()
        .Select(
            subpath =>
            {
                if (GetFileName(subpath.AsSpan(), directorySeparatorChar) is var part && !part.IsEmpty)
                    return part.ToString();
                else
                    return GetDirectoryName(subpath.AsSpan(), directorySeparatorChar) == null ? subpath : subpath[^1..^0];
            });

    /// <summary>
    /// Enumerates subpaths of the specified path.
    /// </summary>
    /// <remarks>
    /// For example, the subpaths of the <c>"C:\Users\Tester\Documents"</c> path are:
    /// <list type="bullet">
    /// <item><c>C:\Users\Tester\Documents</c></item>
    /// <item><c>C:\Users\Tester</c></item>
    /// <item><c>C:\Users</c></item>
    /// <item><c>C:\</c></item>
    /// </list>
    /// </remarks>
    /// <param name="path">The path to enumerate subpaths for.</param>
    /// <param name="directorySeparatorChar">The directory separator character.</param>
    /// <returns>The sequence of subpaths of the specified path.</returns>
    static IEnumerable<string> EnumerateSubpaths(string? path, char directorySeparatorChar)
    {
        for (string? i = path; !string.IsNullOrEmpty(i); i = GetDirectoryName(i.AsSpan(), directorySeparatorChar).ToNullableString())
            yield return i.ToString();
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
    /// Concatenates a span of parts into a single path
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
    /// <para>
    /// Canonicalizes a specified path.
    /// </para>
    /// <para>
    /// The alternative directory separators are replaced with native ones;
    /// the duplicate adjacent separators are removed.
    /// </para>
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="directorySeparatorChar">The directory separator character.</param>
    /// <returns>The canonicalized path.</returns>
    public static string Canonicalize(ReadOnlySpan<char> path, char directorySeparatorChar = DirectorySeparatorChar)
    {
        var sb = new StringBuilder(path.Length);

        char? prevChar = null;
        foreach (char c in path)
        {
            char ch = c;
            if (ch == AltDirectorySeparatorChar)
                ch = directorySeparatorChar;

            if (prevChar is { } prev && ch == directorySeparatorChar && ch == prev)
                continue;

            prevChar = ch;
            sb.Append(ch);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns the directory information for the specified path represented by a character span.
    /// </summary>
    /// <param name="path">The path to retrieve the directory information from.</param>
    /// <param name="directorySeparatorChar">The directory separator character.</param>
    /// <returns>
    /// Directory information for <paramref name="path"/>,
    /// or an empty span representing <see langword="null"/> if <paramref name="path"/> denotes a root directory or is empty.
    /// Returns an empty span if <paramref name="path"/> does not contain directory information.
    /// </returns>
    public static ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path, char directorySeparatorChar = DirectorySeparatorChar)
    {
        if (path.IsEmpty)
            return null;

        int end = GetDirectoryNameOffset(path);
        return end >= 0 ? path[..end] : null;

        int GetDirectoryNameOffset(ReadOnlySpan<char> path)
        {
            int rootLength = GetPathRoot(path).Length;
            int end = path.Length;
            if (end <= rootLength)
                return -1;

            while (end > rootLength && !IsDirectorySeparator(path[--end], directorySeparatorChar)) ;

            // Trim off any remaining separators (to deal with C:\foo\\bar)
            while (end > rootLength && IsDirectorySeparator(path[end - 1], directorySeparatorChar))
                end--;

            return end;
        }
    }

    /// <summary>
    /// Returns the file name and extension of a file path that is represented by a read-only character span.
    /// </summary>
    /// <param name="path">A read-only span that contains the path from which to obtain the file name and extension.</param>
    /// <param name="directorySeparatorChar">The directory separator character.</param>
    /// <returns>
    /// The characters after the last directory separator character in <paramref name="path"/>.
    /// If the last character of <paramref name="path"/> is a directory separator character, this method returns an empty span.
    /// If <paramref name="path"/> represents <see langword="null"/>, this method returns an empty span representing <see langword="null"/>.
    /// </returns>
    public static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path, char directorySeparatorChar = DirectorySeparatorChar)
    {
        int root = GetPathRoot(path).Length;

        // We don't want to cut off "C:\file.txt:stream" (i.e. should be "file.txt:stream")
        // but we *do* want "C:Foo" => "Foo". This necessitates checking for the root.

        int i = path.LastIndexOfAny(directorySeparatorChar, AltDirectorySeparatorChar);

        return path[(i < root ? root : i + 1)..];
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
            return string.Empty.AsSpan();
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
    public static bool IsDirectorySeparator(char c, char directorySeparatorChar = DirectorySeparatorChar) =>
         c == directorySeparatorChar ||
         c == AltDirectorySeparatorChar;

    /// <summary>
    /// Gets the default directory separator character.
    /// </summary>
    public const char DirectorySeparatorChar = '/';

    /// <summary>
    /// Gets the alternate directory separator character.
    /// </summary>
    public const char AltDirectorySeparatorChar = '/';
}

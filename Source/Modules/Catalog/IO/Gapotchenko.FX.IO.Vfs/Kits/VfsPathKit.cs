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
            // Get the effective name of the part by trimming directory separators.
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
    /// Combines a sequence of strings into a path.
    /// </summary>
    /// <param name="parts">The parts of the path.</param>
    /// <returns>The combined path.</returns>
    [return: NotNullIfNotNull(nameof(parts))]
    public static string? Combine(IEnumerable<string?>? parts) =>
        parts is null
            ? null
            : string.Join(
                "/",
                parts.Where(x => !string.IsNullOrEmpty(x)));

    /// <summary>
    /// Combines a span of strings into a path.
    /// </summary>
    /// <param name="parts">The parts of the path.</param>
    /// <returns>The combined path.</returns>
    [OverloadResolutionPriority(1)]
    [return: NotNullIfNotNull(nameof(parts))]
    public static string? Combine(ReadOnlySpan<string> parts)
    {
        if (parts == null)
            return null!;

        const char separator = '/';

        var sb = new StringBuilder();
        foreach (string? part in parts)
        {
            if (string.IsNullOrEmpty(part))
                continue;
            if (sb.Length != 0)
                sb.Append(separator);
            sb.Append(part);
        }
        return sb.ToString();
    }
}

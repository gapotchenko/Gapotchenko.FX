// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides path manipulation primitives for a virtual file system.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class VfsPathKit
{
    /// <summary>
    /// Splits the specified path into parts.
    /// </summary>
    /// <remarks>
    /// The split path is normalized to eliminate references to <c>"."</c> (current) and <c>".."</c> (previous) directories.
    /// </remarks>
    /// <param name="path">The path to split.</param>
    /// <returns>
    /// The array of parts of the path, or <see langword="null"/> if the <paramref name="path"/> is <see langword="null"/>, empty or points outside of the root hierarchy.
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
            switch (part.AsSpan())
            {
                case "" or ".":
                    continue;

                case "..":
                    if (list.Count is > 0 and var count)
                    {
                        list.RemoveAt(count - 1);
                    }
                    else
                    {
                        // The path points to a directory outside of the root hierarchy.
                        return null;
                    }
                    break;

                case var x when x.TrimStart("/\\".AsSpan()).Length is 0:
                    break;

                default:
                    list.Add(part);
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
    public static string? Combine(IEnumerable<string>? parts) =>
        parts is null
            ? null
            : string.Join("/", parts);

    /// <summary>
    /// Combines a span of strings into a path.
    /// </summary>
    /// <param name="parts">The parts of the path.</param>
    /// <returns>The combined path.</returns>
    [OverloadResolutionPriority(1)]
    [return: NotNullIfNotNull(nameof(parts))]
    public static string? Combine(ReadOnlySpan<string> parts) =>
        parts == null
            ? null!
            :
#if NET9_0_OR_GREATER
        string.Join('/', parts!);
#else
        string.Join("/", [.. parts]);
#endif
}

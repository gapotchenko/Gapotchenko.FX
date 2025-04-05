// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

#if NETCOREAPP3_0_OR_GREATER
#define TFF_PATH_JOIN
#endif

using System.Text;

namespace Gapotchenko.FX.IO;

partial class PathEx
{
#if TFF_PATH_JOIN
    /// <summary>
    /// Concatenates path components into a single path.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <see cref="Path.Join(string?[])"/>
    /// method.
    /// </remarks>
    /// <param name="paths">The path components.</param>
    /// <returns>The combined paths.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    /// <summary>
    /// Concatenates path components into a single path.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <c>Path.Join(string?[])</c>
    /// method.
    /// </remarks>
    /// <param name="paths">The path components.</param>
    /// <returns>The combined paths.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
#endif
    public static string Join(params string?[] paths) =>
#if TFF_PATH_JOIN
        Path.Join(paths);
#else
        Join((IEnumerable<string?>)paths);
#endif

    /// <summary>
    /// Concatenates path components into a single path.
    /// </summary>
    /// <param name="paths">The path components.</param>
    /// <returns>The combined paths.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
    public static string Join(IEnumerable<string?> paths)
    {
        if (paths == null)
            throw new ArgumentNullException(nameof(paths));

        var builder = new StringBuilder();

        foreach (string? path in paths)
        {
            if (string.IsNullOrEmpty(path))
                continue;

            if (builder.Length != 0)
            {
                if (!IsDirectorySeparator(builder[^1]) &&
                    !IsDirectorySeparator(path[0]))
                {
                    builder.Append(Path.DirectorySeparatorChar);
                }
            }

            builder.Append(path);
        }

        return builder.ToString();
    }
}

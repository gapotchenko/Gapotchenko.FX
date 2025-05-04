// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_PATH_GETRELATIVEPATH
#endif

#if NETCOREAPP3_0_OR_GREATER
#define TFF_PATH_ENDSINDIRECTORYSEPARATOR
#define TFF_PATH_JOIN
#define TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR
#endif

using Gapotchenko.FX.IO.Pal;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Provides polyfill extension methods for <see cref="Path"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class PathPolyfills
{
    // This class is partial. Please take a look at the neighboring source files.

    /// <summary>
    /// Provides member polyfills for <see cref="Path"/> class.
    /// </summary>
    extension(Path)
    {
        #region GetRelativePath

        /// <summary>
        /// Returns a relative path from one path to another.
        /// </summary>
        /// <remarks>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// </remarks>
        /// <param name="relativeTo">
        /// The source path the result should be relative to.
        /// This path is always considered to be a directory.
        /// </param>
        /// <param name="path">
        /// The destination path.
        /// </param>
        /// <returns>
        /// The relative path, or <paramref name="path"/> if the paths do not share the same root.
        /// </returns>
#if TFF_PATH_GETRELATIVEPATH
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string GetRelativePath(string relativeTo, string path) =>
#if TFF_PATH_GETRELATIVEPATH
            Path.GetRelativePath(relativeTo, path);
#else
            GetRelativePathCore(relativeTo, path);
#endif

#if !TFF_PATH_GETRELATIVEPATH
        static string GetRelativePathCore(string relativeTo, string path)
        {
            if (relativeTo == null)
                throw new ArgumentNullException(nameof(relativeTo));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            path = Path.GetFullPath(path);
            relativeTo = Path.GetFullPath(relativeTo);

            char[] separators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];
            IReadOnlyList<string> p1 = path.Split(separators);
            IReadOnlyList<string> p2 = relativeTo.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            var sc = FileSystem.PathComparison;

            int i;
            int n = Math.Min(p1.Count, p2.Count);
            for (i = 0; i < n; i++)
                if (!string.Equals(p1[i], p2[i], sc))
                    break;

            if (i == 0)
            {
                // Cannot make a relative path, for example if the path resides on another drive.
                return path;
            }

            p1 = [.. p1.Skip(i).Take(p1.Count - i)];

            if (p1.Count == 1 && p1[0].Length == 0)
                p1 = [];

            string relativePath = string.Join(
                new string(Path.DirectorySeparatorChar, 1),
                Enumerable.Repeat("..", p2.Count - i).Concat(p1));

            if (relativePath.Length == 0)
                relativePath = ".";

            return relativePath;
        }
#endif

        #endregion

        #region EndsInDirectorySeparator

        /// <summary>
        /// Returns a value that indicates whether the specified path ends in a directory separator.
        /// </summary>
        /// <remarks>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// method.
        /// </remarks>
        /// <param name="path">The path to analyze.</param>
        /// <returns>
        /// <see langword="true"/> if the path ends in a directory separator; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
#if TFF_PATH_ENDSINDIRECTORYSEPARATOR
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool EndsInDirectorySeparator([NotNullWhen(true)] string? path)
        {
#if TFF_PATH_ENDSINDIRECTORYSEPARATOR
            return Path.EndsInDirectorySeparator(path!);
#else
            return EndsInDirectorySeparator(path.AsSpan());
#endif
        }

        /// <summary>
        /// Returns a value that indicates whether the path, specified as a read-only span, ends in a directory separator.
        /// </summary>
        /// <remarks>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// </remarks>
        /// <param name="path">The path to analyze.</param>
        /// <returns>
        /// <see langword="true"/> if the path ends in a directory separator; 
        /// otherwise, <see langword="false"/>.
        /// </returns>
#if TFF_PATH_ENDSINDIRECTORYSEPARATOR
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool EndsInDirectorySeparator(ReadOnlySpan<char> path)
        {
#if TFF_PATH_ENDSINDIRECTORYSEPARATOR
            return Path.EndsInDirectorySeparator(path);
#else
            return
                path.Length > 0 &&
                IsDirectorySeparator(path[^1]);
#endif
        }

        #endregion

        #region Join

        /// <summary>
        /// Concatenates path components into a single path.
        /// </summary>
        /// <remarks>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// method.
        /// </remarks>
        /// <param name="paths">The path components.</param>
        /// <returns>The combined paths.</returns>
#if TFF_PATH_JOIN
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Join(params ReadOnlySpan<string?> paths)
        {
#if TFF_PATH_JOIN
            return Path.Join(paths);
#else
            var builder = new StringBuilder();
            foreach (string? path in paths)
                JoinPath(builder, path);
            return builder.ToString();
#endif
        }

        /// <summary>
        /// Concatenates path components into a single path.
        /// </summary>
        /// <remarks>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// method.
        /// </remarks>
        /// <param name="paths">The path components.</param>
        /// <returns>The combined paths.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
        public static string Join(params IEnumerable<string?> paths)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            var builder = new StringBuilder();
            foreach (string? path in paths)
                JoinPath(builder, path);
            return builder.ToString();
        }

        static void JoinPath(StringBuilder builder, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return;

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

        #endregion

        #region TrimEndingDirectorySeparator

        /// <summary>
        /// Trims one trailing directory separator beyond the root of the specified path.
        /// </summary>
        /// <remarks>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// </remarks>
        /// <param name="path">The path to trim.</param>
        /// <returns>The path without any trailing directory separators.</returns>
#if TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string TrimEndingDirectorySeparator(string path) =>
#if TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR
            Path.TrimEndingDirectorySeparator(path);
#else
            TrimEndingDirectorySeparatorCore(path);
#endif

        /// <summary>
        /// Trims one trailing directory separator beyond the root of the specified path.
        /// </summary>
        /// <remarks>
        /// This is Gapotchenko.FX polyfill for
        /// <c>System.IO.Path.TrimEndingDirectorySeparator(ReadOnlySpan&lt;char&gt;)</c>
        /// method.
        /// </remarks>
        /// <param name="path">The path to trim.</param>
        /// <returns>The path without any trailing directory separators.</returns>
#if TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) =>
#if TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR
            Path.TrimEndingDirectorySeparator(path);
#else
            TrimEndingDirectorySeparatorCore(path);
#endif

#if !TFF_PATH_TRIMENDINGDIRECTORYSEPARATOR

        [return: NotNullIfNotNull(nameof(path))]
        static string? TrimEndingDirectorySeparatorCore(string? path) =>
            path != null && EndsInDirectorySeparator(path) && !IsRootPath(path.AsSpan()) ?
                path[..^1] :
                path;

        static ReadOnlySpan<char> TrimEndingDirectorySeparatorCore(ReadOnlySpan<char> path) =>
            EndsInDirectorySeparator(path) && !IsRootPath(path) ?
                path[..^1] :
                path;

        static bool IsRootPath(ReadOnlySpan<char> path) => path.Length == GetRootPathLength(path);

        static int GetRootPathLength(ReadOnlySpan<char> path)
        {
            if (PalServices.Adapter is not null and var pal)
            {
                return pal.GetRootPathLength(path);
            }
            else
            {
                // A graceful fallback.
                return path.Length > 0 && IsDirectorySeparator(path[0]) ? 1 : 0;
            }
        }

#endif

        #endregion
    }

    static bool IsDirectorySeparator(char c) =>
        c == Path.DirectorySeparatorChar ||
        c == Path.AltDirectorySeparatorChar;
}

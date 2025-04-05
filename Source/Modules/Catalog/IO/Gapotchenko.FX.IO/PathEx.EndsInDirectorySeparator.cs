// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

#if NETCOREAPP3_0_OR_GREATER
#define TFF_PATH_ENDSINDIRECTORYSEPARATOR
#endif

namespace Gapotchenko.FX.IO;

partial class PathEx
{
#if TFF_PATH_ENDSINDIRECTORYSEPARATOR
    /// <summary>
    /// Returns a value that indicates whether the specified path ends in a directory separator.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <see cref="Path.EndsInDirectorySeparator(string?)"/>
    /// method.
    /// </remarks>
    /// <param name="path">The path to analyze.</param>
    /// <returns>
    /// <see langword="true"/> if the path ends in a directory separator; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    /// <summary>
    /// Returns a value that indicates whether the specified path ends in a directory separator.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <c>Path.EndsInDirectorySeparator(string?)</c>
    /// method.
    /// </remarks>
    /// <param name="path">The path to analyze.</param>
    /// <returns>
    /// <see langword="true"/> if the path ends in a directory separator; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
#endif
    public static bool EndsInDirectorySeparator([NotNullWhen(true)] string? path)
    {
#if TFF_PATH_ENDSINDIRECTORYSEPARATOR
        return Path.EndsInDirectorySeparator(path!);
#else
        return EndsInDirectorySeparator(path.AsSpan());
#endif
    }

#if TFF_PATH_ENDSINDIRECTORYSEPARATOR
    /// <summary>
    /// Returns a value that indicates whether the path, specified as a read-only span, ends in a directory separator.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <see cref="Path.EndsInDirectorySeparator(ReadOnlySpan{char})"/>
    /// method.
    /// </remarks>
    /// <param name="path">The path to analyze.</param>
    /// <returns>
    /// <see langword="true"/> if the path ends in a directory separator; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
#else
    /// <summary>
    /// Returns a value that indicates whether the path, specified as a read-only span, ends in a directory separator.
    /// </summary>
    /// <remarks>
    /// This is Gapotchenko.FX polyfill for
    /// <c>Path.EndsInDirectorySeparator(ReadOnlySpan{char})</c>
    /// method.
    /// </remarks>
    /// <param name="path">The path to analyze.</param>
    /// <returns>
    /// <see langword="true"/> if the path ends in a directory separator; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
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
}

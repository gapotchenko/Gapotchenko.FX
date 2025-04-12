// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a read-only virtual file-system view.
/// </summary>
public interface IReadOnlyFileSystemView
{
    #region Capabilities

    /// <summary>
    /// Gets a value indicating whether the current file storage supports reading.
    /// </summary>
    bool CanRead { get; }

    /// <summary>
    /// Gets a value indicating whether the current file storage supports the date and time of a last write to a file or a directory.
    /// </summary>
    bool SupportsLastWriteTime { get; }

    /// <summary>
    /// Gets a value indicating whether the current file storage supports the date and time of a creation of a file or a directory.
    /// </summary>
    bool SupportsCreationTime { get; }

    /// <summary>
    /// Gets a value indicating whether the current file storage supports the date and time of a last access to a file or a directory.
    /// </summary>
    bool SupportsLastAccessTime { get; }

    #endregion

    #region Files

    /// <inheritdoc cref="File.Exists(string?)"/>
    bool FileExists([NotNullWhen(true)] string? path);

    /// <inheritdoc cref="Directory.EnumerateFiles(string)"/>
    IEnumerable<string> EnumerateFiles(string path);

    /// <inheritdoc cref="Directory.EnumerateFiles(string, string)"/>
    IEnumerable<string> EnumerateFiles(string path, string searchPattern);

    /// <inheritdoc cref="Directory.EnumerateFiles(string, string, SearchOption)"/>
    IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);

    /// <inheritdoc cref="File.OpenRead(string)"/>
    Stream ReadFile(string path);

    #endregion

    #region Directories

    /// <summary>
    /// Determines whether the given path refers to an existing file system directory.
    /// </summary>
    /// <inheritdoc cref="Directory.Exists(string?)"/>
    bool DirectoryExists([NotNullWhen(true)] string? path);

    /// <inheritdoc cref="Directory.EnumerateDirectories(string)"/>
    IEnumerable<string> EnumerateDirectories(string path);

    /// <inheritdoc cref="Directory.EnumerateDirectories(string, string)"/>
    IEnumerable<string> EnumerateDirectories(string path, string searchPattern);

    /// <inheritdoc cref="Directory.EnumerateDirectories(string, string, SearchOption)"/>
    IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);

    /// <summary>
    /// Determines whether the given path refers to an existing file or directory in the file system.
    /// </summary>
    /// <inheritdoc cref="FileSystem.EntryExists(string?)"/>
    bool EntryExists([NotNullWhen(true)] string? path);

    #endregion

    #region Entries

    /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string)"/>
    IEnumerable<string> EnumerateEntries(string path);

    /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string, string)"/>
    IEnumerable<string> EnumerateEntries(string path, string searchPattern);

    /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string, string, SearchOption)"/>
    IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption);

    /// <remarks>
    /// If the file or directory described in the <paramref name="path"/> parameter does not exist,
    /// this method returns <see cref="DateTime.MinValue"/>.
    /// </remarks>
    /// <inheritdoc cref="File.GetLastWriteTimeUtc(string)"/>
    DateTime GetLastWriteTime(string path);

    /// <remarks>
    /// If the file or directory described in the <paramref name="path"/> parameter does not exist,
    /// this method returns <see cref="DateTime.MinValue"/>.
    /// </remarks>
    /// <inheritdoc cref="File.GetCreationTimeUtc(string)"/>
    DateTime GetCreationTime(string path);

    /// <remarks>
    /// If the file or directory described in the <paramref name="path"/> parameter does not exist,
    /// this method returns <see cref="DateTime.MinValue"/>.
    /// </remarks>
    /// <inheritdoc cref="File.GetLastAccessTimeUtc(string)"/>
    DateTime GetLastAccessTime(string path);

    #endregion

    #region Paths

    /// <summary>
    /// Gets a character used to separate directory levels in a path string
    /// that reflects a hierarchical file system organization.
    /// </summary>
    char DirectorySeparatorChar { get; }

    /// <summary>
    /// Gets an alternate character used to separate directory levels in a path string
    /// that reflects a hierarchical file system organization.
    /// </summary>
    char AltDirectorySeparatorChar { get; }

    /// <inheritdoc cref="FileSystem.PathComparer"/>
    StringComparer PathComparer { get; }

    /// <summary>
    /// Combines a sequence of strings into a path.
    /// </summary>
    /// <param name="paths">A sequence of parts of the path.</param>
    /// <returns>The combined paths.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
    string CombinePaths(params IEnumerable<string?> paths);

    /// <summary>
    /// Combines a span of strings into a path.
    /// </summary>
    /// <param name="paths">A span of parts of the path.</param>
    /// <returns>The combined paths.</returns>
    [OverloadResolutionPriority(1)]
    string CombinePaths(params ReadOnlySpan<string?> paths);

    /// <inheritdoc cref="Path.GetFullPath(string)"/>
    [return: NotNullIfNotNull(nameof(path))]
    string? GetFullPath(string? path);

    /// <summary>
    /// Returns the directory information for the specified path.
    /// </summary>
    /// <param name="path">The path to retrieve the directory information from.</param>
    /// <returns>
    /// Directory information for <paramref name="path"/>,
    /// or <see langword="null"/> if <paramref name="path"/> denotes a root directory or is <see langword="null"/>.
    /// Returns <see cref="string.Empty"/> if <paramref name="path"/> does not contain directory information.
    /// </returns>
    /// <inheritdoc cref="IReadOnlyFileSystemView.GetDirectoryName(ReadOnlySpan{char})"/>
    string? GetDirectoryName(string? path);

    /// <summary>
    /// Returns the directory information for the specified path represented by a character span.
    /// </summary>
    /// <param name="path">The path to retrieve the directory information from.</param>
    /// <returns>
    /// Directory information for <paramref name="path"/>,
    /// or an empty span representing <see langword="null"/> if <paramref name="path"/> denotes a root directory or is empty.
    /// Returns an empty span if <paramref name="path"/> does not contain directory information.
    /// </returns>
    /// <exception cref="PathTooLongException">
    /// The <paramref name="path"/> parameter is longer than the maximum length defined by the file system.
    /// </exception>
    ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path);

    /// <summary>
    /// Returns the file name and extension of the specified path string.
    /// </summary>
    /// <param name="path">The path string from which to obtain the file name and extension.</param>
    /// <returns>
    /// The characters after the last directory separator character in <paramref name="path"/>.
    /// If the last character of <paramref name="path"/> is a directory separator character, this method returns <see cref="string.Empty"/>.
    /// If <paramref name="path"/> is <see langword="null"/>, this method returns <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(path))]
    string? GetFileName(string? path);

    /// <summary>
    /// Returns the file name and extension of a file path that is represented by a read-only character span.
    /// </summary>
    /// <param name="path">A read-only span that contains the path from which to obtain the file name and extension.</param>
    /// <returns>
    /// The characters after the last directory separator character in <paramref name="path"/>.
    /// If the last character of <paramref name="path"/> is a directory separator character, this method returns an empty span.
    /// If <paramref name="path"/> represents <see langword="null"/>, this method returns an empty span representing <see langword="null"/>.
    /// </returns>
    ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path);

    /// <summary>
    /// Returns a value indicating whether the specified path string contains a root.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns><see langword="true"/> if <paramref name="path"/> contains a root; otherwise, <see langword="false"/>.</returns>
    bool IsPathRooted([NotNullWhen(true)] string? path);

    /// <summary>
    /// Returns a value indicating whether the specified path string contains a root.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns><see langword="true"/> if <paramref name="path"/> contains a root; otherwise, <see langword="false"/>.</returns>
    bool IsPathRooted(ReadOnlySpan<char> path);

    /// <summary>
    /// Gets the root directory information from the path contained in the specified string.
    /// </summary>
    /// <param name="path">A string containing the path from which to obtain root directory information.</param>
    /// <returns>
    /// A string containing the root directory of <paramref name="path"/>,
    /// or <see cref="string.Empty"/> if <paramref name="path"/> does not contain root directory information.
    /// Returns <see langword="null"/> if <paramref name="path"/> is <see langword="null"/> or is effectively empty.
    /// </returns>
    string? GetPathRoot(string? path);

    /// <summary>
    /// Gets the root directory information from the path contained in the specified character span.
    /// </summary>
    /// <param name="path">A read-only span of characters containing the path from which to obtain root directory information.</param>
    /// <returns>
    /// A read-only span of characters containing the root directory of <paramref name="path"/>,
    /// or an empty span if <paramref name="path"/> does not contain root directory information.
    /// Returns an empty span representing <see langword="null"/> if <paramref name="path"/> is effectively empty.
    /// </returns>
    ReadOnlySpan<char> GetPathRoot(ReadOnlySpan<char> path);

    /// <summary>
    /// Trims one trailing directory separator beyond the root of the specified path.
    /// </summary>
    /// <param name="path">The path to trim.</param>
    /// <returns>The <paramref name="path"/> without any trailing directory separators.</returns>
    [return: NotNullIfNotNull(nameof(path))]
    string? TrimEndingDirectorySeparator(string? path);

    /// <summary>
    /// Trims one trailing directory separator beyond the root of the specified path.
    /// </summary>
    /// <param name="path">The path to trim.</param>
    /// <returns>The <paramref name="path"/> without any trailing directory separators.</returns>
    ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path);

    /// <summary>
    /// Returns a value that indicates whether the specified path ends in a directory separator.
    /// </summary>
    /// <param name="path">The path to analyze.</param>
    /// <returns>
    /// <see langword="true"/> if the path ends in a directory separator; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool EndsInDirectorySeparator([NotNullWhen(true)] string? path);

    /// <summary>
    /// Returns a value that indicates whether the path, specified as a read-only span, ends in a directory separator.
    /// </summary>
    /// <param name="path">The path to analyze.</param>
    /// <returns>
    /// <see langword="true"/> if the path ends in a directory separator; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool EndsInDirectorySeparator(ReadOnlySpan<char> path);

    #endregion
}

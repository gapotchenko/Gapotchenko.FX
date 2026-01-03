// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Runtime.CompilerServices;
using System.Security;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a read-only view on a virtual file system.
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

    /// <summary>
    /// Gets a value indicating whether the current file storage supports file and directory attributes.
    /// </summary>
    bool SupportsAttributes { get; }

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

    /// <summary>
    /// Returns an enumerable collection of full file names
    /// that match a search pattern and enumeration options in a specified path,
    /// and optionally searches subdirectories.
    /// </summary>
    /// <param name="path">
    /// The relative or absolute path to the directory to search.
    /// This string is not case-sensitive.
    /// </param>
    /// <param name="searchPattern">
    /// The search string to match against the names of files in path.
    /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters,
    /// but it doesn't support regular expressions.
    /// </param>
    /// <param name="enumerationOptions">An object that describes the search and enumeration configuration to use.</param>
    /// <returns>
    /// An enumerable collection of the full names (including paths) for the files
    /// in the directory specified by <paramref name="path"/>
    /// and that match the specified search pattern and enumeration options.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="searchPattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> does not contain a valid path.</exception>
    /// <exception cref="ArgumentException"><paramref name="searchPattern"/> does not contain a valid pattern.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="enumerationOptions"/> is not a valid <see cref="EnumerationOptions"/> value.</exception>
    /// <exception cref="DirectoryNotFoundException"><paramref name="path"/> is invalid, such as referring to an unmapped drive.</exception>
    /// <exception cref="IOException"><paramref name="path"/> is a file name.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or combined exceed the file system-defined maximum length.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions);

    /// <summary>
    /// Opens an existing file for reading.
    /// </summary>
    /// <param name="path">The file to be opened for reading.</param>
    /// <returns>A read-only <see cref="Stream"/> on the specified path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> does not contain a valid path.</exception>
    /// <exception cref="DirectoryNotFoundException"><paramref name="path"/> is invalid, such as referring to an unmapped drive.</exception>
    /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or combined exceed the file system-defined maximum length.</exception>
    /// <exception cref="UnauthorizedAccessException"><paramref name="path"/> is a directory name.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    Stream ReadFile(string path);

    /// <summary>
    /// Asynchronously opens an existing file for reading.
    /// </summary>
    /// <inheritdoc cref="ReadFile(string)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken = default);

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
    /// Returns an enumerable collection of the directory full names
    /// that match a search pattern in a specified path,
    /// and optionally searches subdirectories.
    /// </summary>
    /// <param name="path">
    /// The relative or absolute path to the directory to search.
    /// This string is not case-sensitive.
    /// </param>
    /// <param name="searchPattern">
    /// The search string to match against the names of directories in path.
    /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters,
    /// but it doesn't support regular expressions.
    /// </param>
    /// <param name="enumerationOptions">An object that describes the search and enumeration configuration to use.</param>
    /// <returns>
    /// An enumerable collection of the full names (including paths) for the directories
    /// in the directory specified by <paramref name="path"/>
    /// and that match the specified search pattern and enumeration options.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="searchPattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> does not contain a valid path.</exception>
    /// <exception cref="ArgumentException"><paramref name="searchPattern"/> does not contain a valid pattern.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="enumerationOptions"/> is not a valid <see cref="EnumerationOptions"/> value.</exception>
    /// <exception cref="DirectoryNotFoundException"><paramref name="path"/> is invalid, such as referring to an unmapped drive.</exception>
    /// <exception cref="IOException"><paramref name="path"/> is a file name.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or combined exceed the file system-defined maximum length.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    IEnumerable<string> EnumerateDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions);

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

    /// <summary>
    /// Returns an enumerable collection of file names and directory names
    /// that match a search pattern and enumeration options in a specified path.
    /// </summary>
    /// <param name="path">
    /// The relative or absolute path to the directory to search.
    /// This string is not case-sensitive.
    /// </param>
    /// <param name="searchPattern">
    /// The search string to match against the names of files and directories in path.
    /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters,
    /// but it doesn't support regular expressions.
    /// </param>
    /// <param name="enumerationOptions">An object that describes the search and enumeration configuration to use.</param>
    /// <returns>
    /// An enumerable collection of file-system entries
    /// in the directory specified by <paramref name="path"/>
    /// that match the specified search pattern and the specified enumeration options.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="searchPattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> does not contain a valid path.</exception>
    /// <exception cref="ArgumentException"><paramref name="searchPattern"/> does not contain a valid pattern.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="enumerationOptions"/> is not a valid <see cref="EnumerationOptions"/> value.</exception>
    /// <exception cref="DirectoryNotFoundException"><paramref name="path"/> is invalid, such as referring to an unmapped drive.</exception>
    /// <exception cref="IOException"><paramref name="path"/> is a file name.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or combined exceed the file system-defined maximum length.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    IEnumerable<string> EnumerateEntries(string path, string searchPattern, EnumerationOptions enumerationOptions);

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

    /// <inheritdoc cref="GetCreationTime(string)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<DateTime> GetCreationTimeAsync(string path, CancellationToken cancellationToken = default);

    /// <remarks>
    /// If the file or directory described in the <paramref name="path"/> parameter does not exist,
    /// this method returns <see cref="DateTime.MinValue"/>.
    /// </remarks>
    /// <inheritdoc cref="File.GetLastAccessTimeUtc(string)"/>
    DateTime GetLastAccessTime(string path);

    /// <summary>
    /// Gets the <see cref="FileAttributes"/> of the specified file or directory.
    /// </summary>
    /// <param name="path">The file or directory for which to obtain attribute information.</param>
    /// <inheritdoc cref="File.GetAttributes(string)"/>
    FileAttributes GetAttributes(string path);

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

    /// <inheritdoc cref="FileSystem.PathComparison"/>
    StringComparison PathComparison { get; }

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

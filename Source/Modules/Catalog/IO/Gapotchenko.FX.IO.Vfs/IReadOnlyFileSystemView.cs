// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a read-only virtual file-system view.
/// </summary>
public interface IReadOnlyFileSystemView
{
    /// <summary>
    /// Gets a value indicating whether the current file system supports reading.
    /// </summary>
    bool CanRead { get; }

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
    Stream OpenReadableFile(string path);

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

    #endregion

    #region Paths

    /// <summary>
    /// Gets a character used to separate directory levels in a path string
    /// that reflects a hierarchical file system organization.
    /// </summary>
    char DirectorySeparatorChar { get; }

    /// <inheritdoc cref="FileSystem.PathComparer"/>
    StringComparer PathComparer { get; }

    /// <inheritdoc cref="Path.GetFullPath(string)"/>
    [return: NotNullIfNotNull(nameof(path))]
    string? GetFullPath(string? path);

    /// <summary>
    /// Returns a value indicating whether the specified path string contains a root.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns><see langword="true"/> if <paramref name="path"/> contains a root; otherwise, <see langword="false"/>.</returns>
    bool IsPathRooted(ReadOnlySpan<char> path);

    /// <summary>
    /// Combines a sequence of strings into a path.
    /// </summary>
    /// <param name="paths">A sequence of parts of the path.</param>
    /// <returns>The combined paths.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="paths"/> is <see langword="null"/>.</exception>
    string CombinePaths(params IEnumerable<string?> paths);

    #endregion
}

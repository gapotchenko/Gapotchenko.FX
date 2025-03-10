// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Diagnostics;

namespace Gapotchenko.FX.IO;

/// <summary>
/// Represents a virtual file system view of the local file system.
/// </summary>
sealed class LocalFileSystemView : IFileSystemView
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static LocalFileSystemView Instance { get; } = new();

    #region File

    public bool FileExists([NotNullWhen(true)] string? path) => File.Exists(path);

    public IEnumerable<string> EnumerateFiles(string path) => Directory.EnumerateFiles(path);

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern) => Directory.EnumerateFiles(path, searchPattern);

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateFiles(path, searchPattern, searchOption);

    public Stream OpenFileForReading(string path) => File.OpenRead(path);

    #endregion

    #region Path

    [return: NotNullIfNotNull(nameof(path))]
    public string? GetFullPath(string? path) => path is null ? null : Path.GetFullPath(path);

    #endregion

    #region Directory

    public bool DirectoryExists([NotNullWhen(true)] string? path) => Directory.Exists(path);

    public IEnumerable<string> EnumerateDirectories(string path) => Directory.EnumerateDirectories(path);

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern) => Directory.EnumerateDirectories(path, searchPattern);

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateDirectories(path, searchPattern, searchOption);

    #endregion

    #region Entry

    public bool EntryExists([NotNullWhen(true)] string? path) => FileSystem.EntryExists(path);

    public IEnumerable<string> EnumerateEntries(string path) => Directory.EnumerateFileSystemEntries(path);

    public IEnumerable<string> EnumerateEntries(string path, string searchPattern) => Directory.EnumerateFileSystemEntries(path, searchPattern);

    public IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

    #endregion
}

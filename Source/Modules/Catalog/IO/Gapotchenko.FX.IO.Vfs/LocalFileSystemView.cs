// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Linq;
using System.Diagnostics;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a virtual file system view of the local file system.
/// </summary>
sealed class LocalFileSystemView : IFileSystemView
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static LocalFileSystemView Instance { get; } = new();

    #region Capabilities

    public bool CanRead => true;

    public bool CanWrite => true;

    #endregion

    #region Files

    public bool FileExists([NotNullWhen(true)] string? path) => File.Exists(path);

    public IEnumerable<string> EnumerateFiles(string path) => Directory.EnumerateFiles(path);

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern) => Directory.EnumerateFiles(path, searchPattern);

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateFiles(path, searchPattern, searchOption);

    public Stream OpenFileRead(string path) => File.OpenRead(path);

    public Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share) => File.Open(path, mode, access, share);

    public void DeleteFile(string path) => File.Delete(path);

    #endregion

    #region Directories

    public bool DirectoryExists([NotNullWhen(true)] string? path) => Directory.Exists(path);

    public IEnumerable<string> EnumerateDirectories(string path) => Directory.EnumerateDirectories(path);

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern) => Directory.EnumerateDirectories(path, searchPattern);

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateDirectories(path, searchPattern, searchOption);

    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public void DeleteDirectory(string path) => Directory.Delete(path);

    public void DeleteDirectory(string path, bool recursive) => Directory.Delete(path, recursive);

    #endregion

    #region Entries

    public bool EntryExists([NotNullWhen(true)] string? path) => FileSystem.EntryExists(path);

    public IEnumerable<string> EnumerateEntries(string path) => Directory.EnumerateFileSystemEntries(path);

    public IEnumerable<string> EnumerateEntries(string path, string searchPattern) => Directory.EnumerateFileSystemEntries(path, searchPattern);

    public IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

    #endregion

    #region Paths

    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;

    public StringComparer PathComparer => FileSystem.PathComparer;

    [return: NotNullIfNotNull(nameof(path))]
    public string? GetFullPath(string? path) => path is null ? null : Path.GetFullPath(path);

    public bool IsPathRooted(ReadOnlySpan<char> path) =>
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        Path.IsPathRooted(path);
#else
        Path.IsPathRooted(path.ToString());
#endif

    public string CombinePaths(params IEnumerable<string?> paths)
    {
        if (paths is null)
            throw new ArgumentNullException(nameof(paths));

        string?[] arr = EnumerableEx.AsArray(paths);

        if (Array.IndexOf(arr, null) != -1)
            arr = arr.Where(x => x != null).ToArray();

        return Path.Combine(arr!);
    }

    #endregion
}

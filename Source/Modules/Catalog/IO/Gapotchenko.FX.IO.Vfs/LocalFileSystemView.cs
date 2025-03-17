// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.Linq;
using System.Diagnostics;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a virtual file-system view of the local file system.
/// </summary>
sealed class LocalFileSystemView : FileSystemViewKit
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static LocalFileSystemView Instance { get; } = new();

    #region Capabilities

    public override bool CanRead => true;

    public override bool CanWrite => true;

    #endregion

    #region Files

    public override bool FileExists([NotNullWhen(true)] string? path) => File.Exists(path);

    public override IEnumerable<string> EnumerateFiles(string path) => Directory.EnumerateFiles(path);

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern) => Directory.EnumerateFiles(path, searchPattern);

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateFiles(path, searchPattern, searchOption);

    public override Stream OpenFileRead(string path) => File.OpenRead(path);

    public override Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share) => File.Open(path, mode, access, share);

    public override void DeleteFile(string path) => File.Delete(path);

    public override void CopyFile(string sourcePath, string destinationPath, bool overwrite) =>
        File.Copy(sourcePath, destinationPath, overwrite);

    #endregion

    #region Directories

    public override bool DirectoryExists([NotNullWhen(true)] string? path) => Directory.Exists(path);

    public override IEnumerable<string> EnumerateDirectories(string path) => Directory.EnumerateDirectories(path);

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern) => Directory.EnumerateDirectories(path, searchPattern);

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateDirectories(path, searchPattern, searchOption);

    public override void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public override void DeleteDirectory(string path, bool recursive) => Directory.Delete(path, recursive);

    #endregion

    #region Entries

    public override bool EntryExists([NotNullWhen(true)] string? path) => FileSystem.EntryExists(path);

    public override IEnumerable<string> EnumerateEntries(string path) => Directory.EnumerateFileSystemEntries(path);

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern) => Directory.EnumerateFileSystemEntries(path, searchPattern);

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption) =>
        Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

    #endregion

    #region Paths

    public override char DirectorySeparatorChar => Path.DirectorySeparatorChar;

    public override StringComparer PathComparer => FileSystem.PathComparer;

    protected override string GetFullPathCore(string path) => Path.GetFullPath(path);

    public override bool IsPathRooted(ReadOnlySpan<char> path) =>
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        Path.IsPathRooted(path);
#else
        Path.IsPathRooted(path.ToString());
#endif

    public override string CombinePaths(params IEnumerable<string?> paths)
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

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.Vfs.Utils;

namespace Gapotchenko.FX.IO.Vfs;

sealed class FileSystemViewWithCapabilities(IFileSystemView baseView, bool canRead, bool canWrite) :
    FileSystemViewProxyKit(baseView)
{
    #region Capabilities

    public override bool CanRead => canRead && base.CanRead;

    public override bool CanWrite => canWrite && base.CanWrite;

    void EnsureCanRead()
    {
        if (!canRead)
            ThrowHelper.CannotReadFS();
    }

    void EnsureCanWrite()
    {
        if (!canWrite)
            ThrowHelper.CannotWriteFS();
    }

    void EnsureCanReadAndWrite()
    {
        EnsureCanRead();
        EnsureCanWrite();
    }

    void EnsureCanOpenFile(FileMode mode, FileAccess access) =>
        VfsValidationKit.Capabilities.EnsureCanOpenFile(mode, access, canRead, canWrite);

    #endregion

    #region Files

    public override bool FileExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return base.FileExists(path);
    }

    public override Task<bool> FileExistsAsync([NotNullWhen(true)] string? path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.FileExistsAsync(path, cancellationToken);
    }

    public override IEnumerable<string> EnumerateFiles(string path)
    {
        EnsureCanRead();
        return base.EnumerateFiles(path);
    }

    public override IAsyncEnumerable<string> EnumerateFilesAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateFilesAsync(path, cancellationToken);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern)
    {
        EnsureCanRead();
        return base.EnumerateFiles(path, searchPattern);
    }

    public override IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateFilesAsync(path, searchPattern, cancellationToken);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        EnsureCanRead();
        return base.EnumerateFiles(path, searchPattern, searchOption);
    }

    public override IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern, SearchOption searchOption, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateFilesAsync(path, searchPattern, searchOption, cancellationToken);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        EnsureCanRead();
        return base.EnumerateFiles(path, searchPattern, enumerationOptions);
    }

    public override IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern, EnumerationOptions enumerationOptions, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateFilesAsync(path, searchPattern, enumerationOptions, cancellationToken);
    }

    public override Stream ReadFile(string path)
    {
        EnsureCanRead();
        return base.ReadFile(path);
    }

    public override Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.ReadFileAsync(path, cancellationToken);
    }

    public override Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
    {
        EnsureCanOpenFile(mode, access);
        return base.OpenFile(path, mode, access, share);
    }

    public override Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share, CancellationToken cancellationToken = default)
    {
        EnsureCanOpenFile(mode, access);
        return base.OpenFileAsync(path, mode, access, share, cancellationToken);
    }

    public override void DeleteFile(string path)
    {
        EnsureCanWrite();
        base.DeleteFile(path);
    }

    public override Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureCanWrite();
        return base.DeleteFileAsync(path, cancellationToken);
    }

    public override void CopyFile(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options)
    {
        EnsureCanReadAndWrite();
        base.CopyFile(sourcePath, destinationPath, overwrite, options);
    }

    public override Task CopyFileAsync(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options, CancellationToken cancellationToken = default)
    {
        EnsureCanReadAndWrite();
        return base.CopyFileAsync(sourcePath, destinationPath, overwrite, options, cancellationToken);
    }

    public override void MoveFile(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options)
    {
        EnsureCanReadAndWrite();
        base.MoveFile(sourcePath, destinationPath, overwrite, options);
    }

    public override Task MoveFileAsync(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options, CancellationToken cancellationToken = default)
    {
        EnsureCanReadAndWrite();
        return base.MoveFileAsync(sourcePath, destinationPath, overwrite, options, cancellationToken);
    }

    #endregion

    #region Directories

    public override bool DirectoryExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return base.DirectoryExists(path);
    }

    public override Task<bool> DirectoryExistsAsync([NotNullWhen(true)] string? path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.DirectoryExistsAsync(path, cancellationToken);
    }

    public override IEnumerable<string> EnumerateDirectories(string path)
    {
        EnsureCanRead();
        return base.EnumerateDirectories(path);
    }

    public override IAsyncEnumerable<string> EnumerateDirectoriesAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateDirectoriesAsync(path, cancellationToken);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
    {
        EnsureCanRead();
        return base.EnumerateDirectories(path, searchPattern);
    }

    public override IAsyncEnumerable<string> EnumerateDirectoriesAsync(string path, string searchPattern, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateDirectoriesAsync(path, searchPattern, cancellationToken);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        EnsureCanRead();
        return base.EnumerateDirectories(path, searchPattern, searchOption);
    }

    public override IAsyncEnumerable<string> EnumerateDirectoriesAsync(string path, string searchPattern, SearchOption searchOption, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateDirectoriesAsync(path, searchPattern, searchOption, cancellationToken);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        EnsureCanRead();
        return base.EnumerateDirectories(path, searchPattern, enumerationOptions);
    }

    public override IAsyncEnumerable<string> EnumerateDirectoriesAsync(string path, string searchPattern, EnumerationOptions enumerationOptions, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateDirectoriesAsync(path, searchPattern, enumerationOptions, cancellationToken);
    }

    public override void CreateDirectory(string path)
    {
        EnsureCanWrite();
        base.CreateDirectory(path);
    }

    public override Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureCanWrite();
        return base.CreateDirectoryAsync(path, cancellationToken);
    }

    public override void DeleteDirectory(string path, bool recursive)
    {
        EnsureCanWrite();
        base.DeleteDirectory(path, recursive);
    }

    public override Task DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken = default)
    {
        EnsureCanWrite();
        return base.DeleteDirectoryAsync(path, recursive, cancellationToken);
    }

    public override void CopyDirectory(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options)
    {
        EnsureCanReadAndWrite();
        base.CopyDirectory(sourcePath, destinationPath, overwrite, options);
    }

    public override void MoveDirectory(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options)
    {
        EnsureCanReadAndWrite();
        base.MoveDirectory(sourcePath, destinationPath, overwrite, options);
    }

    #endregion

    #region Entries

    public override bool EntryExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return base.EntryExists(path);
    }

    public override Task<bool> EntryExistsAsync([NotNullWhen(true)] string? path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EntryExistsAsync(path, cancellationToken);
    }

    public override IEnumerable<string> EnumerateEntries(string path)
    {
        EnsureCanRead();
        return base.EnumerateEntries(path);
    }

    public override IAsyncEnumerable<string> EnumerateEntriesAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateEntriesAsync(path, cancellationToken);
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern)
    {
        EnsureCanRead();
        return base.EnumerateEntries(path, searchPattern);
    }

    public override IAsyncEnumerable<string> EnumerateEntriesAsync(string path, string searchPattern, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateEntriesAsync(path, searchPattern, cancellationToken);
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption)
    {
        EnsureCanRead();
        return base.EnumerateEntries(path, searchPattern, searchOption);
    }

    public override IAsyncEnumerable<string> EnumerateEntriesAsync(string path, string searchPattern, SearchOption searchOption, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateEntriesAsync(path, searchPattern, searchOption, cancellationToken);
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        EnsureCanRead();
        return base.EnumerateEntries(path, searchPattern, enumerationOptions);
    }

    public override IAsyncEnumerable<string> EnumerateEntriesAsync(string path, string searchPattern, EnumerationOptions enumerationOptions, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.EnumerateEntriesAsync(path, searchPattern, enumerationOptions, cancellationToken);
    }

    public override DateTime GetCreationTime(string path)
    {
        EnsureCanRead();
        return base.GetCreationTime(path);
    }

    public override Task<DateTime> GetCreationTimeAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.GetCreationTimeAsync(path, cancellationToken);
    }

    public override void SetCreationTime(string path, DateTime lastWriteTime)
    {
        EnsureCanWrite();
        base.SetCreationTime(path, lastWriteTime);
    }

    public override Task SetCreationTimeAsync(string path, DateTime creationTime, CancellationToken cancellationToken = default)
    {
        EnsureCanWrite();
        return base.SetCreationTimeAsync(path, creationTime, cancellationToken);
    }

    public override DateTime GetLastWriteTime(string path)
    {
        EnsureCanRead();
        return base.GetLastWriteTime(path);
    }

    public override Task<DateTime> GetLastWriteTimeAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.GetLastWriteTimeAsync(path, cancellationToken);
    }

    public override void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
        EnsureCanWrite();
        base.SetLastWriteTime(path, lastWriteTime);
    }

    public override Task SetLastWriteTimeAsync(string path, DateTime lastWriteTime, CancellationToken cancellationToken = default)
    {
        EnsureCanWrite();
        return base.SetLastWriteTimeAsync(path, lastWriteTime, cancellationToken);
    }

    public override DateTime GetLastAccessTime(string path)
    {
        EnsureCanRead();
        return base.GetLastAccessTime(path);
    }

    public override Task<DateTime> GetLastAccessTimeAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.GetLastAccessTimeAsync(path, cancellationToken);
    }

    public override void SetLastAccessTime(string path, DateTime lastAccessTime)
    {
        EnsureCanWrite();
        base.SetLastAccessTime(path, lastAccessTime);
    }

    public override Task SetLastAccessTimeAsync(string path, DateTime lastAccessTime, CancellationToken cancellationToken = default)
    {
        EnsureCanWrite();
        return base.SetLastAccessTimeAsync(path, lastAccessTime, cancellationToken);
    }

    public override FileAttributes GetAttributes(string path)
    {
        EnsureCanRead();
        return base.GetAttributes(path);
    }

    public override Task<FileAttributes> GetAttributesAsync(string path, CancellationToken cancellationToken = default)
    {
        EnsureCanRead();
        return base.GetAttributesAsync(path, cancellationToken);
    }

    public override void SetAttributes(string path, FileAttributes attributes)
    {
        EnsureCanWrite();
        base.SetAttributes(path, attributes);
    }

    public override Task SetAttributesAsync(string path, FileAttributes attributes, CancellationToken cancellationToken = default)
    {
        EnsureCanWrite();
        return base.SetAttributesAsync(path, attributes, cancellationToken);
    }

    #endregion
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Threading.Tasks;

namespace Gapotchenko.FX.IO.Vfs.Tests.Utils;

/// <summary>
/// Swaps synchronous and asynchronous VFS operations.
/// </summary>
/// <param name="baseView">The base file system view.</param>
sealed class VfsSyncAsyncSwapTransform(IFileSystemView baseView) : FileSystemViewProxyKit(baseView), IVfsTransform
{
    public IFileSystemView UnderlyingVfs => BaseView;

    public void Dispose()
    {
        if (BaseView is IDisposable disposable)
            disposable.Dispose();
    }

    // ------------------------------------------------------------------------

    #region Files

    public override bool FileExists([NotNullWhen(true)] string? path)
    {
        return TaskBridge.Execute(cancellationToken => base.FileExistsAsync(path, cancellationToken));
    }

    public override Task<bool> FileExistsAsync([NotNullWhen(true)] string? path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.FileExists(path), cancellationToken);
    }

    public override IEnumerable<string> EnumerateFiles(string path)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateFilesAsync(path));
    }

    public override IAsyncEnumerable<string> EnumerateFilesAsync(string path, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateFiles(path), cancellationToken);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateFilesAsync(path, searchPattern));
    }

    public override IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateFiles(path, searchPattern), cancellationToken);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateFilesAsync(path, searchPattern, searchOption));
    }

    public override IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern, SearchOption searchOption, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateFiles(path, searchPattern, searchOption), cancellationToken);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateFilesAsync(path, searchPattern, enumerationOptions));
    }

    public override IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern, EnumerationOptions enumerationOptions, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateFiles(path, searchPattern, enumerationOptions), cancellationToken);
    }

    public override long GetFileSize(string path)
    {
        return TaskBridge.Execute(cancellationToken => base.GetFileSizeAsync(path, cancellationToken));
    }

    public override Task<long> GetFileSizeAsync(string path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.GetFileSize(path), cancellationToken);
    }

    public override Stream ReadFile(string path)
    {
        return TaskBridge.Execute(cancellationToken => base.ReadFileAsync(path, cancellationToken));
    }

    public override Task<Stream> ReadFileAsync(string path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.ReadFile(path), cancellationToken);
    }

    public override Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
    {
        return TaskBridge.Execute(cancellationToken => base.OpenFileAsync(path, mode, access, share, cancellationToken));
    }

    public override Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.OpenFile(path, mode, access, share), cancellationToken);
    }

    public override void DeleteFile(string path)
    {
        TaskBridge.Execute(cancellationToken => base.DeleteFileAsync(path, cancellationToken));
    }

    public override Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.DeleteFile(path), cancellationToken);
    }

    public override void CopyFile(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options)
    {
        TaskBridge.Execute(cancellationToken => base.CopyFileAsync(sourcePath, destinationPath, overwrite, options, cancellationToken));
    }

    public override Task CopyFileAsync(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.CopyFile(sourcePath, destinationPath, overwrite, options), cancellationToken);
    }

    public override void MoveFile(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options)
    {
        TaskBridge.Execute(cancellationToken => base.MoveFileAsync(sourcePath, destinationPath, overwrite, options, cancellationToken));
    }

    public override Task MoveFileAsync(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.MoveFile(sourcePath, destinationPath, overwrite, options), cancellationToken);
    }

    #endregion

    #region Directories

    public override bool DirectoryExists([NotNullWhen(true)] string? path)
    {
        return TaskBridge.Execute(cancellationToken => base.DirectoryExistsAsync(path, cancellationToken));
    }

    public override Task<bool> DirectoryExistsAsync([NotNullWhen(true)] string? path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.DirectoryExists(path), cancellationToken);
    }

    public override IEnumerable<string> EnumerateDirectories(string path)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateDirectoriesAsync(path));
    }

    public override IAsyncEnumerable<string> EnumerateDirectoriesAsync(string path, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateDirectories(path), cancellationToken);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateDirectoriesAsync(path, searchPattern));
    }

    public override IAsyncEnumerable<string> EnumerateDirectoriesAsync(string path, string searchPattern, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateDirectories(path, searchPattern), cancellationToken);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateDirectoriesAsync(path, searchPattern, searchOption));
    }

    public override IAsyncEnumerable<string> EnumerateDirectoriesAsync(string path, string searchPattern, SearchOption searchOption, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateDirectories(path, searchPattern, searchOption), cancellationToken);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateDirectoriesAsync(path, searchPattern, enumerationOptions));
    }

    public override IAsyncEnumerable<string> EnumerateDirectoriesAsync(string path, string searchPattern, EnumerationOptions enumerationOptions, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateDirectories(path, searchPattern, enumerationOptions), cancellationToken);
    }

    public override void CreateDirectory(string path)
    {
        TaskBridge.Execute(cancellationToken => base.CreateDirectoryAsync(path, cancellationToken));
    }

    public override Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.CreateDirectory(path), cancellationToken);
    }

    public override void DeleteDirectory(string path, bool recursive)
    {
        TaskBridge.Execute(cancellationToken => base.DeleteDirectoryAsync(path, recursive, cancellationToken));
    }

    public override Task DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.DeleteDirectory(path, recursive), cancellationToken);
    }

    public override void CopyDirectory(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options)
    {
        TaskBridge.Execute(cancellationToken => base.CopyDirectoryAsync(sourcePath, destinationPath, overwrite, options, cancellationToken));
    }

    public override Task CopyDirectoryAsync(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.CopyDirectory(sourcePath, destinationPath, overwrite, options), cancellationToken);
    }

    public override void MoveDirectory(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options)
    {
        TaskBridge.Execute(cancellationToken => base.MoveDirectoryAsync(sourcePath, destinationPath, overwrite, options, cancellationToken));
    }

    public override Task MoveDirectoryAsync(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.MoveDirectory(sourcePath, destinationPath, overwrite, options), cancellationToken);
    }

    #endregion

    #region Entries

    public override bool EntryExists([NotNullWhen(true)] string? path)
    {
        return TaskBridge.Execute(cancellationToken => base.EntryExistsAsync(path, cancellationToken));
    }

    public override Task<bool> EntryExistsAsync([NotNullWhen(true)] string? path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.EntryExists(path), cancellationToken);
    }

    public override IEnumerable<string> EnumerateEntries(string path)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateEntriesAsync(path));
    }

    public override IAsyncEnumerable<string> EnumerateEntriesAsync(string path, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateEntries(path), cancellationToken);
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateEntriesAsync(path, searchPattern));
    }

    public override IAsyncEnumerable<string> EnumerateEntriesAsync(string path, string searchPattern, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateEntries(path, searchPattern), cancellationToken);
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateEntriesAsync(path, searchPattern, searchOption));
    }

    public override IAsyncEnumerable<string> EnumerateEntriesAsync(string path, string searchPattern, SearchOption searchOption, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateEntries(path, searchPattern, searchOption), cancellationToken);
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        return AsyncEnumerableBridge.Enumerate(base.EnumerateEntriesAsync(path, searchPattern, enumerationOptions));
    }

    public override IAsyncEnumerable<string> EnumerateEntriesAsync(string path, string searchPattern, EnumerationOptions enumerationOptions, CancellationToken cancellationToken = default)
    {
        return AsyncEnumerableBridge.EnumerateAsync(() => base.EnumerateEntries(path, searchPattern, enumerationOptions), cancellationToken);
    }

    public override DateTime GetLastWriteTime(string path)
    {
        return TaskBridge.Execute(cancellationToken => base.GetLastWriteTimeAsync(path, cancellationToken));
    }

    public override Task<DateTime> GetLastWriteTimeAsync(string path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.GetLastWriteTime(path), cancellationToken);
    }

    public override void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
        TaskBridge.Execute(cancellationToken => base.SetLastWriteTimeAsync(path, lastWriteTime, cancellationToken));
    }

    public override Task SetLastWriteTimeAsync(string path, DateTime lastWriteTime, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.SetLastWriteTime(path, lastWriteTime), cancellationToken);
    }

    public override DateTime GetCreationTime(string path)
    {
        return TaskBridge.Execute(cancellationToken => base.GetCreationTimeAsync(path, cancellationToken));
    }

    public override Task<DateTime> GetCreationTimeAsync(string path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.GetCreationTime(path), cancellationToken);
    }

    public override void SetCreationTime(string path, DateTime creationTime)
    {
        TaskBridge.Execute(cancellationToken => base.SetCreationTimeAsync(path, creationTime, cancellationToken));
    }

    public override Task SetCreationTimeAsync(string path, DateTime creationTime, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.SetCreationTime(path, creationTime), cancellationToken);
    }

    public override DateTime GetLastAccessTime(string path)
    {
        return TaskBridge.Execute(cancellationToken => base.GetLastAccessTimeAsync(path, cancellationToken));
    }

    public override Task<DateTime> GetLastAccessTimeAsync(string path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.GetLastAccessTime(path), cancellationToken);
    }

    public override void SetLastAccessTime(string path, DateTime lastAccessTime)
    {
        TaskBridge.Execute(cancellationToken => base.SetLastAccessTimeAsync(path, lastAccessTime, cancellationToken));
    }

    public override Task SetLastAccessTimeAsync(string path, DateTime lastAccessTime, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.SetLastAccessTime(path, lastAccessTime), cancellationToken);
    }

    public override FileAttributes GetAttributes(string path)
    {
        return TaskBridge.Execute(cancellationToken => base.GetAttributesAsync(path, cancellationToken));
    }

    public override Task<FileAttributes> GetAttributesAsync(string path, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.GetAttributes(path), cancellationToken);
    }

    public override void SetAttributes(string path, FileAttributes attributes)
    {
        TaskBridge.Execute(cancellationToken => base.SetAttributesAsync(path, attributes, cancellationToken));
    }

    public override Task SetAttributesAsync(string path, FileAttributes attributes, CancellationToken cancellationToken = default)
    {
        return TaskBridge.ExecuteAsync(() => base.SetAttributes(path, attributes), cancellationToken);
    }

    #endregion
}

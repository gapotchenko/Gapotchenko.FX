// Gapotchenko.FX
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
        FileSystemViewCapabilities.EnsureCanOpenFile(mode, access, canRead, canWrite);

    #endregion

    #region Files

    public override bool FileExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return base.FileExists(path);
    }

    public override IEnumerable<string> EnumerateFiles(string path)
    {
        EnsureCanRead();
        return base.EnumerateFiles(path);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern)
    {
        EnsureCanRead();
        return base.EnumerateFiles(path, searchPattern);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        EnsureCanRead();
        return base.EnumerateFiles(path, searchPattern, searchOption);
    }

    public override Stream OpenFileRead(string path)
    {
        EnsureCanRead();
        return base.OpenFileRead(path);
    }

    public override Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
    {
        EnsureCanOpenFile(mode, access);
        return base.OpenFile(path, mode, access, share);
    }

    public override void DeleteFile(string path)
    {
        EnsureCanWrite();
        base.DeleteFile(path);
    }

    public override void CopyFile(string sourcePath, string destinationPath, bool overwrite)
    {
        EnsureCanReadAndWrite();
        base.CopyFile(sourcePath, destinationPath, overwrite);
    }

    public override void MoveFile(string sourcePath, string destinationPath, bool overwrite)
    {
        EnsureCanReadAndWrite();
        base.MoveFile(sourcePath, destinationPath, overwrite);
    }

    #endregion

    #region Directories

    public override bool DirectoryExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return base.DirectoryExists(path);
    }

    public override IEnumerable<string> EnumerateDirectories(string path)
    {
        EnsureCanRead();
        return base.EnumerateDirectories(path);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
    {
        EnsureCanRead();
        return base.EnumerateDirectories(path, searchPattern);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        EnsureCanRead();
        return base.EnumerateDirectories(path, searchPattern, searchOption);
    }

    public override void CreateDirectory(string path)
    {
        EnsureCanWrite();
        base.CreateDirectory(path);
    }

    public override void DeleteDirectory(string path, bool recursive)
    {
        EnsureCanWrite();
        base.DeleteDirectory(path, recursive);
    }

    public override void CopyDirectory(string sourcePath, string destinationPath, bool overwrite)
    {
        EnsureCanReadAndWrite();
        base.CopyDirectory(sourcePath, destinationPath, overwrite);
    }

    public override void MoveDirectory(string sourcePath, string destinationPath, bool overwrite)
    {
        EnsureCanReadAndWrite();
        base.MoveDirectory(sourcePath, destinationPath, overwrite);
    }

    #endregion

    #region Entries

    public override bool EntryExists([NotNullWhen(true)] string? path)
    {
        EnsureCanRead();
        return base.EntryExists(path);
    }

    public override IEnumerable<string> EnumerateEntries(string path)
    {
        EnsureCanRead();
        return base.EnumerateEntries(path);
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern)
    {
        EnsureCanRead();
        return base.EnumerateEntries(path, searchPattern);
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption)
    {
        EnsureCanRead();
        return base.EnumerateEntries(path, searchPattern, searchOption);
    }

    #endregion
}

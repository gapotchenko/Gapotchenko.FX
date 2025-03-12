// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.IO.Vfs;

sealed class FileSystemViewWithCapabilities(IFileSystemView baseView, bool canRead, bool canWrite) :
    FileSystemViewProxyKit(baseView)
{
    #region Capabilities

    public override bool CanRead => canRead && base.CanRead;

    public override bool CanWrite => canWrite && base.CanWrite;

    void ValidateRead()
    {
        if (!canRead)
            throw new NotSupportedException("File system view does not support reading.");
    }

    void ValidateWrite()
    {
        if (!canWrite)
            throw new NotSupportedException("File system view does not support writing.");
    }

    #endregion

    #region Files

    public override bool FileExists([NotNullWhen(true)] string? path)
    {
        ValidateRead();
        return base.FileExists(path);
    }

    public override IEnumerable<string> EnumerateFiles(string path)
    {
        ValidateRead();
        return base.EnumerateFiles(path);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern)
    {
        ValidateRead();
        return base.EnumerateFiles(path, searchPattern);
    }

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        ValidateRead();
        return base.EnumerateFiles(path, searchPattern, searchOption);
    }

    public override Stream OpenFileForReading(string path)
    {
        ValidateRead();
        return base.OpenFileForReading(path);
    }

    public override void DeleteFile(string path)
    {
        ValidateWrite();
        base.DeleteFile(path);
    }

    #endregion

    #region Directories

    public override bool DirectoryExists([NotNullWhen(true)] string? path)
    {
        ValidateRead();
        return base.DirectoryExists(path);
    }

    public override IEnumerable<string> EnumerateDirectories(string path)
    {
        ValidateRead();
        return base.EnumerateDirectories(path);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
    {
        ValidateRead();
        return base.EnumerateDirectories(path, searchPattern);
    }

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        ValidateRead();
        return base.EnumerateDirectories(path, searchPattern, searchOption);
    }

    public override void CreateDirectory(string path)
    {
        ValidateWrite();
        base.CreateDirectory(path);
    }

    public override void DeleteDirectory(string path)
    {
        ValidateWrite();
        base.DeleteDirectory(path);
    }

    public override void DeleteDirectory(string path, bool recursive)
    {
        ValidateWrite();
        base.DeleteDirectory(path, recursive);
    }

    #endregion

    #region Entries

    public override bool EntryExists([NotNullWhen(true)] string? path)
    {
        ValidateRead();
        return base.EntryExists(path);
    }

    public override IEnumerable<string> EnumerateEntries(string path)
    {
        ValidateRead();
        return base.EnumerateEntries(path);
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern)
    {
        ValidateRead();
        return base.EnumerateEntries(path, searchPattern);
    }

    public override IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption)
    {
        ValidateRead();
        return base.EnumerateEntries(path, searchPattern, searchOption);
    }

    #endregion
}

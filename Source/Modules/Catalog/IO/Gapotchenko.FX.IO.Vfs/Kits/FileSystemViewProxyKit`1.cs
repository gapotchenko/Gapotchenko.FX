﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides a base implementation of <see cref="IFileSystemView"/> proxy.
/// </summary>
/// <typeparam name="T">The type of the base file-system view.</typeparam>
public abstract class FileSystemViewProxyKit<T> : IFileSystemView
    where T : IFileSystemView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemViewProxyKit{T}"/> class with the specified base stream.
    /// </summary>
    /// <param name="baseView">The base file-system view to create the proxy for.</param>
    /// <exception cref="ArgumentNullException"><paramref name="baseView"/> is <see langword="null"/>.</exception>
    protected FileSystemViewProxyKit(T baseView)
    {
        if (baseView == null)
            throw new ArgumentNullException(nameof(baseView));

        BaseView = baseView;
    }

    #region Capabilities

    /// <inheritdoc/>
    public virtual bool CanRead => BaseView.CanRead;

    /// <inheritdoc/>
    public virtual bool CanWrite => BaseView.CanWrite;

    /// <inheritdoc/>
    public virtual bool SupportsCreationTime => BaseView.SupportsCreationTime;

    /// <inheritdoc/>
    public virtual bool SupportsLastWriteTime => BaseView.SupportsLastWriteTime;

    /// <inheritdoc/>
    public virtual bool SupportsLastAccessTime => BaseView.SupportsLastAccessTime;

    /// <inheritdoc/>
    public virtual bool SupportsAttributes => BaseView.SupportsAttributes;

    #endregion

    #region Files

    /// <inheritdoc/>
    public virtual bool FileExists([NotNullWhen(true)] string? path) => BaseView.FileExists(path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateFiles(string path) => BaseView.EnumerateFiles(path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateFiles(string path, string searchPattern) =>
        BaseView.EnumerateFiles(path, searchPattern);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) =>
        BaseView.EnumerateFiles(path, searchPattern, searchOption);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        BaseView.EnumerateFiles(path, searchPattern, enumerationOptions);

    /// <inheritdoc/>
    public virtual Stream ReadFile(string path) => BaseView.ReadFile(path);

    /// <inheritdoc/>
    public virtual Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share) =>
        BaseView.OpenFile(path, mode, access, share);

    /// <inheritdoc/>
    public virtual void DeleteFile(string path) => BaseView.DeleteFile(path);

    /// <inheritdoc/>
    public virtual void CopyFile(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options) =>
        BaseView.CopyFile(sourcePath, destinationPath, overwrite, options);

    /// <inheritdoc/>
    public virtual void MoveFile(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options) =>
        BaseView.MoveFile(sourcePath, destinationPath, overwrite, options);

    #endregion

    #region Directories

    /// <inheritdoc/>
    public virtual bool DirectoryExists([NotNullWhen(true)] string? path) => BaseView.DirectoryExists(path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path) => BaseView.EnumerateDirectories(path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path, string searchPattern) =>
        BaseView.EnumerateDirectories(path, searchPattern);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        BaseView.EnumerateDirectories(path, searchPattern, searchOption);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        BaseView.EnumerateDirectories(path, searchPattern, enumerationOptions);

    /// <inheritdoc/>
    public virtual void CreateDirectory(string path) => BaseView.CreateDirectory(path);

    /// <inheritdoc/>
    public virtual void DeleteDirectory(string path, bool recursive) => BaseView.DeleteDirectory(path, recursive);

    /// <inheritdoc/>
    public virtual void CopyDirectory(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options) =>
        BaseView.CopyDirectory(sourcePath, destinationPath, overwrite, options);

    /// <inheritdoc/>
    public virtual void MoveDirectory(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options) =>
        BaseView.MoveDirectory(sourcePath, destinationPath, overwrite, options);

    #endregion

    #region Entries

    /// <inheritdoc/>
    public virtual bool EntryExists([NotNullWhen(true)] string? path) => BaseView.EntryExists(path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path) => BaseView.EnumerateEntries(path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path, string searchPattern) =>
        BaseView.EnumerateEntries(path, searchPattern);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption) =>
        BaseView.EnumerateEntries(path, searchPattern, searchOption);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        BaseView.EnumerateEntries(path, searchPattern, enumerationOptions);

    /// <inheritdoc/>
    public virtual DateTime GetCreationTime(string path) => BaseView.GetCreationTime(path);

    /// <inheritdoc/>
    public virtual void SetCreationTime(string path, DateTime creationTime) => BaseView.SetCreationTime(path, creationTime);

    /// <inheritdoc/>
    public virtual DateTime GetLastWriteTime(string path) => BaseView.GetLastWriteTime(path);

    /// <inheritdoc/>
    public virtual void SetLastWriteTime(string path, DateTime lastWriteTime) => BaseView.SetLastWriteTime(path, lastWriteTime);

    /// <inheritdoc/>
    public virtual DateTime GetLastAccessTime(string path) => BaseView.GetLastAccessTime(path);

    /// <inheritdoc/>
    public virtual void SetLastAccessTime(string path, DateTime lastAccessTime) => BaseView.SetLastAccessTime(path, lastAccessTime);

    /// <inheritdoc/>
    public virtual FileAttributes GetAttributes(string path) => BaseView.GetAttributes(path);

    /// <inheritdoc/>
    public virtual void SetAttributes(string path, FileAttributes attributes) => BaseView.SetAttributes(path, attributes);

    #endregion

    #region Paths

    /// <inheritdoc/>
    public virtual char DirectorySeparatorChar => BaseView.DirectorySeparatorChar;

    /// <inheritdoc/>
    public virtual char AltDirectorySeparatorChar => BaseView.AltDirectorySeparatorChar;

    /// <inheritdoc/>
    public virtual StringComparer PathComparer => BaseView.PathComparer;

    /// <inheritdoc/>
    public virtual StringComparison PathComparison => BaseView.PathComparison;

    /// <inheritdoc/>
    public virtual string CombinePaths(params IEnumerable<string?> paths) => BaseView.CombinePaths(paths);

    /// <inheritdoc/>
    public virtual string CombinePaths(params ReadOnlySpan<string?> paths) => BaseView.CombinePaths(paths);

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public virtual string? GetFullPath(string? path) => BaseView.GetFullPath(path);

    /// <inheritdoc/>
    public virtual string? GetDirectoryName(string? path) => BaseView.GetDirectoryName(path);

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path) => BaseView.GetDirectoryName(path);

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public virtual string? GetFileName(string? path) => BaseView.GetFileName(path);

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path) => BaseView.GetFileName(path);

    /// <inheritdoc/>
    public virtual bool IsPathRooted([NotNullWhen(true)] string? path) => BaseView.IsPathRooted(path);

    /// <inheritdoc/>
    public virtual bool IsPathRooted(ReadOnlySpan<char> path) => BaseView.IsPathRooted(path);

    /// <inheritdoc/>
    public virtual string? GetPathRoot(string? path) => BaseView.GetPathRoot(path);

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> GetPathRoot(ReadOnlySpan<char> path) => BaseView.GetPathRoot(path);

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public virtual string? TrimEndingDirectorySeparator(string? path) => BaseView.TrimEndingDirectorySeparator(path);

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) =>
         BaseView.TrimEndingDirectorySeparator(path);

    /// <inheritdoc/>
    public virtual bool EndsInDirectorySeparator([NotNullWhen(true)] string? path) => BaseView.EndsInDirectorySeparator(path);

    /// <inheritdoc/>
    public virtual bool EndsInDirectorySeparator(ReadOnlySpan<char> path) => BaseView.EndsInDirectorySeparator(path);

    #endregion

    /// <summary>
    /// Gets the base file-system view.
    /// </summary>
    protected T BaseView { get; }
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides a base implementation of a proxy for <see cref="IFileSystemView"/>.
/// </summary>
/// <typeparam name="T">The type of the base file system view.</typeparam>
public abstract class FileSystemViewProxyKit<T> : IFileSystemView
    where T : IFileSystemView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemViewProxyKit{T}"/> class with the specified base stream.
    /// </summary>
    /// <param name="baseView">The base file system view to create the proxy for.</param>
    /// <exception cref="ArgumentNullException"><paramref name="baseView"/> is <see langword="null"/>.</exception>
    protected FileSystemViewProxyKit(T baseView)
    {
        if (baseView is null)
            throw new ArgumentNullException(nameof(baseView));

        BaseView = baseView;
    }

    #region Capabilities

    /// <inheritdoc/>
    public virtual bool CanRead => BaseView.CanRead;

    /// <inheritdoc/>
    public virtual bool CanWrite => BaseView.CanWrite;

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
    public virtual Stream OpenFileForReading(string path) => BaseView.OpenFileForReading(path);

    /// <inheritdoc/>
    public virtual void DeleteFile(string path) => BaseView.DeleteFile(path);

    #endregion

    #region Directories

    /// <inheritdoc/>
    public virtual bool DirectoryExists([NotNullWhen(true)] string? path) => BaseView.DirectoryExists(path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path) => EnumerateDirectories(path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path, string searchPattern) =>
        EnumerateDirectories(path, searchPattern);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateDirectories(path, searchPattern, searchOption);

    /// <inheritdoc/>
    public virtual void CreateDirectory(string path) => BaseView.CreateDirectory(path);

    /// <inheritdoc/>
    public virtual void DeleteDirectory(string path) => BaseView.DeleteDirectory(path);

    /// <inheritdoc/>
    public virtual void DeleteDirectory(string path, bool recursive) => BaseView.DeleteDirectory(path, recursive);

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

    #endregion

    #region Paths

    /// <inheritdoc/>
    public virtual char DirectorySeparatorChar => BaseView.DirectorySeparatorChar;

    /// <inheritdoc/>
    public virtual StringComparer PathComparer => BaseView.PathComparer;

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public virtual string? GetFullPath(string? path) => BaseView.GetFullPath(path);

    #endregion

    /// <summary>
    /// Gets the base file system view.
    /// </summary>
    protected T BaseView { get; }
}

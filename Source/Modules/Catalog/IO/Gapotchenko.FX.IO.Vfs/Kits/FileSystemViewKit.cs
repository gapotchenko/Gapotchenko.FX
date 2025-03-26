// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Utils;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides a base implementation of <see cref="IFileSystemView"/> interface.
/// </summary>
/// <remarks>
/// This class provides a foundation for a virtual file system (VFS) implementation.
/// While it is not obligatory to use this class, doing so may significantly simplify a particular VFS implementation.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class FileSystemViewKit : IFileSystemView
{
    #region Capabilities

    /// <inheritdoc/>
    public abstract bool CanRead { get; }

    /// <inheritdoc/>
    public abstract bool CanWrite { get; }

    /// <summary>
    /// Ensures that the file system supports reading.
    /// </summary>
    /// <exception cref="NotSupportedException">File system does not support reading.</exception>
    protected void EnsureCanRead()
    {
        if (!CanRead)
            ThrowHelper.CannotReadFS();
    }

    /// <summary>
    /// Ensures that the file system supports writing.
    /// </summary>
    /// <exception cref="NotSupportedException">File system does not support writing.</exception>
    protected void EnsureCanWrite()
    {
        if (!CanWrite)
            ThrowHelper.CannotWriteFS();
    }

    /// <summary>
    /// Ensures that the file system can open a file with the specified mode and access.
    /// </summary>
    /// <exception cref="NotSupportedException">File system does not support reading.</exception>
    /// <exception cref="NotSupportedException">File system does not support writing.</exception>
    protected void EnsureCanOpenFile(FileMode mode, FileAccess access) =>
        FileSystemViewCapabilities.EnsureCanOpenFile(mode, access, CanRead, CanWrite);

    #endregion

    #region Files

    /// <inheritdoc/>
    public abstract bool FileExists([NotNullWhen(true)] string? path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateFiles(string path) => EnumerateFiles(path, "*");

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateFiles(string path, string searchPattern) =>
        EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);

    /// <inheritdoc/>
    public abstract IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);

    /// <inheritdoc/>
    public virtual Stream ReadFile(string path) =>
        OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);

    /// <inheritdoc/>
    public abstract Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);

    /// <inheritdoc/>
    public abstract void DeleteFile(string path);

    /// <inheritdoc/>
    public virtual void CopyFile(string sourcePath, string destinationPath, bool overwrite)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);

        IOHelper.CopyFileNaive(this, sourcePath, this, destinationPath, overwrite);
    }

    /// <inheritdoc/>
    public virtual void MoveFile(string sourcePath, string destinationPath, bool overwrite)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);

        IOHelper.MoveFileNaive(this, sourcePath, this, destinationPath, overwrite);
    }

    #endregion

    #region Directories

    /// <inheritdoc/>
    public abstract bool DirectoryExists([NotNullWhen(true)] string? path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path) => EnumerateDirectories(path, "*");

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateDirectories(string path, string searchPattern) =>
        EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);

    /// <inheritdoc/>
    public abstract IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption);

    /// <inheritdoc/>
    public abstract void CreateDirectory(string path);

    /// <inheritdoc/>
    public abstract void DeleteDirectory(string path, bool recursive);

    /// <inheritdoc/>
    public virtual void CopyDirectory(string sourcePath, string destinationPath, bool overwrite)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);

        IOHelper.CopyDirectoryNaive(this, sourcePath, this, destinationPath, overwrite);
    }

    /// <inheritdoc/>
    public virtual void MoveDirectory(string sourcePath, string destinationPath, bool overwrite)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);

        IOHelper.MoveDirectoryNaive(this, sourcePath, this, destinationPath, overwrite);
    }

    #endregion

    #region Entries

    /// <inheritdoc/>
    public virtual bool EntryExists([NotNullWhen(true)] string? path) => FileExists(path) || DirectoryExists(path);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path) =>
        EnumerateEntries(path, "*");

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path, string searchPattern) =>
        EnumerateEntries(path, searchPattern, SearchOption.TopDirectoryOnly);

    /// <inheritdoc/>
    public virtual IEnumerable<string> EnumerateEntries(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateFiles(path, searchPattern, searchOption)
        .Concat(EnumerateDirectories(path, searchPattern, searchOption));

    #endregion

    #region Paths

    /// <inheritdoc/>
    public virtual char DirectorySeparatorChar => '/';

    /// <inheritdoc/>
    public abstract StringComparer PathComparer { get; }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public string? GetFullPath(string? path)
    {
        if (path is null)
        {
            return null;
        }
        else if (path.Length == 0)
        {
            throw new ArgumentException(VfsResourceKit.PathIsEmpty, nameof(path));
        }
        else
        {
            string fullPath = GetFullPathCore(path);

            if (fullPath.Length != 0)
            {
                char directorySeparatorChar = DirectorySeparatorChar;
                if (fullPath[^1] != directorySeparatorChar &&
                    VfsPathKit.IsDirectorySeparator(path[^1], directorySeparatorChar))
                {
                    fullPath += directorySeparatorChar;
                }
            }

            return fullPath;
        }
    }

    /// <inheritdoc cref="GetFullPath(string?)"/>
    protected virtual string GetFullPathCore(string path)
    {
        string[] parts =
            VfsPathKit.Split(path) ??
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

        char directorySeparatorChar = DirectorySeparatorChar;
        return directorySeparatorChar + VfsPathKit.Join(parts, directorySeparatorChar);
    }

    /// <inheritdoc/>
    public virtual bool IsPathRooted(ReadOnlySpan<char> path) =>
        !path.IsEmpty &&
        VfsPathKit.IsDirectorySeparator(path[0], DirectorySeparatorChar);

    /// <inheritdoc/>
    public virtual string CombinePaths(params IEnumerable<string?> paths)
    {
        if (paths is null)
            throw new ArgumentNullException(nameof(paths));

        char directorySeparatorChar = DirectorySeparatorChar;
        var builder = new StringBuilder();

        foreach (string? path in paths)
        {
            if (string.IsNullOrEmpty(path))
                continue;

            if (IsPathRooted(path.AsSpan()))
            {
                builder.Clear();
            }
            else if (builder.Length != 0)
            {
                if (!VfsPathKit.IsDirectorySeparator(builder[^1], directorySeparatorChar) &&
                    !VfsPathKit.IsDirectorySeparator(path[0], directorySeparatorChar))
                {
                    builder.Append(directorySeparatorChar);
                }
            }

            builder.Append(path);
        }

        return builder.ToString();
    }

    #endregion
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
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

    /// <inheritdoc/>
    public virtual bool SupportsCreationTime => false;

    /// <inheritdoc/>
    public virtual bool SupportsLastWriteTime => false;

    /// <inheritdoc/>
    public virtual bool SupportsLastAccessTime => false;

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
        VfsCapabilitiesKit.EnsureCanOpenFile(mode, access, CanRead, CanWrite);

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
    public virtual void CopyFile(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);
        VfsValidationKit.Arguments.ValidateCopyOptions(options);

        IOHelper.CopyFileNaive(this, sourcePath, this, destinationPath, overwrite, options);
    }

    /// <inheritdoc/>
    public virtual void MoveFile(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);
        VfsValidationKit.Arguments.ValidateMoveOptions(options);

        IOHelper.MoveFileNaive(this, sourcePath, this, destinationPath, overwrite, options);
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
    public virtual void CopyDirectory(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);
        VfsValidationKit.Arguments.ValidateCopyOptions(options);

        IOHelper.CopyDirectoryNaive(this, sourcePath, this, destinationPath, overwrite, options);
    }

    /// <inheritdoc/>
    public virtual void MoveDirectory(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);
        VfsValidationKit.Arguments.ValidateMoveOptions(options);

        IOHelper.MoveDirectoryNaive(this, sourcePath, this, destinationPath, overwrite, options);
    }

    #endregion

    #region Entries

    /// <inheritdoc/>
    public virtual bool EntryExists([NotNullWhen(true)] string? path) =>
        FileExists(path) ||
        DirectoryExists(path);

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

    /// <inheritdoc/>
    public virtual DateTime GetCreationTime(string path) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual void SetCreationTime(string path, DateTime creationTime) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual DateTime GetLastWriteTime(string path) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual void SetLastWriteTime(string path, DateTime lastWriteTime) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual DateTime GetLastAccessTime(string path) => throw new NotSupportedException();

    /// <inheritdoc/>
    public virtual void SetLastAccessTime(string path, DateTime lastAccessTime) => throw new NotSupportedException();

    #endregion

    #region Paths

    /// <inheritdoc/>
    public virtual char DirectorySeparatorChar => VfsPathKit.DirectorySeparatorChar;

    /// <inheritdoc/>
    public virtual char AltDirectorySeparatorChar => VfsPathKit.AltDirectorySeparatorChar;

    /// <inheritdoc/>
    public abstract StringComparer PathComparer { get; }

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

    /// <inheritdoc/>
    public virtual string CombinePaths(params ReadOnlySpan<string?> paths)
    {
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
        char directorySeparatorChar = DirectorySeparatorChar;

        string[] parts =
            VfsPathKit.Split(path, directorySeparatorChar) ??
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindPartOfPath(path));

        return directorySeparatorChar + VfsPathKit.Join(parts, directorySeparatorChar);
    }

    /// <inheritdoc/>
    public virtual string? GetDirectoryName(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        else if (GetDirectoryName(path.AsSpan()) is var result && result != null)
            return result.ToString();
        else
            return null;
    }

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path) =>
        VfsPathKit.GetDirectoryName(path, DirectorySeparatorChar);

    /// <inheritdoc/>
    public virtual string? GetFileName(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        var result = GetFileName(path.AsSpan());
        if (path.Length == result.Length)
            return path;

        return result.ToString();
    }

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path) =>
        VfsPathKit.GetFileName(path, DirectorySeparatorChar);

    /// <inheritdoc/>
    public virtual bool IsPathRooted([NotNullWhen(true)] string? path) => IsPathRooted(path.AsSpan());

    /// <inheritdoc/>
    public virtual bool IsPathRooted(ReadOnlySpan<char> path) => !GetPathRoot(path).IsEmpty;

    /// <inheritdoc/>
    public virtual string? GetPathRoot(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        else if (GetPathRoot(path.AsSpan()) is var result && result != null)
            return result.ToString();
        else
            return null;
    }

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> GetPathRoot(ReadOnlySpan<char> path) =>
        VfsPathKit.GetPathRoot(path, DirectorySeparatorChar);

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(path))]
    public virtual string? TrimEndingDirectorySeparator(string? path) =>
        path != null && EndsInDirectorySeparator(path) && !IsRootPath(path.AsSpan()) ?
            path[..^1] :
            path;

    /// <inheritdoc/>
    public virtual ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) =>
        EndsInDirectorySeparator(path) && !IsRootPath(path) ?
            path[..^1] :
            path;

    bool IsRootPath(ReadOnlySpan<char> path) => path.Length == GetPathRoot(path).Length;

    /// <inheritdoc/>
    public virtual bool EndsInDirectorySeparator([NotNullWhen(true)] string? path) =>
        EndsInDirectorySeparator(path.AsSpan());

    /// <inheritdoc/>
    public virtual bool EndsInDirectorySeparator(ReadOnlySpan<char> path) =>
        path.Length > 0 &&
        VfsPathKit.IsDirectorySeparator(path[^1], DirectorySeparatorChar);

    #endregion
}

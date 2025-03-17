// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Utils;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides a base implementation of <see cref="IFileSystemView"/>.
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
            ThrowHelper.FSDoesNotSupportReading();
    }

    /// <summary>
    /// Ensures that the file system supports writing.
    /// </summary>
    /// <exception cref="NotSupportedException">File system does not support writing.</exception>
    protected void EnsureCanWrite()
    {
        if (!CanWrite)
            ThrowHelper.FSDoesNotSupportWriting();
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
    public virtual Stream OpenFileRead(string path) =>
        OpenFile(path, FileMode.Open, FileAccess.Read, FileShare.Read);

    /// <inheritdoc/>
    public abstract Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);

    /// <inheritdoc/>
    public abstract void DeleteFile(string path);

    /// <inheritdoc/>
    public void CopyFile(string sourcePath, string destinationPath, bool overwrite)
    {
        VfsValidationKit.Arguments.ValidatePath(sourcePath);
        VfsValidationKit.Arguments.ValidatePath(destinationPath);

        CopyFileCore(sourcePath, destinationPath, overwrite);
    }

    /// <inheritdoc cref="CopyFile(string, string, bool)"/>
    protected virtual void CopyFileCore(string sourcePath, string destinationPath, bool overwrite)
    {
        CopyFileToCore(sourcePath, this, destinationPath, overwrite);
    }

    void CopyFileToCore(string sourcePath, IFileSystemView destinationView, string destinationPath, bool overwrite)
    {
        using var sourceStream = OpenFileRead(sourcePath);
        using var destinationStream = destinationView.OpenFile(
            destinationPath,
            overwrite ? FileMode.Create : FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);

        sourceStream.CopyTo(destinationStream);
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
            return null;
        else if (path.Length == 0)
            throw new ArgumentException(VfsResourceKit.PathIsEmpty, nameof(path));
        else
            return GetFullPathCore(path);
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
        path[0] == DirectorySeparatorChar;

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
                if (!IsDirectorySeparator(builder[^1], directorySeparatorChar) &&
                    !IsDirectorySeparator(path[0], directorySeparatorChar))
                {
                    builder.Append(directorySeparatorChar);
                }
            }

            builder.Append(path);
        }

        return builder.ToString();
    }

    static bool IsDirectorySeparator(char c, char directorySeparatorChar) =>
        c == directorySeparatorChar ||
        c is '/' or '\\';

    #endregion
}

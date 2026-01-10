// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a view on virtual file system.
/// </summary>
public interface IFileSystemView : IReadOnlyFileSystemView
{
    #region Capabilities

    /// <summary>
    /// Gets a value indicating whether the current file system supports writing.
    /// </summary>
    bool CanWrite { get; }

    #endregion

    #region Files

    /// <summary>
    /// Opens a <see cref="Stream"/> on the specified path,
    /// having the specified mode with read, write, or read/write access and the specified sharing option.
    /// </summary>
    /// <param name="path">The path of a file to open.</param>
    /// <param name="mode">
    /// The <see cref="FileMode"/> value that specifies whether a file is created if one does not exist,
    /// and determines whether the contents of existing files are retained or overwritten.
    /// </param>
    /// <param name="access">The <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.</param>
    /// <param name="share">The <see cref="FileShare"/> value specifying the type of access other streams have to the file.</param>
    /// <returns>
    /// A <see cref="Stream"/> on the specified path,
    /// having the specified mode with read, write, or read/write access and the specified sharing option.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> does not contain a valid path.</exception>
    /// <exception cref="DirectoryNotFoundException"><paramref name="path"/> is invalid, such as referring to an unmapped drive.</exception>
    /// <exception cref="IOException">An I/O error occurred while working the file.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or combined exceed the file system-defined maximum length.</exception>
    /// <exception cref="UnauthorizedAccessException"><paramref name="path"/> is a directory name.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);

    /// <summary>
    /// Asynchronously opens a <see cref="Stream"/> on the specified path,
    /// having the specified mode with read, write, or read/write access and the specified sharing option.
    /// </summary>
    /// <inheritdoc cref="OpenFile(string, FileMode, FileAccess, FileShare)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="mode"><inheritdoc/></param>
    /// <param name="access"><inheritdoc/></param>
    /// <param name="share"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="path">
    /// The name of the file to be deleted.
    /// Wildcard characters are not supported.
    /// </param>
    /// <inheritdoc cref="File.Delete(string)"/>
    void DeleteFile(string path);

    /// <summary>
    /// Asynchronously deletes the specified file.
    /// </summary>
    /// <inheritdoc cref="DeleteFile(string)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous file deletion operation.</returns>
    Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies an existing file to a new file.
    /// Overwriting a file of the same name is controlled by the <paramref name="overwrite"/> parameter.
    /// Additional operation options are controlled by the <paramref name="options"/> parameter.
    /// </summary>
    /// <param name="sourcePath">The path of the file to copy.</param>
    /// <param name="destinationPath">
    /// The path of the destination file.
    /// This cannot be a directory.
    /// </param>
    /// <param name="overwrite">
    /// <see langword="true"/> if the destination file should be replaced if it already exists;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="options">The operation options.</param>
    void CopyFile(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options);

    /// <summary>
    /// Asynchronously copies an existing file to a new file.
    /// Overwriting a file of the same name is controlled by the <paramref name="overwrite"/> parameter.
    /// Additional operation options are controlled by the <paramref name="options"/> parameter.
    /// </summary>
    /// <inheritdoc cref="CopyFile(string, string, bool, VfsCopyOptions)"/>
    /// <param name="sourcePath"><inheritdoc/></param>
    /// <param name="destinationPath"><inheritdoc/></param>
    /// <param name="overwrite"><inheritdoc/></param>
    /// <param name="options"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous file copy operation.</returns>
    Task CopyFileAsync(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a specified file to a new location,
    /// providing the options to specify a new file name and
    /// to overwrite the destination file if it already exists.
    /// Additional operation options are controlled by the <paramref name="options"/> parameter.
    /// </summary>
    /// <param name="sourcePath">The path of the file to move.</param>
    /// <param name="destinationPath">The new path and name for the file.</param>
    /// <param name="overwrite">
    /// <see langword="true"/> to overwrite the destination file if it already exists;
    /// <see langword="false"/> otherwise.
    /// </param>
    /// <param name="options">The operation options.</param>
    void MoveFile(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options);

    #endregion

    #region Directories

    /// <returns/>
    /// <inheritdoc cref="Directory.CreateDirectory(string)"/>
    void CreateDirectory(string path);

    /// <summary>
    /// Asynchronously creates all directories and subdirectories in the specified path unless they already exist.
    /// </summary>
    /// <inheritdoc cref="CreateDirectory(string)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous directory creation operation.</returns>
    Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified directory and, if indicated, any subdirectories and files in the directory.
    /// </summary>
    /// <param name="path">The name of the directory to remove.</param>
    /// <param name="recursive">
    /// <see langword="true"/> to remove directories, subdirectories, and files in <paramref name="path"/>;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <inheritdoc cref="Directory.Delete(string, bool)"/>
    void DeleteDirectory(string path, bool recursive);

    /// <summary>
    /// Asynchronously deletes the specified directory and, if indicated, any subdirectories and files in the directory.
    /// </summary>
    /// <inheritdoc cref="DeleteDirectory(string, bool)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="recursive"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous directory deletion operation.</returns>
    Task DeleteDirectoryAsync(string path, bool recursive, CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies an existing directory to a new directory.
    /// Overwriting a directory of the same name is controlled by the <paramref name="overwrite"/> parameter.
    /// Additional operation options are controlled by the <paramref name="options"/> parameter.
    /// </summary>
    /// <param name="sourcePath">The path of the directory to copy.</param>
    /// <param name="destinationPath">The path of the destination directory.</param>
    /// <param name="overwrite">
    /// <see langword="true"/> if the destination directory should be overwritten if it already exists;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="options">The operation options.</param>
    void CopyDirectory(string sourcePath, string destinationPath, bool overwrite, VfsCopyOptions options);

    /// <summary>
    /// Moves a specified directory to a new location,
    /// providing the options to specify a new directory name and
    /// to overwrite the destination directory if it already exists.
    /// Additional operation options are controlled by the <paramref name="options"/> parameter.
    /// </summary>
    /// <param name="sourcePath">The path of the directory to move.</param>
    /// <param name="destinationPath">The new path and name for the directory.</param>
    /// <param name="overwrite">
    /// <see langword="true"/> to replace the destination directory if it already exists;
    /// <see langword="false"/> otherwise.
    /// </param>
    /// <param name="options">The operation options.</param>
    void MoveDirectory(string sourcePath, string destinationPath, bool overwrite, VfsMoveOptions options);

    #endregion

    #region Entries

    /// <summary>
    /// Sets the date and time when the specified file or directory was last written to.
    /// </summary>
    /// <inheritdoc cref="Directory.SetLastWriteTimeUtc(string, DateTime)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="lastWriteTime">
    /// The date and time the file or directory was last written to.
    /// If not otherwise specified, this value is expressed in UTC time.
    /// </param>
    void SetLastWriteTime(string path, DateTime lastWriteTime);

    /// <summary>
    /// Asynchronously sets the date and time when the specified file or directory was last written to.
    /// </summary>
    /// <inheritdoc cref="SetLastWriteTime(string, DateTime)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="lastWriteTime"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SetLastWriteTimeAsync(string path, DateTime lastWriteTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the date and time when the specified file or directory was created.
    /// </summary>
    /// <inheritdoc cref="Directory.SetCreationTimeUtc(string, DateTime)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="creationTime">
    /// An object that contains the value to set for the creation date and time of <paramref name="path"/>. 
    /// If not otherwise specified, this value is expressed in UTC time.
    /// </param>
    void SetCreationTime(string path, DateTime creationTime);

    /// <summary>
    /// Asynchronously sets the date and time when the specified file or directory was created.
    /// </summary>
    /// <inheritdoc cref="SetCreationTime(string, DateTime)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="creationTime"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SetCreationTimeAsync(string path, DateTime creationTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the date and time when the specified file or directory was last accessed.
    /// </summary>
    /// <inheritdoc cref="Directory.SetLastAccessTimeUtc(string, DateTime)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="lastAccessTime">
    /// An object that contains the value to set for the access date and time of <paramref name="path"/>. 
    /// If not otherwise specified, this value is expressed in UTC time.
    /// </param>
    void SetLastAccessTime(string path, DateTime lastAccessTime);

    /// <summary>
    /// Asynchronously sets the date and time when the specified file or directory was last accessed.
    /// </summary>
    /// <inheritdoc cref="SetLastAccessTime(string, DateTime)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="lastAccessTime"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SetLastAccessTimeAsync(string path, DateTime lastAccessTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the specified <see cref="FileAttributes"/> of the file or directory on the specified path.
    /// </summary>
    /// <param name="path">The file or directory for which to set the attribute information.</param>
    /// <param name="attributes">The attributes to set.</param>
    /// <inheritdoc cref="File.SetAttributes(string, FileAttributes)"/>
    void SetAttributes(string path, FileAttributes attributes);

    /// <summary>
    /// Asynchronously sets the specified <see cref="FileAttributes"/> of the file or directory on the specified path.
    /// </summary>
    /// <inheritdoc cref="SetAttributes(string, FileAttributes)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="attributes"><inheritdoc/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SetAttributesAsync(string path, FileAttributes attributes, CancellationToken cancellationToken = default);

    #endregion
}

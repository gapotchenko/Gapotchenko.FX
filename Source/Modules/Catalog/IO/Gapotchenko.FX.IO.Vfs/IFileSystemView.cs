// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a virtual file-system view.
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

    /// <inheritdoc cref="File.Open(string, FileMode, FileAccess, FileShare)"/>
    Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);

    /// <inheritdoc cref="File.Delete(string)"/>
    void DeleteFile(string path);

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

    /// <inheritdoc cref="Directory.Delete(string, bool)"/>
    void DeleteDirectory(string path, bool recursive);

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
    /// Sets the date and time that the specified file or directory was last written to.
    /// </summary>
    /// <inheritdoc cref="Directory.SetLastWriteTimeUtc(string, DateTime)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="lastWriteTime">
    /// The date and time the file or directory was last written to.
    /// If not otherwise specified, this value is expressed in UTC time.
    /// </param>
    void SetLastWriteTime(string path, DateTime lastWriteTime);

    /// <summary>
    /// Sets the date and time that the specified file or directory was created.
    /// </summary>
    /// <inheritdoc cref="Directory.SetCreationTimeUtc(string, DateTime)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="creationTime">
    /// An object that contains the value to set for the creation date and time of <paramref name="path"/>. 
    /// If not otherwise specified, this value is expressed in UTC time.
    /// </param>
    void SetCreationTime(string path, DateTime creationTime);

    /// <summary>
    /// Sets the date and time that the specified file or directory was last accessed.
    /// </summary>
    /// <inheritdoc cref="Directory.SetLastAccessTimeUtc(string, DateTime)"/>
    /// <param name="path"><inheritdoc/></param>
    /// <param name="lastAccessTime">
    /// An object that contains the value to set for the access date and time of <paramref name="path"/>. 
    /// If not otherwise specified, this value is expressed in UTC time.
    /// </param>
    void SetLastAccessTime(string path, DateTime lastAccessTime);

    #endregion
}

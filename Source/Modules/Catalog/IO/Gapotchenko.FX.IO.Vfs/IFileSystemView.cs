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
    /// <summary>
    /// Gets a value indicating whether the current file system supports writing.
    /// </summary>
    bool CanWrite { get; }

    #region Files

    /// <inheritdoc cref="File.Open(string, FileMode, FileAccess, FileShare)"/>
    Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);

    /// <inheritdoc cref="File.Delete(string)"/>
    void DeleteFile(string path);

    /// <summary>
    /// Copies an existing file to a new file.
    /// Overwriting a file of the same name is controlled by the <paramref name="overwrite"/> parameter.
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
    void CopyFile(string sourcePath, string destinationPath, bool overwrite);

    /// <summary>
    /// Moves a specified file to a new location,
    /// providing the options to specify a new file name and
    /// to overwrite the destination file if it already exists.
    /// </summary>
    /// <param name="sourcePath">The path of the file to move.</param>
    /// <param name="destinationPath">The new path and name for the file.</param>
    /// <param name="overwrite">
    /// <see langword="true"/> to overwrite the destination file if it already exists;
    /// <see langword="false"/> otherwise.
    /// </param>
    void MoveFile(string sourcePath, string destinationPath, bool overwrite);

    #endregion

    #region Directories

    /// <returns/>
    /// <inheritdoc cref="Directory.CreateDirectory(string)"/>
    void CreateDirectory(string path);

    /// <inheritdoc cref="Directory.Delete(string, bool)"/>
    void DeleteDirectory(string path, bool recursive);

    #endregion
}

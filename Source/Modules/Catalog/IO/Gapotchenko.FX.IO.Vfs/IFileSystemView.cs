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
    /// Copies an existing file from the specified source <see cref="IReadOnlyFileSystemView"/> to a new file in the current <see cref="IFileSystemView"/>.
    /// Overwriting a file of the same name is controlled by the <paramref name="overwrite"/> parameter.
    /// </summary>
    /// <param name="sourceView">The source <see cref="IReadOnlyFileSystemView"/> to copy the file from.</param>
    /// <param name="sourcePath">The path of the file to copy from the source <see cref="IReadOnlyFileSystemView"/>.</param>
    /// <param name="destinationPath">
    /// The path of the destination file in the current <see cref="IFileSystemView"/>.
    /// This cannot be a directory.
    /// </param>
    /// <param name="overwrite">
    /// <see langword="true"/> if the destination file should be replaced if it already exists;
    /// otherwise, <see langword="false"/>.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    void CopyFile(IReadOnlyFileSystemView sourceView, string sourcePath, string destinationPath, bool overwrite);

    #endregion

    #region Directories

    /// <returns/>
    /// <inheritdoc cref="Directory.CreateDirectory(string)"/>
    void CreateDirectory(string path);

    /// <inheritdoc cref="Directory.Delete(string, bool)"/>
    void DeleteDirectory(string path, bool recursive);

    #endregion
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Represents a virtual file system view.
/// </summary>
public interface IFileSystemView : IReadOnlyFileSystemView
{
    /// <summary>
    /// Gets a value indicating whether the current file system supports writing.
    /// </summary>
    bool CanWrite { get; }

    #region Files

    /// <inheritdoc cref="File.Delete(string)"/>
    void DeleteFile(string path);

    #endregion

    #region Directories

    /// <inheritdoc cref="Directory.Delete(string)"/>
    void DeleteDirectory(string path);

    /// <inheritdoc cref="Directory.Delete(string, bool)"/>
    void DeleteDirectory(string path, bool recursive);

    #endregion
}

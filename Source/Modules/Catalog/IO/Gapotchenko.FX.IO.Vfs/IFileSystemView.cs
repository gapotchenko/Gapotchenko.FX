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
    /// <inheritdoc cref="File.Delete(string)"/>
    void DeleteFile(string path);

    /// <inheritdoc cref="Directory.Delete(string)"/>
    void DeleteDirectory(string path);

    /// <inheritdoc cref="Directory.Delete(string, bool)"/>
    void DeleteDirectory(string path, bool recursive);
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides static methods for virtual file system views.
/// </summary>
public static class FileSystemView
{
    /// <summary>
    /// Gets the virtual file system view of the local file system.
    /// </summary>
    public static IFileSystemView Local => LocalFileSystemView.Instance;
}

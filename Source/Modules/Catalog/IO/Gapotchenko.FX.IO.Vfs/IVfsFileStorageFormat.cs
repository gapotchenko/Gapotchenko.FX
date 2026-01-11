// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides description and operations for the file-mountable virtual file system format.
/// </summary>
public interface IVfsFileStorageFormat : IVfsStorageFormat
{
    /// <summary>
    /// Gets the list of file extensions that are typically represent the data in this format.
    /// </summary>
    IReadOnlyList<string> FileExtensions { get; }
}

/// <summary>
/// Provides strongly typed description and operations for the file-mountable virtual file system format.
/// </summary>
/// <typeparam name="TVfs">The type of the virtual file system.</typeparam>
/// <typeparam name="TOptions">The type of the virtual file system options.</typeparam>
public interface IVfsFileStorageFormat<TVfs, TOptions> : IVfsFileStorageFormat, IVfsStorageFormat<TVfs, TOptions>
    where TVfs : IVirtualFileSystem
    where TOptions : VfsOptions
{
}

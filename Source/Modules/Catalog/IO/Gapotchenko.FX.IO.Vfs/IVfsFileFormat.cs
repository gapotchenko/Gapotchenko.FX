// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides description and operations for the file-mountable virtual file system format.
/// </summary>
public interface IVfsFileFormat : IVfsFormat
{
    /// <summary>
    /// Gets the list of file extensions that are used to store the virtual file system.
    /// </summary>
    IReadOnlyList<string> FileExtensions { get; }
}

/// <summary>
/// Provides strongly typed description and operations for the file-mountable virtual file system format.
/// </summary>
/// <typeparam name="TVfs">The type of the virtual file system.</typeparam>
/// <typeparam name="TOptions">The type of the virtual file system options.</typeparam>
public interface IVfsFileFormat<out TVfs, TOptions> : IVfsFileFormat, IVfsFormat<TVfs, TOptions>
    where TVfs : IVirtualFileSystem
    where TOptions : VfsOptions
{
}

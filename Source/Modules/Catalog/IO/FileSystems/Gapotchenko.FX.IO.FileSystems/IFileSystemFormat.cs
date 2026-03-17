// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.IO.FileSystems;

/// <summary>
/// Provides description and operations for the file system format.
/// </summary>
public interface IFileSystemFormat : IVfsFileStorageFormat
{
}

/// <summary>
/// Provides strongly typed description and operations for the file system format.
/// </summary>
/// <typeparam name="TFileSystem">The type of the file system.</typeparam>
/// <typeparam name="TOptions">The type of the file system options.</typeparam>
public interface IFileSystemFormat<TFileSystem, TOptions> :
    IFileSystemFormat,
    IVfsFileStorageFormat<TFileSystem, TOptions>
    where TFileSystem : IFileSystem
    where TOptions : FileSystemOptions
{
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.IO.FileSystems;

/// <summary>
/// Provides storage operations for working with <typeparamref name="TFileSystem"/> file systems.
/// </summary>
/// <typeparam name="TFileSystem">The type of the file system.</typeparam>
/// <typeparam name="TOptions">The type of the file system options.</typeparam>
public interface IFileSystemStorage<TFileSystem, TOptions> : IVfsStorage<TFileSystem, TOptions>
    where TFileSystem : IFileSystem
    where TOptions : FileSystemOptions
{
    /// <summary>
    /// Gets the storage format for the <typeparamref name="TFileSystem"/>.
    /// </summary>
    new IFileSystemFormat<TFileSystem, TOptions> Format { get; }
}

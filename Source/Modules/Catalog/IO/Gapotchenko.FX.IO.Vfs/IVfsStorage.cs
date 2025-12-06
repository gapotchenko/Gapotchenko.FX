// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides storage operations for working with <typeparamref name="TVfs"/> virtual file systems.
/// </summary>
/// <typeparam name="TVfs">The type of the virtual file system.</typeparam>
/// <typeparam name="TOptions">The type of the virtual file system options.</typeparam>
public interface IVfsStorage<out TVfs, TOptions>
    where TVfs : IVirtualFileSystem
    where TOptions : VfsOptions
{
    /// <summary>
    /// Gets the file format of the <typeparamref name="TVfs"/> storage.
    /// </summary>
    IVfsFileStorageFormat<TVfs, TOptions> Format { get; }
}

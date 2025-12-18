// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Defines the interface of a virtual file system mountable on a storage.
/// </summary>
public interface IStorageMountableVfs : IVirtualFileSystem
{
    /// <summary>
    /// Gets the file system location in the parent storage,
    /// or <see langword="null"/> if the location is unavailable.
    /// </summary>
    VfsReadOnlyLocation? Location { get; }
}

/// <summary>
/// Defines the interface of a virtual file system mountable on a storage.
/// </summary>
/// <typeparam name="TVfs">The type of the virtual file system.</typeparam>
/// <typeparam name="TOptions">The type of the virtual file system options.</typeparam>
public interface IStorageMountableVfs<out TVfs, TOptions> : IStorageMountableVfs
    where TVfs : IVirtualFileSystem
    where TOptions : StorageMountableVfsOptions
{
#if TFF_STATIC_INTERFACE
    /// <summary>
    /// Gets the object for <typeparamref name="TVfs"/> storage manipulation.
    /// </summary>
    static abstract IVfsStorage<TVfs, TOptions> Storage { get; }
#endif
}

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
/// <typeparam name="TVfs">The type of the virtual file system.</typeparam>
/// <typeparam name="TOptions">The type of the virtual file system options.</typeparam>
public interface IStorageMountableVfs<out TVfs, TOptions> : IVirtualFileSystem
    where TVfs : IVirtualFileSystem
    where TOptions : VfsOptions
{
#if TFF_STATIC_INTERFACE
    /// <summary>
    /// Gets the object for <typeparamref name="TVfs"/> storage manipulation.
    /// </summary>
    static abstract IVfsStorage<TVfs, TOptions> Storage { get; }
#endif
}

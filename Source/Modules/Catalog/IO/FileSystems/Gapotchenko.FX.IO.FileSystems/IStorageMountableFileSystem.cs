// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.IO.FileSystems;

/// <summary>
/// Defines the interface of a file system mountable on a storage.
/// </summary>
/// <typeparam name="TFileSystem">The type of the file system.</typeparam>
/// <typeparam name="TOptions">The type of the file system options.</typeparam>
public interface IStorageMountableFileSystem<TFileSystem, TOptions> :
    IFileSystem,
    IStorageMountableVfs<TFileSystem, TOptions>
    where TFileSystem : IFileSystem
    where TOptions : FileSystemOptions
{
#if TFF_STATIC_INTERFACE
    /// <summary>
    /// Gets the object for <typeparamref name="TFileSystem"/> storage manipulation.
    /// </summary>
    static new abstract IFileSystemStorage<TFileSystem, TOptions> Storage { get; }
#endif
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.IO.FileSystems.Kits;

/// <summary>
/// Provides the base implementation of <see cref="IFileSystemFormat{TFileSystem, TOptions}"/> interface.
/// </summary>
/// <typeparam name="TFileSystem">The type of the file system.</typeparam>
/// <typeparam name="TOptions">The type of the file system options.</typeparam>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class FileSystemFormatKit<TFileSystem, TOptions> :
    VfsFileStorageFormatKit<TFileSystem, TOptions>,
    IFileSystemFormat<TFileSystem, TOptions>
    where TFileSystem : IFileSystem
    where TOptions : FileSystemOptions
{
}

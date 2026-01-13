// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides the base implementation of <see cref="IVfsFileStorageFormat{TVfs, TOptions}"/> interface.
/// </summary>
/// <remarks/>
/// <inheritdoc/>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class VfsFileStorageFormatKit<TVfs, TOptions> :
    VfsStorageFormatKit<TVfs, TOptions>,
    IVfsFileStorageFormat<TVfs, TOptions>
    where TVfs : IVirtualFileSystem
    where TOptions : VfsOptions
{
    /// <inheritdoc/>
    public IReadOnlyList<string> FileExtensions => field ??= GetFileExtensionsCore();

    /// <inheritdoc cref="FileExtensions"/>
    protected abstract IReadOnlyList<string> GetFileExtensionsCore();
}

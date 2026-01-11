// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <inheritdoc/>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class VirtualFileSystemProxyKit : VirtualFileSystemProxyKit<IVirtualFileSystem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualFileSystemProxyKit"/> class with the specified base file system.
    /// </summary>
    /// <inheritdoc/>
    protected VirtualFileSystemProxyKit(IVirtualFileSystem baseView) :
        base(baseView)
    {
    }
}

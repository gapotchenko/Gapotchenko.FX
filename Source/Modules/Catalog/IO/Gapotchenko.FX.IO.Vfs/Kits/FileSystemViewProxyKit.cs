// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <inheritdoc/>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class FileSystemViewProxyKit : FileSystemViewProxyKit<IFileSystemView>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemViewProxyKit"/> class with the specified base stream.
    /// </summary>
    /// <inheritdoc/>
    protected FileSystemViewProxyKit(IFileSystemView baseView) :
        base(baseView)
    {
    }
}

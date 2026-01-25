// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.Data.Archives.Zip;

/// <summary>
/// Represents a package of compressed files in the ZIP archive format.
/// </summary>
public sealed partial class ZipArchive :
    VirtualFileSystemProxyKit<IZipArchive>,
    IZipArchive,
    IStorageMountableDataArchive<IZipArchive, ZipArchiveOptions>
{
    /// <inheritdoc/>
    public VfsReadOnlyLocation? Location { get; }
}

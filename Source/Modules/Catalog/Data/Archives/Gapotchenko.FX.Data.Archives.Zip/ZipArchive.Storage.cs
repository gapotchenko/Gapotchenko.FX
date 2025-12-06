// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives.Zip;

partial class ZipArchive
{
    /// <summary>
    /// Gets the object for ZIP files manipulation.
    /// </summary>
    public static IDataArchiveStorage<IZipArchive, ZipArchiveOptions> Storage => ZipArchiveStorage.Instance;

#if TFF_STATIC_INTERFACE
    static IVfsStorage<IZipArchive, ZipArchiveOptions> IStorageMountableVfs<IZipArchive, ZipArchiveOptions>.Storage => Storage;
#endif
}

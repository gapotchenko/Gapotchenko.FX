// Gapotchenko.FX
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
    public static IDataArchiveFile<IZipArchive, ZipArchiveOptions> File => ZipArchiveFile.Instance;

#if TFF_STATIC_INTERFACE
    static IVfsFile<IZipArchive, ZipArchiveOptions> IFileVfs<IZipArchive, ZipArchiveOptions>.File => File;
#endif
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives.Zip;

sealed class ZipArchiveFile : IDataArchiveFile<IZipArchive, ZipArchiveOptions>
{
    public static ZipArchiveFile Instance { get; } = new();

    IVfsFileFormat<IZipArchive, ZipArchiveOptions> IVfsFile<IZipArchive, ZipArchiveOptions>.Format => Format;

    public IDataArchiveFileFormat<IZipArchive, ZipArchiveOptions> Format => ZipArchiveFileFormat.Instance;
}

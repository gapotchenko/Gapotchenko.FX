// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives.Zip;

sealed class ZipFile : IDataArchiveFile<IZipArchive, ZipArchiveOptions>
{
    public static ZipFile Instance { get; } = new();

    public IDataArchiveFileFormat<IZipArchive, ZipArchiveOptions> Format => throw new NotImplementedException();

    IVfsFileFormat<IZipArchive, ZipArchiveOptions> IVfsFile<IZipArchive, ZipArchiveOptions>.Format => Format;
}

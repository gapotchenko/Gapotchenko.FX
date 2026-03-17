// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;

namespace Gapotchenko.FX.Data.Archives.Zip;

sealed class ZipArchiveStorage : IDataArchiveStorage<IZipArchive, ZipArchiveOptions>
{
    public static ZipArchiveStorage Instance { get; } = new();

    public IDataArchiveFormat<IZipArchive, ZipArchiveOptions> Format => ZipArchiveFormat.Instance;

    IVfsFileStorageFormat<IZipArchive, ZipArchiveOptions> IVfsStorage<IZipArchive, ZipArchiveOptions>.Format => Format;
}

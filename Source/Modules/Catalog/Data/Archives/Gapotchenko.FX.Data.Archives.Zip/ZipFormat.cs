// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Data.Archives.Kits;

namespace Gapotchenko.FX.Data.Archives.Zip;

sealed class ZipFormat : DataArchiveFileFormatKit<IZipArchive, ZipArchiveOptions>
{
    public static ZipFormat Instance { get; } = new();

    protected override string[] FileExtensionsCore => [".zip"];

    protected override IZipArchive CreateCore(Stream stream, bool leaveOpen, ZipArchiveOptions? options)
    {
        stream.SetLength(0);
        return new ZipArchive(stream, true, leaveOpen);
    }

    protected override IZipArchive MountCore(Stream stream, bool writable, bool leaveOpen, ZipArchiveOptions? options)
    {
        return new ZipArchive(stream, writable, leaveOpen);
    }

    protected override bool IsMountableCore(Stream stream, ZipArchiveOptions? options)
    {
        throw new NotImplementedException();
    }
}

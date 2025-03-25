// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Data.Archives.Kits;
using Gapotchenko.FX.IO;

namespace Gapotchenko.FX.Data.Archives.Zip;

sealed class ZipFormat : DataArchiveFileFormatKit<IZipArchive, ZipArchiveOptions>
{
    public static ZipFormat Instance { get; } = new();

    protected override string[] FileExtensionsCore => [".zip"];

    protected override IZipArchive CreateCore(Stream stream, bool leaveOpen, ZipArchiveOptions? options)
    {
        stream.SetLength(0);
        return new ZipArchive(stream, true, leaveOpen, options);
    }

    protected override IZipArchive MountCore(Stream stream, bool writable, bool leaveOpen, ZipArchiveOptions? options)
    {
        return new ZipArchive(stream, writable, leaveOpen, options);
    }

    protected override bool IsMountableCore(Stream stream, ZipArchiveOptions? options)
    {
        // This is a simplistic implementation which does not cover self-extracting archives:
        // https://stackoverflow.com/a/1887113

        const int n = 4;

        Span<byte> buffer = stackalloc byte[n];
        int bytesRead = stream.ReadAtLeast(buffer, n, false);
        if (bytesRead != n)
            return false;

        // https://www.loc.gov/preservation/digital/formats/fdd/fdd000354.shtml

        if (!buffer[..2].SequenceEqual<byte>([0x50, 0x4b]))
            return false;

        buffer = buffer[2..];

        return
            buffer.SequenceEqual<byte>([0x03, 0x04]) ||
            buffer.SequenceEqual<byte>([0x05, 0x06]); // end of central directory record
    }
}

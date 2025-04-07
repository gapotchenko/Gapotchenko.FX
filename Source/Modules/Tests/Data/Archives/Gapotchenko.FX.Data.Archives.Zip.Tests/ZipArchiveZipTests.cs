﻿using Gapotchenko.FX.Data.Archives.Zip.Tests.Kits;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests;

[TestClass]
public sealed class ZipArchiveZipTests : ZipArchiveTestsKit
{
    protected override IZipArchive Mount(Stream stream) => new ZipArchive(stream, stream.CanWrite);

    protected override IDataArchiveFormat<IZipArchive, ZipArchiveOptions> Format => ZipArchive.File.Format;
}

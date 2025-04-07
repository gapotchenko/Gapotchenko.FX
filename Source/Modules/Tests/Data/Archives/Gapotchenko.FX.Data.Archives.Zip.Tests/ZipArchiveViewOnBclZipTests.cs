using Gapotchenko.FX.Data.Archives.Zip.Tests.Kits;
using System.IO.Compression;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests;

[TestClass]
[TestCategory("bcl")]
public sealed class ZipArchiveViewOnBclZipTests : ZipArchiveTestsKit
{
    protected override IZipArchive Mount(Stream stream) =>
        ZipArchive.CreateView(
            new System.IO.Compression.ZipArchive(
                stream,
                stream.CanWrite ? ZipArchiveMode.Update : ZipArchiveMode.Read));
}

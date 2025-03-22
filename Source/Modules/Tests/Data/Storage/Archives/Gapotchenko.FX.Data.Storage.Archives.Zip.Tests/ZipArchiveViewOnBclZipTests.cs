using System.IO.Compression;

namespace Gapotchenko.FX.Data.Storage.Archives.Zip.Tests;

[TestClass]
[TestCategory("bcl")]
public sealed class ZipArchiveViewOnBclZipTests : ZipArchiveTests
{
    protected override IZipArchive Mount(Stream stream) =>
        ZipArchive.CreateView(
            new System.IO.Compression.ZipArchive(
                stream,
                stream.CanWrite ? ZipArchiveMode.Update : ZipArchiveMode.Read));
}

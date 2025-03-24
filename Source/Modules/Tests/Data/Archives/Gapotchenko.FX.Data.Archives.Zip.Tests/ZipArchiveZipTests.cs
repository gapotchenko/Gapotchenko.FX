namespace Gapotchenko.FX.Data.Archives.Zip.Tests;

[TestClass]
public sealed class ZipArchiveZipTests : ZipArchiveTests
{
    protected override IZipArchive Mount(Stream stream) => new ZipArchive(stream, stream.CanWrite);
}

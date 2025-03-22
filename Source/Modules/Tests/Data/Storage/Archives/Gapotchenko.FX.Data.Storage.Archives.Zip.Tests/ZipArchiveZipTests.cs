namespace Gapotchenko.FX.Data.Storage.Archives.Zip.Tests;

[TestClass]
public sealed class ZipArchiveZipTests : ZipArchiveTests
{
    protected override IZipArchive Mount(Stream stream) => new ZipArchive(stream, stream.CanWrite);
}

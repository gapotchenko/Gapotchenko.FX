namespace Gapotchenko.FX.Data.Compression.Zip.Tests;

[TestClass]
public class ZipArchiveTests
{
    [TestMethod]
    public void Zip_Empty()
    {
        using var archive = new ZipArchive(Assets.OpenStream("Empty.zip"));

        Assert.IsFalse(archive.EnumerateDirectories(".").Any());
        Assert.IsFalse(archive.EnumerateFiles(".").Any());
        Assert.IsFalse(archive.EnumerateEntries(".").Any());
    }
}

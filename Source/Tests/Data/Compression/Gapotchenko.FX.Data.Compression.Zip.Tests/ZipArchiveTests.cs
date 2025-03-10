namespace Gapotchenko.FX.Data.Compression.Zip.Tests;

[TestClass]
public class ZipArchiveTests
{
    [TestMethod]
    public void Zip_Empty()
    {
        using var archive = new ZipArchive(Assets.OpenStream("Empty.zip"));

        Assert.IsFalse(archive.EnumerateDirectories("/").Any());
        Assert.IsFalse(archive.EnumerateFiles("/").Any());
        Assert.IsFalse(archive.EnumerateEntries("/").Any());

        Assert.IsFalse(archive.DirectoryExists(""));

        Assert.IsTrue(archive.DirectoryExists("/"));
        Assert.IsTrue(archive.DirectoryExists("\\"));
        Assert.IsTrue(archive.DirectoryExists("."));

        Assert.IsFalse(archive.FileExists("/"));
        Assert.IsFalse(archive.FileExists("\\"));
        Assert.IsFalse(archive.FileExists("."));
    }

    [TestMethod]
    public void Zip_EmptyDirectory()
    {
        using var archive = new ZipArchive(Assets.OpenStream("EmptyDirectory.zip"));

        Assert.IsTrue(archive.EnumerateDirectories("/").SequenceEqual(["/Empty"]));
        Assert.IsFalse(archive.EnumerateFiles("/").Any());
        Assert.IsTrue(archive.EnumerateEntries("/").SequenceEqual(["/Empty"]));

        foreach (string prefix in new[] { "/", "", "./", "\\", ".\\" })
        {
            string path = prefix + "Empty";
            Assert.IsTrue(archive.DirectoryExists(path));
            Assert.IsFalse(archive.FileExists(path));
            Assert.IsTrue(archive.EntryExists(path));
        }
    }

    [TestMethod]
    public void Zip_NestedEmptyDirectory()
    {
        using var archive = new ZipArchive(Assets.OpenStream("NestedEmptyDirectory.zip"));

        Assert.IsTrue(archive.EnumerateDirectories("/").SequenceEqual(["/Container"]));
        Assert.IsFalse(archive.EnumerateFiles("/").Any());
        Assert.IsTrue(archive.EnumerateEntries("/").SequenceEqual(["/Container"]));

        Assert.IsTrue(archive.EnumerateDirectories("Container").SequenceEqual(["/Container/Empty"]));
        Assert.IsFalse(archive.EnumerateFiles("Container").Any());
        Assert.IsTrue(archive.EnumerateEntries("Container").SequenceEqual(["/Container/Empty"]));

        Assert.IsTrue(archive.DirectoryExists("/Container/Empty"));
        Assert.IsFalse(archive.FileExists("/Container/Empty"));
        Assert.IsTrue(archive.EntryExists("/Container/Empty"));
    }
}

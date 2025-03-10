using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.Linq;

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

        Assert.ThrowsException<FileNotFoundException>(() => archive.OpenTextFile("File.txt"));
        Assert.ThrowsException<DirectoryNotFoundException>(() => archive.OpenTextFile("Directory/File.txt"));
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
    public void Zip_EmptyNestedDirectory()
    {
        using var archive = new ZipArchive(Assets.OpenStream("EmptyNestedDirectory.zip"));

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

    [TestMethod]
    public void Zip_OneFile()
    {
        using var archive = new ZipArchive(Assets.OpenStream("OneFile.zip"));

        Assert.IsFalse(archive.EnumerateDirectories("/").Any());
        Assert.IsTrue(archive.EnumerateFiles("/").SequenceEqual(["/1.txt"]));
        Assert.IsTrue(archive.EnumerateEntries("/").SequenceEqual(["/1.txt"]));

        Assert.AreEqual("The first file.", archive.ReadAllTextFromFile("1.txt"));
    }

    [TestMethod]
    public void Zip_TwoFiles()
    {
        using var archive = new ZipArchive(Assets.OpenStream("TwoFiles.zip"));

        Assert.IsTrue(archive.EnumerateFiles("/").Order().SequenceEqual(["/1.txt", "/2.txt"]));

        Assert.AreEqual("The first file.", archive.ReadAllTextFromFile("1.txt"));
        Assert.AreEqual("The second file.", archive.ReadAllTextFromFile("2.txt"));
        Assert.ThrowsException<FileNotFoundException>(() => archive.ReadAllTextFromFile("3.txt"));
    }
}

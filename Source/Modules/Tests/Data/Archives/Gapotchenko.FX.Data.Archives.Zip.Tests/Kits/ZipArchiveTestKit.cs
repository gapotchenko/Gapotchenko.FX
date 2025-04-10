// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.Linq;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests.Kits;

[TestCategory("zip")]
public abstract partial class ZipArchiveTestKit
{
    protected abstract IZipArchive Mount(Stream stream);

    [TestMethod]
    public void Zip_Capabilities()
    {
        using (var archive = Mount(Assets.OpenStream("Empty.zip")))
        {
            Assert.IsTrue(archive.CanRead);
            Assert.IsFalse(archive.CanWrite);
        }

        using (var archive = Mount(Assets.OpenStream("Empty.zip", true)))
        {
            Assert.IsTrue(archive.CanRead);
            Assert.IsTrue(archive.CanWrite);
        }
    }

    [TestMethod]
    public void Zip_Empty()
    {
        using var archive = Mount(Assets.OpenStream("Empty.zip"));

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

        Assert.ThrowsException<FileNotFoundException>(() => archive.ReadTextFile("File.txt"));
        Assert.ThrowsException<DirectoryNotFoundException>(() => archive.ReadTextFile("Directory/File.txt"));
    }

    [TestMethod]
    public void Zip_EmptyDirectory()
    {
        using var archive = Mount(Assets.OpenStream("EmptyDirectory.zip"));

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
        using var archive = Mount(Assets.OpenStream("EmptyNestedDirectory.zip"));

        Assert.IsTrue(archive.EnumerateDirectories("/").SequenceEqual(["/Container"]));
        Assert.IsFalse(archive.EnumerateFiles("/").Any());
        Assert.IsTrue(archive.EnumerateEntries("/").SequenceEqual(["/Container"]));

        Assert.IsTrue(archive.EnumerateDirectories("Container").SequenceEqual(["Container/Empty"]));
        Assert.IsFalse(archive.EnumerateFiles("Container").Any());
        Assert.IsTrue(archive.EnumerateEntries("Container").SequenceEqual(["Container/Empty"]));

        Assert.IsTrue(archive.DirectoryExists("/Container/Empty"));
        Assert.IsFalse(archive.FileExists("/Container/Empty"));
        Assert.IsTrue(archive.EntryExists("/Container/Empty"));
    }

    [TestMethod]
    public void Zip_OneFile()
    {
        using var archive = Mount(Assets.OpenStream("OneFile.zip"));

        Assert.IsFalse(archive.EnumerateDirectories("/").Any());
        Assert.IsTrue(archive.EnumerateFiles("/").SequenceEqual(["/1.txt"]));
        Assert.IsTrue(archive.EnumerateEntries("/").SequenceEqual(["/1.txt"]));

        Assert.AreEqual("The first file.", archive.ReadAllFileText("1.txt"));
    }

    [TestMethod]
    public void Zip_TwoFiles()
    {
        using var archive = Mount(Assets.OpenStream("TwoFiles.zip"));

        Assert.IsTrue(archive.EnumerateFiles("/").Order().SequenceEqual(["/1.txt", "/2.txt"]));

        Assert.AreEqual("The first file.", archive.ReadAllFileText("1.txt"));
        Assert.AreEqual("The second file.", archive.ReadAllFileText("2.txt"));
        Assert.ThrowsException<FileNotFoundException>(() => archive.ReadAllFileText("3.txt"));
    }

    [TestMethod]
    public void Zip_TwoNestedFiles()
    {
        using var archive = Mount(Assets.OpenStream("TwoNestedFiles.zip"));

        Assert.IsFalse(archive.EnumerateFiles("/").Any());
        Assert.IsTrue(archive.EnumerateFiles("/", "*", SearchOption.AllDirectories).Order().SequenceEqual(["/Container/1.txt", "/Container/2.txt"]));
        Assert.IsTrue(archive.EnumerateFiles("/Container").Order().SequenceEqual(["/Container/1.txt", "/Container/2.txt"]));

        Assert.AreEqual("The first file.", archive.ReadAllFileText("Container/1.txt"));
        Assert.AreEqual("The second file.", archive.ReadAllFileText("Container/2.txt"));
        Assert.ThrowsException<FileNotFoundException>(() => archive.ReadAllFileText("Container/3.txt"));

        Assert.ThrowsException<FileNotFoundException>(() => archive.ReadAllFileText("1.txt"));
        Assert.ThrowsException<FileNotFoundException>(() => archive.ReadAllFileText("2.txt"));
        Assert.ThrowsException<FileNotFoundException>(() => archive.ReadAllFileText("3.txt"));

        Assert.ThrowsException<DirectoryNotFoundException>(() => archive.ReadAllFileText("Other/1.txt"));
        Assert.ThrowsException<DirectoryNotFoundException>(() => archive.ReadAllFileText("Other/2.txt"));
        Assert.ThrowsException<DirectoryNotFoundException>(() => archive.ReadAllFileText("Other/3.txt"));
    }

    [TestMethod]
    public void Zip_DeleteOneFile()
    {
        using var archive = Mount(Assets.OpenStream("OneFile.zip", true));

        string path = "1.txt";

        Assert.IsTrue(archive.FileExists(path));
        archive.DeleteFile(path);
        Assert.IsFalse(archive.FileExists(path));

        Assert.ThrowsException<FileNotFoundException>(() => archive.DeleteFile(path));
    }

    [TestMethod]
    public void Zip_DeleteOneNestedFile()
    {
        using var archive = Mount(Assets.OpenStream("OneNestedFile.zip", true));

        string path = "Container/1.txt";

        Assert.IsTrue(archive.FileExists(path));
        archive.DeleteFile(path);
        Assert.IsFalse(archive.FileExists(path));
        Assert.IsTrue(archive.DirectoryExists(archive.GetDirectoryName(path)));

        Assert.ThrowsException<FileNotFoundException>(() => archive.DeleteFile(path));
    }

    [TestMethod]
    public void Zip_DeleteEmptyDirectory()
    {
        using var archive = Mount(Assets.OpenStream("EmptyDirectory.zip", true));

        string path = "Empty";

        Assert.IsTrue(archive.DirectoryExists(path));
        archive.DeleteDirectory(path);
        Assert.IsFalse(archive.DirectoryExists(path));

        Assert.ThrowsException<DirectoryNotFoundException>(() => archive.DeleteDirectory(path));
    }

    [TestMethod]
    public void Zip_DeleteOneFileDirectory()
    {
        using var archive = Mount(Assets.OpenStream("OneNestedFile.zip", true));

        string path = "Container";

        Assert.IsTrue(archive.DirectoryExists(path));
        Assert.ThrowsException<IOException>(() => archive.DeleteDirectory(path));
        Assert.ThrowsException<IOException>(() => archive.DeleteDirectory(path, false));

        archive.DeleteDirectory(path, true);

        Assert.IsFalse(archive.DirectoryExists(path));
        Assert.ThrowsException<DirectoryNotFoundException>(() => archive.DeleteDirectory(path));
        Assert.ThrowsException<DirectoryNotFoundException>(() => archive.DeleteDirectory(path, true));
    }
}

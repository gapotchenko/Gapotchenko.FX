// Gapotchenko.FX
//
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

        string ds = $"{archive.DirectorySeparatorChar}";
        string ads = $"{archive.AltDirectorySeparatorChar}";

        Assert.IsFalse(archive.EnumerateDirectories(ds).Any());
        Assert.IsFalse(archive.EnumerateFiles(ds).Any());
        Assert.IsFalse(archive.EnumerateEntries(ds).Any());

        Assert.IsFalse(archive.DirectoryExists(""));

        Assert.IsTrue(archive.DirectoryExists(ds));
        Assert.IsTrue(archive.DirectoryExists(ads));
        Assert.IsTrue(archive.DirectoryExists("."));

        Assert.IsFalse(archive.FileExists(ds));
        Assert.IsFalse(archive.FileExists(ads));
        Assert.IsFalse(archive.FileExists("."));

        Assert.ThrowsExactly<FileNotFoundException>(() => archive.ReadTextFile("File.txt"));
        Assert.ThrowsExactly<DirectoryNotFoundException>(() => archive.ReadTextFile(archive.JoinPaths("Directory", "File.txt")));
    }

    [TestMethod]
    public void Zip_EmptyDirectory()
    {
        using var archive = Mount(Assets.OpenStream("EmptyDirectory.zip"));

        string ds = $"{archive.DirectorySeparatorChar}";
        string ads = $"{archive.AltDirectorySeparatorChar}";

        string emptyPath = archive.JoinPaths(ds, "Empty");
        Assert.IsTrue(archive.EnumerateDirectories(ds).SequenceEqual([emptyPath]));
        Assert.IsFalse(archive.EnumerateFiles(ds).Any());
        Assert.IsTrue(archive.EnumerateEntries(ds).SequenceEqual([emptyPath]));

        foreach (string prefix in new[] { ds, "", $".{ds}", ads, $".{ads}" })
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

        string ds = $"{archive.DirectorySeparatorChar}";

        string containerPath = archive.JoinPaths(ds, "Container");
        Assert.IsTrue(archive.EnumerateDirectories(ds).SequenceEqual([containerPath]));
        Assert.IsFalse(archive.EnumerateFiles(ds).Any());
        Assert.IsTrue(archive.EnumerateEntries(ds).SequenceEqual([containerPath]));

        string emptyPath = archive.JoinPaths(containerPath, "Empty");
        Assert.IsTrue(archive.EnumerateDirectories(containerPath).SequenceEqual([emptyPath]));
        Assert.IsFalse(archive.EnumerateFiles(containerPath).Any());
        Assert.IsTrue(archive.EnumerateEntries(containerPath).SequenceEqual([emptyPath]));

        Assert.IsTrue(archive.DirectoryExists(emptyPath));
        Assert.IsFalse(archive.FileExists(emptyPath));
        Assert.IsTrue(archive.EntryExists(emptyPath));
    }

    [TestMethod]
    public void Zip_OneFile()
    {
        using var archive = Mount(Assets.OpenStream("OneFile.zip"));

        string ds = $"{archive.DirectorySeparatorChar}";

        string filePath = archive.JoinPaths(ds, "1.txt");

        Assert.IsFalse(archive.EnumerateDirectories(ds).Any());
        Assert.IsTrue(archive.EnumerateFiles(ds).SequenceEqual([filePath]));
        Assert.IsTrue(archive.EnumerateEntries(ds).SequenceEqual([filePath]));

        Assert.AreEqual("The first file.", archive.ReadAllFileText(filePath));
    }

    [TestMethod]
    public void Zip_TwoFiles()
    {
        using var archive = Mount(Assets.OpenStream("TwoFiles.zip"));

        string ds = $"{archive.DirectorySeparatorChar}";

        string filePath1 = archive.JoinPaths(ds, "1.txt");
        string filePath2 = archive.JoinPaths(ds, "2.txt");

        Assert.IsTrue(archive.EnumerateFiles(ds).Order().SequenceEqual([filePath1, filePath2]));

        Assert.AreEqual("The first file.", archive.ReadAllFileText(filePath1));
        Assert.AreEqual("The second file.", archive.ReadAllFileText(filePath2));

        Assert.ThrowsExactly<FileNotFoundException>(() => archive.ReadAllFileText(archive.JoinPaths(ds, "3.txt")));
    }

    [TestMethod]
    public void Zip_TwoNestedFiles()
    {
        using var archive = Mount(Assets.OpenStream("TwoNestedFiles.zip"));

        string ds = $"{archive.DirectorySeparatorChar}";

        string containerPath = archive.JoinPaths(ds, "Container");
        string filePath1 = archive.JoinPaths(containerPath, "1.txt");
        string filePath2 = archive.JoinPaths(containerPath, "2.txt");

        Assert.IsFalse(archive.EnumerateFiles(ds).Any());
        Assert.IsTrue(archive.EnumerateFiles(ds, "*", SearchOption.AllDirectories).Order().SequenceEqual([filePath1, filePath2]));
        Assert.IsTrue(archive.EnumerateFiles(containerPath).Order().SequenceEqual([filePath1, filePath2]));

        Assert.AreEqual("The first file.", archive.ReadAllFileText(filePath1));
        Assert.AreEqual("The second file.", archive.ReadAllFileText(filePath2));
        Assert.ThrowsExactly<FileNotFoundException>(() => archive.ReadAllFileText(archive.JoinPaths(ds, "Container", "3.txt")));

        Assert.ThrowsExactly<FileNotFoundException>(() => archive.ReadAllFileText(archive.JoinPaths(ds, "1.txt")));
        Assert.ThrowsExactly<FileNotFoundException>(() => archive.ReadAllFileText(archive.JoinPaths(ds, "2.txt")));
        Assert.ThrowsExactly<FileNotFoundException>(() => archive.ReadAllFileText(archive.JoinPaths(ds, "3.txt")));

        Assert.ThrowsExactly<DirectoryNotFoundException>(() => archive.ReadAllFileText(archive.JoinPaths(ds, "Other", "1.txt")));
        Assert.ThrowsExactly<DirectoryNotFoundException>(() => archive.ReadAllFileText(archive.JoinPaths(ds, "Other", "2.txt")));
        Assert.ThrowsExactly<DirectoryNotFoundException>(() => archive.ReadAllFileText(archive.JoinPaths(ds, "Other", "3.txt")));
    }

    [TestMethod]
    public void Zip_DeleteOneFile()
    {
        using var archive = Mount(Assets.OpenStream("OneFile.zip", true));

        string filePath = "1.txt";

        Assert.IsTrue(archive.FileExists(filePath));
        archive.DeleteFile(filePath);
        Assert.IsFalse(archive.FileExists(filePath));

        Assert.ThrowsExactly<FileNotFoundException>(() => archive.DeleteFile(filePath));
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

        Assert.ThrowsExactly<FileNotFoundException>(() => archive.DeleteFile(path));
    }

    [TestMethod]
    public void Zip_DeleteEmptyDirectory()
    {
        using var archive = Mount(Assets.OpenStream("EmptyDirectory.zip", true));

        string directoryPath = "Empty";

        Assert.IsTrue(archive.DirectoryExists(directoryPath));
        archive.DeleteDirectory(directoryPath);
        Assert.IsFalse(archive.DirectoryExists(directoryPath));

        Assert.ThrowsExactly<DirectoryNotFoundException>(() => archive.DeleteDirectory(directoryPath));
    }

    [TestMethod]
    public void Zip_DeleteOneFileDirectory()
    {
        using var archive = Mount(Assets.OpenStream("OneNestedFile.zip", true));

        string direcotyPath = "Container";

        Assert.IsTrue(archive.DirectoryExists(direcotyPath));

        Assert.ThrowsExactly<IOException>(() => archive.DeleteDirectory(direcotyPath));
        Assert.ThrowsExactly<IOException>(() => archive.DeleteDirectory(direcotyPath, false));
        archive.DeleteDirectory(direcotyPath, true);

        Assert.IsFalse(archive.DirectoryExists(direcotyPath));
        Assert.ThrowsExactly<DirectoryNotFoundException>(() => archive.DeleteDirectory(direcotyPath));
        Assert.ThrowsExactly<DirectoryNotFoundException>(() => archive.DeleteDirectory(direcotyPath, true));
    }
}

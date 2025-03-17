// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Linq;

namespace Gapotchenko.FX.IO.Vfs.Tests;

public abstract partial class FileSystemViewVfsTests
{
    [TestMethod]
    public void FileSystemView_Vfs_Empty()
    {
        RunVfsTest(Verify);

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.IsPathRooted(rootPath.AsSpan()));
            Assert.IsTrue(vfs.DirectoryExists(rootPath));
            Assert.IsFalse(vfs.EnumerateEntries(rootPath).Any());
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_CreateEmptyDirectory()
    {
        RunVfsTest(Mutate, Verify);

        const string directoryPath = "Empty";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, directoryPath));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, directoryPath)));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_CreateNestedDirectory()
    {
        RunVfsTest(Mutate, Verify);

        const string directoryPath = "Nested/Empty";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, directoryPath));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, directoryPath)));
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, Path.GetDirectoryName(directoryPath))));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_CreatePartialNestedDirectory()
    {
        RunVfsTest(Mutate, Verify);

        const string directoryPath = "Container/Nested/Empty";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, directoryPath, "..", ".."));
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, Path.GetDirectoryName(Path.GetDirectoryName(directoryPath)))));
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, directoryPath));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, directoryPath)));
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, Path.GetDirectoryName(directoryPath))));
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, Path.GetDirectoryName(Path.GetDirectoryName(directoryPath)))));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_WriteReadAllFileText()
    {
        RunVfsTest(Mutate, Verify);

        const string fileName = "A.txt";
        const string contents = "This is a sample text.";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            vfs.WriteAllFileText(filePath, contents);
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            Assert.AreEqual(contents, vfs.ReadAllFileText(filePath));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_WriteAppendReadAllFileText()
    {
        RunVfsTest(Mutate, Verify);

        const string fileName = "A.txt";
        const string contents = "This is a sample text.";
        const string appendedContents = " This is an appended text.";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            vfs.WriteAllFileText(filePath, contents);
            vfs.AppendAllFileText(filePath, appendedContents);
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            Assert.AreEqual(
                contents + appendedContents,
                vfs.ReadAllFileText(filePath));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_CopyFile()
    {
        RunVfsTest(Mutate, Verify);

        const string fileNameA = "A.txt";
        const string fileNameB = "B.txt";
        const string fileNameC = "C.txt";
        const string fileNameD = "Container/D.txt";
        const string fileContents = "This is a sample text.";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePathA = vfs.CombinePaths(rootPath, fileNameA);
            vfs.WriteAllFileText(filePathA, fileContents);

            string filePathB = vfs.CombinePaths(rootPath, fileNameB);
            vfs.CopyFile(filePathA, filePathB);
            Assert.IsTrue(vfs.FileExists(filePathA));

            Assert.ThrowsException<IOException>(() => vfs.CopyFile(filePathB, filePathA));
            Assert.ThrowsException<IOException>(() => vfs.CopyFile(filePathB, filePathA, false));
            vfs.CopyFile(filePathB, filePathA, true);

            string filePathC = vfs.CombinePaths(rootPath, fileNameC);
            Assert.ThrowsException<FileNotFoundException>(() => vfs.CopyFile(filePathC, filePathA));

            string filePathD = vfs.CombinePaths(rootPath, fileNameD);
            Assert.ThrowsException<DirectoryNotFoundException>(() => vfs.CopyFile(filePathB, filePathD));
            vfs.CreateDirectory(vfs.CombinePaths(filePathD, ".."));
            vfs.CopyFile(filePathB, filePathD);
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.That.VfsEntriesAre(
                vfs,
                rootPath,
                [fileNameA, fileNameB, vfs.CombinePaths(fileNameD, ".."), fileNameD]);

            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameA)));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameB)));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameD)));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_CopyFileTo()
    {
        RunVfsTest(Mutate, Verify);

        const string sourceFileName = "Source.txt";
        const string destinationFileName = "Destination.txt";
        const string fileContents = "This is a sample text.";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            using var sourceVfs = new TempLocalVfs();
            string sourceFilePath = sourceVfs.CombinePaths(sourceVfs.RootPath, sourceFileName);
            sourceVfs.WriteAllFileText(sourceFilePath, fileContents);

            var destinationVfs = vfs;
            string destinationFilePath = destinationVfs.CombinePaths(rootPath, destinationFileName);
            sourceVfs.CopyFile(sourceFilePath, destinationVfs, destinationFilePath);
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.That.VfsEntriesAre(
                vfs,
                rootPath,
                [destinationFileName]);

            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, destinationFileName)));
        }
    }
}

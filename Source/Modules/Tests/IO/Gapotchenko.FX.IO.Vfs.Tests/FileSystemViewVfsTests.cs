// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

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
    public void FileSystemView_Vfs_ReadWriteAllFileText()
    {
        RunVfsTest(Mutate, Verify);

        const string fileName = "A.txt";
        const string text = "This is a sample text.";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            vfs.WriteAllFileText(filePath, text);
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            Assert.AreEqual(text, vfs.ReadAllFileText(filePath));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_ReadWriteAppendAllFileText()
    {
        RunVfsTest(Mutate, Verify);

        const string fileName = "A.txt";
        const string text = "This is a sample text.";
        const string appendedText = " This is an appended text.";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            vfs.WriteAllFileText(filePath, text);
            vfs.AppendAllFileText(filePath, appendedText);
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            Assert.AreEqual(
                text + appendedText,
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
        const string text = "This is a sample text.";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePathA = vfs.CombinePaths(rootPath, fileNameA);
            vfs.WriteAllFileText(filePathA, text);

            string filePathB = vfs.CombinePaths(rootPath, fileNameB);
            vfs.CopyFile(filePathA, filePathB);
            Assert.IsTrue(vfs.FileExists(filePathA));

            Assert.ThrowsException<IOException>(() => vfs.CopyFile(filePathB, filePathA));
            Assert.ThrowsException<IOException>(() => vfs.CopyFile(filePathB, filePathA, false));
            vfs.CopyFile(filePathB, filePathA, true);

            string filePathC = vfs.CombinePaths(rootPath, fileNameC);
            Assert.ThrowsException<FileNotFoundException>(() => vfs.CopyFile(filePathC, filePathA));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.AreEqual(text, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameA)));
            Assert.AreEqual(text, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameB)));
            Assert.IsFalse(vfs.FileExists(vfs.CombinePaths(rootPath, fileNameC)));
        }
    }
}

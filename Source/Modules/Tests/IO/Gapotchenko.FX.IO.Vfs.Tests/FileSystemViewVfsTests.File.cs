// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

partial class FileSystemViewVfsTests
{
    #region Read/write/append text

    [TestMethod]
    public void FileSystemView_Vfs_File_WriteReadAllText()
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
    public void FileSystemView_Vfs_File_WriteAppendReadAllText()
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

    #endregion

    #region Copy

    [TestMethod]
    public void FileSystemView_Vfs_File_Copy()
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
            if (vfs.SupportsLastWriteTime)
                vfs.SetLastWriteTime(filePathA, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));

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
            Assert.That.VfsHierarchyIs(
                vfs,
                rootPath,
                [fileNameA, fileNameB, vfs.CombinePaths(fileNameD, ".."), fileNameD]);

            var expectedAttributes = VfsTestEntryAttributes.Get(vfs, vfs.CombinePaths(rootPath, fileNameA));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameA)));

            Assert.AreEqual(
                expectedAttributes,
                VfsTestEntryAttributes.Get(vfs, vfs.CombinePaths(rootPath, fileNameB)));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameB)));

            Assert.AreEqual(
                expectedAttributes,
                VfsTestEntryAttributes.Get(vfs, vfs.CombinePaths(rootPath, fileNameD)));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameD)));
        }
    }

    [TestMethod]
    [DataRow(false), DataRow(true)]
    public void FileSystemView_Vfs_File_CopyTo(bool reverse)
    {
        using var sourceVfs = CreateTemporaryVfs(out string sourceRootPath);

        RunVfsTest(Mutate, Verify);

        const string sourceFileName = "Source.txt";
        const string destinationFileName = "Destination.txt";
        const string fileContents = "This is a sample text.";

        void Mutate(IFileSystemView destinationVfs, string destinationRootPath)
        {
            #region Epilogue

            (IFileSystemView sVfs, string sr) = (sourceVfs, sourceRootPath);
            var (dVfs, dr) = (destinationVfs, destinationRootPath);

            if (reverse)
                (sVfs, sr, dVfs, dr) = (dVfs, dr, sVfs, sr);

            string SR(string path) => sVfs.CombinePaths(sr, path);
            string DR(string path) => dVfs.CombinePaths(dr, path);

            #endregion

            sVfs.WriteAllFileText(SR(sourceFileName), fileContents);
            if (sVfs.SupportsLastWriteTime)
                sVfs.SetLastWriteTime(SR(sourceFileName), new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            sVfs.CopyFile(SR(sourceFileName), dVfs, DR(destinationFileName));
        }

        void Verify(IReadOnlyFileSystemView destinationVfs, string destinationRootPath)
        {
            #region Epilogue

            (IReadOnlyFileSystemView sVfs, string sr) = (sourceVfs, sourceRootPath);
            var (dVfs, dr) = (destinationVfs, destinationRootPath);

            if (reverse)
                (sVfs, sr, dVfs, dr) = (dVfs, dr, sVfs, sr);

            string SR(string path) => sVfs.CombinePaths(sr, path);
            string DR(string path) => dVfs.CombinePaths(dr, path);

            #endregion

            Assert.AreEqual(
                fileContents,
                sVfs.ReadAllFileText(SR(sourceFileName)));
            var expectedAttributes = VfsTestEntryAttributes.Get(sVfs, SR(sourceFileName));

            Assert.AreEqual(
                expectedAttributes,
                VfsTestEntryAttributes.Get(dVfs, DR(destinationFileName)));
            Assert.AreEqual(
                fileContents,
                dVfs.ReadAllFileText(DR(destinationFileName)));
            Assert.That.VfsHierarchyIs(dVfs, dr, [destinationFileName]);
        }
    }

    #endregion

    #region Move

    [TestMethod]
    public void FileSystemView_Vfs_File_Move()
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
            vfs.MoveFile(filePathA, filePathB);
            Assert.IsFalse(vfs.FileExists(filePathA));

            vfs.CopyFile(filePathB, filePathA);
            Assert.ThrowsException<IOException>(() => vfs.MoveFile(filePathB, filePathA));
            Assert.ThrowsException<IOException>(() => vfs.MoveFile(filePathB, filePathA, false));
            vfs.MoveFile(filePathB, filePathA, true);

            vfs.MoveFile(filePathA, filePathB);
            vfs.CopyFile(filePathB, filePathA);

            string filePathC = vfs.CombinePaths(rootPath, fileNameC);
            Assert.ThrowsException<FileNotFoundException>(() => vfs.MoveFile(filePathC, filePathA));

            string filePathD = vfs.CombinePaths(rootPath, fileNameD);
            Assert.ThrowsException<DirectoryNotFoundException>(() => vfs.MoveFile(filePathA, filePathD));
            vfs.CreateDirectory(vfs.CombinePaths(filePathD, ".."));
            vfs.MoveFile(filePathA, filePathD);
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.That.VfsHierarchyIs(
                vfs,
                rootPath,
                [fileNameB, vfs.CombinePaths(fileNameD, ".."), fileNameD]);

            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameB)));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameD)));
        }
    }

    [TestMethod]
    [DataRow(false), DataRow(true)]
    public void FileSystemView_Vfs_File_MoveTo(bool reverse)
    {
        using var sourceVfs = CreateTemporaryVfs(out string sourceRootPath);

        RunVfsTest(Mutate, Verify);

        const string sourceFileName = "Source.txt";
        const string destinationFileName = "Destination.txt";
        const string fileContents = "This is a sample text.";

        void Mutate(IFileSystemView destinationVfs, string destinationRootPath)
        {
            #region Epilogue

            (IFileSystemView sVfs, string sr) = (sourceVfs, sourceRootPath);
            var (dVfs, dr) = (destinationVfs, destinationRootPath);

            if (reverse)
                (sVfs, sr, dVfs, dr) = (dVfs, dr, sVfs, sr);

            string SR(string path) => sVfs.CombinePaths(sr, path);
            string DR(string path) => dVfs.CombinePaths(dr, path);

            #endregion

            sVfs.WriteAllFileText(SR(sourceFileName), fileContents);

            sVfs.MoveFile(SR(sourceFileName), dVfs, DR(destinationFileName));
        }

        void Verify(IReadOnlyFileSystemView destinationVfs, string destinationRootPath)
        {
            #region Epilogue

            (IReadOnlyFileSystemView sVfs, string sr) = (sourceVfs, sourceRootPath);
            var (dVfs, dr) = (destinationVfs, destinationRootPath);

            if (reverse)
                (sVfs, sr, dVfs, dr) = (dVfs, dr, sVfs, sr);

            string SR(string path) => sVfs.CombinePaths(sr, path);
            string DR(string path) => dVfs.CombinePaths(dr, path);

            #endregion

            Assert.IsFalse(sVfs.FileExists(SR(sourceFileName)));

            Assert.AreEqual(fileContents, dVfs.ReadAllFileText(DR(destinationFileName)));
            Assert.That.VfsHierarchyIs(dVfs, dr, [destinationFileName]);
        }
    }

    #endregion
}

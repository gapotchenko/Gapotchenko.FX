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
        var creationTime = VfsTestHelper.SpecialUtcTime1;
        var lastWriteTime = VfsTestHelper.SpecialUtcTime2;
        var lastAccessTime = VfsTestHelper.SpecialUtcTime3;

        RunVfsTest(Mutate, Verify);

        const string fileNameA = "A.txt";
        const string fileNameB = "B.txt";
        const string fileNameC = "C.txt";
        const string fileNameD = "Container/D.txt";
        const string fileContents = "This is a sample text.";

        void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePathA = vfs.CombinePaths(rootPath, fileNameA);
            vfs.WriteAllFileText(filePathA, fileContents);
            if (vfs.SupportsCreationTime)
                vfs.SetCreationTime(filePathA, creationTime);
            if (vfs.SupportsLastWriteTime)
                vfs.SetLastWriteTime(filePathA, lastWriteTime);
            if (vfs.SupportsLastAccessTime)
                vfs.SetLastAccessTime(filePathA, lastAccessTime);

            string filePathB = vfs.CombinePaths(rootPath, fileNameB);
            vfs.CopyFile(filePathA, filePathB);
            Assert.IsTrue(vfs.FileExists(filePathA));

            Assert.ThrowsException<IOException>(() => vfs.CopyFile(filePathA, filePathB));
            Assert.ThrowsException<IOException>(() => vfs.CopyFile(filePathA, filePathB, false));
            vfs.CopyFile(filePathA, filePathB, true);

            string filePathC = vfs.CombinePaths(rootPath, fileNameC);
            Assert.ThrowsException<FileNotFoundException>(() => vfs.CopyFile(filePathC, filePathA));

            string filePathD = vfs.CombinePaths(rootPath, fileNameD);
            Assert.ThrowsException<DirectoryNotFoundException>(() => vfs.CopyFile(filePathB, filePathD));
            vfs.CreateDirectory(vfs.CombinePaths(filePathD, ".."));
            vfs.CopyFile(filePathB, filePathD);
        }

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            if (vfs.SupportsCreationTime)
                Assert.AreEqual(creationTime, vfs.GetCreationTime(vfs.CombinePaths(rootPath, fileNameA)));
            if (vfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, vfs.GetLastWriteTime(vfs.CombinePaths(rootPath, fileNameA)));
            if (vfs.SupportsLastAccessTime)
                Assert.AreEqual(lastAccessTime, vfs.GetLastAccessTime(vfs.CombinePaths(rootPath, fileNameA)));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameA)));

            if (vfs.SupportsCreationTime)
                Assert.AreNotEqual(creationTime, vfs.GetCreationTime(vfs.CombinePaths(rootPath, fileNameB)));
            if (vfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, vfs.GetLastWriteTime(vfs.CombinePaths(rootPath, fileNameB)));
            if (vfs.SupportsLastAccessTime)
                Assert.AreNotEqual(lastAccessTime, vfs.GetLastAccessTime(vfs.CombinePaths(rootPath, fileNameB)));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameB)));

            if (vfs.SupportsCreationTime)
                Assert.AreNotEqual(creationTime, vfs.GetCreationTime(vfs.CombinePaths(rootPath, fileNameD)));
            if (vfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, vfs.GetLastWriteTime(vfs.CombinePaths(rootPath, fileNameD)));
            if (vfs.SupportsLastAccessTime)
                Assert.AreNotEqual(lastAccessTime, vfs.GetLastAccessTime(vfs.CombinePaths(rootPath, fileNameD)));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameD)));

            Assert.That.VfsHierarchyIs(
                vfs,
                rootPath,
                [fileNameA, fileNameB, vfs.CombinePaths(fileNameD, ".."), fileNameD]);
        }
    }

    [TestMethod]
    [DataRow(false), DataRow(true)]
    public void FileSystemView_Vfs_File_CopyTo(bool reverse)
    {
        var creationTime = VfsTestHelper.SpecialUtcTime1;
        var lastWriteTime = VfsTestHelper.SpecialUtcTime2;
        var lastAccessTime = VfsTestHelper.SpecialUtcTime3;

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
            if (sVfs.SupportsCreationTime)
                sVfs.SetCreationTime(SR(sourceFileName), creationTime);
            if (sVfs.SupportsLastWriteTime)
                sVfs.SetLastWriteTime(SR(sourceFileName), lastWriteTime);
            if (sVfs.SupportsLastAccessTime)
                sVfs.SetLastAccessTime(SR(sourceFileName), lastAccessTime);

            sVfs.CopyFile(SR(sourceFileName), dVfs, DR(destinationFileName));
        }

        void Verify(IReadOnlyFileSystemView destinationVfs, string destinationRootPath, int phase)
        {
            #region Epilogue

            (IReadOnlyFileSystemView sVfs, string sr) = (sourceVfs, sourceRootPath);
            var (dVfs, dr) = (destinationVfs, destinationRootPath);

            if (reverse)
                (sVfs, sr, dVfs, dr) = (dVfs, dr, sVfs, sr);

            string SR(string path) => sVfs.CombinePaths(sr, path);
            string DR(string path) => dVfs.CombinePaths(dr, path);

            #endregion

            if (sVfs.SupportsCreationTime)
                Assert.AreEqual(creationTime, sVfs.GetCreationTime(SR(sourceFileName)));
            if (sVfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, sVfs.GetLastWriteTime(SR(sourceFileName)));
            if (phase == 0 && sVfs.SupportsLastAccessTime)
                Assert.AreEqual(lastAccessTime, sVfs.GetLastAccessTime(SR(sourceFileName)));
            Assert.AreEqual(fileContents, sVfs.ReadAllFileText(SR(sourceFileName)));

            if (dVfs.SupportsCreationTime)
                Assert.AreNotEqual(creationTime, dVfs.GetCreationTime(DR(destinationFileName)));
            if (dVfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, dVfs.GetLastWriteTime(DR(destinationFileName)));
            if (dVfs.SupportsLastAccessTime)
                Assert.AreNotEqual(lastWriteTime, dVfs.GetLastAccessTime(DR(destinationFileName)));
            Assert.AreEqual(fileContents, dVfs.ReadAllFileText(DR(destinationFileName)));

            Assert.That.VfsHierarchyIs(dVfs, dr, [destinationFileName]);
        }
    }

    #endregion

    #region Move

    [TestMethod]
    public void FileSystemView_Vfs_File_Move()
    {
        var creationTime = VfsTestHelper.SpecialUtcTime1;
        var lastWriteTime = VfsTestHelper.SpecialUtcTime2;
        var lastAccessTime = VfsTestHelper.SpecialUtcTime3;

        RunVfsTest(Mutate, Verify);

        const string fileNameA = "A.txt";
        const string fileNameB = "B.txt";
        const string fileNameC = "C.txt";
        const string fileNameD = "Container/D.txt";
        const string fileContents = "This is a sample text.";

        void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePathA = vfs.CombinePaths(rootPath, fileNameA);
            vfs.WriteAllFileText(filePathA, fileContents);
            if (vfs.SupportsCreationTime)
                vfs.SetCreationTime(filePathA, creationTime);
            if (vfs.SupportsLastWriteTime)
                vfs.SetLastWriteTime(filePathA, lastWriteTime);
            if (vfs.SupportsLastAccessTime)
                vfs.SetLastAccessTime(filePathA, lastAccessTime);

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

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            if (vfs.SupportsCreationTime)
                Assert.AreEqual(creationTime, vfs.GetCreationTime(vfs.CombinePaths(rootPath, fileNameB)));
            if (vfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, vfs.GetLastWriteTime(vfs.CombinePaths(rootPath, fileNameB)));
            if (vfs.SupportsLastAccessTime)
                Assert.AreEqual(lastAccessTime, vfs.GetLastAccessTime(vfs.CombinePaths(rootPath, fileNameB)));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameB)));

            Assert.AreEqual(fileContents, vfs.ReadAllFileText(vfs.CombinePaths(rootPath, fileNameD)));

            Assert.That.VfsHierarchyIs(
                vfs,
                rootPath,
                [fileNameB, vfs.CombinePaths(fileNameD, ".."), fileNameD]);
        }
    }

    [TestMethod]
    [DataRow(false), DataRow(true)]
    public void FileSystemView_Vfs_File_MoveTo(bool reverse)
    {
        var creationTime = VfsTestHelper.SpecialUtcTime1;
        var lastWriteTime = VfsTestHelper.SpecialUtcTime2;
        var lastAccessTime = VfsTestHelper.SpecialUtcTime3;

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

            string sourceFilePath = SR(sourceFileName);
            sVfs.WriteAllFileText(sourceFilePath, fileContents);
            if (sVfs.SupportsCreationTime)
                sVfs.SetCreationTime(sourceFilePath, creationTime);
            if (sVfs.SupportsLastWriteTime)
                sVfs.SetLastWriteTime(sourceFilePath, lastWriteTime);
            if (sVfs.SupportsLastAccessTime)
                sVfs.SetLastAccessTime(sourceFilePath, lastAccessTime);

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

            string destinationFilePath = DR(destinationFileName);
            if (sVfs.SupportsCreationTime && dVfs.SupportsCreationTime)
                Assert.AreEqual(creationTime, dVfs.GetCreationTime(destinationFilePath));
            if (sVfs.SupportsLastWriteTime && dVfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, dVfs.GetLastWriteTime(destinationFilePath));
            if (sVfs.SupportsLastAccessTime && dVfs.SupportsLastAccessTime)
                Assert.AreEqual(lastAccessTime, dVfs.GetLastAccessTime(destinationFilePath));
            Assert.AreEqual(fileContents, dVfs.ReadAllFileText(destinationFilePath));

            Assert.That.VfsHierarchyIs(dVfs, dr, [destinationFileName]);
        }
    }

    #endregion
}

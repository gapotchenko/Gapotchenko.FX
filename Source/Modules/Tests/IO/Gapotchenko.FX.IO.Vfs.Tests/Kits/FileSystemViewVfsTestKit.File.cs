// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

partial class FileSystemViewVfsTestKit
{
    #region Create

    [TestMethod]
    public void FileSystemView_Vfs_File_CreateEmpty()
    {
        RunVfsTest(Mutate, Verify);

        const string fileName = "Empty.txt";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            vfs.CreateFile(filePath).Dispose();
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            Assert.IsTrue(vfs.FileExists(filePath));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_File_CreateWithExistingDirectoryNameClash()
    {
        RunVfsTest(Mutate, Verify);

        const string entryName = "Entry";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string entryPath = vfs.CombinePaths(rootPath, entryName);
            vfs.CreateDirectory(entryPath);
            Assert.ThrowsException<UnauthorizedAccessException>(() => vfs.CreateFile(entryPath));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string entryPath = vfs.CombinePaths(rootPath, entryName);
            Assert.IsTrue(vfs.DirectoryExists(entryPath));
            Assert.IsFalse(vfs.FileExists(entryPath));
        }
    }

    #endregion

    #region Open

    [TestMethod]
    public void FileSystemView_Vfs_File_OpenFile_ReadAccess()
    {
        RunVfsTest(Mutate, Verify);

        const string fileName = "File.txt";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            vfs.WriteAllFileBytes(
                filePath,
                VfsTestContentsKit.GetDefaultFileContents(vfs, filePath));
        }

        static void Verify(IFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            using var stream = vfs.OpenFile(filePath, FileMode.Open, FileAccess.Read);

            Assert.IsTrue(stream.CanRead);
            Assert.IsFalse(stream.CanWrite);

            Assert.IsTrue(
                VfsTestContentsKit.GetDefaultFileContents(vfs, filePath)
                .SequenceEqual(stream.AsEnumerable()));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_File_OpenFile_WriteAccess()
    {
        RunVfsTest(Mutate, Verify);

        const string fileName = "File.txt";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            using var stream = vfs.OpenFile(filePath, FileMode.CreateNew, FileAccess.Write);

            Assert.IsFalse(stream.CanRead);
            Assert.IsTrue(stream.CanWrite);

            byte[] contents = VfsTestContentsKit.GetDefaultFileContents(vfs, filePath);
            stream.Write(contents, 0, contents.Length);
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            using var stream = vfs.ReadFile(filePath);

            Assert.IsTrue(
                VfsTestContentsKit.GetDefaultFileContents(vfs, filePath)
                .SequenceEqual(stream.AsEnumerable()));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_File_ReadFile()
    {
        RunVfsTest(Mutate, Verify);

        const string fileName = "File.txt";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            vfs.WriteAllFileBytes(
                filePath,
                VfsTestContentsKit.GetDefaultFileContents(vfs, filePath));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string filePath = vfs.CombinePaths(rootPath, fileName);
            using var stream = vfs.ReadFile(filePath);

            Assert.IsTrue(stream.CanRead);
            Assert.IsFalse(stream.CanWrite);

            Assert.IsTrue(
                VfsTestContentsKit.GetDefaultFileContents(vfs, filePath)
                .SequenceEqual(stream.AsEnumerable()));
        }
    }

    #endregion

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
        var creationTime = VfsTestContentsKit.SpecialUtcTime1;
        var lastWriteTime = VfsTestContentsKit.SpecialUtcTime2;
        var lastAccessTime = VfsTestContentsKit.SpecialUtcTime3;

        RunVfsTest(Mutate, Verify);

        const string fileNameA = "A.txt";
        const string fileNameB = "B.txt";
        const string fileNameC = "C.txt";
        const string fileNameD = "Container/D.txt";
        const string fileNameE = "E.txt";
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

            string filePathE = vfs.CombinePaths(rootPath, fileNameE);
            vfs.CopyFile(filePathA, filePathE, false, VfsCopyOptions.Archive);
        }

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string filePathA = vfs.CombinePaths(rootPath, fileNameA);
            if (vfs.SupportsCreationTime)
                Assert.AreEqual(creationTime, vfs.GetCreationTime(filePathA));
            if (vfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, vfs.GetLastWriteTime(filePathA));
            if (vfs.SupportsLastAccessTime)
                Assert.AreEqual(lastAccessTime, vfs.GetLastAccessTime(filePathA));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(filePathA));

            string filePathB = vfs.CombinePaths(rootPath, fileNameB);
            if (vfs.SupportsCreationTime)
                Assert.AreNotEqual(creationTime, vfs.GetCreationTime(filePathB));
            if (vfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, vfs.GetLastWriteTime(filePathB));
            if (vfs.SupportsLastAccessTime)
                Assert.AreNotEqual(lastAccessTime, vfs.GetLastAccessTime(filePathB));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(filePathB));

            string filePathD = vfs.CombinePaths(rootPath, fileNameD);
            if (vfs.SupportsCreationTime)
                Assert.AreNotEqual(creationTime, vfs.GetCreationTime(filePathD));
            if (vfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, vfs.GetLastWriteTime(filePathD));
            if (vfs.SupportsLastAccessTime)
                Assert.AreNotEqual(lastAccessTime, vfs.GetLastAccessTime(filePathD));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(filePathD));

            string filePathE = vfs.CombinePaths(rootPath, fileNameE);
            if (vfs.SupportsCreationTime)
                Assert.AreEqual(creationTime, vfs.GetCreationTime(filePathE));
            if (vfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, vfs.GetLastWriteTime(filePathE));
            if (vfs.SupportsLastAccessTime)
                Assert.AreEqual(lastAccessTime, vfs.GetLastAccessTime(filePathE));
            Assert.AreEqual(fileContents, vfs.ReadAllFileText(filePathE));

            Assert.That.VfsHierarchyIs(
                vfs,
                rootPath,
                [fileNameA, fileNameB, vfs.CombinePaths(fileNameD, ".."), fileNameD, fileNameE]);
        }
    }

    [TestMethod]
    [DataRow(false), DataRow(true)]
    public void FileSystemView_Vfs_File_CopyTo(bool reverse)
    {
        var creationTime = VfsTestContentsKit.SpecialUtcTime1;
        var lastWriteTime = VfsTestContentsKit.SpecialUtcTime2;
        var lastAccessTime = VfsTestContentsKit.SpecialUtcTime3;

        using var sourceVfs = CreateTemporaryVfs(out string sourceRootPath);

        RunVfsTest(Mutate, Verify);

        const string sourceFileName = "Source.txt";
        const string destinationFileNameA = "Destination A.txt";
        const string destinationFileNameB = "Destination B.txt";
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
            sVfs.CopyFile(sourceFilePath, dVfs, DR(destinationFileNameA));

            if (sVfs.SupportsLastAccessTime)
                sVfs.SetLastAccessTime(sourceFilePath, lastAccessTime);
            sVfs.CopyFile(sourceFilePath, dVfs, DR(destinationFileNameB), options: VfsCopyOptions.Archive);
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

            string sourceFilePath = SR(sourceFileName);
            if (sVfs.SupportsCreationTime)
                Assert.AreEqual(creationTime, sVfs.GetCreationTime(sourceFilePath));
            if (sVfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, sVfs.GetLastWriteTime(sourceFilePath));
            Assert.AreEqual(fileContents, sVfs.ReadAllFileText(sourceFilePath));

            string destinationFilePathA = DR(destinationFileNameA);
            if (sVfs.SupportsCreationTime && dVfs.SupportsCreationTime)
                Assert.AreNotEqual(creationTime, dVfs.GetCreationTime(destinationFilePathA));
            if (sVfs.SupportsLastWriteTime && dVfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, dVfs.GetLastWriteTime(destinationFilePathA));
            if (sVfs.SupportsLastAccessTime && dVfs.SupportsLastAccessTime)
                Assert.AreNotEqual(lastAccessTime, dVfs.GetLastAccessTime(destinationFilePathA));
            Assert.AreEqual(fileContents, dVfs.ReadAllFileText(destinationFilePathA));

            string destinationFilePathB = DR(destinationFileNameB);
            if (sVfs.SupportsCreationTime && dVfs.SupportsCreationTime)
                Assert.AreEqual(creationTime, dVfs.GetCreationTime(destinationFilePathB));
            if (sVfs.SupportsLastWriteTime && dVfs.SupportsLastWriteTime)
                Assert.AreEqual(lastWriteTime, dVfs.GetLastWriteTime(destinationFilePathB));
            if (sVfs.SupportsLastAccessTime && dVfs.SupportsLastAccessTime)
                Assert.AreEqual(lastAccessTime, dVfs.GetLastAccessTime(destinationFilePathB));
            Assert.AreEqual(fileContents, dVfs.ReadAllFileText(destinationFilePathB));

            Assert.That.VfsHierarchyIs(dVfs, dr, [destinationFileNameA, destinationFileNameB]);
        }
    }

    #endregion

    #region Move

    [TestMethod]
    public void FileSystemView_Vfs_File_Move()
    {
        var creationTime = VfsTestContentsKit.SpecialUtcTime1;
        var lastWriteTime = VfsTestContentsKit.SpecialUtcTime2;
        var lastAccessTime = VfsTestContentsKit.SpecialUtcTime3;

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
                [fileNameB, vfs.GetDirectoryName(fileNameD), fileNameD]);
        }
    }

    [TestMethod]
    [DataRow(false), DataRow(true)]
    public void FileSystemView_Vfs_File_MoveTo(bool reverse)
    {
        var creationTime = VfsTestContentsKit.SpecialUtcTime1;
        var lastWriteTime = VfsTestContentsKit.SpecialUtcTime2;
        var lastAccessTime = VfsTestContentsKit.SpecialUtcTime3;

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

    #region Enumerate

    [TestMethod]
    public void FileSystemView_Vfs_File_Enumerate()
    {
        RunVfsTest(Mutate, Verify);

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            VfsTestContentsKit.CreateHierarchy(
                vfs,
                rootPath,
                [
                    "A", "B", "C",
                    "Empty" + vfs.DirectorySeparatorChar
                ]);
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string directoryPath = vfs.CombinePaths(rootPath, "Empty", "..");

            var filePaths = vfs.EnumerateFiles(directoryPath).ToList();

            Assert.AreEqual(3, filePaths.Count);

            Assert.IsTrue(
                filePaths.All(x => x.StartsWith(directoryPath + vfs.DirectorySeparatorChar, StringComparison.Ordinal)),
                "Enumerated file paths do not preserve the path of a directory being enumerated.");

            Assert.IsTrue(
                filePaths.All(vfs.FileExists),
                "Enumerated file paths are pointing to non-existing files.");
        }
    }

    #endregion
}

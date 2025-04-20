// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

partial class FileSystemViewVfsTestKit
{
    [TestMethod]
    public void FileSystemView_Vfs_Entry_Exists()
    {
        RunVfsTest(Mutate, Verify);

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "directory"));
            vfs.CreateFile(vfs.CombinePaths(rootPath, "file")).Dispose();
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            char dsc = vfs.DirectorySeparatorChar;

            string nonexistentPath = vfs.CombinePaths(rootPath, "nonexistent");
            Assert.IsFalse(vfs.EntryExists(nonexistentPath));
            Assert.IsFalse(vfs.EntryExists(nonexistentPath + dsc));

            string directoryPath = vfs.CombinePaths(rootPath, "directory");
            Assert.IsTrue(vfs.EntryExists(directoryPath));
            Assert.IsTrue(vfs.EntryExists(directoryPath + dsc));

            string filePath = vfs.CombinePaths(rootPath, "file");
            Assert.IsTrue(vfs.EntryExists(filePath));
            Assert.IsFalse(vfs.EntryExists(filePath + dsc));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Entry_CreationTime()
    {
        FileSystemView_Vfs_Entry_XxxTime(
            vfs => vfs.SupportsCreationTime,
            (vfs, path) => vfs.GetCreationTime(path),
            (vfs, path, time) => vfs.SetCreationTime(path, time));
    }

    [TestMethod]
    public void FileSystemView_Vfs_Entry_LastWriteTime()
    {
        FileSystemView_Vfs_Entry_XxxTime(
            vfs => vfs.SupportsLastWriteTime,
            (vfs, path) => vfs.GetLastWriteTime(path),
            (vfs, path, time) => vfs.SetLastWriteTime(path, time));
    }

    [TestMethod]
    public void FileSystemView_Vfs_Entry_LastAccessTime()
    {
        FileSystemView_Vfs_Entry_XxxTime(
            vfs => vfs.SupportsLastAccessTime,
            (vfs, path) => vfs.GetLastAccessTime(path),
            (vfs, path, time) => vfs.SetLastAccessTime(path, time));
    }

    void FileSystemView_Vfs_Entry_XxxTime(
        Func<IReadOnlyFileSystemView, bool> supportsXxxTime,
        Func<IReadOnlyFileSystemView, string, DateTime> getXxxTime,
        Action<IFileSystemView, string, DateTime> setXxxTime)
    {
        var specialTime = VfsTestContentsKit.SpecialUtcTime1;

        RunVfsTest(Mutate, Verify);

        void Mutate(IFileSystemView vfs, string rootPath)
        {
            #region No support

            if (!supportsXxxTime(vfs))
            {
                Assert.ThrowsException<NotSupportedException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent"), specialTime));
                return;
            }

            #endregion

            #region Arguments

            Assert.ThrowsException<ArgumentException>(() => setXxxTime(vfs, "", specialTime));

            #endregion

            #region File system structure initialization

            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "directory"));
            vfs.CreateFile(vfs.CombinePaths(rootPath, "file")).Dispose();

            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "container", "directory-a"));
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "container", "directory-b"));
            vfs.CreateFile(vfs.CombinePaths(rootPath, "container", "file")).Dispose();

            #endregion

            char dsc = vfs.DirectorySeparatorChar;

            Assert.ThrowsException<FileNotFoundException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent"), specialTime));
            Assert.ThrowsException<FileNotFoundException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent" + dsc), specialTime));
            Assert.ThrowsException<DirectoryNotFoundException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent-container", "nonexistent"), specialTime));
            Assert.ThrowsException<DirectoryNotFoundException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent-container", "nonexistent") + dsc, specialTime));

            setXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-a"), specialTime);
            setXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-b") + dsc, specialTime);
            setXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "file"), specialTime);
            Assert.ThrowsException<IOException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "file") + dsc, specialTime));
        }

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            #region No support

            if (!supportsXxxTime(vfs))
            {
                Assert.ThrowsException<NotSupportedException>(() => getXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent")));
                return;
            }

            #endregion

            #region Arguments

            Assert.ThrowsException<ArgumentException>(() => getXxxTime(vfs, ""));

            #endregion

            char dsc = vfs.DirectorySeparatorChar;

            Assert.AreEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent")));
            Assert.AreEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent") + dsc));
            Assert.AreNotEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "directory")));
            Assert.AreNotEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "directory") + dsc));
            Assert.AreNotEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "file")));
            Assert.AreEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "file") + dsc));

            Assert.AreEqual(specialTime, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-a")));
            Assert.AreEqual(specialTime, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-a") + dsc));
            Assert.AreEqual(specialTime, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-b")));
            Assert.AreEqual(specialTime, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-b") + dsc));
            Assert.AreEqual(specialTime, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "file")));
            Assert.AreEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "file") + dsc));
        }
    }
}

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
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "B"));
            vfs.CreateFile(vfs.CombinePaths(rootPath, "C")).Dispose();
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            char dsc = vfs.DirectorySeparatorChar;

            string pathA = vfs.CombinePaths(rootPath, "A");
            Assert.IsFalse(vfs.EntryExists(pathA));
            Assert.IsFalse(vfs.EntryExists(pathA + dsc));

            string pathB = vfs.CombinePaths(rootPath, "B");
            Assert.IsTrue(vfs.EntryExists(pathB));
            Assert.IsTrue(vfs.EntryExists(pathB + dsc));

            string pathC = vfs.CombinePaths(rootPath, "C");
            Assert.IsTrue(vfs.EntryExists(pathC));
            Assert.IsFalse(vfs.EntryExists(pathC + dsc));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Entry_LastWriteTime()
    {
        RunVfsTest(Mutate, Verify);

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            if (!vfs.SupportsLastWriteTime)
                return;

            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "directory"));
            vfs.CreateFile(vfs.CombinePaths(rootPath, "file")).Dispose();

            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "container", "directory"));
            vfs.CreateFile(vfs.CombinePaths(rootPath, "container", "file")).Dispose();

            char dsc = vfs.DirectorySeparatorChar;
            var specialTime = VfsTestContentsKit.SpecialUtcTime1;

            Assert.ThrowsException<FileNotFoundException>(() => vfs.SetLastWriteTime(vfs.CombinePaths(rootPath, "non-existent"), specialTime));
            Assert.ThrowsException<FileNotFoundException>(() => vfs.SetLastWriteTime(vfs.CombinePaths(rootPath, "non-existent" + dsc), specialTime));
            Assert.ThrowsException<DirectoryNotFoundException>(() => vfs.SetLastWriteTime(vfs.CombinePaths(rootPath, "non-existent-container", "non-existent"), specialTime));
            Assert.ThrowsException<DirectoryNotFoundException>(() => vfs.SetLastWriteTime(vfs.CombinePaths(rootPath, "non-existent-container", "non-existent") + dsc, specialTime));

            vfs.SetLastWriteTime(vfs.CombinePaths(rootPath, "container", "directory"), specialTime);
            vfs.SetLastWriteTime(vfs.CombinePaths(rootPath, "container", "file"), specialTime);
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            if (!vfs.SupportsLastWriteTime)
                return;

            Assert.AreEqual(DateTime.MinValue, vfs.GetLastWriteTime(vfs.CombinePaths(rootPath, "non-existent")));
            Assert.AreNotEqual(DateTime.MinValue, vfs.GetLastWriteTime(vfs.CombinePaths(rootPath, "file")));
            Assert.AreNotEqual(DateTime.MinValue, vfs.GetLastWriteTime(vfs.CombinePaths(rootPath, "directory")));

            var specialTime = VfsTestContentsKit.SpecialUtcTime1;

            Assert.AreEqual(specialTime, vfs.GetLastWriteTime(vfs.CombinePaths(rootPath, "container", "directory")));
            Assert.AreEqual(specialTime, vfs.GetLastWriteTime(vfs.CombinePaths(rootPath, "container", "file")));
        }
    }
}

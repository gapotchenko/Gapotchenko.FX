// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

partial class FileSystemViewVfsTestKit
{
    [TestMethod]
    public void FileSystemView_Vfs_Capabilities_LastWriteTime()
    {
        RunVfsTest(Test);

        void Test(IFileSystemView vfs, string rootPath)
        {
            string path = vfs.CombinePaths(rootPath, "Entry");
            var time = VfsTestContentsKit.SpecialUtcTime1;

            if (vfs.SupportsLastWriteTime)
            {
                Assert.ThrowsException<ArgumentException>(() => vfs.GetLastWriteTime(""));
                Assert.ThrowsException<ArgumentException>(() => vfs.SetLastWriteTime("", time));

                Assert.AreEqual(DateTime.MinValue, vfs.GetLastWriteTime(path));
                Assert.ThrowsException<FileNotFoundException>(() => vfs.SetLastWriteTime(path, time));

                // File
                vfs.CreateFile(path).Dispose();
                Assert.AreNotEqual(DateTime.MinValue, vfs.GetLastWriteTime(path));
                vfs.SetLastWriteTime(path, time);
                Assert.AreEqual(time, vfs.GetLastWriteTime(path));
                vfs.DeleteFile(path);

                // Directory
                vfs.CreateDirectory(path);
                Assert.AreNotEqual(DateTime.MinValue, vfs.GetLastWriteTime(path));
                vfs.SetLastWriteTime(path, time);
                Assert.AreEqual(time, vfs.GetLastWriteTime(path));
                vfs.DeleteDirectory(path);
            }
            else
            {
                Assert.ThrowsException<NotSupportedException>(() => vfs.GetLastWriteTime(path));
                Assert.ThrowsException<NotSupportedException>(() => vfs.SetLastWriteTime(path, time));
            }
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Capabilities_CreationTime()
    {
        RunVfsTest(Test);

        void Test(IFileSystemView vfs, string rootPath)
        {
            string path = vfs.CombinePaths(rootPath, "Entry");
            var time = VfsTestContentsKit.SpecialUtcTime1;

            if (vfs.SupportsCreationTime)
            {
                Assert.ThrowsException<ArgumentException>(() => vfs.GetCreationTime(""));
                Assert.ThrowsException<ArgumentException>(() => vfs.SetCreationTime("", time));

                Assert.AreEqual(DateTime.MinValue, vfs.GetCreationTime(path));
                Assert.ThrowsException<FileNotFoundException>(() => vfs.SetCreationTime(path, time));

                // File
                vfs.CreateFile(path).Dispose();
                Assert.AreNotEqual(DateTime.MinValue, vfs.GetCreationTime(path));
                vfs.SetCreationTime(path, time);
                Assert.AreEqual(time, vfs.GetCreationTime(path));
                vfs.DeleteFile(path);

                // Directory
                vfs.CreateDirectory(path);
                Assert.AreNotEqual(DateTime.MinValue, vfs.GetCreationTime(path));
                vfs.SetCreationTime(path, time);
                Assert.AreEqual(time, vfs.GetCreationTime(path));
                vfs.DeleteDirectory(path);
            }
            else
            {
                Assert.ThrowsException<NotSupportedException>(() => vfs.GetCreationTime(path));
                Assert.ThrowsException<NotSupportedException>(() => vfs.SetCreationTime(path, time));
            }
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Capabilities_LastAccessTime()
    {
        RunVfsTest(Test);

        void Test(IFileSystemView vfs, string rootPath)
        {
            string path = vfs.CombinePaths(rootPath, "Entry");
            var time = VfsTestContentsKit.SpecialUtcTime1;

            if (vfs.SupportsLastAccessTime)
            {
                Assert.ThrowsException<ArgumentException>(() => vfs.GetLastAccessTime(""));
                Assert.ThrowsException<ArgumentException>(() => vfs.SetLastAccessTime("", time));

                Assert.AreEqual(DateTime.MinValue, vfs.GetLastAccessTime(path));
                Assert.ThrowsException<FileNotFoundException>(() => vfs.SetLastAccessTime(path, time));

                // File
                vfs.CreateFile(path).Dispose();
                Assert.AreNotEqual(DateTime.MinValue, vfs.GetLastAccessTime(path));
                vfs.SetLastAccessTime(path, time);
                Assert.AreEqual(time, vfs.GetLastAccessTime(path));
                vfs.DeleteFile(path);

                // Directory
                vfs.CreateDirectory(path);
                Assert.AreNotEqual(DateTime.MinValue, vfs.GetLastAccessTime(path));
                vfs.SetLastAccessTime(path, time);
                Assert.AreEqual(time, vfs.GetLastAccessTime(path));
                vfs.DeleteDirectory(path);
            }
            else
            {
                Assert.ThrowsException<NotSupportedException>(() => vfs.GetLastAccessTime(path));
                Assert.ThrowsException<NotSupportedException>(() => vfs.SetLastAccessTime(path, time));
            }
        }
    }
}

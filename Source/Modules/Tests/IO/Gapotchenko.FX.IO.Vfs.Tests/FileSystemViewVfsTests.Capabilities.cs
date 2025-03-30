// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

partial class FileSystemViewVfsTests
{
    [TestMethod]
    public void FileSystemView_Vfs_Capabilities_LastWriteTime()
    {
        RunVfsTest(Test);

        void Test(IFileSystemView vfs, string rootPath)
        {
            string path = vfs.CombinePaths(rootPath, "Entry");
            var now = DateTime.UtcNow;

            if (vfs.SupportsLastWriteTime)
            {
                Assert.ThrowsException<ArgumentException>(() => vfs.GetLastWriteTime(""));
                Assert.ThrowsException<ArgumentException>(() => vfs.SetLastWriteTime("", now));

                Assert.AreEqual(DateTime.MinValue, vfs.GetLastWriteTime(path));
                Assert.ThrowsException<FileNotFoundException>(() => vfs.SetLastWriteTime(path, now));

                // File
                vfs.CreateFile(path).Dispose();
                Assert.AreNotEqual(DateTime.MinValue, vfs.GetLastWriteTime(path));
                vfs.SetLastWriteTime(path, now);
                Assert.AreEqual(now, vfs.GetLastWriteTime(path));
                vfs.DeleteFile(path);

                // Directory
                vfs.CreateDirectory(path);
                Assert.AreNotEqual(DateTime.MinValue, vfs.GetLastWriteTime(path));
                vfs.SetLastWriteTime(path, now);
                Assert.AreEqual(now, vfs.GetLastWriteTime(path));
                vfs.DeleteDirectory(path);
            }
            else
            {
                Assert.ThrowsException<NotSupportedException>(() => vfs.GetLastWriteTime(path));
                Assert.ThrowsException<NotSupportedException>(() => vfs.SetLastWriteTime(path, now));
            }
        }
    }
}

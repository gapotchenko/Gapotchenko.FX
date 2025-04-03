// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

partial class FileSystemViewVfsTests
{
    [TestMethod]
    public void FileSystemView_Vfs_Path_GetFullPath()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsNull(vfs.GetFullPath(null));
            Assert.ThrowsException<ArgumentException>(() => vfs.GetFullPath(""));
            Assert.IsFalse(string.IsNullOrEmpty(vfs.GetFullPath("entry")));
        }
    }
}

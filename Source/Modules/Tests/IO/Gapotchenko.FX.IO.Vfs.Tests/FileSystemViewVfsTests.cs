using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.IO.Vfs.Tests;

public abstract partial class FileSystemViewVfsTests
{
    [TestMethod]
    public void Vfs_FileSystemView_Empty()
    {
        RunVfsTest(Test);

        static void Test(IFileSystemView vfs, string rootPath)
        {
            Assert.IsFalse(vfs.EnumerateEntries(rootPath).Any());
            Assert.IsFalse(vfs.DirectoryExists(vfs.CombinePaths(rootPath, "Empty")));
        }
    }

    [TestMethod]
    public void Vfs_FileSystemView_CreateDirectory()
    {
        string directoryPath = "Empty";

        RunVfsTest(Test, PostTest);

        void Test(IFileSystemView vfs, string rootPath)
        {
            directoryPath = vfs.CombinePaths(rootPath, directoryPath);
            vfs.CreateDirectory(directoryPath);
        }

        void PostTest(IFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.DirectoryExists(directoryPath));
        }
    }
}

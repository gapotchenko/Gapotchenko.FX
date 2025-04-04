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

    [TestMethod]
    public void FileSystemView_Vfs_Path_IsPathRooted()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsFalse(IsPathRooted(null));
            Assert.IsFalse(IsPathRooted(""));

            Assert.IsFalse(IsPathRooted("entry"));

            string root = $"{vfs.DirectorySeparatorChar}";
            Assert.IsTrue(IsPathRooted(root));
            Assert.IsTrue(IsPathRooted(root + "entry"));

            bool IsPathRooted(string? path) => vfs.IsPathRooted(path.AsSpan());
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Path_GetPathRoot()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsNull(GetPathRoot(null));
            Assert.IsNull(GetPathRoot(""));

            Assert.AreEqual("", GetPathRoot("entry"));

            string root = $"{vfs.DirectorySeparatorChar}";
            Assert.AreEqual(root, GetPathRoot(root));
            Assert.AreEqual(root, GetPathRoot(root + "entry"));

            string? GetPathRoot(string? path)
            {
                string? resultA = vfs.GetPathRoot(path);
                string? resultB = vfs.GetPathRoot(path.AsSpan()).ToNullableString();
                Assert.AreEqual(resultA, resultB);
                return resultA;
            }
        }
    }
}

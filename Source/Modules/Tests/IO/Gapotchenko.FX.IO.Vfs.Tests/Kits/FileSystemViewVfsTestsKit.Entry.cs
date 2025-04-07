// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

partial class FileSystemViewVfsTestsKit
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
}

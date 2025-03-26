// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

partial class FileSystemViewVfsTests
{
    #region Create

    [TestMethod]
    public void FileSystemView_Vfs_Directory_CreateEmpty()
    {
        RunVfsTest(Mutate, Verify);

        const string directoryPath = "Empty";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, directoryPath));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, directoryPath)));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Directory_CreateNested()
    {
        RunVfsTest(Mutate, Verify);

        const string directoryPath = "Nested/Empty";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, directoryPath));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, directoryPath)));
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, Path.GetDirectoryName(directoryPath))));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Directory_CreatePartialNested()
    {
        RunVfsTest(Mutate, Verify);

        const string directoryPath = "Container/Nested/Empty";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, directoryPath, "..", ".."));
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, Path.GetDirectoryName(Path.GetDirectoryName(directoryPath)))));
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, directoryPath));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, directoryPath)));
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, Path.GetDirectoryName(directoryPath))));
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, Path.GetDirectoryName(Path.GetDirectoryName(directoryPath)))));
        }
    }

    #endregion

    #region Copy

    [TestMethod]
    public void FileSystemView_Vfs_Directory_Copy()
    {
        string[] copiedHierarchy = ["A.txt", "B.txt", "Dir1/C.txt", "Dir1/Dir2/D.txt", "Dir1/Dir3/", "Dir4/"];
        string[] existingHierarchy = ["E/D.txt"];

        RunVfsTest(Mutate, Verify);

        void Mutate(IFileSystemView vfs, string rootPath)
        {
            // A
            VfsHelper.CreateHierarchy(vfs, Root("A"), copiedHierarchy, VfsHelper.GetDefaultFileContents);

            // B
            vfs.CopyDirectory(Root("A"), Root("B"));

            // C
            vfs.CreateDirectory(Root("C"));
            Assert.ThrowsException<IOException>(() => vfs.CopyDirectory(Root("A"), Root("C")));

            // D
            vfs.CreateDirectory(Root("D"));
            vfs.CopyDirectory(Root("A"), Root("D"), true);

            // E
            VfsHelper.CreateHierarchy(vfs, Root("E"), existingHierarchy, VfsHelper.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => vfs.CopyDirectory(Root("A"), Root("E")));
            Assert.That.VfsHierarchyIs(vfs, Root("E"), existingHierarchy, VfsHelper.GetDefaultFileContents);
            vfs.CopyDirectory(Root("A"), Root("E"), true);

            string Root(string path) => vfs.CombinePaths(rootPath, path);
        }

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.That.VfsHierarchyIs(vfs, Root("A"), copiedHierarchy, VfsHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("B"), copiedHierarchy, VfsHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("C"), []);
            Assert.That.VfsHierarchyIs(vfs, Root("D"), copiedHierarchy, VfsHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("E"), [.. existingHierarchy, .. copiedHierarchy], VfsHelper.GetDefaultFileContents);

            string Root(string path) => vfs.CombinePaths(rootPath, path);
        }
    }

    #endregion
}

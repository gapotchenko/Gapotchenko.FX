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
        string[] existingHierarchy = ["Sub1/D.txt"];

        RunVfsTest(Mutate, Verify);

        void Mutate(IFileSystemView vfs, string rootPath)
        {
            // A
            VfsTestHelper.CreateHierarchy(vfs, Root("A"), copiedHierarchy, VfsTestHelper.GetDefaultFileContents);

            // B
            vfs.CopyDirectory(Root("A"), Root("B"));

            // C
            vfs.CreateDirectory(Root("C"));
            Assert.ThrowsException<IOException>(() => vfs.CopyDirectory(Root("A"), Root("C")));

            // D
            vfs.CreateDirectory(Root("D"));
            vfs.CopyDirectory(Root("A"), Root("D"), true);

            // E
            VfsTestHelper.CreateHierarchy(vfs, Root("E"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => vfs.CopyDirectory(Root("A"), Root("E")));
            Assert.That.VfsHierarchyIs(vfs, Root("E"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            vfs.CopyDirectory(Root("A"), Root("E"), true);

            string Root(string path) => vfs.CombinePaths(rootPath, path);
        }

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.That.VfsHierarchyIs(vfs, Root("A"), copiedHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("B"), copiedHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("C"), []);
            Assert.That.VfsHierarchyIs(vfs, Root("D"), copiedHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("E"), [.. existingHierarchy, .. copiedHierarchy], VfsTestHelper.GetDefaultFileContents);

            string Root(string path) => vfs.CombinePaths(rootPath, path);
        }
    }

    [TestMethod]
    [DataRow(false), DataRow(true)]
    public void FileSystemView_Vfs_Directory_CopyTo(bool reverse)
    {
        string[] copiedHierarchy = ["A.txt", "B.txt", "Dir1/C.txt", "Dir1/Dir2/D.txt", "Dir1/Dir3/", "Dir4/"];
        string[] existingHierarchy = ["Sub1/D.txt"];

        using var destinationVfs = CreateTemporaryVfs(out string destinationRootPath);

        RunVfsTest(Mutate, Verify);

        void Mutate(IFileSystemView sourceVfs, string sourceRootPath)
        {
            #region Epilogue

            var (sVfs, sr) = (sourceVfs, sourceRootPath);
            (IFileSystemView dVfs, string dr) = (destinationVfs, destinationRootPath);

            if (reverse)
                (sVfs, sr, dVfs, dr) = (dVfs, dr, sVfs, sr);

            string SR(string path) => sVfs.CombinePaths(sr, path);
            string DR(string path) => dVfs.CombinePaths(dr, path);

            #endregion

            // A
            VfsTestHelper.CreateHierarchy(sVfs, SR("A"), copiedHierarchy, VfsTestHelper.GetDefaultFileContents);

            // B
            sVfs.CopyDirectory(SR("A"), dVfs, DR("B"));

            // C
            dVfs.CreateDirectory(DR("C"));
            Assert.ThrowsException<IOException>(() => sVfs.CopyDirectory(SR("A"), dVfs, DR("C")));

            // D
            dVfs.CreateDirectory(DR("D"));
            sVfs.CopyDirectory(SR("A"), dVfs, DR("D"), true);

            // E
            VfsTestHelper.CreateHierarchy(dVfs, DR("E"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => sVfs.CopyDirectory(SR("A"), dVfs, DR("E")));
            Assert.That.VfsHierarchyIs(dVfs, DR("E"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            sVfs.CopyDirectory(SR("A"), dVfs, DR("E"), true);
        }

        void Verify(IReadOnlyFileSystemView sourceVfs, string sourceRootPath)
        {
            #region Epilogue

            var (sVfs, sr) = (sourceVfs, sourceRootPath);
            (IReadOnlyFileSystemView dVfs, string dr) = (destinationVfs, destinationRootPath);

            if (reverse)
                (sVfs, sr, dVfs, dr) = (dVfs, dr, sVfs, sr);

            string SR(string path) => sVfs.CombinePaths(sr, path);
            string DR(string path) => dVfs.CombinePaths(dr, path);

            #endregion

            Assert.That.VfsHierarchyIs(sVfs, SR("A"), copiedHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(dVfs, DR("B"), copiedHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(dVfs, DR("C"), []);
            Assert.That.VfsHierarchyIs(dVfs, DR("D"), copiedHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(dVfs, DR("E"), [.. existingHierarchy, .. copiedHierarchy], VfsTestHelper.GetDefaultFileContents);
        }
    }

    #endregion

    #region Move

    [TestMethod]
    public void FileSystemView_Vfs_Directory_Move()
    {
        string[] movedHierarchy = ["A.txt", "B.txt", "Dir1/C.txt", "Dir1/Dir2/D.txt", "Dir1/Dir3/", "Dir4/"];
        string[] existingHierarchy = ["Sub1/D.txt"];

        RunVfsTest(Mutate, Verify);

        void Mutate(IFileSystemView vfs, string rootPath)
        {
            // A
            VfsTestHelper.CreateHierarchy(vfs, Root("A"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);

            // B
            vfs.MoveDirectory(Root("A"), Root("B"));

            // C
            VfsTestHelper.CreateHierarchy(vfs, Root("C"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);

            // D
            VfsTestHelper.CreateHierarchy(vfs, Root("D"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => vfs.MoveDirectory(Root("C"), Root("D")));

            // E
            VfsTestHelper.CreateHierarchy(vfs, Root("E"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);

            // F
            VfsTestHelper.CreateHierarchy(vfs, Root("F"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => vfs.MoveDirectory(Root("E"), Root("F")));
            Assert.That.VfsHierarchyIs(vfs, Root("F"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            vfs.MoveDirectory(Root("E"), Root("F"), true);

            string Root(string path) => vfs.CombinePaths(rootPath, path);
        }

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsFalse(vfs.DirectoryExists(Root("A")));
            Assert.That.VfsHierarchyIs(vfs, Root("B"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("C"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("D"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.IsFalse(vfs.DirectoryExists(Root("E")));
            Assert.That.VfsHierarchyIs(vfs, Root("F"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);

            string Root(string path) => vfs.CombinePaths(rootPath, path);
        }
    }

    [TestMethod]
    [DataRow(false), DataRow(true)]
    public void FileSystemView_Vfs_Directory_MoveTo(bool reverse)
    {
        string[] movedHierarchy = ["A.txt", "B.txt", "Dir1/C.txt", "Dir1/Dir2/D.txt", "Dir1/Dir3/", "Dir4/"];
        string[] existingHierarchy = ["Sub1/D.txt"];

        using var destinationVfs = CreateTemporaryVfs(out string destinationRootPath);

        RunVfsTest(Mutate, Verify);

        void Mutate(IFileSystemView sourceVfs, string sourceRootPath)
        {
            #region Epilogue

            var (sVfs, sr) = (sourceVfs, sourceRootPath);
            (IFileSystemView dVfs, string dr) = (destinationVfs, destinationRootPath);

            if (reverse)
                (sVfs, sr, dVfs, dr) = (dVfs, dr, sVfs, sr);

            string SR(string path) => sVfs.CombinePaths(sr, path);
            string DR(string path) => dVfs.CombinePaths(dr, path);

            #endregion

            // A
            VfsTestHelper.CreateHierarchy(sVfs, SR("A"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);

            // B
            sVfs.MoveDirectory(SR("A"), dVfs, DR("B"));

            // C
            VfsTestHelper.CreateHierarchy(sVfs, SR("C"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);

            // D
            VfsTestHelper.CreateHierarchy(dVfs, DR("D"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => sVfs.MoveDirectory(SR("C"), dVfs, DR("D")));

            // E
            VfsTestHelper.CreateHierarchy(sVfs, SR("E"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);

            // F
            VfsTestHelper.CreateHierarchy(dVfs, DR("F"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => sVfs.MoveDirectory(SR("E"), dVfs, DR("F")));
            Assert.That.VfsHierarchyIs(dVfs, DR("F"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            sVfs.MoveDirectory(SR("E"), dVfs, DR("F"), true);
        }

        void Verify(IReadOnlyFileSystemView sourceVfs, string sourceRootPath)
        {
            #region Epilogue

            var (sVfs, sr) = (sourceVfs, sourceRootPath);
            (IReadOnlyFileSystemView dVfs, string dr) = (destinationVfs, destinationRootPath);

            if (reverse)
                (sVfs, sr, dVfs, dr) = (dVfs, dr, sVfs, sr);

            string SR(string path) => sVfs.CombinePaths(sr, path);
            string DR(string path) => dVfs.CombinePaths(dr, path);

            #endregion

            Assert.IsFalse(sVfs.DirectoryExists(SR("A")));
            Assert.That.VfsHierarchyIs(dVfs, DR("B"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(sVfs, SR("C"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(dVfs, DR("D"), existingHierarchy, VfsTestHelper.GetDefaultFileContents);
            Assert.IsFalse(sVfs.DirectoryExists(DR("E")));
            Assert.That.VfsHierarchyIs(dVfs, DR("F"), movedHierarchy, VfsTestHelper.GetDefaultFileContents);
        }
    }

    #endregion
}

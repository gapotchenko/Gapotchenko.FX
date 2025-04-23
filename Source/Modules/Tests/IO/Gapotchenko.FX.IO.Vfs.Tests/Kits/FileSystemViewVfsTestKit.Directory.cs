// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

partial class FileSystemViewVfsTestKit
{
    #region Create

    [TestMethod]
    public void FileSystemView_Vfs_Directory_CreateEmpty()
    {
        RunVfsTest(Mutate, Verify);

        const string directoryName = "Empty";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, directoryName));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, directoryName)));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_File_CreateWithExistingFileNameClash()
    {
        RunVfsTest(Mutate, Verify);

        const string entryName = "Entry";

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            string entryPath = vfs.CombinePaths(rootPath, entryName);
            vfs.CreateFile(entryPath).Dispose();
            Assert.ThrowsException<IOException>(() => vfs.CreateDirectory(entryPath));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string entryPath = vfs.CombinePaths(rootPath, entryName);
            Assert.IsTrue(vfs.FileExists(entryPath));
            Assert.IsFalse(vfs.DirectoryExists(entryPath));
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
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, vfs.GetDirectoryName(directoryPath))));
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
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, vfs.GetDirectoryName(vfs.GetDirectoryName(directoryPath)))));
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, directoryPath));
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, directoryPath)));
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, vfs.GetDirectoryName(directoryPath))));
            Assert.IsTrue(vfs.DirectoryExists(vfs.CombinePaths(rootPath, vfs.GetDirectoryName(vfs.GetDirectoryName(directoryPath)))));
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
            VfsTestContentKit.CreateHierarchy(vfs, Root("A"), copiedHierarchy, VfsTestContentKit.GetDefaultFileContents);

            // B
            vfs.CopyDirectory(Root("A"), Root("B"));

            // C
            vfs.CreateDirectory(Root("C"));
            Assert.ThrowsException<IOException>(() => vfs.CopyDirectory(Root("A"), Root("C")));

            // D
            vfs.CreateDirectory(Root("D"));
            vfs.CopyDirectory(Root("A"), Root("D"), true);

            // E
            VfsTestContentKit.CreateHierarchy(vfs, Root("E"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => vfs.CopyDirectory(Root("A"), Root("E")));
            Assert.That.VfsHierarchyIs(vfs, Root("E"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
            vfs.CopyDirectory(Root("A"), Root("E"), true);

            string Root(string path) => vfs.CombinePaths(rootPath, path);
        }

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.That.VfsHierarchyIs(vfs, Root("A"), copiedHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("B"), copiedHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("C"), []);
            Assert.That.VfsHierarchyIs(vfs, Root("D"), copiedHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("E"), [.. existingHierarchy, .. copiedHierarchy], VfsTestContentKit.GetDefaultFileContents);

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
            VfsTestContentKit.CreateHierarchy(sVfs, SR("A"), copiedHierarchy, VfsTestContentKit.GetDefaultFileContents);

            // B
            sVfs.CopyDirectory(SR("A"), dVfs, DR("B"));

            // C
            dVfs.CreateDirectory(DR("C"));
            Assert.ThrowsException<IOException>(() => sVfs.CopyDirectory(SR("A"), dVfs, DR("C")));

            // D
            dVfs.CreateDirectory(DR("D"));
            sVfs.CopyDirectory(SR("A"), dVfs, DR("D"), true);

            // E
            VfsTestContentKit.CreateHierarchy(dVfs, DR("E"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => sVfs.CopyDirectory(SR("A"), dVfs, DR("E")));
            Assert.That.VfsHierarchyIs(dVfs, DR("E"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
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

            Assert.That.VfsHierarchyIs(sVfs, SR("A"), copiedHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(dVfs, DR("B"), copiedHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(dVfs, DR("C"), []);
            Assert.That.VfsHierarchyIs(dVfs, DR("D"), copiedHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(dVfs, DR("E"), [.. existingHierarchy, .. copiedHierarchy], VfsTestContentKit.GetDefaultFileContents);
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
            VfsTestContentKit.CreateHierarchy(vfs, Root("A"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);

            // B
            vfs.MoveDirectory(Root("A"), Root("B"));

            // C
            VfsTestContentKit.CreateHierarchy(vfs, Root("C"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);

            // D
            VfsTestContentKit.CreateHierarchy(vfs, Root("D"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => vfs.MoveDirectory(Root("C"), Root("D")));

            // E
            VfsTestContentKit.CreateHierarchy(vfs, Root("E"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);

            // F
            VfsTestContentKit.CreateHierarchy(vfs, Root("F"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => vfs.MoveDirectory(Root("E"), Root("F")));
            Assert.That.VfsHierarchyIs(vfs, Root("F"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
            vfs.MoveDirectory(Root("E"), Root("F"), true);

            string Root(string path) => vfs.CombinePaths(rootPath, path);
        }

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsFalse(vfs.DirectoryExists(Root("A")));
            Assert.That.VfsHierarchyIs(vfs, Root("B"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("C"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(vfs, Root("D"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.IsFalse(vfs.DirectoryExists(Root("E")));
            Assert.That.VfsHierarchyIs(vfs, Root("F"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);

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
            VfsTestContentKit.CreateHierarchy(sVfs, SR("A"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);

            // B
            sVfs.MoveDirectory(SR("A"), dVfs, DR("B"));

            // C
            VfsTestContentKit.CreateHierarchy(sVfs, SR("C"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);

            // D
            VfsTestContentKit.CreateHierarchy(dVfs, DR("D"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => sVfs.MoveDirectory(SR("C"), dVfs, DR("D")));

            // E
            VfsTestContentKit.CreateHierarchy(sVfs, SR("E"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);

            // F
            VfsTestContentKit.CreateHierarchy(dVfs, DR("F"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.ThrowsException<IOException>(() => sVfs.MoveDirectory(SR("E"), dVfs, DR("F")));
            Assert.That.VfsHierarchyIs(dVfs, DR("F"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
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
            Assert.That.VfsHierarchyIs(dVfs, DR("B"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(sVfs, SR("C"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.That.VfsHierarchyIs(dVfs, DR("D"), existingHierarchy, VfsTestContentKit.GetDefaultFileContents);
            Assert.IsFalse(sVfs.DirectoryExists(DR("E")));
            Assert.That.VfsHierarchyIs(dVfs, DR("F"), movedHierarchy, VfsTestContentKit.GetDefaultFileContents);
        }
    }

    #endregion

    #region Enumerate

    [TestMethod]
    public void FileSystemView_Vfs_Directory_Enumerate()
    {
        FileSystemView_Vfs_Entry_Enumerate(false, true);
    }

    #endregion
}

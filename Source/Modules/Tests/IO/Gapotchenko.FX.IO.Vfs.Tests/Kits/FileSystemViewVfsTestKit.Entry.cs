// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

partial class FileSystemViewVfsTestKit
{
    [TestMethod]
    public void FileSystemView_Vfs_Entry_Exists()
    {
        RunVfsTest(Mutate, Verify);

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "directory"));
            vfs.CreateFile(vfs.CombinePaths(rootPath, "file")).Dispose();
        }

        static void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            char dsc = vfs.DirectorySeparatorChar;

            string nonexistentPath = vfs.CombinePaths(rootPath, "nonexistent");
            Assert.IsFalse(vfs.EntryExists(nonexistentPath));
            Assert.IsFalse(vfs.EntryExists(nonexistentPath + dsc));

            string directoryPath = vfs.CombinePaths(rootPath, "directory");
            Assert.IsTrue(vfs.EntryExists(directoryPath));
            Assert.IsTrue(vfs.EntryExists(directoryPath + dsc));

            string filePath = vfs.CombinePaths(rootPath, "file");
            Assert.IsTrue(vfs.EntryExists(filePath));
            Assert.IsFalse(vfs.EntryExists(filePath + dsc));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Entry_CreationTime()
    {
        FileSystemView_Vfs_Entry_XxxTime(
            vfs => vfs.SupportsCreationTime,
            (vfs, path) => vfs.GetCreationTime(path),
            (vfs, path, time) => vfs.SetCreationTime(path, time));
    }

    [TestMethod]
    public void FileSystemView_Vfs_Entry_LastWriteTime()
    {
        FileSystemView_Vfs_Entry_XxxTime(
            vfs => vfs.SupportsLastWriteTime,
            (vfs, path) => vfs.GetLastWriteTime(path),
            (vfs, path, time) => vfs.SetLastWriteTime(path, time));
    }

    [TestMethod]
    public void FileSystemView_Vfs_Entry_LastAccessTime()
    {
        FileSystemView_Vfs_Entry_XxxTime(
            vfs => vfs.SupportsLastAccessTime,
            (vfs, path) => vfs.GetLastAccessTime(path),
            (vfs, path, time) => vfs.SetLastAccessTime(path, time));
    }

    void FileSystemView_Vfs_Entry_XxxTime(
        Func<IReadOnlyFileSystemView, bool> supportsXxxTime,
        Func<IReadOnlyFileSystemView, string, DateTime> getXxxTime,
        Action<IFileSystemView, string, DateTime> setXxxTime)
    {
        var specialTime = VfsTestContentsKit.SpecialUtcTime1;

        RunVfsTest(Mutate, Verify);

        void Mutate(IFileSystemView vfs, string rootPath)
        {
            #region No support

            if (!supportsXxxTime(vfs))
            {
                Assert.ThrowsException<NotSupportedException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent"), specialTime));
                return;
            }

            #endregion

            #region Arguments

            Assert.ThrowsException<ArgumentException>(() => setXxxTime(vfs, "", specialTime));

            #endregion

            #region File system structure initialization

            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "directory"));
            vfs.CreateFile(vfs.CombinePaths(rootPath, "file")).Dispose();

            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "container", "directory-a"));
            vfs.CreateDirectory(vfs.CombinePaths(rootPath, "container", "directory-b"));
            vfs.CreateFile(vfs.CombinePaths(rootPath, "container", "file")).Dispose();

            #endregion

            char dsc = vfs.DirectorySeparatorChar;

            Assert.ThrowsException<FileNotFoundException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent"), specialTime));
            Assert.ThrowsException<FileNotFoundException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent" + dsc), specialTime));
            Assert.ThrowsException<DirectoryNotFoundException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent-container", "nonexistent"), specialTime));
            Assert.ThrowsException<DirectoryNotFoundException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent-container", "nonexistent") + dsc, specialTime));

            setXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-a"), specialTime);
            setXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-b") + dsc, specialTime);
            setXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "file"), specialTime);
            Assert.ThrowsException<IOException>(() => setXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "file") + dsc, specialTime));
        }

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            #region No support

            if (!supportsXxxTime(vfs))
            {
                Assert.ThrowsException<NotSupportedException>(() => getXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent")));
                return;
            }

            #endregion

            #region Arguments

            Assert.ThrowsException<ArgumentException>(() => getXxxTime(vfs, ""));

            #endregion

            char dsc = vfs.DirectorySeparatorChar;

            Assert.AreEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent")));
            Assert.AreEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "nonexistent") + dsc));
            Assert.AreNotEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "directory")));
            Assert.AreNotEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "directory") + dsc));
            Assert.AreNotEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "file")));
            Assert.AreEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "file") + dsc));

            Assert.AreEqual(specialTime, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-a")));
            Assert.AreEqual(specialTime, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-a") + dsc));
            Assert.AreEqual(specialTime, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-b")));
            Assert.AreEqual(specialTime, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "directory-b") + dsc));
            Assert.AreEqual(specialTime, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "file")));
            Assert.AreEqual(DateTime.MinValue, getXxxTime(vfs, vfs.CombinePaths(rootPath, "container", "file") + dsc));
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Entry_Enumerate()
    {
        FileSystemView_Vfs_Entry_Enumerate(true, true);
    }

    void FileSystemView_Vfs_Entry_Enumerate(bool enumerateFiles, bool enumerateDirectories)
    {
        RunVfsTest(Mutate, Verify);

        static void Mutate(IFileSystemView vfs, string rootPath)
        {
            char ds = vfs.DirectorySeparatorChar;

            VfsTestContentsKit.CreateHierarchy(
                vfs,
                rootPath,
                [
                    // Scenario 1
                    $"Scenario 1{ds}A", $"Scenario 1{ds}B", $"Scenario 1{ds}C",
                    $"Scenario 1{ds}Empty{ds}",

                    // Scenario 2
                    $"Scenario 2{ds}1.txt",
                    $"Scenario 2{ds}2.txt",
                    $"Scenario 2{ds}3.bin"
                ]);
        }

        void Verify(IReadOnlyFileSystemView vfs, string rootPath)
        {
            char ds = vfs.DirectorySeparatorChar;
            char ads = vfs.AltDirectorySeparatorChar;
            var defaultEnumerationOptions = new EnumerationOptions();

            Scenario1(ds);
            if (ads != ds)
                Scenario1(ads);
            Scenario2();

            #region Scenarios

            void Scenario1(char eds)
            {
                string directoryPath = vfs.JoinPaths(rootPath, $"Scenario 1{eds}Empty{eds}..");
                string filePath = vfs.JoinPaths(directoryPath, "A");

                if (enumerateFiles && enumerateDirectories)
                {
                    var entryPaths = ToUniqueHashSet(vfs.EnumerateEntries(directoryPath));

                    Assert.AreEqual(4, entryPaths.Count);
                    VerifySearchPattern(vfs, directoryPath, null, entryPaths);
                    VerifyEntriesExistence(vfs, entryPaths, 3, 1);

                    Assert.IsTrue(entryPaths.SetEquals(vfs.EnumerateEntries(directoryPath, "*")));
                    Assert.IsTrue(entryPaths.SetEquals(vfs.EnumerateEntries(directoryPath, "*", SearchOption.TopDirectoryOnly)));
                    Assert.IsTrue(entryPaths.SetEquals(vfs.EnumerateEntries(directoryPath, "*", defaultEnumerationOptions)));

                    Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateEntries(filePath)));
                    Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateEntries(filePath, "*")));
                    Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateEntries(filePath, "*", SearchOption.TopDirectoryOnly)));
                    Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateEntries(filePath, "*", defaultEnumerationOptions)));
                }
                else
                {
                    if (enumerateFiles)
                    {
                        var filePaths = ToUniqueHashSet(vfs.EnumerateFiles(directoryPath));

                        Assert.AreEqual(3, filePaths.Count);
                        VerifySearchPattern(vfs, directoryPath, null, filePaths);
                        VerifyFilesExistence(vfs, filePaths);

                        Assert.IsTrue(filePaths.SetEquals(vfs.EnumerateFiles(directoryPath, "*")));
                        Assert.IsTrue(filePaths.SetEquals(vfs.EnumerateFiles(directoryPath, "*", SearchOption.TopDirectoryOnly)));
                        Assert.IsTrue(filePaths.SetEquals(vfs.EnumerateFiles(directoryPath, "*", defaultEnumerationOptions)));

                        Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateFiles(filePath)));
                        Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateFiles(filePath, "*")));
                        Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateFiles(filePath, "*", SearchOption.TopDirectoryOnly)));
                        Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateFiles(filePath, "*", defaultEnumerationOptions)));
                    }

                    if (enumerateDirectories)
                    {
                        var directoryPaths = ToUniqueHashSet(vfs.EnumerateDirectories(directoryPath));

                        Assert.AreEqual(1, directoryPaths.Count);
                        VerifySearchPattern(vfs, directoryPath, null, directoryPaths);
                        VerifyDirectoriesExistence(vfs, directoryPaths);

                        Assert.IsTrue(directoryPaths.SetEquals(vfs.EnumerateDirectories(directoryPath, "*")));
                        Assert.IsTrue(directoryPaths.SetEquals(vfs.EnumerateDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly)));
                        Assert.IsTrue(directoryPaths.SetEquals(vfs.EnumerateDirectories(directoryPath, "*", defaultEnumerationOptions)));

                        Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateDirectories(filePath)));
                        Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateDirectories(filePath, "*")));
                        Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateDirectories(filePath, "*", SearchOption.TopDirectoryOnly)));
                        Assert.ThrowsException<IOException>(() => DiscardAll(vfs.EnumerateDirectories(filePath, "*", defaultEnumerationOptions)));
                    }
                }
            }

            void Scenario2()
            {
                string searchPattern = vfs.JoinPaths("Scenario 2", ".", "*.txt");

                if (enumerateFiles && enumerateDirectories)
                {
                    var entryPaths = ToUniqueHashSet(vfs.EnumerateEntries(rootPath, searchPattern));

                    Assert.AreEqual(2, entryPaths.Count);
                    VerifySearchPattern(vfs, rootPath, searchPattern, entryPaths);
                    VerifyEntriesExistence(vfs, entryPaths, 2, 0);

                    Assert.IsTrue(entryPaths.SetEquals(vfs.EnumerateEntries(rootPath, searchPattern, SearchOption.TopDirectoryOnly)));
                    Assert.IsTrue(entryPaths.SetEquals(vfs.EnumerateEntries(rootPath, searchPattern, defaultEnumerationOptions)));
                }
                else
                {
                    if (enumerateFiles)
                    {
                        var filePaths = ToUniqueHashSet(vfs.EnumerateFiles(rootPath, searchPattern));

                        Assert.AreEqual(2, filePaths.Count);
                        VerifySearchPattern(vfs, rootPath, searchPattern, filePaths);
                        VerifyFilesExistence(vfs, filePaths);

                        Assert.IsTrue(filePaths.SetEquals(vfs.EnumerateFiles(rootPath, searchPattern, SearchOption.TopDirectoryOnly)));
                        Assert.IsTrue(filePaths.SetEquals(vfs.EnumerateFiles(rootPath, searchPattern, defaultEnumerationOptions)));
                    }

                    if (enumerateDirectories)
                    {
                        Assert.IsFalse(vfs.EnumerateDirectories(rootPath, searchPattern).Any());
                        Assert.IsFalse(vfs.EnumerateDirectories(rootPath, searchPattern, SearchOption.TopDirectoryOnly).Any());
                        Assert.IsFalse(vfs.EnumerateDirectories(rootPath, searchPattern, defaultEnumerationOptions).Any());
                    }
                }
            }

            #endregion

            #region Helpers

            static void DiscardAll<T>(IEnumerable<T> source)
            {
                foreach (var i in source)
                    _ = i;
            }

            static void VerifySearchPattern(
                IReadOnlyFileSystemView vfs,
                string path,
                string? searchPattern,
                IEnumerable<string> enumeratedPaths,
                EnumerationOptions? enumerationOptions = null)
            {
                if (searchPattern != null)
                    VfsSearchKit.AdjustPatternPath(vfs, ref path, ref searchPattern);

                Assert.IsTrue(
                    enumeratedPaths.All(x => x.StartsWith(path + vfs.DirectorySeparatorChar, StringComparison.Ordinal)),
                    "Enumerated paths do not preserve the path of a directory being enumerated.");

                var searchExpression = new VfsSearchExpression(
                    searchPattern,
                    vfs.DirectorySeparatorChar,
                    options: VfsSearchKit.GetSearchExpressionOptions(
                        vfs,
                        enumerationOptions?.MatchCasing ?? MatchCasing.PlatformDefault));

                Assert.IsTrue(
                    enumeratedPaths.All(x => searchExpression.IsMatch(vfs.GetFileName(x).AsSpan())),
                    "Enumerated paths do not match the search pattern.");
            }

            static void VerifyEntriesExistence(IReadOnlyFileSystemView vfs, IEnumerable<string> entryPaths, int expectedFileCount, int expectedDirectoryCount)
            {
                Assert.IsTrue(
                    entryPaths.All(vfs.EntryExists),
                    "Enumerated entry paths are pointing to non-existing entries.");

                Assert.AreEqual(
                    expectedFileCount,
                    entryPaths.Count(vfs.FileExists),
                    "Enumerated entry paths are pointing to an invalid number of existing files.");

                Assert.AreEqual(
                    expectedDirectoryCount,
                    entryPaths.Count(vfs.DirectoryExists),
                    "Enumerated entry paths are pointing to an invalid number of existing directories.");
            }

            static void VerifyDirectoriesExistence(IReadOnlyFileSystemView vfs, IEnumerable<string> directoryPaths)
            {
                Assert.IsTrue(
                    directoryPaths.All(vfs.DirectoryExists),
                    "Enumerated directory paths are pointing to non-existing directories.");
            }

            static void VerifyFilesExistence(IReadOnlyFileSystemView vfs, IEnumerable<string> filePaths)
            {
                Assert.IsTrue(
                    filePaths.All(vfs.FileExists),
                    "Enumerated file paths are pointing to non-existing files.");
            }

            static HashSet<string> ToUniqueHashSet(IEnumerable<string> source)
            {
                var hashSet = new HashSet<string>(StringComparer.Ordinal);
                foreach (string i in source)
                    Assert.IsTrue(hashSet.Add(i), "Enumerated paths have duplicates.");
                return hashSet;
            }

            #endregion
        }
    }
}

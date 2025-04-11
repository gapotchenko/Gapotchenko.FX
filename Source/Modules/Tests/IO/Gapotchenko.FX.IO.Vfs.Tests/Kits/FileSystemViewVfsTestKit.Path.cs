// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Memory;

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

partial class FileSystemViewVfsTestKit
{
    [TestMethod]
    public void FileSystemView_Vfs_Path_GetFullPath()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsNull(GetFullPath(null));
            Assert.ThrowsException<ArgumentException>(() => GetFullPath(""));
            Assert.IsFalse(string.IsNullOrEmpty(GetFullPath("entry")));

            [return: NotNullIfNotNull(nameof(path))]
            string? GetFullPath(string? path)
            {
                string? result = vfs.GetFullPath(path);
                Assert.AreEqual(path is null, result is null);
                return result;
            }
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Path_GetDirectoryName()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string dsc = $"{vfs.DirectorySeparatorChar}";

            Assert.IsNull(GetDirectoryName(null));
            Assert.IsNull(GetDirectoryName(""));
            Assert.IsNull(GetDirectoryName(dsc));
            Assert.AreEqual("", GetDirectoryName("container"));
            Assert.AreEqual("container", GetDirectoryName("container" + dsc + "entry"));
            Assert.AreEqual(dsc, GetDirectoryName(dsc + "entry"));

            string? GetDirectoryName(string? path)
            {
                string? resultA = vfs.GetDirectoryName(path);
                string? resultB = vfs.GetDirectoryName(path.AsSpan()).ToNullableString();
                Assert.AreEqual(resultA, resultB);
                return resultA;
            }
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Path_GetFileName()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string dsc = $"{vfs.DirectorySeparatorChar}";

            Assert.IsNull(GetFileName(null));
            Assert.AreEqual("", GetFileName(""));
            Assert.AreEqual("", GetFileName(dsc));
            Assert.AreEqual("entry", GetFileName("entry"));
            Assert.AreEqual("entry", GetFileName("container" + dsc + "entry"));
            Assert.AreEqual("entry", GetFileName(dsc + "entry"));

            string? GetFileName(string? path)
            {
                string? resultA = vfs.GetFileName(path);
                string? resultB = vfs.GetFileName(path.AsSpan()).ToNullableString();
                Assert.AreEqual(resultA, resultB);
                Assert.AreEqual(path is null, resultA is null);
                return resultA;
            }
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Path_IsPathRooted()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string dsc = $"{vfs.DirectorySeparatorChar}";

            Assert.IsFalse(IsPathRooted(null));
            Assert.IsFalse(IsPathRooted(""));
            Assert.IsFalse(IsPathRooted("entry"));
            Assert.IsTrue(IsPathRooted(dsc));
            Assert.IsTrue(IsPathRooted(dsc + "entry"));

            bool IsPathRooted(string? path)
            {
                bool resultA = vfs.IsPathRooted(path);
                bool resultB = vfs.IsPathRooted(path.AsSpan());
                Assert.AreEqual(resultA, resultB);
                if (resultA)
                    Assert.IsNotNull(path);
                return resultA;
            }
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Path_GetPathRoot()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string dsc = $"{vfs.DirectorySeparatorChar}";

            Assert.IsNull(GetPathRoot(null));
            Assert.IsNull(GetPathRoot(""));
            Assert.AreEqual("", GetPathRoot("entry"));
            Assert.AreEqual(dsc, GetPathRoot(dsc));
            Assert.AreEqual(dsc, GetPathRoot(dsc + "entry"));

            string? GetPathRoot(string? path)
            {
                string? resultA = vfs.GetPathRoot(path);
                string? resultB = vfs.GetPathRoot(path.AsSpan()).ToNullableString();
                Assert.AreEqual(resultA, resultB);
                return resultA;
            }
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Path_TrimEndingDirectorySeparator()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string dsc = $"{vfs.DirectorySeparatorChar}";

            Assert.IsNull(TrimEndingDirectorySeparator(null));
            Assert.AreEqual("", TrimEndingDirectorySeparator(""));
            Assert.AreEqual(dsc, TrimEndingDirectorySeparator(dsc));
            Assert.AreEqual("entry", TrimEndingDirectorySeparator("entry"));
            Assert.AreEqual(dsc + "entry", TrimEndingDirectorySeparator(dsc + "entry"));
            Assert.AreEqual("entry", TrimEndingDirectorySeparator("entry" + dsc));
            Assert.AreEqual("entry" + dsc, TrimEndingDirectorySeparator("entry" + dsc + dsc));
            Assert.AreEqual("entry" + dsc + dsc, TrimEndingDirectorySeparator("entry" + dsc + dsc + dsc));

            string? TrimEndingDirectorySeparator(string? path)
            {
                string? resultA = vfs.TrimEndingDirectorySeparator(path);
                string? resultB = vfs.TrimEndingDirectorySeparator(path.AsSpan()).ToNullableString();
                Assert.AreEqual(resultA, resultB);
                Assert.AreEqual(path is null, resultA is null);
                return resultA;
            }
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Path_EndsInDirectorySeparator()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            string dsc = $"{vfs.DirectorySeparatorChar}";

            Assert.IsFalse(EndsInDirectorySeparator(null));
            Assert.IsFalse(EndsInDirectorySeparator(""));
            Assert.IsFalse(EndsInDirectorySeparator("entry"));
            Assert.IsTrue(EndsInDirectorySeparator(dsc));
            Assert.IsFalse(EndsInDirectorySeparator(dsc + "entry"));
            Assert.IsTrue(EndsInDirectorySeparator("entry" + dsc));
            Assert.IsFalse(EndsInDirectorySeparator("container" + dsc + "entry"));

            bool EndsInDirectorySeparator(string? path)
            {
                bool resultA = vfs.EndsInDirectorySeparator(path);
                bool resultB = vfs.EndsInDirectorySeparator(path.AsSpan());
                Assert.AreEqual(resultA, resultB);
                if (resultA)
                    Assert.IsNotNull(path);
                return resultA;
            }
        }
    }
}

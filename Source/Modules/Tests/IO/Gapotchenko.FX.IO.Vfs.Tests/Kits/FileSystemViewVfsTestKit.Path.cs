// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Memory;

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

partial class FileSystemViewVfsTestKit
{
    [TestMethod]
    public void FileSystemView_Vfs_Path_JoinPaths()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            char ds = vfs.DirectorySeparatorChar;
            char ads = vfs.AltDirectorySeparatorChar;

            Assert.AreEqual("", JoinPaths());
            Assert.AreEqual("a", JoinPaths("a"));
            Assert.AreEqual($"a{ds}b", JoinPaths("a", "b"));

            Assert.AreEqual($"a{ds}b", JoinPaths("a", $"{ds}b"));
            Assert.AreEqual($"a{ds}b", JoinPaths($"a{ds}", "b"));
            Assert.AreEqual($"a{ds}{ds}b", JoinPaths($"a{ds}", $"{ds}b"));

            Assert.AreEqual($"a{ads}b", JoinPaths("a", $"{ads}b"));
            Assert.AreEqual($"a{ads}b", JoinPaths($"a{ads}", "b"));
            Assert.AreEqual($"a{ads}{ads}b", JoinPaths($"a{ads}", $"{ads}b"));

            Assert.AreEqual($"a{ds}{ads}b", JoinPaths($"a{ds}", $"{ads}b"));
            Assert.AreEqual($"a{ads}{ds}b", JoinPaths($"a{ads}", $"{ds}b"));

            foreach (string?[] i in PermuteValueInsertions(["a", "b", "c"], [null, ""], 2))
            {
                Assert.AreEqual(
                    $"a{ds}b{ds}c",
                    JoinPaths(i),
                    "Null/empty value handling is violated for: " + string.Join(", ", i.Select(x => x ?? "<null>")));
            }

            string JoinPaths(params string?[] paths)
            {
                string resultA = vfs.JoinPaths(paths.AsEnumerable());
                string resultB = vfs.JoinPaths(paths.AsSpan());
                Assert.AreEqual(resultA, resultB);
                return resultA;
            }
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Path_CombinePaths()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            char ds = vfs.DirectorySeparatorChar;
            char ads = vfs.AltDirectorySeparatorChar;

            Assert.AreEqual("", CombinePaths());
            Assert.AreEqual("a", CombinePaths("a"));
            Assert.AreEqual($"a{ds}b", CombinePaths("a", "b"));

            Assert.AreEqual($"{ds}b", CombinePaths("a", $"{ds}b"));
            Assert.AreEqual($"a{ds}b", CombinePaths($"a{ds}", "b"));
            Assert.AreEqual($"{ds}b", CombinePaths($"a{ds}", $"{ds}b"));

            Assert.AreEqual($"{ads}b", CombinePaths("a", $"{ads}b"));
            Assert.AreEqual($"a{ads}b", CombinePaths($"a{ads}", "b"));
            Assert.AreEqual($"{ads}b", CombinePaths($"a{ads}", $"{ads}b"));
            Assert.AreEqual($"{ads}b", CombinePaths($"a{ds}", $"{ads}b"));
            Assert.AreEqual($"{ds}b", CombinePaths($"a{ads}", $"{ds}b"));

            foreach (string?[] i in PermuteValueInsertions(["a", "b", "c"], [null, ""], 2))
            {
                Assert.AreEqual(
                    $"a{ds}b{ds}c",
                    CombinePaths(i),
                    "Null/empty value handling is violated for: " + string.Join(", ", i.Select(x => x ?? "<null>")));
            }

            string CombinePaths(params string?[] paths)
            {
                string resultA = vfs.CombinePaths(paths.AsEnumerable());
                string resultB = vfs.CombinePaths(paths.AsSpan());
                Assert.AreEqual(resultA, resultB);
                return resultA;
            }
        }
    }

    [TestMethod]
    public void FileSystemView_Vfs_Path_GetFullPath()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsNull(GetFullPath(null));
            Assert.ThrowsExactly<ArgumentException>(() => GetFullPath(""));
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
    public void FileSystemView_Vfs_Path_CanonicalizePath()
    {
        RunVfsTest(Test);

        static void Test(IReadOnlyFileSystemView vfs, string rootPath)
        {
            Assert.IsNull(CanonicalizePath(null));
            Assert.IsEmpty(CanonicalizePath(string.Empty));

            char ds = vfs.DirectorySeparatorChar;
            char ads = vfs.AltDirectorySeparatorChar;

            Assert.AreEqual($"abc{ds}def", CanonicalizePath($"abc{ds}def"));
            Assert.AreEqual($"abc{ds}def", CanonicalizePath($"abc{ads}def"));

            Assert.AreEqual($"abc{ds}def", CanonicalizePath($"abc{ds}{ds}def"));
            Assert.AreEqual($"abc{ds}def", CanonicalizePath($"abc{ds}{ads}def"));
            Assert.AreEqual($"abc{ds}def", CanonicalizePath($"abc{ads}{ds}def"));
            Assert.AreEqual($"abc{ds}def", CanonicalizePath($"abc{ads}{ads}def"));

            Assert.AreEqual($"abc{ds}def{ds}", CanonicalizePath($"abc{ds}def{ds}"));
            Assert.AreEqual($"abc{ds}def{ds}", CanonicalizePath($"abc{ads}def{ads}"));

            Assert.AreEqual($"abc{ds}def{ds}", CanonicalizePath($"abc{ds}{ds}def{ds}{ds}"));
            Assert.AreEqual($"abc{ds}def{ds}", CanonicalizePath($"abc{ads}{ads}def{ads}{ads}"));

            [return: NotNullIfNotNull(nameof(path))]
            string? CanonicalizePath(string? path)
            {
                string? result = vfs.CanonicalizePath(path);
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

    #region Helpers

    static IEnumerable<T[]> PermuteValueInsertions<T>(T[] source, T[] insertionValues, int maxInsertionLength)
        where T : class?
    {
        int en = source.Length;
        int cn = checked((int)Math.Round(Math.Pow(2, en + 1)));

        foreach (var value in insertionValues)
        {
            for (int insertionLength = 1; insertionLength <= maxInsertionLength; ++insertionLength)
            {
                for (int c = 0; c < cn; ++c)
                {
                    var list = new List<T>(en * 2 + 1);

                    for (int i = 0; i < en; ++i)
                    {
                        if (hasBit(i))
                            addValue();
                        list.Add(source[i]);
                    }
                    if (hasBit(en))
                        addValue();

                    yield return list.ToArray();

                    // --------------------------------------------

                    bool hasBit(int n) => (c & (1 << n)) != 0;

                    void addValue()
                    {
                        for (int i = 0; i < insertionLength; ++i)
                            list.Add(value);
                    }
                }
            }
        }
    }

    #endregion
}

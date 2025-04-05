// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Linq;

namespace Gapotchenko.FX.IO.Vfs.Tests;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class AssertExtensions
{
    public static void VfsHierarchyIs(
        this Assert _,
        IReadOnlyFileSystemView vfs,
        string directoryPath,
        IEnumerable<string?> entriesPaths,
        Func<IReadOnlyFileSystemView, string, byte[]>? getFileContents = null)
    {
        var actual =
            vfs.EnumerateEntries(directoryPath, "*", SearchOption.AllDirectories)
            .Select(x => vfs.GetFullPath(x));

        var expected = entriesPaths
            .Where(x => x is not null)
            .Select(x => vfs.GetFullPath(vfs.CombinePaths(directoryPath, x)))
            .ToHashSet(vfs.PathComparer);

        bool useDirectoryTrail = expected.Any(vfs.EndsInDirectorySeparator);
        if (useDirectoryTrail)
            actual = actual.Select(x => vfs.DirectoryExists(x) ? x + vfs.DirectorySeparatorChar : x);

        // Add implied subdirectories if they are missing.
        foreach (string entryPath in expected.ToList())
        {
            string? entryDirectoryPath = vfs.GetDirectoryName(entryPath);
            if (!string.IsNullOrEmpty(entryDirectoryPath) &&
                !vfs.PathComparer.Equals(entryDirectoryPath, directoryPath))
            {
                if (useDirectoryTrail)
                    entryDirectoryPath += vfs.DirectorySeparatorChar;
                expected.Add(entryDirectoryPath);
            }
        }

        Assert.IsTrue(
            actual.ToHashSet(vfs.PathComparer).SetEquals(expected),
            $"VFS hierarchy in '{directoryPath}' directory does not correspond to the expected structure.");

        if (getFileContents != null)
        {
            foreach (string entryPath in expected)
            {
                if (vfs.FileExists(entryPath))
                {
                    byte[] expectedFileContents = getFileContents(vfs, entryPath);
                    byte[] actualFileContents = vfs.ReadAllFileBytes(entryPath);
                    Assert.IsTrue(
                        expectedFileContents.SequenceEqual(actualFileContents),
                        "VFS file contents mismatch.");
                }
            }
        }
    }
}

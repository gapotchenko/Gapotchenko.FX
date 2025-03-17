// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

static class AssertExtensions
{
    public static void VfsEntriesAre(
        this Assert _,
        IReadOnlyFileSystemView vfs,
        string rootPath,
        IEnumerable<string?> entryPaths)
    {
        var actual = vfs
            .EnumerateEntries(rootPath, "*", SearchOption.AllDirectories)
            .ToHashSet(vfs.PathComparer);

        var expected = entryPaths
            .Where(x => x is not null)
            .Select(x => vfs.GetFullPath(vfs.CombinePaths(rootPath, x)));

        Assert.IsTrue(actual.SetEquals(expected));
    }
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class AssertExtensions
{
    public static void VfsHierarchyIs(
        this Assert _,
        IReadOnlyFileSystemView vfs,
        string directoryPath,
        IEnumerable<string?> entriesPaths)
    {
        var actual = vfs
            .EnumerateEntries(directoryPath, "*", SearchOption.AllDirectories)
            .ToHashSet(vfs.PathComparer);

        var expected = entriesPaths
            .Where(x => x is not null)
            .Select(x => vfs.GetFullPath(vfs.CombinePaths(directoryPath, x)));

        Assert.IsTrue(actual.SetEquals(expected));
    }
}

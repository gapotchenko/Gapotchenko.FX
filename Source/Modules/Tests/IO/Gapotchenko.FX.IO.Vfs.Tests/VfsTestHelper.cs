// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

using Gapotchenko.FX.Text;
using System.Text;

public static class VfsTestHelper
{
    public static void CreateHierarchy(
        IFileSystemView vfs,
        string directoryPath,
        IEnumerable<string> entriesPaths,
        Func<IReadOnlyFileSystemView, string, byte[]>? getFileContents = null)
    {
        foreach (string entryPath in entriesPaths)
        {
            string fullPath = vfs.CombinePaths(directoryPath, entryPath);
            if (IsDirectoryName(vfs, entryPath))
            {
                vfs.CreateDirectory(fullPath);
            }
            else
            {
                string? fullDirectoryPath = Path.GetDirectoryName(fullPath);
                if (fullDirectoryPath != null)
                    vfs.CreateDirectory(fullDirectoryPath);

                if (getFileContents != null)
                    vfs.WriteAllFileBytes(fullPath, getFileContents(vfs, entryPath));
                else
                    vfs.CreateFile(fullPath).Dispose();
            }
        }
    }

    public static bool IsDirectoryName(IReadOnlyFileSystemView vfs, string name) =>
        name.EndsWith('/') ||
        name.EndsWith(vfs.DirectorySeparatorChar);

    public static byte[] GetDefaultFileContents(IReadOnlyFileSystemView vfs, string path)
    {
        _ = vfs;
        return Encoding.UTF8.GetBytes(
            string.Format(
                "This is default contents of the '{0}' file.",
                Path.GetFileName(path)));
    }
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Text;

namespace Gapotchenko.FX.IO.Vfs.Tests.Kits;

public static class VfsTestContentsKit
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
            if (vfs.EndsInDirectorySeparator(entryPath))
            {
                vfs.CreateDirectory(fullPath);
            }
            else
            {
                string? fullDirectoryPath = vfs.GetDirectoryName(fullPath);
                if (fullDirectoryPath != null)
                    vfs.CreateDirectory(fullDirectoryPath);

                if (getFileContents != null)
                    vfs.WriteAllFileBytes(fullPath, getFileContents(vfs, entryPath));
                else
                    vfs.CreateFile(fullPath).Dispose();
            }
        }
    }

    public static byte[] GetDefaultFileContents(IReadOnlyFileSystemView vfs, string path)
    {
        return Encoding.UTF8.GetBytes(
            string.Format(
                "This is default contents of the '{0}' file.",
                vfs.GetFileName(path)));
    }

    public static DateTime SpecialUtcTime1 { get; } = GetSpecialUtcTime(1);

    public static DateTime SpecialUtcTime2 { get; } = GetSpecialUtcTime(2);

    public static DateTime SpecialUtcTime3 { get; } = GetSpecialUtcTime(3);

    static DateTime GetSpecialUtcTime(int n) => new(2000, 1, 1, 0, n, 0, DateTimeKind.Utc);
}

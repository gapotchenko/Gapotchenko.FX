// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Utils;

static class IOHelper
{
    public static void CopyFileOptimized(
        IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite)
    {
        if (sourceView == destinationView)
            destinationView.CopyFile(sourcePath, destinationPath, overwrite);
        else
            CopyFileNaive(sourceView, sourcePath, destinationView, destinationPath, overwrite);
    }

    public static void CopyFileNaive(
        IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite)
    {
        using var sourceStream = sourceView.OpenFileRead(sourcePath);
        using var destinationStream = destinationView.OpenFile(
            destinationPath,
            overwrite ? FileMode.Create : FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);

        sourceStream.CopyTo(destinationStream);
    }
}

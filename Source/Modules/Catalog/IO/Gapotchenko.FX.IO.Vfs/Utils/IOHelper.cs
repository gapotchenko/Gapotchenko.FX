// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.IO.Vfs.Utils;

static class IOHelper
{
    public static void MoveDirectoryOptimized(
        IFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite)
    {
        if (sourceView == destinationView)
            destinationView.MoveDirectory(sourcePath, destinationPath, overwrite);
        else
            MoveDirectoryNaive(sourceView, sourcePath, destinationView, destinationPath, overwrite);
    }

    public static void MoveDirectoryNaive(
        IFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite)
    {
        if (!sourceView.DirectoryExists(sourcePath))
        {
            // Bailout early when the source directory cannot be read
            // to avoid making unwarranted modifications in the destination.
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindDirectory(sourcePath));
        }

        if (destinationView.DirectoryExists(destinationPath))
        {
            if (overwrite)
                destinationView.DeleteDirectory(destinationPath, true);
            else
                throw new IOException(VfsResourceKit.DirectoryAlreadyExists(destinationPath));
        }
        else
        {
            VfsValidationKit.Arguments.ValidatePath(destinationPath);
        }

        MoveDirectoryCore(sourcePath, destinationPath);

        void MoveDirectoryCore(string sourcePath, string destinationPath)
        {
            destinationView.CreateDirectory(destinationPath);

            foreach (string sourceEntryPath in sourceView.EnumerateEntries(sourcePath))
            {
                string destinationEntryPath = destinationView.CombinePaths(
                    destinationPath,
                    Path.GetFileName(sourceEntryPath));

                if (sourceView.FileExists(sourceEntryPath))
                    sourceView.MoveFile(sourceEntryPath, destinationView, destinationEntryPath, overwrite);
                else
                    MoveDirectoryCore(sourceEntryPath, destinationEntryPath);
            }

            CopyEntryAttributes(sourceView, sourcePath, destinationView, destinationPath);
            sourceView.DeleteDirectory(sourcePath);
        }
    }

    public static void CopyDirectoryOptimized(
        IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite)
    {
        if (sourceView == destinationView)
            destinationView.CopyDirectory(sourcePath, destinationPath, overwrite);
        else
            CopyDirectoryNaive(sourceView, sourcePath, destinationView, destinationPath, overwrite);
    }

    public static void CopyDirectoryNaive(
        IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite)
    {
        if (!sourceView.DirectoryExists(sourcePath))
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindDirectory(sourcePath));

        if (!overwrite && destinationView.DirectoryExists(destinationPath))
            throw new IOException(VfsResourceKit.DirectoryAlreadyExists(destinationPath));
        else
            VfsValidationKit.Arguments.ValidatePath(destinationPath);

        CopyDirectoryCore(sourcePath, destinationPath);

        void CopyDirectoryCore(string sourcePath, string destinationPath)
        {
            destinationView.CreateDirectory(destinationPath);

            foreach (string sourceEntryPath in sourceView.EnumerateEntries(sourcePath))
            {
                string destinationEntryPath = destinationView.CombinePaths(
                    destinationPath,
                    Path.GetFileName(sourceEntryPath));

                if (sourceView.FileExists(sourceEntryPath))
                    sourceView.CopyFile(sourceEntryPath, destinationView, destinationEntryPath, overwrite);
                else
                    CopyDirectoryCore(sourceEntryPath, destinationEntryPath);
            }

            CopyEntryAttributes(sourceView, sourcePath, destinationView, destinationPath);
        }
    }

    public static void MoveFileOptimized(
        IFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite)
    {
        if (sourceView == destinationView)
            destinationView.MoveFile(sourcePath, destinationPath, overwrite);
        else
            MoveFileNaive(sourceView, sourcePath, destinationView, destinationPath, overwrite);
    }

    public static void MoveFileNaive(
        IFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite)
    {
        sourceView.CopyFile(sourcePath, destinationView, destinationPath, overwrite);
        try
        {
            sourceView.DeleteFile(sourcePath);
        }
        catch
        {
            // Rollback the copy if the move is not possible to complete.
            destinationView.DeleteFile(destinationPath);
            throw;
        }
    }

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
        using (var sourceStream = sourceView.ReadFile(sourcePath))
        using (var destinationStream = destinationView.OpenFile(
            destinationPath,
            overwrite ? FileMode.Create : FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None))
        {
            sourceStream.CopyTo(destinationStream);
        }

        CopyEntryAttributes(sourceView, sourcePath, destinationView, destinationPath);
    }

    static void CopyEntryAttributes(
        IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath)
    {
        if (sourceView.SupportsLastWriteTime && destinationView.SupportsLastWriteTime)
        {
            var lastWriteTime = sourceView.GetLastWriteTime(sourcePath);
            if (lastWriteTime == DateTime.MinValue)
                throw new FileNotFoundException(VfsResourceKit.CouldNotFindFile(sourcePath));
            destinationView.SetLastWriteTime(destinationPath, lastWriteTime);
        }
    }
}

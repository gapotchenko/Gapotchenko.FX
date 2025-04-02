// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.IO.Vfs.Utils;

static class IOHelper
{
    #region Directory

    #region Move

    public static void MoveDirectoryOptimized(
        IFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite,
        VfsMoveOptions options)
    {
        if (sourceView == destinationView)
            destinationView.MoveDirectory(sourcePath, destinationPath, overwrite, options);
        else
            MoveDirectoryNaive(sourceView, sourcePath, destinationView, destinationPath, overwrite, options);
    }

    public static void MoveDirectoryNaive(
        IFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite,
        VfsMoveOptions options)
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
            {
                destinationView.DeleteDirectory(destinationPath, true);
                if (sourceView == destinationView)
                {
                    // Give an opportunity to use a more optimized operation implementation
                    // that exists but does not support overwriting.
                    destinationView.MoveDirectory(sourcePath, destinationPath, false, options);
                    return;
                }
            }
            else
            {
                throw new IOException(VfsResourceKit.DirectoryAlreadyExists(destinationPath));
            }
        }
        else
        {
            VfsValidationKit.Arguments.ValidatePath(destinationPath);
        }

        MoveDirectoryCore(sourcePath, destinationPath);

        void MoveDirectoryCore(string sourcePath, string destinationPath)
        {
            var metadata = EntryMetadata.GetFrom(sourceView, sourcePath, destinationView);

            destinationView.CreateDirectory(destinationPath);

            foreach (string sourceEntryPath in sourceView.EnumerateEntries(sourcePath))
            {
                string destinationEntryPath = destinationView.CombinePaths(
                    destinationPath,
                    Path.GetFileName(sourceEntryPath));

                if (sourceView.FileExists(sourceEntryPath))
                    sourceView.MoveFile(sourceEntryPath, destinationView, destinationEntryPath, overwrite, options);
                else
                    MoveDirectoryCore(sourceEntryPath, destinationEntryPath);
            }

            CopyEntryAttributes(sourceView, sourcePath, destinationView, destinationPath);
            metadata.SetTo(destinationView, destinationPath);

            sourceView.DeleteDirectory(sourcePath);
        }
    }

    #endregion

    #region Copy

    public static void CopyDirectoryOptimized(
        IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite,
        VfsCopyOptions options)
    {
        if (sourceView == destinationView)
            destinationView.CopyDirectory(sourcePath, destinationPath, overwrite, options);
        else
            CopyDirectoryNaive(sourceView, sourcePath, destinationView, destinationPath, overwrite, options);
    }

    public static void CopyDirectoryNaive(
        IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite,
        VfsCopyOptions options)
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
            EntryMetadata? metadata = (options & VfsCopyOptions.Archive) != 0
                ? EntryMetadata.GetFrom(sourceView, sourcePath, destinationView)
                : null;

            destinationView.CreateDirectory(destinationPath);

            foreach (string sourceEntryPath in sourceView.EnumerateEntries(sourcePath))
            {
                string destinationEntryPath = destinationView.CombinePaths(
                    destinationPath,
                    Path.GetFileName(sourceEntryPath));

                if (sourceView.FileExists(sourceEntryPath))
                    sourceView.CopyFile(sourceEntryPath, destinationView, destinationEntryPath, overwrite, options);
                else
                    CopyDirectoryCore(sourceEntryPath, destinationEntryPath);
            }

            CopyEntryAttributes(sourceView, sourcePath, destinationView, destinationPath);
            metadata?.SetTo(destinationView, destinationPath);
        }
    }

    #endregion

    #endregion

    #region File

    #region Move

    public static void MoveFileOptimized(
        IFileSystemView sourceView, string sourcePath,
        IFileSystemView destinationView, string destinationPath,
        bool overwrite,
        VfsMoveOptions options)
    {
        if (sourceView == destinationView)
            destinationView.MoveFile(sourcePath, destinationPath, overwrite, options);
        else
            MoveFileNaive(sourceView, sourcePath, destinationView, destinationPath, overwrite, options);
    }

    public static void MoveFileNaive(
        IFileSystemView sourceView, string sourcePath,
        IFileSystemView destinationView, string destinationPath,
        bool overwrite,
        VfsMoveOptions options)
    {
        // Copy to the destination.
        sourceView.CopyFile(sourcePath, destinationView, destinationPath, overwrite, VfsCopyOptions.Archive);

        // Delete from the source.
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

    #endregion

    #region Copy

    public static void CopyFileOptimized(
        IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite,
        VfsCopyOptions options)
    {
        if (sourceView == destinationView)
            destinationView.CopyFile(sourcePath, destinationPath, overwrite, options);
        else
            CopyFileNaive(sourceView, sourcePath, destinationView, destinationPath, overwrite, options);
    }

    public static void CopyFileNaive(
        IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath,
        bool overwrite,
        VfsCopyOptions options)
    {
        if ((options & VfsCopyOptions.Archive) != 0)
        {
            var metadata = EntryMetadata.GetFrom(sourceView, sourcePath, destinationView);

            CopyFileOptimized(
                sourceView, sourcePath,
                destinationView, destinationPath,
                overwrite,
                options & ~VfsCopyOptions.Archive);

            metadata.SetTo(destinationView, destinationPath);
        }
        else
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
    }

    #endregion

    #endregion

    #region Entry

    static void CopyEntryAttributes(
        IReadOnlyFileSystemView sourceView,
        string sourcePath,
        IFileSystemView destinationView,
        string destinationPath)
    {
        if (sourceView.SupportsLastWriteTime && destinationView.SupportsLastWriteTime)
        {
            var lastWriteTime = sourceView.GetLastWriteTime(sourcePath);
            EnsureEntryExist(sourcePath, lastWriteTime);
            destinationView.SetLastWriteTime(destinationPath, lastWriteTime);
        }
    }

    /// <summary>
    /// Represents file-system entry metadata other than the last write time.
    /// </summary>
    readonly struct EntryMetadata
    {
        public static EntryMetadata GetFrom(
            IReadOnlyFileSystemView view,
            string path,
            IReadOnlyFileSystemView? capabilitiesHint = null) =>
            new(view, path, capabilitiesHint);

        EntryMetadata(IReadOnlyFileSystemView view, string path, IReadOnlyFileSystemView? capabilitiesHint)
        {
            if (view.SupportsCreationTime && (capabilitiesHint?.SupportsCreationTime ?? true))
            {
                var creationTime = view.GetCreationTime(path);
                EnsureEntryExist(path, creationTime);
                CreationTime = creationTime;
            }

            if (view.SupportsLastAccessTime && (capabilitiesHint?.SupportsLastAccessTime ?? true))
            {
                var lastAccessTime = view.GetLastAccessTime(path);
                EnsureEntryExist(path, lastAccessTime);
                LastAccessTime = lastAccessTime;
            }
        }

        public void SetTo(IFileSystemView view, string path)
        {
            if (CreationTime.HasValue && view.SupportsCreationTime)
                view.SetCreationTime(path, CreationTime.Value);

            if (LastAccessTime.HasValue && view.SupportsLastAccessTime)
                view.SetLastAccessTime(path, LastAccessTime.Value);
        }

        public DateTime? CreationTime { get; }
        public DateTime? LastAccessTime { get; }
    }

    static void EnsureEntryExist(string path, DateTime time)
    {
        if (time == DateTime.MinValue)
            throw new FileNotFoundException(VfsResourceKit.CouldNotFindFile(path), path);
    }

    #endregion
}

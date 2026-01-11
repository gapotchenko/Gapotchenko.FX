// Gapotchenko.FX
//
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
        in VfsLocation source,
        in VfsLocation destination,
        bool overwrite,
        VfsMoveOptions options)
    {
        if (source.View == destination.View)
            destination.View.MoveDirectory(source.Path, destination.Path, overwrite, options);
        else
            MoveDirectoryNaive(source, destination, overwrite, options);
    }

    public static void MoveDirectoryNaive(
        in VfsLocation source,
        in VfsLocation destination,
        bool overwrite,
        VfsMoveOptions options)
    {
        var (sourceView, sourcePath) = source;
        if (!sourceView.DirectoryExists(sourcePath))
        {
            // Bailout early when the source directory cannot be read
            // to avoid making unwarranted modifications in the destination.
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindDirectory(sourcePath));
        }

        var (destinationView, destinationPath) = destination;
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
            var sourceLocation = new VfsReadOnlyLocation(sourceView, sourcePath);
            var metadata = EntryMetadata.GetFrom(sourceLocation, destinationView);

            destinationView.CreateDirectory(destinationPath);

            foreach (string sourceEntryPath in sourceView.EnumerateEntries(sourcePath))
            {
                string destinationEntryPath = destinationView.CombinePaths(
                    destinationPath,
                    sourceView.GetFileName(sourceEntryPath));

                if (sourceView.FileExists(sourceEntryPath))
                    sourceView.MoveFile(sourceEntryPath, new VfsLocation(destinationView, destinationEntryPath), overwrite, options);
                else
                    MoveDirectoryCore(sourceEntryPath, destinationEntryPath);
            }

            var destinationLocation = new VfsLocation(destinationView, destinationPath);
            CopyEntryAttributes(sourceLocation, destinationLocation);
            metadata.SetTo(destinationLocation);

            sourceView.DeleteDirectory(sourcePath);
        }
    }

    public static async Task MoveDirectoryNaiveAsync(
        VfsLocation source,
        VfsLocation destination,
        bool overwrite,
        VfsMoveOptions options,
        CancellationToken cancellationToken)
    {
        var (sourceView, sourcePath) = source;
        if (!await sourceView.DirectoryExistsAsync(sourcePath, cancellationToken).ConfigureAwait(false))
        {
            // Bailout early when the source directory cannot be read
            // to avoid making unwarranted modifications in the destination.
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindDirectory(sourcePath));
        }

        var (destinationView, destinationPath) = destination;
        if (await destinationView.DirectoryExistsAsync(destinationPath, cancellationToken).ConfigureAwait(false))
        {
            if (overwrite)
            {
                await destinationView.DeleteDirectoryAsync(destinationPath, true, cancellationToken).ConfigureAwait(false);
                if (sourceView == destinationView)
                {
                    // Give an opportunity to use a more optimized operation implementation
                    // that exists but does not support overwriting.
                    // Note: MoveDirectoryAsync doesn't exist yet, so we fall through to naive implementation.
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

        await MoveDirectoryCoreAsync(sourcePath, destinationPath, cancellationToken).ConfigureAwait(false);

        async Task MoveDirectoryCoreAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            var sourceLocation = new VfsReadOnlyLocation(sourceView, sourcePath);
            var metadata = await EntryMetadata.GetFromAsync(sourceLocation, destinationView, cancellationToken).ConfigureAwait(false);

            await destinationView.CreateDirectoryAsync(destinationPath, cancellationToken).ConfigureAwait(false);

            await foreach (string sourceEntryPath in sourceView.EnumerateEntriesAsync(sourcePath, cancellationToken).ConfigureAwait(false))
            {
                string destinationEntryPath = destinationView.CombinePaths(
                    destinationPath,
                    sourceView.GetFileName(sourceEntryPath));

                if (await sourceView.FileExistsAsync(sourceEntryPath, cancellationToken).ConfigureAwait(false))
                    await sourceView.MoveFileAsync(sourceEntryPath, new VfsLocation(destinationView, destinationEntryPath), overwrite, options, cancellationToken).ConfigureAwait(false);
                else
                    await MoveDirectoryCoreAsync(sourceEntryPath, destinationEntryPath, cancellationToken).ConfigureAwait(false);
            }

            var destinationLocation = new VfsLocation(destinationView, destinationPath);
            await CopyEntryAttributesAsync(sourceLocation, destinationLocation, cancellationToken).ConfigureAwait(false);
            await metadata.SetToAsync(destinationLocation, cancellationToken).ConfigureAwait(false);

            await sourceView.DeleteDirectoryAsync(sourcePath, false, cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    #region Copy

    public static void CopyDirectoryOptimized(
        in VfsReadOnlyLocation source,
        in VfsLocation destination,
        bool overwrite,
        VfsCopyOptions options)
    {
        if (source.View == destination.View)
            destination.View.CopyDirectory(source.Path, destination.Path, overwrite, options);
        else
            CopyDirectoryNaive(source, destination, overwrite, options);
    }

    public static Task CopyDirectoryOptimizedAsync(
        VfsReadOnlyLocation source,
        VfsLocation destination,
        bool overwrite,
        VfsCopyOptions options,
        CancellationToken cancellationToken)
    {
        if (source.View == destination.View)
            return destination.View.CopyDirectoryAsync(source.Path, destination.Path, overwrite, options, cancellationToken);
        else
            return CopyDirectoryNaiveAsync(source, destination, overwrite, options, cancellationToken);
    }

    public static void CopyDirectoryNaive(
        in VfsReadOnlyLocation source,
        in VfsLocation destination,
        bool overwrite,
        VfsCopyOptions options)
    {
        var (sourceView, sourcePath) = source;
        if (!sourceView.DirectoryExists(sourcePath))
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindDirectory(sourcePath));

        var (destinationView, destinationPath) = destination;
        if (!overwrite && destinationView.DirectoryExists(destinationPath))
            throw new IOException(VfsResourceKit.DirectoryAlreadyExists(destinationPath));
        else
            VfsValidationKit.Arguments.ValidatePath(destinationPath);

        CopyDirectoryCore(sourcePath, destinationPath);

        void CopyDirectoryCore(string sourcePath, string destinationPath)
        {
            var sourceLocation = new VfsReadOnlyLocation(sourceView, sourcePath);
            EntryMetadata? metadata = (options & VfsCopyOptions.Archive) != 0
                ? EntryMetadata.GetFrom(sourceLocation, destinationView)
                : null;

            destinationView.CreateDirectory(destinationPath);

            foreach (string sourceEntryPath in sourceView.EnumerateEntries(sourcePath))
            {
                string destinationEntryPath = destinationView.CombinePaths(
                    destinationPath,
                    sourceView.GetFileName(sourceEntryPath));

                if (sourceView.FileExists(sourceEntryPath))
                    sourceView.CopyFile(sourceEntryPath, new VfsLocation(destinationView, destinationEntryPath), overwrite, options);
                else
                    CopyDirectoryCore(sourceEntryPath, destinationEntryPath);
            }

            var destinationLocation = new VfsLocation(destinationView, destinationPath);
            CopyEntryAttributes(sourceLocation, destinationLocation);
            metadata?.SetTo(destinationLocation);
        }
    }

    public static async Task CopyDirectoryNaiveAsync(
        VfsReadOnlyLocation source,
        VfsLocation destination,
        bool overwrite,
        VfsCopyOptions options,
        CancellationToken cancellationToken)
    {
        var (sourceView, sourcePath) = source;
        if (!await sourceView.DirectoryExistsAsync(sourcePath, cancellationToken).ConfigureAwait(false))
            throw new DirectoryNotFoundException(VfsResourceKit.CouldNotFindDirectory(sourcePath));

        var (destinationView, destinationPath) = destination;
        if (!overwrite && await destinationView.DirectoryExistsAsync(destinationPath, cancellationToken).ConfigureAwait(false))
            throw new IOException(VfsResourceKit.DirectoryAlreadyExists(destinationPath));
        else
            VfsValidationKit.Arguments.ValidatePath(destinationPath);

        await CopyDirectoryCoreAsync(sourcePath, destinationPath, cancellationToken).ConfigureAwait(false);

        async Task CopyDirectoryCoreAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
        {
            var sourceLocation = new VfsReadOnlyLocation(sourceView, sourcePath);
            EntryMetadata? metadata = (options & VfsCopyOptions.Archive) != 0
                ? await EntryMetadata.GetFromAsync(sourceLocation, destinationView, cancellationToken).ConfigureAwait(false)
                : null;

            await destinationView.CreateDirectoryAsync(destinationPath, cancellationToken).ConfigureAwait(false);

            await foreach (string sourceEntryPath in sourceView.EnumerateEntriesAsync(sourcePath, cancellationToken).ConfigureAwait(false))
            {
                string destinationEntryPath = destinationView.CombinePaths(
                    destinationPath,
                    sourceView.GetFileName(sourceEntryPath));

                if (await sourceView.FileExistsAsync(sourceEntryPath, cancellationToken).ConfigureAwait(false))
                    await sourceView.CopyFileAsync(sourceEntryPath, new VfsLocation(destinationView, destinationEntryPath), overwrite, options, cancellationToken).ConfigureAwait(false);
                else
                    await CopyDirectoryCoreAsync(sourceEntryPath, destinationEntryPath, cancellationToken).ConfigureAwait(false);
            }

            var destinationLocation = new VfsLocation(destinationView, destinationPath);
            await CopyEntryAttributesAsync(sourceLocation, destinationLocation, cancellationToken).ConfigureAwait(false);
            if (metadata is { } actualMetadata)
                await actualMetadata.SetToAsync(destinationLocation, cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    #endregion

    #region File

    #region Move

    public static void MoveFileOptimized(
        in VfsLocation source,
        in VfsLocation destination,
        bool overwrite,
        VfsMoveOptions options)
    {
        if (source.View == destination.View)
            destination.View.MoveFile(source.Path, destination.Path, overwrite, options);
        else
            MoveFileNaive(source, destination, overwrite, options);
    }

    public static Task MoveFileOptimizedAsync(
        VfsLocation source,
        VfsLocation destination,
        bool overwrite,
        VfsMoveOptions options,
        CancellationToken cancellationToken)
    {
        if (source.View == destination.View)
            return destination.View.MoveFileAsync(source.Path, destination.Path, overwrite, options, cancellationToken);
        else
            return MoveFileNaiveAsync(source, destination, overwrite, options, cancellationToken);
    }

    public static void MoveFileNaive(
        in VfsLocation source,
        in VfsLocation destination,
        bool overwrite,
        VfsMoveOptions options)
    {
        var (sourceView, sourcePath) = source;

        // Copy to the destination.
        sourceView.CopyFile(sourcePath, destination, overwrite, VfsCopyOptions.Archive);

        // Delete from the source.
        try
        {
            sourceView.DeleteFile(sourcePath);
        }
        catch
        {
            // Rollback the copy if the move is not possible to complete.
            destination.View.DeleteFile(destination.Path);
            throw;
        }
    }

    public static async Task MoveFileNaiveAsync(
        VfsLocation source,
        VfsLocation destination,
        bool overwrite,
        VfsMoveOptions options,
        CancellationToken cancellationToken)
    {
        var (sourceView, sourcePath) = source;

        // Copy to the destination.
        await sourceView.CopyFileAsync(
            sourcePath,
            destination,
            overwrite,
            VfsCopyOptions.Archive,
            cancellationToken).ConfigureAwait(false);

        // Delete from the source.
        try
        {
            await sourceView.DeleteFileAsync(sourcePath, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // Rollback the copy if the move is not possible to complete.
            await destination.View.DeleteFileAsync(destination.Path, cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    #endregion

    #region Copy

    public static void CopyFileOptimized(
        in VfsReadOnlyLocation source,
        in VfsLocation destination,
        bool overwrite,
        VfsCopyOptions options)
    {
        if (source.View == destination.View)
            destination.View.CopyFile(source.Path, destination.Path, overwrite, options);
        else
            CopyFileNaive(source, destination, overwrite, options);
    }

    public static Task CopyFileOptimizedAsync(
        VfsReadOnlyLocation source,
        VfsLocation destination,
        bool overwrite,
        VfsCopyOptions options,
        CancellationToken cancellationToken)
    {
        if (source.View == destination.View)
            return destination.View.CopyFileAsync(source.Path, destination.Path, overwrite, options, cancellationToken);
        else
            return CopyFileNaiveAsync(source, destination, overwrite, options, cancellationToken);
    }

    public static void CopyFileNaive(
        in VfsReadOnlyLocation source,
        in VfsLocation destination,
        bool overwrite,
        VfsCopyOptions options)
    {
        if ((options & VfsCopyOptions.Archive) != 0)
        {
            var metadata = EntryMetadata.GetFrom(source, destination.View);

            CopyFileOptimized(
                source,
                destination,
                overwrite,
                options & ~VfsCopyOptions.Archive);

            metadata.SetTo(destination);
        }
        else
        {
            using (var sourceStream = source.View.ReadFile(source.Path))
            using (var destinationStream = destination.View.OpenFile(
                destination.Path,
                overwrite ? FileMode.Create : FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                sourceStream.CopyTo(destinationStream);
            }

            CopyEntryAttributes(source, destination);
        }
    }

    public static async Task CopyFileNaiveAsync(
        VfsReadOnlyLocation source,
        VfsLocation destination,
        bool overwrite,
        VfsCopyOptions options,
        CancellationToken cancellationToken)
    {
        if ((options & VfsCopyOptions.Archive) != 0)
        {
            var metadata = await EntryMetadata.GetFromAsync(source, destination.View, cancellationToken).ConfigureAwait(false);

            await CopyFileOptimizedAsync(
                source,
                destination,
                overwrite,
                options & ~VfsCopyOptions.Archive,
                cancellationToken).ConfigureAwait(false);

            await metadata.SetToAsync(destination, cancellationToken).ConfigureAwait(false);
        }
        else
        {
#if TFF_ASYNC_DISPOSABLE
            await
#endif
            using (var sourceStream = await source.View.ReadFileAsync(source.Path, cancellationToken).ConfigureAwait(false))
#if TFF_ASYNC_DISPOSABLE
            await
#endif
            using (var destinationStream = await destination.View.OpenFileAsync(
                destination.Path,
                overwrite ? FileMode.Create : FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                cancellationToken).ConfigureAwait(false))
            {
                await sourceStream.CopyToAsync(
                    destinationStream,
#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER)
                    65536,
#endif
                    cancellationToken)
                    .ConfigureAwait(false);
            }

            await CopyEntryAttributesAsync(source, destination, cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    #endregion

    #region Entry

    static void CopyEntryAttributes(
        in VfsReadOnlyLocation source,
        in VfsLocation destination)
    {
        if (source.View.SupportsLastWriteTime && destination.View.SupportsLastWriteTime)
        {
            var lastWriteTime = source.View.GetLastWriteTime(source.Path);
            EnsureEntryExist(source.Path, lastWriteTime);
            destination.View.SetLastWriteTime(destination.Path, lastWriteTime);
        }
    }

    static async Task CopyEntryAttributesAsync(
        VfsReadOnlyLocation source,
        VfsLocation destination,
        CancellationToken cancellationToken)
    {
        if (source.View.SupportsLastWriteTime && destination.View.SupportsLastWriteTime)
        {
            var lastWriteTime = await source.View.GetLastWriteTimeAsync(source.Path, cancellationToken).ConfigureAwait(false);
            EnsureEntryExist(source.Path, lastWriteTime);
            await destination.View.SetLastWriteTimeAsync(destination.Path, lastWriteTime, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Represents file-system entry metadata other than the last write time.
    /// </summary>
    readonly struct EntryMetadata
    {
        public static EntryMetadata GetFrom(
            in VfsReadOnlyLocation location,
            IReadOnlyFileSystemView? capabilitiesHint)
        {
            var (view, path) = location;

            DateTime? creationTime = null;
            if (view.SupportsCreationTime && (capabilitiesHint?.SupportsCreationTime ?? true))
            {
                var time = view.GetCreationTime(path);
                EnsureEntryExist(location, time);
                creationTime = time;
            }

            DateTime? lastAccessTime = null;
            if (view.SupportsLastAccessTime && (capabilitiesHint?.SupportsLastAccessTime ?? true))
            {
                var time = view.GetLastAccessTime(path);
                EnsureEntryExist(location, time);
                lastAccessTime = time;
            }

            return new(creationTime, lastAccessTime);
        }

        public static async Task<EntryMetadata> GetFromAsync(
            VfsReadOnlyLocation location,
            IReadOnlyFileSystemView? capabilitiesHint,
            CancellationToken cancellationToken)
        {
            var (view, path) = location;

            DateTime? creationTime = null;
            if (view.SupportsCreationTime && (capabilitiesHint?.SupportsCreationTime ?? true))
            {
                var time = await view.GetCreationTimeAsync(path, cancellationToken).ConfigureAwait(false);
                EnsureEntryExist(location, time);
                creationTime = time;
            }

            DateTime? lastAccessTime = null;
            if (view.SupportsLastAccessTime && (capabilitiesHint?.SupportsLastAccessTime ?? true))
            {
                var time = await view.GetLastAccessTimeAsync(path, cancellationToken).ConfigureAwait(false);
                EnsureEntryExist(location, time);
                lastAccessTime = time;
            }

            return new(creationTime, lastAccessTime);
        }

        EntryMetadata(DateTime? creationTime, DateTime? lastAccessTime)
        {
            CreationTime = creationTime;
            LastAccessTime = lastAccessTime;
        }

        public void SetTo(in VfsLocation location)
        {
            var (view, path) = location;

            if (CreationTime.HasValue && view.SupportsCreationTime)
                view.SetCreationTime(path, CreationTime.Value);

            if (LastAccessTime.HasValue && view.SupportsLastAccessTime)
                view.SetLastAccessTime(path, LastAccessTime.Value);
        }

        public async Task SetToAsync(VfsLocation location, CancellationToken cancellationToken)
        {
            var (view, path) = location;

            if (CreationTime.HasValue && view.SupportsCreationTime)
                await view.SetCreationTimeAsync(path, CreationTime.Value, cancellationToken);

            if (LastAccessTime.HasValue && view.SupportsLastAccessTime)
                await view.SetLastAccessTimeAsync(path, LastAccessTime.Value, cancellationToken);
        }

        public DateTime? CreationTime { get; }
        public DateTime? LastAccessTime { get; }
    }

    static void EnsureEntryExist(in VfsReadOnlyLocation location, DateTime time)
    {
        if (time == DateTime.MinValue)
        {
            string path = location.ToString();
            throw new FileNotFoundException(VfsResourceKit.CouldNotFindFile(path), path);
        }
    }

    #endregion
}

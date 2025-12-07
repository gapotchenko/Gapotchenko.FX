// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.Vfs.Utils;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides extension methods for <see cref="IVfsStorage{TVfs, TOptions}"/> interface.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class VfsStorageExtensions
{
    /// <summary>
    /// Opens an existing file representing a <typeparamref name="TVfs"/> storage for reading
    /// in the specified location.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="storage">The <see cref="IVfsStorage{TVfs, TOptions}"/> instance.</param>
    /// <param name="location">The location of a file to be opened for reading.</param>
    /// <param name="options">The storage options.</param>
    /// <returns>A read-only <typeparamref name="TVfs"/> instance opened in the specified location.</returns>
    public static TVfs ReadFile<TVfs, TOptions>(
        this IVfsStorage<TVfs, TOptions> storage,
        VfsReadOnlyLocation location,
        TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        ArgumentNullException.ThrowIfNull(storage);

        return storage.Format.Mount(location.View.ReadFile(location.Path), options: options);
    }

    /// <summary>
    /// Opens an existing file or creates a new file representing a <typeparamref name="TVfs"/> storage for writing
    /// in the specified location.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="storage">The <see cref="IVfsStorage{TVfs, TOptions}"/> instance.</param>
    /// <param name="location">The location of a file to be opened for writing.</param>
    /// <param name="options">The storage options.</param>
    /// <returns>An unshared <typeparamref name="TVfs"/> instance opened for the specified location with read/write access.</returns>
    public static TVfs WriteFile<TVfs, TOptions>(
        this IVfsStorage<TVfs, TOptions> storage,
        VfsLocation location,
        TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        ArgumentNullException.ThrowIfNull(storage);

        return WriteFileCore(storage.Format, location, FileAccess.ReadWrite, FileShare.None, options);
    }

    /// <summary>
    /// Creates or overwrites a file representing a <typeparamref name="TVfs"/> storage in the specified location.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="storage">The <see cref="IVfsStorage{TVfs, TOptions}"/> instance.</param>
    /// <param name="location">The location of the file create.</param>
    /// <param name="options">The storage options.</param>
    public static TVfs CreateFile<TVfs, TOptions>(
        this IVfsStorage<TVfs, TOptions> storage,
        VfsLocation location,
        TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        ArgumentNullException.ThrowIfNull(storage);

        return OpenNewFileCore(storage.Format, location, FileMode.Create, FileAccess.ReadWrite, FileShare.None, options);
    }

    /// <summary>
    /// Opens a file representing a <typeparamref name="TVfs"/> storage in the specified location,
    /// with the specified mode, access, and sharing options.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="storage">The <see cref="IVfsStorage{TVfs, TOptions}"/> instance.</param>
    /// <param name="location">The location of a file to open.</param>
    /// <param name="mode">The file mode.</param>
    /// <param name="access">The file access.</param>
    /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
    /// <param name="options">The storage options.</param>
    /// <returns>An <typeparamref name="TVfs"/> instance opened for the specified location with the specified mode, access, and sharing options taken into account.</returns>
    public static TVfs OpenFile<TVfs, TOptions>(
        this IVfsStorage<TVfs, TOptions> storage,
        VfsLocation location,
        FileMode mode,
        FileAccess access = FileAccess.ReadWrite,
        FileShare share = FileShare.None,
        TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        ArgumentNullException.ThrowIfNull(storage);

        VfsValidationKit.Arguments.ValidateFileAccess(access);
        if (access == FileAccess.Write)
            throw new ArgumentException("File storage cannot be opened when only write access is requested.", nameof(access));

        var format = storage.Format;

        switch (mode)
        {
            case FileMode.Open:
                return OpenExistingFileCore(format, location, mode, access, share, options);

            case FileMode.Create:
            case FileMode.CreateNew:
            case FileMode.Truncate:
                return OpenNewFileCore(format, location, mode, access, share, options);

            case FileMode.OpenOrCreate:
                return WriteFileCore(format, location, access, share, options);

            case FileMode.Append:
                throw new ArgumentException(
                    ResourceHelper.FileStorageCannotBeOpenedInMode(mode),
                    nameof(mode));

            default:
                VfsValidationKit.Arguments.ValidateFileMode(mode);
                throw new SwitchExpressionException(mode);
        }
    }

    static TVfs WriteFileCore<TVfs, TOptions>(
        IVfsFileStorageFormat<TVfs, TOptions> format,
        VfsLocation location,
        FileAccess access,
        FileShare share,
        TOptions? options)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        if (location.View.FileExists(location.Path))
            return OpenExistingFileCore(format, location, FileMode.Open, access, share, options);
        else
            return OpenNewFileCore(format, location, FileMode.CreateNew, access, share, options);
    }

    static TVfs OpenExistingFileCore<TVfs, TOptions>(
        IVfsFileStorageFormat<TVfs, TOptions> format,
        VfsLocation location,
        FileMode mode,
        FileAccess access,
        FileShare share,
        TOptions? options)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        return format.Mount(
            location.View.OpenFile(location.Path, mode, access, share),
            (access & FileAccess.Write) != 0,
            options: options);
    }

    static TVfs OpenNewFileCore<TVfs, TOptions>(
        IVfsFileStorageFormat<TVfs, TOptions> format,
        VfsLocation location,
        FileMode mode,
        FileAccess access,
        FileShare share,
        TOptions? options)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        return format.Create(
            location.View.OpenFile(location.Path, mode, access, share),
            options: options);
    }
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
using Gapotchenko.FX.IO.Vfs.Utils;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides extension methods for <see cref="IVfsFile{TVfs, TOptions}"/> interface.
/// </summary>
public static class VfsFileExtensions
{
    /// <summary>
    /// Opens an existing file representing a <typeparamref name="TVfs"/> storage for reading.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="file">The <see cref="IVfsFile{TVfs, TOptions}"/> instance.</param>
    /// <param name="path">The file to be opened for reading.</param>
    /// <param name="options">The storage options.</param>
    /// <returns>A read-only <typeparamref name="TVfs"/> instance opened for the specified path.</returns>
    public static TVfs OpenRead<TVfs, TOptions>(this IVfsFile<TVfs, TOptions> file, string path, TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions =>
        OpenRead(file, FileSystemView.Local, path, options);

    /// <summary>
    /// Opens an existing file representing a <typeparamref name="TVfs"/> storage for reading
    /// in the specified file-system view.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="file">The <see cref="IVfsFile{TVfs, TOptions}"/> instance.</param>
    /// <param name="view">The file-system view to open the file at.</param>
    /// <param name="path">The file to be opened for reading.</param>
    /// <param name="options">The storage options.</param>
    /// <returns>A read-only <typeparamref name="TVfs"/> instance opened for the specified path.</returns>
    public static TVfs OpenRead<TVfs, TOptions>(
        this IVfsFile<TVfs, TOptions> file,
        IReadOnlyFileSystemView view,
        string path,
        TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        if (file is null)
            throw new ArgumentNullException(nameof(file));

        return file.Format.Mount(view.ReadFile(path), options: options);
    }

    /// <summary>
    /// Opens an existing file or creates a new file representing a <typeparamref name="TVfs"/> storage for writing.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="file">The <see cref="IVfsFile{TVfs, TOptions}"/> instance.</param>
    /// <param name="path">The file to be opened for writing.</param>
    /// <param name="options">The storage options.</param>
    /// <returns>An unshared <typeparamref name="TVfs"/> instance opened for the specified path with read/write access.</returns>
    public static TVfs OpenWrite<TVfs, TOptions>(this IVfsFile<TVfs, TOptions> file, string path, TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions =>
        OpenWrite(file, FileSystemView.Local, path, options);

    /// <summary>
    /// Opens an existing file or creates a new file representing a <typeparamref name="TVfs"/> storage for writing
    /// in the specified file-system view.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="file">The <see cref="IVfsFile{TVfs, TOptions}"/> instance.</param>
    /// <param name="view">The file-system view to open the file at.</param>
    /// <param name="path">The file to be opened for writing.</param>
    /// <param name="options">The storage options.</param>
    /// <returns>An unshared <typeparamref name="TVfs"/> instance opened for the specified path with read/write access.</returns>
    public static TVfs OpenWrite<TVfs, TOptions>(
        this IVfsFile<TVfs, TOptions> file,
        IFileSystemView view,
        string path,
        TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        if (file is null)
            throw new ArgumentNullException(nameof(file));

        return OpenWriteCore(file.Format, view, path, FileAccess.ReadWrite, FileShare.None, options);
    }

    /// <summary>
    /// Creates or overwrites a file representing a <typeparamref name="TVfs"/> storage in the specified path.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="file">The <see cref="IVfsFile{TVfs, TOptions}"/> instance.</param>
    /// <param name="path">The path of the file create.</param>
    /// <param name="options">The storage options.</param>
    public static TVfs Create<TVfs, TOptions>(
        this IVfsFile<TVfs, TOptions> file,
        string path,
        TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions =>
        Create(file, FileSystemView.Local, path, options);

    /// <summary>
    /// Creates or overwrites a file representing a <typeparamref name="TVfs"/> storage in the specified path
    /// of the file-system view.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="file">The <see cref="IVfsFile{TVfs, TOptions}"/> instance.</param>
    /// <param name="view">The file-system view to create the file at.</param>
    /// <param name="path">The path of the file create.</param>
    /// <param name="options">The storage options.</param>
    public static TVfs Create<TVfs, TOptions>(
        this IVfsFile<TVfs, TOptions> file,
        IFileSystemView view,
        string path,
        TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        if (file is null)
            throw new ArgumentNullException(nameof(file));

        return OpenNewCore(file.Format, view, path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, options);
    }

    /// <summary>
    /// Opens a file representing a <typeparamref name="TVfs"/> storage on the specified path,
    /// with the specified mode, access, and sharing options.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="file">The <see cref="IVfsFile{TVfs, TOptions}"/> instance.</param>
    /// <param name="path">The file to open.</param>
    /// <param name="mode">The file mode.</param>
    /// <param name="access">The file access.</param>
    /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
    /// <param name="options">The storage options.</param>
    /// <returns>An <typeparamref name="TVfs"/> instance opened for the specified path with the specified mode, access, and sharing options taken into account.</returns>
    public static TVfs Open<TVfs, TOptions>(
        this IVfsFile<TVfs, TOptions> file,
        string path,
        FileMode mode,
        FileAccess access = FileAccess.ReadWrite,
        FileShare share = FileShare.None,
        TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions =>
        Open(file, FileSystemView.Local, path, mode, access, share, options);

    /// <summary>
    /// Opens a file representing a <typeparamref name="TVfs"/> storage on the specified path
    /// in the specified file-system view,
    /// with the specified mode, access, and sharing options.
    /// </summary>
    /// <typeparam name="TVfs">The type of the file storage.</typeparam>
    /// <typeparam name="TOptions">The type of the file storage options.</typeparam>
    /// <param name="file">The <see cref="IVfsFile{TVfs, TOptions}"/> instance.</param>
    /// <param name="view">The file-system view to open the file at.</param>
    /// <param name="path">The file to open.</param>
    /// <param name="mode">The file mode.</param>
    /// <param name="access">The file access.</param>
    /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
    /// <param name="options">The storage options.</param>
    /// <returns>An <typeparamref name="TVfs"/> instance opened for the specified path with the specified mode, access, and sharing options taken into account.</returns>
    public static TVfs Open<TVfs, TOptions>(
        this IVfsFile<TVfs, TOptions> file,
        IFileSystemView view,
        string path,
        FileMode mode,
        FileAccess access = FileAccess.ReadWrite,
        FileShare share = FileShare.None,
        TOptions? options = null)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        if (file is null)
            throw new ArgumentNullException(nameof(file));

        VfsValidationKit.Arguments.ValidateFileAccess(access);
        if (access == FileAccess.Write)
            throw new ArgumentException("File storage cannot be opened when only write access is requested.", nameof(access));

        var format = file.Format;

        switch (mode)
        {
            case FileMode.Open:
                return OpenExistingCore(format, view, path, mode, access, share, options);

            case FileMode.Create:
            case FileMode.CreateNew:
            case FileMode.Truncate:
                return OpenNewCore(format, view, path, mode, access, share, options);

            case FileMode.OpenOrCreate:
                return OpenWriteCore(format, view, path, access, share, options);

            case FileMode.Append:
                throw new ArgumentException(
                    ResourceHelper.FileStorageCannotBeOpenedInMode(mode),
                    nameof(mode));

            default:
                VfsValidationKit.Arguments.ValidateFileMode(mode);
                throw new SwitchExpressionException(mode);
        }
    }

    static TVfs OpenWriteCore<TVfs, TOptions>(
        IVfsFileFormat<TVfs, TOptions> format,
        IFileSystemView view,
        string path,
        FileAccess access,
        FileShare share,
        TOptions? options)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        if (view.FileExists(path))
            return OpenExistingCore(format, view, path, FileMode.Open, access, share, options);
        else
            return OpenNewCore(format, view, path, FileMode.CreateNew, access, share, options);
    }

    static TVfs OpenExistingCore<TVfs, TOptions>(
        IVfsFileFormat<TVfs, TOptions> format,
        IFileSystemView view,
        string path,
        FileMode mode,
        FileAccess access,
        FileShare share,
        TOptions? options)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        return format.Mount(
            view.OpenFile(path, mode, access, share),
            (access & FileAccess.Write) != 0,
            options: options);
    }

    static TVfs OpenNewCore<TVfs, TOptions>(
        IVfsFileFormat<TVfs, TOptions> format,
        IFileSystemView view,
        string path,
        FileMode mode,
        FileAccess access,
        FileShare share,
        TOptions? options)
        where TVfs : IVirtualFileSystem
        where TOptions : VfsOptions
    {
        return format.Create(
            view.OpenFile(path, mode, access, share),
            options: options);
    }
}

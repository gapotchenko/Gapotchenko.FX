// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides description and operations for the virtual file system format.
/// </summary>
public interface IVfsFormat
{
    /// <summary>
    /// Creates a new file system in the specified stream.
    /// The existing data in the stream, if any, will be overwritten.
    /// </summary>
    /// <remarks>
    /// This method is equivalent to the <c>format</c> operation of a conventional file system.
    /// </remarks>
    /// <param name="stream">The stream.</param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the stream open after the <see cref="IVirtualFileSystem"/> object is disposed;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="options">The file system options.</param>
    /// <returns>The <see cref="IVirtualFileSystem"/> instance for the created file system.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    IVirtualFileSystem Create(Stream stream, bool leaveOpen = false, VfsOptions? options = null);

    /// <summary>
    /// Mounts (opens) an existing file system
    /// using the specified stream
    /// and with the <see cref="IFileSystemView.CanWrite"/> property set as specified.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To unmount the file system,
    /// the returned <see cref="IVirtualFileSystem"/> instance should be disposed.
    /// </para>
    /// <para>
    /// This method is equivalent to the <c>mount</c> operation of a conventional file system.
    /// </para>
    /// </remarks>
    /// <param name="stream">The stream.</param>
    /// <param name="writable">
    /// The setting of the <see cref="IFileSystemView.CanWrite"/> property, 
    /// which determines whether the file system supports writing.
    /// </param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the stream open after the <see cref="IVirtualFileSystem"/> object is disposed;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="options">The file system options.</param>
    /// <returns>The <see cref="IVirtualFileSystem"/> instance representing the mounted file system.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    IVirtualFileSystem Mount(Stream stream, bool writable = false, bool leaveOpen = false, VfsOptions? options = null);

    /// <summary>
    /// Determines whether the specified stream can be mounted using the current storage format.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="options">The file system options.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="stream"/> can be mounted using the current storage format;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException"><paramref name="stream"/> does not support seeking.</exception>
    bool IsMountable(Stream stream, VfsOptions? options = null);
}

/// <summary>
/// Provides strongly typed description and operations for the virtual file system format.
/// </summary>
/// <typeparam name="TVfs">The type of the virtual file system.</typeparam>
/// <typeparam name="TOptions">The type of the virtual file system options.</typeparam>
public interface IVfsFormat<out TVfs, TOptions> : IVfsFormat
    where TVfs : IVirtualFileSystem
    where TOptions : VfsOptions
{
    /// <summary>
    /// Creates a new <typeparamref name="TVfs"/> storage in the specified stream.
    /// The existing data in the stream, if any, will be overwritten.
    /// </summary>
    /// <param name="stream"><inheritdoc/></param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the stream open after the <typeparamref name="TVfs"/> object is disposed;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="options"><inheritdoc/></param>
    /// <returns>The <typeparamref name="TVfs"/> instance for the created storage.</returns>
    /// <inheritdoc cref="IVfsFormat.Create(Stream, bool, VfsOptions?)"/>
    TVfs Create(Stream stream, bool leaveOpen = false, TOptions? options = null);

    /// <summary>
    /// Mounts (opens) an existing <typeparamref name="TVfs"/> storage
    /// using the specified stream.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To unmount the <typeparamref name="TVfs"/> storage,
    /// the returned instance should be disposed.
    /// </para>
    /// <para>
    /// This method is equivalent to the <c>mount</c> operation of a conventional file system.
    /// </para>
    /// </remarks>
    /// <param name="stream"><inheritdoc/></param>
    /// <param name="writable"><inheritdoc/></param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the stream open after the <typeparamref name="TVfs"/> object is disposed;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="options"><inheritdoc/></param>
    /// <returns>The <typeparamref name="TVfs"/> instance representing the mounted storage.</returns>
    /// <inheritdoc cref="IVfsFormat.Mount(Stream, bool, bool, VfsOptions?)"/>
    TVfs Mount(Stream stream, bool writable = false, bool leaveOpen = false, TOptions? options = null);

    /// <inheritdoc cref="IVfsFormat.IsMountable(Stream, VfsOptions?)"/>
    bool IsMountable(Stream stream, TOptions? options = null);
}

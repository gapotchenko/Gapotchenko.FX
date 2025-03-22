// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides operations describing and supporting a particular virtual file system format.
/// </summary>
public interface IVfsFormat
{
    /// <summary>
    /// Creates a new file system in the specified stream.
    /// The existing data in the stream, if any, will be overwritten.
    /// </summary>
    /// <remarks>
    /// This method is equivalent to <c>format</c> operation of a conventional file system.
    /// </remarks>
    /// <param name="stream">The stream.</param>
    /// <returns>The <see cref="IVirtualFileSystem"/> instance for the created file system.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    IVirtualFileSystem Create(Stream stream);

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
    /// This method is equivalent to <c>mount</c> operation of a conventional file system.
    /// </para>
    /// </remarks>
    /// <param name="stream">The stream.</param>
    /// <param name="writable">
    /// The setting of the <see cref="IFileSystemView.CanWrite"/> property, 
    /// which determines whether the file system supports writing.
    /// </param>
    /// <returns>The <see cref="IVirtualFileSystem"/> instance representing the mounted file system.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    IVirtualFileSystem Mount(Stream stream, bool writable);

    /// <summary>
    /// Determines whether the specified stream can be mounted using the current file storage format.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="stream"/> can be mounted using the current file storage format;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    bool IsMountable(Stream stream);
}

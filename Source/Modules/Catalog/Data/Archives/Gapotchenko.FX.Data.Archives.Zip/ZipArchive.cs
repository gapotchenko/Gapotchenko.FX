// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;
using System.IO.Compression;

namespace Gapotchenko.FX.Data.Archives.Zip;

/// <summary>
/// Represents a package of compressed files in the ZIP archive format.
/// </summary>
public sealed partial class ZipArchive :
    VirtualFileSystemProxyKit<IZipArchive>,
    IZipArchive,
    IStorageMountableDataArchive<IZipArchive, ZipArchiveOptions>
{
    // For now, the implementation is just a wrapper around System.IO.Compression.ZipArchive,
    // which is not ideal because System.IO.Compression.ZipArchive is somewhat lacking
    // in terms of supported compression methods and features.

    /// <summary>
    /// Initializes a new read-only instance of the <see cref="ZipArchive"/> class
    /// from the specified stream.
    /// </summary>
    /// <param name="stream">The stream that contains archive to read.</param>
    public ZipArchive(Stream stream) :
        this(stream, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchive"/> class
    /// from the specified stream
    /// and with the <see cref="IFileSystemView.CanWrite"/> property set as specified.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="writable">
    /// The setting of the <see cref="IFileSystemView.CanWrite"/> property, 
    /// which determines whether the archive supports writing.
    /// </param>
    public ZipArchive(Stream stream, bool writable) :
        this(stream, writable, false)
    {
    }

    internal ZipArchive(Stream stream, bool writable, bool leaveOpen, ZipArchiveOptions? options, VfsStorageContext? context) :
        this(stream, writable, leaveOpen)
    {
        if (options?.TrackLocation ?? true)
            Location = context?.Location;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchive"/> class
    /// from the specified stream and with the <see cref="IFileSystemView.CanWrite"/> property set as specified,
    /// and optionally leaves the stream open.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="writable">
    /// The setting of the <see cref="IFileSystemView.CanWrite"/> property, 
    /// which determines whether the archive supports writing.
    /// </param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the stream open after the <see cref="ZipArchive"/> object is disposed;
    /// otherwise, <see langword="false"/>.
    /// </param>
    public ZipArchive(Stream stream, bool writable, bool leaveOpen) :
        base(
            CreateViewOnBclImpl(
                stream ?? throw new ArgumentNullException(nameof(stream)),
                writable,
                leaveOpen))
    {
    }

    /// <inheritdoc/>
    public VfsReadOnlyLocation? Location { get; }

    static IZipArchiveView<System.IO.Compression.ZipArchive> CreateViewOnBclImpl(Stream stream, bool writable, bool leaveOpen)
    {
        var mode = GetZipArchiveMode(stream, writable);
        try
        {
            return CreateView(new System.IO.Compression.ZipArchive(stream, mode, leaveOpen));
        }
        catch
        {
            if (!leaveOpen)
                stream.Dispose();
            throw;
        }
    }

    static ZipArchiveMode GetZipArchiveMode(Stream stream, bool writable)
    {
        if (writable)
        {
            if (!stream.CanSeek)
                return ZipArchiveMode.Create;
            else
                return ZipArchiveMode.Update;
        }
        else
        {
            return ZipArchiveMode.Read;
        }
    }
}

// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.Vfs.Kits;
using System.IO.Compression;

namespace Gapotchenko.FX.Data.Compression.Zip;

/// <summary>
/// Represents a package of compressed files in the ZIP archive format.
/// </summary>
public sealed partial class ZipArchive :
    FileSystemViewProxyKit<IZipArchiveView<System.IO.Compression.ZipArchive>>,
    IZipArchive
{
    // For now, the implementation is just a wrapper around System.IO.Compression.ZipArchive.
    // Which is not ideal because System.IO.Compression.ZipArchive is somewhat lacking
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
    /// from the specified stream and with the <see cref="IFileSystemView.CanWrite"/> property set as specified.
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
            CreateViewOnBclImplementation(
                stream ?? throw new ArgumentNullException(nameof(stream)),
                writable,
                leaveOpen))
    {
    }

    static IZipArchiveView<System.IO.Compression.ZipArchive> CreateViewOnBclImplementation(
        Stream stream,
        bool writable,
        bool leaveOpen)
    {
        ZipArchiveMode mode;
        if (writable)
        {
            if (!stream.CanSeek)
                mode = ZipArchiveMode.Create;
            else
                mode = ZipArchiveMode.Update;
        }
        else
        {
            mode = ZipArchiveMode.Read;
        }

        return CreateView(
            new System.IO.Compression.ZipArchive(stream, mode, leaveOpen),
            leaveOpen);
    }

    /// <inheritdoc/>
    public void Dispose() => BaseView.Dispose();
}

// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs;
using System.IO.Compression;

namespace Gapotchenko.FX.Data.Archives.Zip;

partial class ZipArchive
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
        this(stream, writable, leaveOpen, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipArchive"/> class from the specified stream
    /// and with the <see cref="IFileSystemView.CanWrite"/> property set as specified,
    /// optionally leaves the stream open,
    /// and uses the specified options.
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
    /// <param name="options">The ZIP archive options.</param>
    public ZipArchive(Stream stream, bool writable, bool leaveOpen, ZipArchiveOptions? options) :
        this(stream, writable, leaveOpen, options, null)
    {
    }

    internal ZipArchive(Stream stream, bool writable, bool leaveOpen, ZipArchiveOptions? options, VfsStorageContext? context) :
        this(
            CreateViewOnBclImpl(
                stream ?? throw new ArgumentNullException(nameof(stream)),
                writable,
                leaveOpen,
                options),
            options,
            context)
    {
    }

    ZipArchive(IZipArchive baseView, ZipArchiveOptions? options, VfsStorageContext? context) :
        base(baseView)
    {
        if (options?.TrackLocation ?? true)
            Location = context?.Location;
    }

    static IZipArchiveView<System.IO.Compression.ZipArchive> CreateViewOnBclImpl(Stream stream, bool writable, bool leaveOpen, ZipArchiveOptions? options)
    {
        var mode = GetZipArchiveMode(stream, writable);
        try
        {
            return CreateView(new System.IO.Compression.ZipArchive(stream, mode, leaveOpen, options?.EntryNameEncoding));
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
